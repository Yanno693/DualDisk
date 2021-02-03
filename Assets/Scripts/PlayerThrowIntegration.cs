using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowIntegration : NetworkBehaviour
{
    //public Transform spawn;
    public GameObject prefabBall;
    public float speed;

    [Command]
    public void CmdDirac(Vector3 pos, Quaternion rot, Vector3 dir) {
        Debug.Log("Dirac");

        FindObjectOfType<NetworkManagerCustomIntegration>().SpawnProj(pos, rot, dir);
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) { 
            if(Input.GetButtonDown("Fire1")) {
                Transform camera_target = transform.GetChild(0);
                Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                CmdDirac(transform.position + direction , transform.rotation, direction);
            }
        }
    }
}
