using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecordController : MonoBehaviour
{
    // Start is called before the first frame update
    float w;
    Animator animator;

    public GameObject disk;

    float elapsed;
    float dissolve;
    bool trigger1 = false;
    bool trigger2 = false;
    bool trigger3 = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        w = 0;
        elapsed = 0;
        dissolve = 0f;

        //animator.SetTrigger("Die");

        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve", dissolve);
        transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Dissolve", dissolve);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        
        if(elapsed > 1.0f) {
            if(dissolve < 1.0f) {
                transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_Dissolve", dissolve);
                transform.Find("Ch44").GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_Dissolve", dissolve);
                dissolve += Time.deltaTime * 0.25f;
            }
        }
        
        
        /*elapsed += Time.deltaTime;
        //if(Input.GetKeyDown(KeyCode.A)) {
        //}
        if (elapsed > 0.8f && !trigger3) {
            trigger3 = true;
            animator.SetTrigger("Dodge Right");            
        }
        
        if (elapsed > 2.7f && !trigger1) {
            trigger1 = true;
            w = 1.0f;
            animator.Play("Throw_Revert", animator.GetLayerIndex("Throw Layer"), 0.0f);            
        }
        
        if (elapsed > 3.3f && !trigger2) {
            trigger2 = true;
            
            GameObject g = Instantiate(disk,
            transform.position + transform.forward * 0.5f + new Vector3(0, 1.2f, 0),
            transform.rotation);          
        }

        
        
        transform.position += transform.right * Time.deltaTime * 1.6f;

        animator.SetLayerWeight(1, w);
        w -= Time.deltaTime * 0.2f;*/
    }
}
