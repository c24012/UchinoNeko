using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResultCursor : MonoBehaviour
{

    [SerializeField] Texture2D cursorHand;
    [SerializeField] Vector2 hotSpot;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] int speed;
    [SerializeField] GameObject[] cat;
    [SerializeField] GameObject catChange;
  
    Animator animMg; //�A�j���[�V�����}�l�[�W���[
    Animator change;
    Vector3 mousePos,pos;
    Transform cameraTf;
    float timer;
    int animationNum;

    // Start is called before the first frame update
    void Start()
    {
        animMg = cat[0].GetComponent<Animator>();
        //�N�����Ƀ}�E�X�A�C�R����ύX
        
        if (scoreManager.score == 3)
        {
            animMg = cat[0].GetComponent<Animator>();
        }
        if (scoreManager.score == 4)
        {
            animMg = cat[1].GetComponent<Animator>();
            change = catChange.GetComponent<Animator>();
        }
        if (scoreManager.score == 5)
        {
            animMg = cat[2].GetComponent<Animator>();
        }
        cameraTf = Camera.main.transform;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //�}�E�X�̍��W���擾����
        mousePos = Input.mousePosition;
        //�X�N���[�����W�����[���h���W�ɕϊ�����
        pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 30f));
        //���[���h���W���Q�[���I�u�W�F�N�g�̍��W�ɐݒ肷��
        transform.position = pos;

        //�����ǉ��\��
       if(animationNum == 1)
        {
           
        }
    }

    private void OnTriggerStay2D(Collider2D collision) 
    {
        Vector3 x_Move = Input.GetAxis("Mouse X") * cameraTf.right;
        Vector3 y_Move = Input.GetAxis("Mouse Y") * cameraTf.up;
        Vector3 locVel = transform.InverseTransformDirection(x_Move + y_Move);

        //�R���C�_�[�̒��œ����Ă���Ƃ��̏���
        if (collision.CompareTag("Good"))
        {
            animMg.SetBool("goodCat", true);   
        }

        if (collision.CompareTag("Bad"))
        {
            animMg.SetBool("badCat", true);
        }

        if (collision.CompareTag("VeryGood") && cat[1].activeSelf)
        {
            Debug.Log("dekiteru");
            animMg.SetBool("Normal", true);
            if (locVel == Vector3.zero)
            {
                if (timer >= 0)
                {
                    Debug.Log("tugi"+timer);
                    timer += Time.deltaTime;
                    if (timer >= 5)
                    {
                        cat[1].SetActive(false);
                        catChange.SetActive(true);
                        change.SetBool("Change", true);
                        animMg = cat[2].GetComponent<Animator>();
                        Debug.Log("dekiteru");
                        timer = -1;
                    }
                }
            }         
        }
        if (collision.CompareTag("VeryGood") && catChange.activeSelf)
        {
            change.SetBool("VeryIdle", true);
        }

        if (collision.CompareTag("Excellent"))
        {
            //Debug.Log("�G�N�Z�����g");
            if(timer >= 0)
            {
                if (locVel != Vector3.zero)
                {
                    timer += Time.deltaTime;
                    if (timer >= 3f /*Random.Range(3f, 7f)*/)
                    {
                        animMg.SetInteger("SleepTime" , animMg.GetInteger("SleepTime") +1);
                        if(animMg.GetInteger("SleepTime") == 2)
                        {
                            timer = -1;
                        }
                        else
                        {
                            timer = 0;
                        }
                    }
                   
                }
            }           
        }
    }

    
    
    //�A�C�h����Ԃɖ߂��悤
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Good"))
        {
            animMg.SetBool("goodCat", false);
        }

        if (collision.CompareTag("Bad"))
        {
            animMg.SetBool("badCat", false);
        }
        if (collision.CompareTag("VeryGood") && catChange.activeSelf)
        {
            change.SetBool("VeryIdle", false);
            change.SetBool("Change", false);
        }
        if (collision.CompareTag("VeryGood") && cat[1].activeSelf)
        {
            animMg.SetBool("Normal", false);
        }
        if (collision.CompareTag("Excellent"))
        {
            timer = 0;
        }
    }
}
