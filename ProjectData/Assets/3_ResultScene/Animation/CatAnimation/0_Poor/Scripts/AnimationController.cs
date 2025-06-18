using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] float idleTime;
    [SerializeField] float changeAnim;

    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        idleTime = 0;
        changeAnim = Random.Range(5, 12);
        Debug.Log(changeAnim);
    }

    // Update is called once per frame
    void Update()
    {
        idleTime += Time.deltaTime;

        if(idleTime >= changeAnim)
        {
            animator.SetBool("Anim", true);
            idleTime = 0;
            changeAnim = Random.Range(5, 12);
            Debug.Log(changeAnim);
        }
        else
        {
            animator.SetBool("Anim",false);
        }

    }
}
