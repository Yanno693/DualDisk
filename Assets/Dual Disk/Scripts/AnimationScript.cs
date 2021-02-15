using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : NetworkBehaviour
{
    private Animator animator;
    private float throwWeight;
    private float jumpWeight;
    //private float dodgeWeight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        throwWeight = 0;
        jumpWeight = 0;
        //dodgeWeight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) {
            throwWeight -= Time.deltaTime;
            jumpWeight -= Time.deltaTime;
            animator.SetFloat("X", Input.GetAxis("Horizontal"));
            animator.SetFloat("Y", Input.GetAxis("Vertical"));
            animator.SetLayerWeight(animator.GetLayerIndex("Throw Layer"), throwWeight);
            animator.SetLayerWeight(animator.GetLayerIndex("Jump Layer"), jumpWeight);
            //animator.SetLayerWeight(animator.GetLayerIndex("Dodge Layer"), dodgeWeight);

            if(Input.GetButtonDown("Fire1")) {
                throwWeight = 1.0f;
                animator.Play("Throw_Revert", animator.GetLayerIndex("Throw Layer"), 0.0f);
            }

            if(Input.GetButtonDown("Jump"))
            {
                jumpWeight = 1.0f;
                animator.Play("Full_Jump", animator.GetLayerIndex("Jump Layer"), 0.0f);
            }

            if (Input.GetButtonDown("Dodge"))
            {
                animator.SetBool("Dodge", true);
            }
            else
            {
                animator.SetBool("Dodge", false);
            }
        }
    }
}
