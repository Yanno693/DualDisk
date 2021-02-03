using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public Transform spawn;
    public GameObject prefabBall;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, speed, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -speed, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(prefabBall, spawn.position, transform.rotation);
        }
    }
}
