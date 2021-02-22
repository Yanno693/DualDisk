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

    [HideInInspector] public bool ownerId;
    
    [ClientRpc]
    public void RpcActivate() {
        //GetComponent<TrailRenderer>().material = trailBlue;

        //GetComponent<MeshRenderer>().enabled = true;
        isDestroyed = false;
        dissolve = 0.0f;
        GetComponent<MeshCollider>().enabled = true;
        Transform emitter = transform.Find("Emitter");
        Transform holo = transform.Find("HoloEmitter");
        Transform spark = transform.Find("SparkEmitter");
        emitter.GetComponent<ParticleSystem>().Stop();
        emitter.GetComponent<ParticleSystem>().Clear();
        holo.GetComponent<ParticleSystem>().Stop();
        holo.GetComponent<ParticleSystem>().Clear();
        spark.GetComponent<ParticleSystem>().Stop();
        spark.GetComponent<ParticleSystem>().Clear();
        ownerId = emitter.position.x < 0;

        if(ownerId) {
            emitter.GetComponent<ParticleSystemRenderer>().material = emissionBlue;
            holo.GetComponent<ParticleSystemRenderer>().material = emissionBlue;
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
        GetComponent<AudioSource>().Play();
        transform.Find("Emitter").GetComponent<ParticleSystem>().Play();
        transform.Find("HoloEmitter").GetComponent<ParticleSystem>().Play();
        transform.Find("SparkEmitter").GetComponent<ParticleSystem>().Play();

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
