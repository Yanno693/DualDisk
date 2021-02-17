using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEmitterScript : NetworkBehaviour
{
    private float lifeTime;
    private bool isDestroyed;
    
    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 0.0f;
        isDestroyed = false;  
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDestroyed) {
            lifeTime += Time.deltaTime;
            if(lifeTime > 1.0f) {
                isDestroyed = true;
                FindObjectOfType<NetworkManagerCustomMatch>().DestroyDisk(gameObject);
            }
        }
    }
}
