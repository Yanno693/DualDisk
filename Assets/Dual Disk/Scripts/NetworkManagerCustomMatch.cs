﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerCustomMatch : NetworkManager
{
    public GameObject disk;
    public GameObject floorDisk;
    public GameObject targetDisk;

    private bool hasStarted;
    private float serverTime;
    private GameObject[] players;

    private DataManager datas;
    
    public struct PlayerConnectMessage : NetworkMessage {
        public string data;
    }

    public override void OnStartServer() {
        base.OnStartServer();
        hasStarted = false;

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

    void Update() {
        if(NetworkServer.connections.Count == 2) {
            serverTime += Time.deltaTime;
        }

        if(serverTime > 3 && !hasStarted) {
            hasStarted = true;
            startMatch();
        }
    }

    // Attribue un prefab de Joueur à la personne qui se connecte
    void OnCreatePlayer(NetworkConnection conn, PlayerConnectMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        Camera c = FindObjectOfType<Camera>();
        c.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
        c.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;

        GameObject g = Instantiate<GameObject>(playerPrefab);
        g.GetComponent<NetworkPlayerController>().currentCamera = c;
        //Debug.Log(gameObject.GetComponent<NetworkPlayerController>().currentCamera);

        // call this to use this gameobject as the primary controller
        Debug.Log("Connexion du client " + conn.connectionId + ", Nombre de joueurs : " + NetworkServer.connections.Count);
        NetworkServer.AddPlayerForConnection(conn, g);
    }

    // Permet a un joueur de lancer un disque
    public void SpawnDisk(Vector3 pos, Quaternion rot, Vector3 dir, GameObject player) {
        GameObject g = Instantiate(disk, pos, rot);
        g.GetComponent<DiskMatch>().setTarget(dir);
        g.GetComponent<DiskMatch>().setOwner(player);
  
        NetworkServer.Spawn(g);
        
        if(hasStarted)
            if(player == players[0])
                g.GetComponent<DiskMatch>().RpcSetMaterial();
    }

    public void SpawnFloorDisk(Vector3 pos, Quaternion rot, Vector3 dir, GameObject player) {
        if (hasStarted) 
        {
            GameObject g = Instantiate(floorDisk, pos, rot);
            g.GetComponent<FloorDiskMatch>().setTarget(dir);
            g.GetComponent<FloorDiskMatch>().setOwner(player);
    
            NetworkServer.Spawn(g);
            
            if(player == players[0])
                g.GetComponent<FloorDiskMatch>().RpcSetMaterial();
        }
    }

    public void SpawnTargetDisk(Vector3 pos, Quaternion rot, Vector3 dir, GameObject player) {
        if(hasStarted) {
            GameObject g = Instantiate(targetDisk, pos, rot);
            g.GetComponent<TargetDiskMatch>().setTarget(dir);
            g.GetComponent<TargetDiskMatch>().setOwner(player);

            NetworkServer.Spawn(g);
            
            if(player == players[0]) {
                g.GetComponent<TargetDiskMatch>().RpcSetMaterial();
                g.GetComponent<TargetDiskMatch>().setPlayerTarget(players[1].transform);
            } else {
                g.GetComponent<TargetDiskMatch>().setPlayerTarget(players[0].transform);
            }
        }
    }

    // Permet a un disque de disparaitre
    public void DestroyDisk(GameObject g) {
        NetworkServer.Destroy(g);
    }

    public void DestroyAllDisk() {
        GameObject[] disks = GameObject.FindGameObjectsWithTag("Disk");

        foreach(GameObject disk in disks)
            NetworkServer.Destroy(disk);
    }

    public void RespawnAllHexagon() {
        GameObject[] hexagons = GameObject.FindGameObjectsWithTag("Hexagon");

        foreach(GameObject hexa in hexagons)
            hexa.GetComponent<HexagonScript>().RpcActivate();
    }

    // Replace les joueurs en position initiale
    public void spawnPlayers() {
        DestroyAllDisk();
        RespawnAllHexagon();
        
        players[0].GetComponent<PlayerThrowMatch>().RpcMove(new Vector3(-20, 3, 0), Quaternion.Euler(1, 0, 0));
        players[1].GetComponent<PlayerThrowMatch>().RpcMove(new Vector3(20, 3, 0), Quaternion.Euler(-1, 0, 0));
    }

    public void resetHealth() {
        datas.ResetHealth();
    }

    public void isTouched(GameObject g) {
        if(hasStarted) {

            if(players[0] == g)
                datas.RemoveP1Health();
            else
                datas.RemoveP2Health();

            if(datas.p1Health == 0) {
                datas.ResetHealth();
                datas.AddP2Score();
                spawnPlayers();
            }

            if(datas.p2Health == 0) {
                datas.ResetHealth();
                datas.AddP1Score();
                spawnPlayers();
            }
        }
    }

    public void hasFallen (GameObject g) {
        if(players[0] == g)
            datas.AddP2Score();
        else
            datas.AddP1Score();
        
        datas.ResetHealth();
        spawnPlayers();
    }

    public void initMaterials() {
        players[0].GetComponent<NetworkPlayerController>().RpcChangeMaterials();
    }

    public void initMatchData() {
        datas.InitMatchData();
    }

    public void startMatch() {
        players = GameObject.FindGameObjectsWithTag("Player");
        datas = FindObjectOfType<DataManager>();

        hasStarted = true;
        Debug.Log("Le match peut commencer !");
        spawnPlayers();
        initMatchData();
        initMaterials();
    }
}
