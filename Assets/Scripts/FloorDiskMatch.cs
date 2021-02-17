using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDiskMatch : NetworkBehaviour
{
    public float speed;
    public int max_rebond;
    private float current_life_time;

    private Rigidbody rb;
    private Vector3 target;
    private int nb_rebond; 
    private GameObject owner; 

    // Start is called before the first frame update
    public void setTarget(in Vector3 _target) {
        this.target = _target;
    }

    public void setOwner(in GameObject _owner) {
        this.owner = _owner;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nb_rebond = 0;
        current_life_time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        current_life_time += Time.deltaTime;
        
        rb.velocity = target * speed;
        transform.LookAt(transform.position + target, Vector3.up);

        if(current_life_time > 5.0f) {
            CmdDestroyDisk(gameObject);
        }
    }

    //[Command]
    void CmdDestroyDisk(GameObject g) {

        FindObjectOfType<NetworkManagerCustomMatch>().DestroyDisk(this.gameObject);        
        //NetworkServer.Destroy(g);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player") {
            if(isServer) {
                if(collision.gameObject != owner)
                    FindObjectOfType<NetworkManagerCustomMatch>().isTouched(collision.gameObject);
            }
            CmdDestroyDisk(gameObject);
        }

        if(collision.gameObject.tag == "Hexagon") {
            Debug.Log("J'ai touché un hexagon");
            collision.gameObject.GetComponent<HexagonScript>().RpcDestroy();
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
