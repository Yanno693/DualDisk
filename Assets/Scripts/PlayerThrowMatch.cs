using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowMatch : NetworkBehaviour {
    
    private NetworkManagerCustomMatch networkManager;
    
    [Command]
    public void CmdDirac(Vector3 pos, Quaternion rot, Vector3 dir) {
                //FindObjectOfType<NetworkManagerCustomMatch>().SpawnProj(pos, rot, dir);

        networkManager.SpawnDisk(pos, rot, dir);
    }

    [ClientRpc]
    public void RpcMove(Vector3 pos) {
        this.GetComponent<NetworkPlayerController>().mouvementY = 0.0f;
        GetComponent<NetworkTransform>().ServerTeleport(pos);
        //this.transform.position = pos;

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        networkManager = FindObjectOfType<NetworkManagerCustomMatch>();
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
