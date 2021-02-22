using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayerController : NetworkBehaviour
{
    public Material blueHead;
    public Material blueBody;
    public Material emissionBlue;
    protected CharacterController characterController;
    public Camera currentCamera;
    public float movementSpeed;
    public float rotationSpeed;
    public GameObject energy;
    public GameObject special;
    [HideInInspector] public float mouvementY;
    [HideInInspector] public bool isDodging;
    [HideInInspector] public bool isDead;
    private float dodgeTime;
    private Vector3 dodgeDirection;
    private bool isInPauseMenu;
    private float energy_speed;
    private float special_speed;
    private float dissolve;
    [HideInInspector] public bool isInvincible;
    private float damage;

    [HideInInspector] public bool serverAllowMovement;

    private float stepTime;
    private bool step;

    private void FightingMovement()
    {
        float finalSpeed = movementSpeed;

        Vector3 forward = transform.position - currentCamera.transform.position;
        forward.y = 0;
        Vector2 directionForward = new Vector2(forward.x, forward.z).normalized;
        Vector2 directionRight = new Vector2(forward.z, forward.x).normalized;

        Vector2 directionInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        float angleBetween = Vector2.SignedAngle(Vector2.up, directionForward) * Mathf.Deg2Rad;
        Vector2 directionRotation = Vec2Rotate(directionInput, angleBetween);

        
        if(characterController.isGrounded) {
            mouvementY = 0.0f;
            if (energy.GetComponent<Image>().fillAmount >= 0.5f)
            {
                if (Input.GetButton("Dodge") && !isDodging && serverAllowMovement && !isDead)
                {
                    CmdRemoveCollider();
                    CmdPlayDodge();
                    energy.GetComponent<Image>().fillAmount -= 0.5f;
                    dodgeTime = 0.0f;
                    isDodging = true;

                    Vector2 currentPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    List<KeyValuePair<float, Vector2>> dodgeDictionary = new List<KeyValuePair<float, Vector2>>();
                    dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2(0, 0)).magnitude, new Vector2(0, 0)));
                    dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2(0.5f, 0)).magnitude, new Vector2(0.5f, 0)));
                    dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2(-0.5f, 0)).magnitude, new Vector2(-0.5f, 0)));
                    dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2(0, 0.5f)).magnitude, new Vector2(0, 0.5f)));
                    dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2(0, -0.5f)).magnitude, new Vector2(0, -0.5f)));
                    dodgeDictionary.Sort((x, y) => x.Key.CompareTo(y.Key));

                    GetComponent<AnimationScript>().doDodge(dodgeDictionary[0].Value);

                    Vector2 dir = dodgeDictionary[0].Value * 1.6f;
                    Vector3 f = new Vector3(directionForward.x, 0, directionForward.y) * finalSpeed * Time.deltaTime * dir.y;
                    Vector3 r = new Vector3(directionForward.y, 0, -directionForward.x) * finalSpeed * Time.deltaTime * dir.x;
                    dodgeDirection = f + r;
                }
            }       
        }
        else
            mouvementY -= 5.5f * Time.deltaTime;

        if(Input.GetButton("Jump") && characterController.isGrounded && !isDodging && !isDead) {
            mouvementY = 1.8f;
            GetComponent<AnimationScript>().doJump();
        }

        //characterController.SimpleMove(new Vector3(directionRotation.x, jump, directionRotation.y) * finalSpeed * Time.deltaTime * 500.0f);
        if(serverAllowMovement && !Menu.isPaused && !isDead) {
            if(!isDodging) {
                characterController.Move(new Vector3(directionRotation.x, mouvementY, directionRotation.y) * finalSpeed * Time.deltaTime);
                if(GetComponent<CharacterController>().isGrounded) {
                    float speed = new Vector2(directionRotation.x, directionRotation.y).magnitude;

                    if(
                        (speed > 0.8 && stepTime > 0.25f) || 
                        (speed > 0.5 && stepTime > 0.4f) ||
                        (speed > 0.25 && stepTime > 0.7f)
                        ) {
                            stepTime = 0.0f;
                            step = !step;
                            CmdPlayFootStep(step);
                    }
                }
            } else
                characterController.Move(dodgeDirection);
        } else {
            characterController.Move(new Vector3(0, mouvementY, 0) * finalSpeed * Time.deltaTime);
        }
            //characterController.Move(new Vector3(directionRotation.x * dodgeDirection.y, mouvementY, directionRotation.y * dodgeDirection.x) * finalSpeed * Time.deltaTime);
        //characterController.Sim

        if(!isDodging && !isDead) {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(directionForward.x, 0, directionForward.y)),
                Time.deltaTime * rotationSpeed
            );            
        }
    }

    public static Vector2 Vec2Rotate(Vector2 v, float rad)
    {
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        );
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if(isLocalPlayer) {
            initCamera();
        }
    }

    [Command]
    public void CmdRemoveCollider(){
        RpcRemoveCollider();
    }

    [Command]
    public void CmdResetCollider(){
        RpcResetCollider();
    }

    [ClientRpc]
    public void RpcRemoveCollider()
    {
        //GetComponent<CapsuleCollider>().enabled = false;
        //GetComponent<CharacterController>().detectCollisions = false;
        gameObject.layer = LayerMask.NameToLayer("Barrier");
    }

    [ClientRpc]
    public void RpcResetCollider()
    {
        //GetComponent<CapsuleCollider>().enabled = true;
        //GetComponent<CharacterController>().detectCollisions = true;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void initCamera() {

        dissolve -= Time.deltaTime * 1.0f;
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve", dissolve);
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Dissolve", dissolve);

        if(currentCamera is null) {
            Camera cam = FindObjectOfType<Camera>();
            currentCamera = cam;
        }
        
        //transform.position = new Vector3(0,5,0);
        Cinemachine.CinemachineFreeLook c = currentCamera.gameObject.GetComponent<Cinemachine.CinemachineFreeLook>();
        Debug.Log(c);


        GameObject.Find("UI").GetComponent<Menu>().HideMenu();
        GameObject.Find("UI").GetComponent<Menu>().ShowHUD();

        c.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
        c.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";

        c.m_LookAt = transform.GetChild(0).transform;
        c.m_Follow = transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        dissolve = 1.0f;
        serverAllowMovement = true;
        
        if(currentCamera is null) {
            Camera cam = FindObjectOfType<Camera>();
            currentCamera = cam;
        }
        
        characterController = GetComponent<CharacterController>();

        movementSpeed = 15.0f;
        rotationSpeed = 5.0f;

        energy_speed = 0.1f;
        special_speed = 0.12f;

        energy = GameObject.Find("Energy_fill");
        special = GameObject.Find("Special_fill");

        mouvementY = 0.0f;
        isDodging = false;
        isInPauseMenu = false;

        //currentCamera = FindObjectOfType<Camera>();
        Debug.Log(currentCamera);
        
        if(isLocalPlayer) {
            initCamera();
        }
    }

    private Transform FindDeepChild(Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach(Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    [ClientRpc]
    public void RpcChangeMaterials() {
        Material[] mats = new Material[]{blueHead, blueBody};
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials = mats;

        GameObject d1 = FindDeepChild(transform, "D1").gameObject;
        GameObject d2 = FindDeepChild(transform, "D2").gameObject;
        GameObject d3 = FindDeepChild(transform, "D3").gameObject;

        GameObject[] ds = new GameObject[3]{d1, d2, d3};

        foreach(GameObject d in ds) {
            d.transform.GetChild(1).GetComponent<MeshRenderer>().material = emissionBlue;
            d.transform.GetChild(2).GetComponent<MeshRenderer>().material = emissionBlue;
        }
    }

    [ClientRpc]
    public void RpcForbidMouvement() {
        //if(isLocalPlayer) {
            serverAllowMovement = false;
        //}
    }

    [ClientRpc]
    public void RpcKillPlayer() {
        GetComponent<AnimationScript>().doDie();
        isDead = true;
    }

    [ClientRpc]
    public void RpcSetInvincible() {
        isInvincible = true;
        damage = 1.5f;

        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_Damage", 1);
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetInt("_Damage", 1);
    }

    [Command]
    public void CmdRemoveInvincible() {
        RpcRemoveInvincible();
    }

    [ClientRpc]
    public void RpcRemoveInvincible() {
        isInvincible = false;
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_Damage", 0);
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetInt("_Damage", 0);
    }

    [ClientRpc]
    public void RpcAllowMouvement() {
        //if(isLocalPlayer) {
            serverAllowMovement = true;
        //}
    }

    public void SetPause()
    {
        //RpcForbidMouvement();
        
        GameObject.Find("UI").GetComponent<Menu>().ShowPauseMenu();
        isInPauseMenu = true;
    }

    public void ResumePause()
    {
        //RpcAllowMouvement();
        
        GameObject.Find("UI").GetComponent<Menu>().HidePauseMenu();
        isInPauseMenu = false;
    }


    public void ControlPauseMenu()
    {
        if (!isInPauseMenu)
            SetPause();
        else
            ResumePause();
    }
    
    [ClientRpc]
    public void RpcUpdateDissolve() {
        if(!isDead) {
            if(dissolve < 1.0f) {
                dissolve += Time.deltaTime * 0.3f;
                transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve", dissolve);
                transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Dissolve", dissolve);
            }
        } else {
            if(dissolve > 0.0f) {
                dissolve -= Time.deltaTime * 0.3f;
                transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve", dissolve);
                transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Dissolve", dissolve);
            }
        }
    }

    [ClientRpc]
    public void RefillEnergy()
    {
        energy.GetComponent<Image>().fillAmount = 1.0f;
    }

    public void ResetSpecial()
    {
        special.GetComponent<Image>().fillAmount = 0.0f;
    }

    [Command]
    public void CmdPlayFootStep(bool _step) {
        RpcPlayFootStep(_step);
    }

    [ClientRpc]
    public void RpcPlayFootStep(bool _step) {
        GetComponents<AudioSource>()[_step?0:1].Play();
    }

    [Command]
    public void CmdPlayDodge() {
        RpcPlayDodge();
    }

    [ClientRpc]
    public void RpcPlayDodge() {
        GetComponents<AudioSource>()[2].Play();
    }

    // Update is called once per frame
    void Update()
    {
        stepTime += Time.deltaTime;
        
        RpcUpdateDissolve();

        if(isInvincible) {
            damage -= Time.deltaTime;
            if(damage < 0) {
                CmdRemoveInvincible();
            }
        }
        
        if(isLocalPlayer) {
            FightingMovement();

            if(isDodging) {
                dodgeTime += Time.deltaTime;
                if(dodgeTime > 1.1f)
                {
                    isDodging = false;
                    CmdResetCollider();
                }
                    
            }

            if (Input.GetButtonDown("Cancel"))
            {
                ControlPauseMenu();
            }

            if(energy.GetComponent<Image>().fillAmount < 1)
                energy.GetComponent<Image>().fillAmount += Time.deltaTime * energy_speed;

            if (special.GetComponent<Image>().fillAmount < 1)
                special.GetComponent<Image>().fillAmount = GetComponent<PlayerThrowMatch>().delaySpecial / 12.0f;
        }
    }
}