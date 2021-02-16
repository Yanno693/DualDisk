using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowMatch : NetworkBehaviour {
    
    public float diskDelay;
    public int nbDisk;

    private float[] delays;
    private bool fallen;
    
    private NetworkManagerCustomMatch networkManager;
    
    [Command]
    public void CmdThrow(Vector3 pos, Quaternion rot, Vector3 dir) {
                //FindObjectOfType<NetworkManagerCustomMatch>().SpawnProj(pos, rot, dir);

        networkManager.SpawnDisk(pos, rot, dir, gameObject);
    }

    [Command]
    public void CmdThrowFloor(Vector3 pos, Quaternion rot, Vector3 dir) {
                //FindObjectOfType<NetworkManagerCustomMatch>().SpawnProj(pos, rot, dir);

        networkManager.SpawnFloorDisk(pos, rot, dir, gameObject);
    }

    [Command]
    public void CmdThrowTarget(Vector3 pos, Quaternion rot, Vector3 dir) {
                //FindObjectOfType<NetworkManagerCustomMatch>().SpawnProj(pos, rot, dir);

        networkManager.SpawnTargetDisk(pos, rot, dir, gameObject);
    }

    [ClientRpc]
    public void RpcMove(Vector3 pos, Quaternion rot) {
        fallen = false;
        this.GetComponent<NetworkPlayerController>().mouvementY = 0.0f;
        this.GetComponent<NetworkPlayerController>().isDodging = false;
        this.GetComponent<NetworkPlayerController>().RpcForbidMouvement();

        this.GetComponent<CharacterController>().enabled = false;
        GetComponent<NetworkTransform>().ServerTeleport(pos, rot);
        this.GetComponent<CharacterController>().enabled = true;

        setCamera(rot);
    }

    [ClientRpc]
    public void RpcAddDisk() {
        if(isLocalPlayer)
            for(int i = 0; i < nbDisk; i++)
                if(delays[i] < diskDelay) {
                    delays[i] = diskDelay;
                    break;
                }
    }

    void setCamera(Quaternion rot) {
        if(isLocalPlayer) {
            GetComponent<NetworkPlayerController>().currentCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.Value = 90.0f * rot.eulerAngles.x;
            GetComponent<NetworkPlayerController>().currentCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.Value = 0.8f;
            for(int i = 0; i < nbDisk; i++)
                delays[i] = diskDelay;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        networkManager = FindObjectOfType<NetworkManagerCustomMatch>();
        delays = new float[nbDisk];
        fallen = false;

        for(int i = 0; i < nbDisk; i++)
            delays[i] = diskDelay;
    }

    [Command]
    public void CmdHasFallen(GameObject g) {
        networkManager.hasFallen(g);
    } 

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) { 
            for(int i = 0; i < nbDisk; i++)
                delays[i] += Time.deltaTime;

            if(GetComponent<NetworkPlayerController>().serverAllowMovement) {
                if(Input.GetButtonDown("Fire1")) {
                    for(int i = 0; i < nbDisk; i++) {
                        if(delays[i] >= diskDelay) {
                            Transform camera_target = transform.GetChild(0);
                            Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                            CmdThrow(transform.position + direction * 2.0f + new Vector3(0, 2.0f, 0) , Quaternion.identity, direction);
                            delays[i] = 0.0f;
                            break;
                        }
                    }
                }

                if(Input.GetButtonDown("Fire2")) {
                    for(int i = 0; i < nbDisk; i++) {
                        if(delays[i] >= diskDelay) {
                            Transform camera_target = transform.GetChild(0);
                            Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                            CmdThrowFloor(transform.position + direction * 2.0f + new Vector3(0, 2.0f, 0) , Quaternion.identity, direction);
                            delays[i] = 0.0f;
                            break;
                        }
                    }
                }

                if(Input.GetButtonDown("Fire3")) {
                    for(int i = 0; i < nbDisk; i++) {
                        if(delays[i] >= diskDelay) {
                            Transform camera_target = transform.GetChild(0);
                            Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                            CmdThrowTarget(transform.position + direction * 2.0f + new Vector3(0, 2.0f, 0) , Quaternion.identity, direction);
                            delays[i] = 0.0f;
                            break;
                        }
                    }
                }
            }

            if(transform.position.y < -150) {
                if(!networkManager)
                    networkManager = FindObjectOfType<NetworkManagerCustomMatch>();

                if(!fallen) {
                    fallen = true;
                    CmdHasFallen(gameObject);
                }
            }
        }
    }
}
