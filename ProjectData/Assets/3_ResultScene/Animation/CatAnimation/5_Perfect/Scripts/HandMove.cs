using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandMove : MonoBehaviour
{
     
    [SerializeField] float yPos;
    [SerializeField] float xPos;

    [SerializeField] float max_X;
    [SerializeField] float min_X;
    [SerializeField] int speed;
    Transform cameraTf;

    [SerializeField] Transform handTransform;
    [SerializeField] Rigidbody handRb;

    [SerializeField] GameObject perfectCat;

    [SerializeField] AudioSource catCry;
    [SerializeField] AudioClip catV;

    float startPosZ;
    float timer;
    Animator perfectAnim;
    //���W�p�̕ϐ�
    Vector3 mousePos, worldPos;
    void Start()
    {
        cameraTf = Camera.main.transform;
        startPosZ = transform.localPosition.z;

        //�}�E�X��\���ʒu���Œ�
        Cursor.visible = false;

        perfectAnim = perfectCat.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = true;
        }

        Vector3 restVector = new(handTransform.localPosition.x, yPos, startPosZ);//�ړ������p�ϐ�
        Vector3 x_Move = Input.GetAxis("Mouse X") * speed * cameraTf.right;
        //�ړ������`�F�b�N
        if (handTransform.localPosition.x > max_X && x_Move.x > 0)
        {
            restVector.x = max_X;
            x_Move.x = 0;
        }
        if (handTransform.localPosition.x < min_X && x_Move.x < 0)
        {
            restVector.x = min_X;
            x_Move.x = 0;
        }

        //����
        handTransform.localPosition = restVector;

        

        Vector3 locVel = transform.InverseTransformDirection(x_Move);
        handRb.velocity = x_Move;

        if (locVel != Vector3.zero)
        {
            timer += Time.deltaTime;
            if(timer >= 5f)
            {
                Debug.Log("hoge");
                timer = 0;
                catCry.PlayOneShot(catV);
            }
        }
    }

    
}
