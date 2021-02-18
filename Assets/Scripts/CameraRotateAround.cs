using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{
    public Transform target;

    private float speed = 0.05f;
    private float step = 0.0003f;

    // Update is called once per frame
    void Update()
    {
        // Spin the object around the target at 20 degrees/second.
        // transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);
        if (transform.position.x > 12.0f && speed - step > 0.0f) speed -= step;
        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
    }
}
