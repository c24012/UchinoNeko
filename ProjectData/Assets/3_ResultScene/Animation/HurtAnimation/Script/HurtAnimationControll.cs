using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtAnimationControll : MonoBehaviour
{
    [SerializeField] Animator[] hurtAnim;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public IEnumerator HurtRepeat(int level)
    {
        for(int i = 0; i < level; i++)
        {
            hurtAnim[i].SetTrigger("hurtTrigger");
            yield return new WaitForSeconds(1f);
        }
       
    }

    public IEnumerator Perfect(int level)
    {
        for (int i = 0; i < level; i++)
        {
            hurtAnim[i].SetTrigger("hurtTrigger");
            yield return new WaitForSeconds(1f);
        }

    }
}
