using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskRecord : MonoBehaviour
{
    public Material trailBlue;
    public Material emissionBlue;
    public GameObject collisionGameobject;
    public float speed;
    public int max_rebond;
    private float current_life_time;

    private Rigidbody rb;
    private Vector3 target;
    private int nb_rebond; 
    private GameObject owner; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nb_rebond = 0;
        current_life_time = 0.0f;
        target = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        current_life_time += Time.deltaTime;
        
        rb.velocity = target * speed;
        transform.LookAt(transform.position + target, Vector3.up);
    }

    //[Command]
    void CmdDestroyDisk(GameObject g) {

        //FindObjectOfType<NetworkManagerCustomMatch>().DestroyDisk(this.gameObject);        
        //NetworkServer.Destroy(g);
    }

    private void OnCollisionEnter(Collision collision)
    {        

        GameObject g = Instantiate(collisionGameobject,
        collision.contacts[0].point,
        Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up));
            
            var direction = Vector3.Reflect(target.normalized, collision.contacts[0].normal);

            target = direction;
            rb.velocity = target;
            nb_rebond++;
    
    }
}
