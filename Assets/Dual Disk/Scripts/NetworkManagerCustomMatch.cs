using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerCustomMatch : NetworkManager
{
    public GameObject disk;

    private int p1Score, p2Score, p1Health, p2Health;
    
    public struct PlayerConnectMessage : NetworkMessage {
        public string data;
    }

    public override void OnStartServer() {
        base.OnStartServer();

        NetworkServer.RegisterHandler<PlayerConnectMessage>(OnCreatePlayer);
    }
    
    // Start is called before the first frame update
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        PlayerConnectMessage msg = new PlayerConnectMessage {
            data = "Ok"
        };
        conn.Send(msg);
    }

    void OnCreatePlayer(NetworkConnection conn, PlayerConnectMessage message)
    {
        //Debug.Log("Jello ?");
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        Camera c = FindObjectOfType<Camera>();
        c.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
        c.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;

        GameObject g = Instantiate<GameObject>(playerPrefab);
        g.GetComponent<NetworkPlayerController>().currentCamera = c;
        //Debug.Log(gameObject.GetComponent<NetworkPlayerController>().currentCamera);

        // call this to use this gameobject as the primary controller
        Debug.Log("Connexion de " + conn.connectionId);
        NetworkServer.AddPlayerForConnection(conn, g);

        if(NetworkServer.connections.Count == 2) {
            startMatch();
        }

        //NetworkServer.

    }

    public void SpawnDisk(Vector3 pos, Quaternion rot, Vector3 dir) {
        GameObject g = Instantiate(disk, pos, rot);
        g.GetComponent<DiskMatch>().setTarget(dir);
        
        NetworkServer.Spawn(g);
    }

    public void DestroyDisk(GameObject g) {
        //NetworkServer.Destroy(NetworkIdentity.spawned[_netId].gameObject);
        NetworkServer.Destroy(g);
    }

    public void spawnPlayers() {
        GameObject[] l = GameObject.FindGameObjectsWithTag("Player");
        
        l[0].GetComponent<PlayerThrowMatch>().RpcMove(new Vector3(-20, 3, 0));
        l[1].GetComponent<PlayerThrowMatch>().RpcMove(new Vector3(20, 3, 0));
    }

    public void resetHealth() {
        p1Health = 3;
        p2Health = 3;
    }

    public void isTouched(GameObject g) {
        GameObject[] l = GameObject.FindGameObjectsWithTag("Player");

        if(l[0] == g) {
            p1Health--;
        } else {
            p2Health--;
        }

        if(p1Health == 0) {
            p2Score++;
            resetHealth();
            spawnPlayers();
        }

        if(p2Health == 0) {
            p1Score++;
            resetHealth();
            spawnPlayers();
        }

        Debug.Log("P1 :" + p1Health + " P2 : " + p2Health);
    }

    public void initMatchData(){
        p1Health = 3;
        p1Health = 3;
        p1Score = 0;
        p2Score = 0;
    }

    public void startMatch() {
        Debug.Log("Le match peut commencer !");
        spawnPlayers();
        initMatchData();
    }
}
