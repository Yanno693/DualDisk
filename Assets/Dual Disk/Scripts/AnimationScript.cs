using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : NetworkBehaviour
{
    private Animator animator;
    private float throwWeight;
    private float jumpWeight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        throwWeight = 0;
        jumpWeight = 0;
    }

    public void doJump() {
        GetComponent<NetworkAnimator>().SetTrigger("Jump");
        //jumpWeight = 1.0f;
        //animator.Play("Full_Jump", animator.GetLayerIndex("Jump Layer"), 0.0f);
    }

    public void doDie() {
        GetComponent<NetworkAnimator>().ResetTrigger("Spawn");
        GetComponent<NetworkAnimator>().SetTrigger("Die" + ((int)Time.time % 2 == 0 ? "2" : ""));
    }

    public void doAlive() {
        if(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Death1" || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Death2") {
            GetComponent<NetworkAnimator>().ResetTrigger("Die");
            GetComponent<NetworkAnimator>().ResetTrigger("Die2");
        }
        GetComponent<NetworkAnimator>().SetTrigger("Spawn");
    }


    public void doDodge(Vector2 currentPos) {
        List<KeyValuePair<float, string>> dodgeDictionary = new List<KeyValuePair<float, string>>();
        dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0   , 0   )).magnitude, "Dodge"));
        dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0.5f, 0   )).magnitude, "Dodge Right"));
        dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2(-0.5f, 0   )).magnitude, "Dodge Left"));
        dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0   , 0.5f)).magnitude, "Dodge Front" + ((int)Time.time % 2 == 0 ? "2" : "")));
        dodgeDictionary.Add(new KeyValuePair<float, string>((currentPos - new Vector2( 0   ,-0.5f)).magnitude, "Dodge Back"));
        
        dodgeDictionary.Sort((x, y) => x.Key.CompareTo(y.Key));
        GetComponent<NetworkAnimator>().SetTrigger(dodgeDictionary[0].Value);
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer) {
            throwWeight -= Time.deltaTime;
            jumpWeight -= Time.deltaTime;
            
            if(GetComponent<NetworkPlayerController>().serverAllowMovement && !Menu.isPaused && !GetComponent<NetworkPlayerController>().isDead) {
                animator.SetFloat("X", Input.GetAxis("Horizontal"));
                animator.SetFloat("Y", Input.GetAxis("Vertical"));

                if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3")) {
                    throwWeight = 1.0f;
                    animator.Play("Throw_Revert", animator.GetLayerIndex("Throw Layer"), 0.0f);
                }
            } else {
                animator.SetFloat("X", 0);
                animator.SetFloat("Y", 0);
            }
            
            animator.SetLayerWeight(animator.GetLayerIndex("Throw Layer"), throwWeight);
            animator.SetLayerWeight(animator.GetLayerIndex("Jump Layer"), jumpWeight);
            animator.SetBool("Fall", transform.position.y < -5);
        }
    }

    [ClientRpc]
    public void RpcTaunt() {
        if(isLocalPlayer) {
            GetComponent<NetworkAnimator>().ResetTrigger("Spawn");
            int rand = (1 + (((int)Time.time) % 20));
            Debug.Log(rand);

            GetComponent<NetworkAnimator>().SetTrigger("Taunt" + rand.ToString());
            //GetComponent<NetworkAnimator>().SetTrigger("Taunt1");
        }
    }
}
