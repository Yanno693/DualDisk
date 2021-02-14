using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonScript : NetworkBehaviour
{
    [ClientRpc]
    public void RpcActivate() {
        //GetComponent<TrailRenderer>().material = trailBlue;

        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
    }

    [ClientRpc]
    public void RpcDestroy() {
        //GetComponent<TrailRenderer>().material = trailBlue;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
