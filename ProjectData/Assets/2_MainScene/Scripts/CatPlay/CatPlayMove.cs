using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CatPlayMove : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] CatAi catAi;
    [SerializeField] Transform catPlayTf;
    [SerializeField] Rigidbody catPlayRb;
    Transform cameraTf;

    [SerializeField] int speed;
    float speedRatio = 1;
    public float max_X;
    public float min_X;
    public float max_Y;
    public float min_Y;
    float startPosZ;

    [SerializeField] AudioClip[] bellSeClips;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float minMovementSize;

    private void Awake()
    {
        cameraTf = Camera.main.transform;
        startPosZ = transform.localPosition.z;
    }

    void Update()
    {
        if (gameManager.isReady)
        {
            MoveCatPlay();
        }
    }

    void MoveCatPlay()//�L����炵�̑��쏈��
    {
        //�}�E�X��x,y�ړ��ʎ擾
        Vector3 x_Move = Input.GetAxis("Mouse X") * speed * speedRatio * cameraTf.right;
        Vector3 y_Move = Input.GetAxis("Mouse Y") * speed * speedRatio * cameraTf.up;

        Vector3 restVector = new(catPlayTf.localPosition.x, catPlayTf.localPosition.y, startPosZ);//�ړ������p�ϐ�
        //�ړ������`�F�b�N
        if (catPlayTf.localPosition.x > max_X && x_Move.x > 0)
        {
            restVector.x = max_X;
            x_Move.x = 0;
        }
        if (catPlayTf.localPosition.x < min_X && x_Move.x < 0)
        {
            restVector.x = min_X;
            x_Move.x = 0;
        }

        if (catPlayTf.localPosition.y > max_Y && y_Move.y > 0)
        {
            restVector.y = max_Y;
            y_Move.y = 0;
        }
        if (catPlayTf.localPosition.y < min_Y && y_Move.y < 0)
        {
            restVector.y = min_Y;
            y_Move.y = 0;
        }
        //����
        catPlayTf.localPosition = restVector;

        catPlayTf.localEulerAngles = new Vector3(catPlayTf.localEulerAngles.x, catPlayTf.localEulerAngles.y, -catPlayTf.localPosition.x * 10);

        //�ړ����LAI�ɓ`�B
        Vector3 locVel = transform.InverseTransformDirection(x_Move + y_Move);
        catAi.CatPlayVelocity(locVel, transform);
        catPlayRb.velocity = x_Move + y_Move;
    }
    
    //����˂�����炵�̓����ɂ����Ė炷����
    public void PlayBellSeSound(float maxVelcity)
    {
        float sqrtMaxVelcity = MathF.Sqrt(maxVelcity);
        if (sqrtMaxVelcity > minMovementSize)
        {
            audioSource.volume = sqrtMaxVelcity < 30 ? sqrtMaxVelcity / 30: 1;

            //�����_���Ƀx����炷
            audioSource.PlayOneShot(bellSeClips[UnityEngine.Random.Range(0, bellSeClips.Length)]);
        }
    }

    public void ChangeSpeedRatio(float ratio)
    {
        speedRatio = ratio;
    }
}
