using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskMatch : NetworkBehaviour
{
    public float speed;
    public int max_rebond;

    private Rigidbody rb;
    private Vector3 target;
    private int nb_rebond;  

    // Start is called before the first frame update
    public void setTarget(in Vector3 _target) {
        this.target = _target;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nb_rebond = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = target * speed;
        transform.LookAt(target, Vector3.up);
    }

    //[Command]
    void CmdDestroyDisk(GameObject g) {

        FindObjectOfType<NetworkManagerCustomMatch>().DestroyDisk(this.gameObject);        
        //NetworkServer.Destroy(g);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Touché");

            if(isServer) {
                FindObjectOfType<NetworkManagerCustomMatch>().isTouched(collision.gameObject);
            }
            CmdDestroyDisk(gameObject);
        }

        if(nb_rebond <= max_rebond)
        {
            var direction = Vector3.Reflect(target.normalized, collision.contacts[0].normal);

            target = direction;
            rb.velocity = target;
            nb_rebond++;
        }
        else
        {
            //Destroy(gameObject);
            CmdDestroyDisk(gameObject);
        }
        
    }
}
