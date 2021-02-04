using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerCustomMatch : NetworkManager
{
    public GameObject disk;
    
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
        l[0].GetComponent<PlayerThrowMatch>().RpcMove(new Vector3(0, 100, -5));
        l[1].GetComponent<PlayerThrowMatch>().RpcMove(new Vector3(0, 100, 5));
    }

    public void initMatchData(){

    }

    public void startMatch() {
        Debug.Log("Le match peut commencer !");
        spawnPlayers();
        initMatchData();
    }
}
