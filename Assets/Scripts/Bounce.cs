using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float speed;
    public int max_rebond;

    private Rigidbody rb;
    private Vector3 target;
    private int nb_rebond;  

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = transform.TransformDirection(Vector3.forward);
        nb_rebond = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = target * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(nb_rebond <= max_rebond)
        {
            var direction = Vector3.Reflect(target.normalized, collision.contacts[0].normal);
            Debug.Log(direction);

            target = direction;
            rb.velocity = target;
            nb_rebond++;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
