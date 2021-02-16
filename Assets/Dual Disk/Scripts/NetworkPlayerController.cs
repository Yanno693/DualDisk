using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    public Material blueHead;
    public Material blueBody;
    
    protected CharacterController characterController;
    public Camera currentCamera;
    public float movementSpeed;
    public float rotationSpeed;
    [HideInInspector] public float mouvementY;
    [HideInInspector] public bool isDodging;
    private float dodgeTime;
    private Vector3 dodgeDirection;

    [HideInInspector] public bool serverAllowMovement;

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
            if(Input.GetButton("Dodge") && !isDodging && serverAllowMovement) {
                dodgeTime = 0.0f;
                isDodging = true;

                Vector2 currentPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                List<KeyValuePair<float, Vector2>> dodgeDictionary = new List<KeyValuePair<float, Vector2>>();
                dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2( 0   , 0   )).magnitude, new Vector2( 0   , 0   )));
                dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2( 0.5f, 0   )).magnitude, new Vector2( 0.5f, 0   )));
                dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2(-0.5f, 0   )).magnitude, new Vector2(-0.5f, 0   )));
                dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2( 0   , 0.5f)).magnitude, new Vector2( 0   , 0.5f)));
                dodgeDictionary.Add(new KeyValuePair<float, Vector2>((currentPos - new Vector2( 0   ,-0.5f)).magnitude, new Vector2( 0   ,-0.5f)));
                dodgeDictionary.Sort((x, y) => x.Key.CompareTo(y.Key));

                GetComponent<AnimationScript>().doDodge(dodgeDictionary[0].Value);
                
                Vector2 dir = dodgeDictionary[0].Value * 1.6f;
                Vector3 f = new Vector3(directionForward.x, 0, directionForward.y)  * finalSpeed * Time.deltaTime * dir.y;
                Vector3 r = new Vector3(directionForward.y, 0, -directionForward.x)  * finalSpeed * Time.deltaTime * dir.x;
                dodgeDirection = f + r;
            }
        }
        else
            mouvementY -= 5.5f * Time.deltaTime;

        if(Input.GetButton("Jump") && characterController.isGrounded && !isDodging) {
            mouvementY = 1.8f;
            GetComponent<AnimationScript>().doJump();
        }

        //characterController.SimpleMove(new Vector3(directionRotation.x, jump, directionRotation.y) * finalSpeed * Time.deltaTime * 500.0f);
        if(serverAllowMovement) {
            if(!isDodging)
                characterController.Move(new Vector3(directionRotation.x, mouvementY, directionRotation.y) * finalSpeed * Time.deltaTime);
            else
                characterController.Move(dodgeDirection);
        } else {
            characterController.Move(new Vector3(0, mouvementY, 0) * finalSpeed * Time.deltaTime);
        }
            //characterController.Move(new Vector3(directionRotation.x * dodgeDirection.y, mouvementY, directionRotation.y * dodgeDirection.x) * finalSpeed * Time.deltaTime);
        //characterController.Sim

        if(!isDodging) {
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

    public void initCamera() {

        if(currentCamera is null) {
            Camera cam = FindObjectOfType<Camera>();
            currentCamera = cam;
        }
        
        //transform.position = new Vector3(0,5,0);
        Cinemachine.CinemachineFreeLook c = currentCamera.gameObject.GetComponent<Cinemachine.CinemachineFreeLook>();
        Debug.Log(c);


        Menu menu = FindObjectOfType<Menu>();
        if(menu)
            menu.gameObject.SetActive(false);

       
        c.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
        c.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";

        c.m_LookAt = transform.GetChild(0).transform;
        c.m_Follow = transform;

        Cursor.lockState = CursorLockMode.Locked;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        serverAllowMovement = true;
        
        if(currentCamera is null) {
            Camera cam = FindObjectOfType<Camera>();
            currentCamera = cam;
        }
        
        characterController = GetComponent<CharacterController>();

        movementSpeed = 15.0f;
        rotationSpeed = 5.0f;

        mouvementY = 0.0f;
        isDodging = false;

        //currentCamera = FindObjectOfType<Camera>();
        Debug.Log(currentCamera);
        
        if(isLocalPlayer) {
            initCamera();
        }
    }

    [ClientRpc]
    public void RpcChangeMaterials() {
        Material[] mats = new Material[]{blueHead, blueBody};
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials = mats;
    }

    [ClientRpc]
    public void RpcForbidMouvement() {
        //if(isLocalPlayer) {
            serverAllowMovement = false;
        //}
    }

    [ClientRpc]
    public void RpcAllowMouvement() {
        //if(isLocalPlayer) {
            serverAllowMovement = true;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) {
            FightingMovement();

            if(isDodging) {
                dodgeTime += Time.deltaTime;
                if(dodgeTime > 1.1f)
                    isDodging = false;
            }
        }
    }
}