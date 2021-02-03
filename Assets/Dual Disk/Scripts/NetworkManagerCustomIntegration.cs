using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerCustomIntegration : NetworkManager
{
    public GameObject disk;
    
    public struct PlayerConnectMessage : NetworkMessage {
        public string data;
    }

    public override void OnStartServer() {
        base.OnStartServer();

        NetworkServer.RegisterHandler<PlayerConnectMessage>(OnCreateCharacter);
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

    void OnCreateCharacter(NetworkConnection conn, PlayerConnectMessage message)
    {
        Debug.Log("Jello ?");
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        Camera c = FindObjectOfType<Camera>();
        Debug.Log(c);
        GameObject g = Instantiate<GameObject>(playerPrefab);

        g.GetComponent<NetworkPlayerController>().currentCamera = c;
        //Debug.Log(gameObject.GetComponent<NetworkPlayerController>().currentCamera);

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, g);
    }

    public void SpawnProj(Vector3 pos, Quaternion rot, Vector3 dir) {
        GameObject g = Instantiate(disk, pos, rot);
        g.GetComponent<BounceIntegration>().setTarget(dir);
        
        NetworkServer.Spawn(g);
    }
}
