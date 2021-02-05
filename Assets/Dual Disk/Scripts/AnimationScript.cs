using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : NetworkBehaviour
{
    private Animator animator;
    private float throwWeight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        throwWeight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) {
            throwWeight -= Time.deltaTime;
            animator.SetFloat("X", Input.GetAxis("Horizontal"));
            animator.SetFloat("Y", Input.GetAxis("Vertical"));
            animator.SetLayerWeight(animator.GetLayerIndex("Throw Layer"), throwWeight);

            if(Input.GetButtonDown("Fire1")) {
                throwWeight = 1.0f;
                animator.Play("Throw_Revert", animator.GetLayerIndex("Throw Layer"), 0.0f);
            }
        }
    }
}
