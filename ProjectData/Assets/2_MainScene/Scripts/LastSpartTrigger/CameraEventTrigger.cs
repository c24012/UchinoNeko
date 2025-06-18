using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CameraEventTrigger : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Animator cameraMoveAnim;
    [SerializeField] CatAi catAi;

    private void OnTriggerEnter(Collider other)
    {
        //ねこにぶつかったら..
        if (other.CompareTag("Cat") && catAi.missCounter < catAi.maxMissAllowedCount)
        {
            //ねこを止めて
            catAi.CatMotionStop();
            gameManager.isRunPhase = false;

            //カメラアングルを変えてラストスパート突入
            cameraMoveAnim.SetTrigger("LastAngleTrigger");
            gameManager.isLastSpurt = true;
        }
    }
}
