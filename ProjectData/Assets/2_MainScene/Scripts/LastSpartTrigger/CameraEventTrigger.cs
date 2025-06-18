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
        //�˂��ɂԂ�������..
        if (other.CompareTag("Cat") && catAi.missCounter < catAi.maxMissAllowedCount)
        {
            //�˂����~�߂�
            catAi.CatMotionStop();
            gameManager.isRunPhase = false;

            //�J�����A���O����ς��ă��X�g�X�p�[�g�˓�
            cameraMoveAnim.SetTrigger("LastAngleTrigger");
            gameManager.isLastSpurt = true;
        }
    }
}
