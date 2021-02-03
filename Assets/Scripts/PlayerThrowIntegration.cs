using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowIntegration : MonoBehaviour
{
    //public Transform spawn;
    public GameObject prefabBall;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        if(Input.GetButtonDown("Fire1"))
        {
            Transform camera_target = transform.GetChild(0);
            
            Vector3 direction = (camera_target.position - GetComponent<NetworkPlayerController>().currentCamera.transform.position).normalized;
            GameObject proj = Instantiate<GameObject>(prefabBall, transform.position + direction, transform.rotation);
            proj.GetComponent<BounceIntegration>().setTarget(direction);

            Debug.Log(proj);
        }
    }
}
