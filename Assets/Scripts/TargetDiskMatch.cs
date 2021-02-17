using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDiskMatch : NetworkBehaviour
{
    public float speed;
    public int max_rebond;
    private float current_life_time;

    private Rigidbody rb;
    private Vector3 target;
    private int nb_rebond; 
    private GameObject owner; 
    private Transform playerTarget;

    // Start is called before the first frame update
    public void setTarget(in Vector3 _target) {
        this.target = _target;
    }

    public void setPlayerTarget(in Transform _target) {
        this.playerTarget = _target;
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
        
        Vector3 pTarget = new Vector3(0,4,0);

        if(playerTarget)
            pTarget = playerTarget.transform.position;

        /*Vector3 direction = (target + (pTarget - transform.position).normalized * 0.8f).normalized;
        rb.velocity = direction * speed;
        transform.LookAt(transform.position + target, Vector3.up);*/

        target = (target + ((pTarget - transform.position).normalized * 0.05f)).normalized;

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
            if(current_life_time > 0.2f) {
                if(isServer) {
                    if(collision.gameObject != owner)
                        FindObjectOfType<NetworkManagerCustomMatch>().isTouched(collision.gameObject);
                }
                CmdDestroyDisk(gameObject);
            }
        }

        if(nb_rebond <= max_rebond)
        {
            var direction = Vector3.Reflect(target.normalized, collision.contacts[0].normal);

            target = direction;
            rb.velocity = target;
            nb_rebond++;
        } else {
            //Destroy(gameObject);
            CmdDestroyDisk(gameObject);
        }
    }
}
