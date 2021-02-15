﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : NetworkBehaviour
{
    private Animator animator;
    private float throwWeight;
    private float jumpWeight;
    private float fallWeight;
    //private float dodgeWeight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        throwWeight = 0;
        jumpWeight = 0;
        fallWeight = 0;
        //dodgeWeight = 0;
    }

    public void doJump() {
        jumpWeight = 1.0f;
        animator.Play("Full_Jump", animator.GetLayerIndex("Jump Layer"), 0.0f);
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
            animator.SetBool("Fall", transform.position.y < -5);

            if(Input.GetButtonDown("Fire1")) {
                throwWeight = 1.0f;
                animator.Play("Throw_Revert", animator.GetLayerIndex("Throw Layer"), 0.0f);
            }

            if (Input.GetButtonDown("Dodge"))
            {
                //SortedDictionary<float, string> dodgeDictionary = new SortedDictionary<float, string>();

                Vector2 currentPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                List<KeyValuePair<float, string>> dodgeDictionary = new List<KeyValuePair<float, string>>();
                dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0   , 0   )).magnitude, "Dodge"));
                dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0.5f, 0   )).magnitude, "Dodge Right"));
                dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2(-0.5f, 0   )).magnitude, "Dodge Left"));
                dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0   , 0.5f)).magnitude, "Dodge Front" + ((int)Time.time % 2 == 0 ? "2" : "")));
                dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0   ,-0.5f)).magnitude, "Dodge Back"));
                
                dodgeDictionary.Sort((x, y) => x.Key.CompareTo(y.Key));
                animator.SetBool(dodgeDictionary[0].Value, true);
            }
            else
            {
                animator.SetBool("Dodge Right", false);
                animator.SetBool("Dodge Left", false);
                animator.SetBool("Dodge Front", false);
                animator.SetBool("Dodge Front2", false);
                animator.SetBool("Dodge Back", false);
                animator.SetBool("Dodge", false);
            }
        }
    }
}
