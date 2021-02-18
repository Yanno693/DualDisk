using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonScript : NetworkBehaviour
{
    public Material emissionBlue;
    public Material dissolveBlue;

    private bool isDestroyed;
    private float dissolve;
    
    [ClientRpc]
    public void RpcActivate() {
        //GetComponent<TrailRenderer>().material = trailBlue;

        //GetComponent<MeshRenderer>().enabled = true;
        isDestroyed = false;
        dissolve = 0.0f;
        GetComponent<MeshCollider>().enabled = true;
        Transform emitter = transform.Find("Emitter");
        emitter.GetComponent<ParticleSystem>().Stop();
        emitter.GetComponent<ParticleSystem>().Clear();

        if(emitter.position.x < 0) {
            emitter.GetComponent<ParticleSystemRenderer>().material = emissionBlue;
            GetComponent<MeshRenderer>().material = dissolveBlue;
        }
        GetComponent<MeshRenderer>().material.SetFloat("_Dissolve", 0);

    }

    [ClientRpc]
    public void RpcDestroy() {
        //GetComponent<TrailRenderer>().material = trailBlue;

        //GetComponent<MeshRenderer>().enabled = false;
        isDestroyed = true;
        GetComponent<MeshCollider>().enabled = false;
        transform.Find("Emitter").GetComponent<ParticleSystem>().Play();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        isDestroyed = false;
        dissolve = 0.0f;   
    }

    // Update is called once per frame
    void Update()
    {
        if(isDestroyed) {
            dissolve += Time.deltaTime;
            if(dissolve < 1.0f)
                GetComponent<MeshRenderer>().material.SetFloat("_Dissolve", dissolve);
        }   
    }
}
