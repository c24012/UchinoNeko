using System.Collections;
using UnityEngine;

public class CatMove : MonoBehaviour
{
    #region �ϐ��錾

    [SerializeField] GameManager gameManager;
    [SerializeField] CatPlayMove catPlayMove;
    [SerializeField] CatAi carAi;

    [SerializeField] Rigidbody catRb;
    [SerializeField] Animator catAnim;
    [SerializeField] Animator catMoveAnim;
    [SerializeField] Transform headTf;
    [SerializeField] GameObject catPlay;
    [SerializeField] Transform lookAtObj;
    [SerializeField] Transform failuedLookAtTf;
    [SerializeField] GameObject fadeInPanel;

    public float accelerationRate;

    [SerializeField] float lookAtSpeed = 0.1f;
    [SerializeField] float basicCatMoveSpeed = 0.5f;

    float catIdleToSitTime = 0;
    float catIdleToSitTime_counter = 0;
    [SerializeField, Tooltip("x:�ŏ��l y:�ő�l")] Vector2 catIdleToSitTime_RandomRange = Vector2.zero;

    [SerializeField] float jumpAnimTime = 0;
    float jumpAnimSpeed = 1;

    public float catMoveSpeedRate = 1;
    public bool isRun = false;
    bool isReadyRun = false;
    public bool isIdel = false;
    bool isJump = false;
    public bool isFollow = false;

    const float catRunAnimPlaySpeed = 0.3f;
    [SerializeField] float catRunToSofaAnimPlaySpeed;

    [SerializeField, Header("Debug")] RouteMoveAnimation routeMove_sofa;

    #endregion

    private void Awake()
    {
        fadeInPanel.SetActive(false);
        RondamSetIdleToSitTime();//����܂ł̎��Ԃ������_���ɑ��
    }

    private void Update()
    {
        if (gameManager.isGameStart)
        {
            if (isRun && isReadyRun)
            {
                //�˂��ړ�
                MoveVelocityToCat();
            }
        }

        //���ڃt���O�������Ă���ꍇ�A����L����炵�Ɍ����Ă��炤
        if (isFollow)
        {
            //�˂��̒���(���̉�])
            CatHeadLookAtTarget();
        }

        if (!gameManager.isFailureGame)
        {
            //�A�C�h�����O�A�j���[�V�������ɍ���܂ł̃^�C�}�[�J�E���g
            if (isIdel)
            {
                CountIdleToSitTime();
            }

            if (isJump)
            {
                JumpChangeSpeedAnimation();
            }

            //�˂��̒��ڒn�_��L����炵�ɌŒ肳����
            lookAtObj.transform.position = catPlay.transform.position;
        }
    }

    public void CatMoveToSofa()//�˂��\�t�@�[�܂ł̈ړ��A�j���[�V�����Đ�
    {
        catMoveAnim.SetTrigger("SofaTrigger");
    }

    public void SetCatMoveToSofaAnimSpeed()//�˂��\�t�@�[�܂ł̈ړ��A�j���[�V�����Đ����x�w��
    {
        catAnim.SetFloat("WalkingSpeed", catRunToSofaAnimPlaySpeed);
        catAnim.SetInteger("CatState", 2);
    }

    public void SetDefaultInCatMoveSpeed()//�ړ��A�j���[�V�������x�����ɖ߂�
    {
        catAnim.SetFloat("WalkingSpeed", catRunAnimPlaySpeed);
    }

    public void CatMoveAnim()//�˂��ړ��A�j���[�V�����J��
    {
        isRun = true;
        catAnim.SetInteger("CatState", 2);
        catAnim.SetFloat("WalkingSpeed", catRunAnimPlaySpeed * catMoveSpeedRate);
    }

    public void CatSitAnim()//�˂�����A�j���[�V�����J��
    {
        if (!gameManager.isLastSpurt)
        {
            catAnim.SetInteger("CatState", 1);
        }
    }
    
    public void CatIdleAnim()//�˂���~�A�j���[�V�����J��
    {
        isRun = false;
        catAnim.SetInteger("CatState", 0);
    }

    public void CatJumpAnim()//�˂����X�g�W�����v�A�j���[�V�����Đ�
    {
        isJump = true;
        catAnim.SetTrigger("JumpTrigger");
        fadeInPanel.SetActive(true);
    }

    void JumpChangeSpeedAnimation()//�˂��W�����v�A�j���[�V�����̍Đ����x�ƔL����炵�̈ړ����x�����X�Ɍ���
    {
        jumpAnimSpeed -= jumpAnimSpeed / jumpAnimTime * Time.deltaTime;
        if(jumpAnimSpeed < 0.01f)
        {
            jumpAnimSpeed = 0;
            isJump = false;
        }
        catAnim.SetFloat("JumpPlaySpeed", jumpAnimSpeed);
        catPlayMove.ChangeSpeedRatio(jumpAnimSpeed);
    }

    public void SetIsRunFromAnimator(bool isWitch)//����A�j���[�V�������Đ�����
    {
        if (gameManager.isRunPhase)
        {
            isReadyRun = isWitch;
        }
    }

    public void SetIsIdleFromAnimator(bool isWitch)//�A�C�h�����O�A�j���[�V�������Đ�����
    {
        if (gameManager.isRunPhase || gameManager.isWaitPhase)
        {
            isIdel = isWitch;
            if (!isWitch)
            {
                catIdleToSitTime_counter = 0;
            }
        }
    }
    
    void RondamSetIdleToSitTime()//����܂ł̎��Ԃ������_���ɑ��
    {
        catIdleToSitTime = Random.Range(catIdleToSitTime_RandomRange.x, catIdleToSitTime_RandomRange.y);
    }

    void CountIdleToSitTime()//����܂ł̎��Ԃ��J�E���g
    {
        if (isIdel && gameManager.isRunPhase)
        {
            catIdleToSitTime_counter += Time.deltaTime;
            if (catIdleToSitTime_counter > catIdleToSitTime)
            {
                CatSitAnim();
                catIdleToSitTime_counter = 0;
                RondamSetIdleToSitTime();//����܂ł̎��Ԃ������_���ōđ��
            }
        }
    }

    //public void AccelerationCatSpeed()//�˂����x����
    //{
    //    if (catMoveSpeed + Time.deltaTime * moveAcceleration < maxMoveSpeed)
    //    {
    //        catMoveSpeed += Time.deltaTime * moveAcceleration;
    //    }
    //    else
    //    {
    //        catMoveSpeed = maxMoveSpeed;
    //    }
    //}

    //public void ResetCatSpeed()//�˂����x����
    //{
    //    if (catMoveSpeed - Time.deltaTime * moveAcceleration > 1)
    //    {
    //        catMoveSpeed -= Time.deltaTime * moveAcceleration;
    //    }
    //    else
    //    {
    //        catMoveSpeed = 1;
    //    }
    //}

    void MoveVelocityToCat()//�˂��ړ�����
    {
        switch (carAi.catMoveRoute)
        {
            case 0:
                routeMove_sofa.Move(basicCatMoveSpeed * 0.01f * catMoveSpeedRate/*�׃X�|�W����*/);
                break;
            default:Debug.LogError("[int:catMoveRoute] �z��O�̒l�ł�");
                break;
        }
        
    }

    void CatHeadLookAtTarget()//�˂��̒���(���̉�])����
    {
        //�^�[�Q�b�g�܂ł̃x�N�g���擾
        Vector3 relativePos = lookAtObj.position - headTf.position;
        //�x�N�g������]�ɕϊ�
        Quaternion lookRotation = Quaternion.LookRotation(relativePos);
        //��]���������⊮����
        headTf.rotation = Quaternion.Slerp(headTf.rotation, lookRotation, lookAtSpeed);
    }

    public void CatLookAtTargetMoveCenter()//�˂��̖ڐ��𒆉��Ɉړ�
    {
        //�˂��̒��ڒn�_�𒆉��ɂ���
        lookAtObj.transform.position = Camera.main.transform.position ;
    }
}
