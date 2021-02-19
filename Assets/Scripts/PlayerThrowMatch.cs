using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowMatch : NetworkBehaviour {
    
    public float diskDelay;
    public int nbDisk;

    private bool[] displayDisks;
    private bool displaySpecial;

    private float[] delays;
    private bool fallen;
    private float delaySpecial;
    
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
        delaySpecial = 0.0f;
        this.GetComponent<NetworkPlayerController>().mouvementY = 0.0f;
        this.GetComponent<NetworkPlayerController>().isDodging = false;
        this.GetComponent<NetworkPlayerController>().isDead = false;
        this.GetComponent<NetworkPlayerController>().RpcForbidMouvement();
        this.GetComponent<NetworkPlayerController>().RpcRemoveInvincible();
        this.GetComponent<AnimationScript>().doAlive();

        this.GetComponent<CharacterController>().enabled = false;
        GetComponent<NetworkTransform>().ServerTeleport(pos, rot);
        this.GetComponent<CharacterController>().enabled = true;
        this.GetComponent<NetworkPlayerController>().CmdResetCollider();
        CmdHideDisplayDisk(4);

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
            for(int i = 0; i < nbDisk; i++) {
                delays[i] = diskDelay;
                CmdShowDisplayDisk(i);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        networkManager = FindObjectOfType<NetworkManagerCustomMatch>();
        delays = new float[nbDisk];
        displayDisks = new bool[nbDisk];
        fallen = false;

        for(int i = 0; i < nbDisk; i++) {
            delays[i] = diskDelay;
            displayDisks[i] = true;
        }
        delaySpecial = 0.0f;
        displaySpecial = false;
    }

    [Command]
    public void CmdHasFallen(GameObject g) {
        networkManager.hasFallen(g);
    }

    [Command]
    public void CmdHideDisplayDisk(int i) {
        RpcHideDisplayDisk(i);
    }

    [ClientRpc]
    public void RpcHideDisplayDisk(int i) {
        GameObject disk;
        if(i < 3) {
            disk = FindDeepChild(transform, "D" + (3 - i)).gameObject;
            displayDisks[i] = false;
        } else {
            disk = FindDeepChild(transform, "DS").gameObject;
            displaySpecial = false;
        }

        for(int j = 0; j < disk.transform.childCount; j++)
            disk.transform.GetChild(j).GetComponent<MeshRenderer>().enabled = false;
    }

    [Command]
    public void CmdShowDisplayDisk(int i) {
        RpcShowDisplayDisk(i);
    }

    [ClientRpc]
    //public void RpcShowDisplayDisk(GameObject disk) {
    public void RpcShowDisplayDisk(int i) {
        GameObject disk;
        if(i < 3) {
            disk = FindDeepChild(transform, "D" + (3 - i)).gameObject;
            displayDisks[i] = true;
        } else {
            disk = FindDeepChild(transform, "DS").gameObject;
            displaySpecial = true;
        }
        
        for(int j = 0; j < disk.transform.childCount; j++)
            disk.transform.GetChild(j).GetComponent<MeshRenderer>().enabled = true;
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

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) { 
            for(int i = 0; i < nbDisk; i++) {
                delays[i] += Time.deltaTime;
                if(delays[i] > diskDelay && !displayDisks[i])
                    CmdShowDisplayDisk(i);
            }

            if(delaySpecial > 12.0f && !displaySpecial)
                CmdShowDisplayDisk(4);

            if(GetComponent<NetworkPlayerController>().serverAllowMovement && !GetComponent<NetworkPlayerController>().isDead && !Menu.isPaused) {
                if(Input.GetButtonDown("Fire1")) {
                    for(int i = 0; i < nbDisk; i++) {
                        if(delays[i] >= diskDelay) {
                            Transform camera_target = transform.GetChild(0);
                            Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                            CmdThrow(transform.position + direction * 2.0f + new Vector3(0, 2.0f, 0) , Quaternion.identity, direction);
                            delays[i] = 0.0f;

                            //Debug.Log(transform.Find("D" + (i + 1)));
                            CmdHideDisplayDisk(i);
                            break;
                        }
                    }
                }

                if(delaySpecial >= 12.0f) {
                    if(Input.GetButtonDown("Fire2")) {
                        Transform camera_target = transform.GetChild(0);
                        Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                        CmdThrowFloor(transform.position + direction * 2.0f + new Vector3(0, 2.0f, 0) , Quaternion.identity, direction);
                        CmdHideDisplayDisk(4);
                        delaySpecial = 0.0f;
                    }

                    if(Input.GetButtonDown("Fire3")) {
                        Transform camera_target = transform.GetChild(0);
                        Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;

                        CmdThrowTarget(transform.position + direction * 2.0f + new Vector3(0, 2.0f, 0) , Quaternion.identity, direction);
                        CmdHideDisplayDisk(4);
                        delaySpecial = 0.0f;
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

            if(GetComponent<NetworkPlayerController>().serverAllowMovement && delaySpecial < 12.0f)
                delaySpecial += Time.deltaTime;
        }
    }
}
