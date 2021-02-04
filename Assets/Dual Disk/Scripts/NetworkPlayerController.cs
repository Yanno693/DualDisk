using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    protected CharacterController characterController;
    public Camera currentCamera;
    public float movementSpeed;
    public float rotationSpeed;
    public float mouvementY;

    private void FightingMovement()
    {
        float finalSpeed = movementSpeed;

        //if (Input.GetButton("Run"))
            //finalSpeed *= 2.0f;

        Vector3 forward = transform.position - currentCamera.transform.position;
        forward.y = 0;
        Vector2 directionForward = new Vector2(forward.x, forward.z).normalized;

        Vector2 directionInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        float angleBetween = Vector2.SignedAngle(Vector2.up, directionForward) * Mathf.Deg2Rad;
        Vector2 directionRotation = Vec2Rotate(directionInput, angleBetween);

        if(characterController.isGrounded)
            mouvementY = 0.0f;
        else
            mouvementY -= 6.0f * Time.deltaTime;

        if(Input.GetButton("Jump") && characterController.isGrounded) {
            Debug.Log("Jump !");
            mouvementY = 2.5f;
        }

        //characterController.SimpleMove(new Vector3(directionRotation.x, jump, directionRotation.y) * finalSpeed * Time.deltaTime * 500.0f);
        characterController.Move(new Vector3(directionRotation.x, mouvementY, directionRotation.y) * finalSpeed * Time.deltaTime);
        //characterController.Sim

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(new Vector3(directionForward.x, 0, directionForward.y)),
            Time.deltaTime * rotationSpeed
        );            
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
            Debug.Log("Camera ? On va voir : " + cam);
            currentCamera = cam;
        }
        
        //transform.position = new Vector3(0,5,0);
        Cinemachine.CinemachineFreeLook c = currentCamera.gameObject.GetComponent<Cinemachine.CinemachineFreeLook>();
        Debug.Log(c);

        c.m_LookAt = transform.GetChild(0).transform;
        c.m_Follow = transform;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if(currentCamera is null) {
            Camera cam = FindObjectOfType<Camera>();
            Debug.Log("Camera ? On va voir : " + cam);
            currentCamera = cam;
        }
        
        characterController = GetComponent<CharacterController>();

        movementSpeed = 25.0f;
        rotationSpeed = 5.0f;

        mouvementY = 0.0f;

        //currentCamera = FindObjectOfType<Camera>();
        Debug.Log(currentCamera);
        
        if(isLocalPlayer) {
            initCamera();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) {
            FightingMovement();
        }
    }
}