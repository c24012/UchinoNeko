using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CatAi : MonoBehaviour
{
    #region �ϐ��錾

    [SerializeField, Header("�R���|�[�l���g")] GameManager gameManager;
    [SerializeField] CatPlayMove catPlayMove;
    [SerializeField] CatMove catMove;
    [SerializeField] CameraAnimEvents cameraAnimEvents;
    [SerializeField] GameObject catPlay;
    [SerializeField] Transform funPositionTf;
    [SerializeField] ParticleSystem heartPrticle;
    [SerializeField] ParticleSystem heartPrticle_LastPart;
    [SerializeField] ParticleSystem dissPrticle;
    [SerializeField] ParticleSystem dissPrticle_miss;
    [SerializeField] ParticleSystem bestPositionPrticle;

    //�L����炵���m�p�ϐ�
    [SerializeField,Header("�ϐ�")] float deadZone;
    [SerializeField] float disResetTime;
    float maxPosX = 0;
    float minPosX = 0;
    float maxPosY = 0;
    float minPosY = 0;
    float maxVel = 0;
    bool isRight = false;
    bool isLeft = false;
    bool isUp = false;
    bool isDown = false;
    float resetTimer = 0;
    float distanceX = 0;
    float distanceY = 0;
    //float moveSpeedTimerX = 0;
    //float moveSpeedTimerY = 0;
    bool isMove = false;

    //�����ϐ�&�^�C�}�[
    bool isConditionX = false;
    bool isConditionY = false;
    [SerializeField] float conditionsResetTime = 0;
    float conditionsResetTime_counterX = 0;
    float conditionsResetTime_counterY = 0;

    //�x�X�g�|�W�V�����ϐ�
    [SerializeField] float maxScale_X;
    [SerializeField] float maxScale_Y;
    Vector2 funPositionArea_start = Vector2.zero;
    Vector2 funPositionArea_end = Vector2.zero;

    //�~�X��&�^�C�}�[
    [SerializeField] float missTime = 0;
    float missTime_counter = 0;
    public int missCounter = 0;
    public float maxMissAllowedCount;

    //WaitPhase�^�C�}�[
    [SerializeField] float waitTime = 0;
    float waitTime_counter = 0;

    //�˂������Ăяo���^�C�}�[
    [SerializeField] float catCallTime = 0;
    float catCallTime_counter = 0;

    //���G(�s�@���ɂȂ�Ȃ�)�^�C�}�[
    [SerializeField] float invincibleTime;
    float invincibleTime_counter;
    [SerializeField] bool isInvincible;

    //�C���]���^�C�}�[
    [SerializeField,Tooltip("x:�ŏ��l y:�ő�l")] Vector2 ChangeCatsTypeTime_RandomRange = Vector2.zero;
    [SerializeField] float changeCatsTypeTime = 0;
    float changeCatsTypeTime_counter = 0;

    //���X�g�X�p�[�g�^�C�}�[
    [SerializeField] float lastSpartTime = 0;
    float lastSpartTime_counter = 0;

    //�L����炵�̓���Id
    [Tooltip("-1:�Y���Ȃ� 0:�� 1:�~��"),SerializeField] int catPlayMoveType = -1;
    [SerializeField] int catPlayMoveTypeTotalCount;
    [SerializeField] int nowCatsPreferenceType = -1;
    
    //�L�̓��̂�Id
    [Tooltip("-1:�Y���Ȃ� 0:�\�t�@�[")] public int catMoveRoute = -1;
    [SerializeField] int catMoveRouteTotalCount;

    //�ӂ蕝�Œ����
    [SerializeField] float catMinDistanceX;
    [SerializeField] float catMinDistanceY;

    //�L�̖���
    [SerializeField,Header("���ʉ�")] AudioSource[] catVoiceAs;
    [SerializeField] float catMeowsCoolTime;
    float catMeowsCoolTime_counter;
    bool isMeowsCoolDown = false;
    #endregion

    private void Awake()
    {
        //�˂��̋C������
        RandomSetCatsPreferenceType();

        //���̋C�����߂܂ł̎��Ԃ������_���Ŏw��
        RandomSetChangeCatsTypeTime();

        //�˂��̋߂Â����[�g�������_���Ō��߂�
        RandomSetCatsMoveRoute();
    }

    private void Update()
    {
        if(gameManager.isGameStart)
        {
            //�L����炵�̓����̌`����
            CatJudge();

            //�˂������̋@��(�G�t�F�N�g�A�j���[�V�����Đ�)
            CatConditions();
        }

        //�^�C�}�[�J�E���g����
        TimersCount();
    }

    void RandomSetCatsPreferenceType()//�˂��̋C������
    {
        nowCatsPreferenceType = Random.Range(0, catPlayMoveTypeTotalCount);

        //�x�X�g�|�W�V�����̍X�V�������ɍs��
        RandomSetPosCatsFunPositionObj();

        //���G�t�^
        isInvincible = true;
    }

    void RandomSetChangeCatsTypeTime()//���̋C�����߂܂ł̎��Ԃ������_���Ŏw��
    {
        changeCatsTypeTime = Random.Range(ChangeCatsTypeTime_RandomRange.x, ChangeCatsTypeTime_RandomRange.y);
    }

    void RandomSetCatsMoveRoute()//�˂��̋߂Â����[�g�������_���Ō��߂�
    {
       catMoveRoute = Random.Range(0, catMoveRouteTotalCount);
    }

    void RandomSetPosCatsFunPositionObj()//�x�X�g�|�W�V�����̃����_���z�u
    {
        Vector2 startPoint = new(
            Random.Range(catPlayMove.max_X, catPlayMove.min_X + distanceX + 0.5f/*�ӂ蕝�]���Œ�ۏ�*/),
            Random.Range(catPlayMove.max_Y, catPlayMove.min_Y + distanceY + 0.5f/*�ӂ蕝�]���Œ�ۏ�*/)
            );

        Vector2 areaScale = new(
            Random.Range(catMinDistanceX + 0.5f/*�ӂ蕝�]���Œ�ۏ�*/, maxScale_X),
            Random.Range(catMinDistanceY + 0.5f/*�ӂ蕝�]���Œ�ۏ�*/, maxScale_Y)
            );

        Vector2 position = startPoint - (areaScale / 2);

        funPositionTf.localPosition = new Vector3(position.x, position.y, 3);
        funPositionTf.localScale = new Vector3(areaScale.x, areaScale.y, 0.01f);

        //�͈͂�ϐ��ɑ���擾
        CalculationForFunPositionArea(startPoint);
    }

    void CalculationForFunPositionArea(Vector2 startPos)//�x�X�g�|�W�V�����͈̔͂��擾
    {
        //�͈͂̊J�n�n�_(�ő�_)��vec2�ŕۑ�
        funPositionArea_start = startPos;

        //�͈͂̏I���n�_(�ŏ��_)��vec2�ŕۑ�
        funPositionArea_end = new Vector2(
            (funPositionTf.localPosition.x - (funPositionTf.localScale.x / 2)),
            (funPositionTf.localPosition.y - (funPositionTf.localScale.y / 2))
            );
    }

    public void CatPlayVelocity(Vector3 locVel, Transform pos)//�L����炵�̓������m
    {
        //�L����炵�������Ă��邩
        isMove = (locVel != Vector3.zero);

        //x�̑��x�ƃ|�W�V�������m
        if (locVel.x > deadZone)//�E�ɐU���Ă���
        {
            //�ő�|�W�V�����L�^
            if (maxPosX < pos.localPosition.x)
            {
                maxPosX = pos.localPosition.x;
            }
            //�ō����x�L�^
            if (maxVel < Vector3.SqrMagnitude(locVel))
            {
                maxVel = Vector3.SqrMagnitude(locVel);
            }
            if (!isRight)//�܂�Ԃ�1F��
            {
                isRight = true;
                isLeft = false;
                distanceX = maxPosX - minPosX;
                ConditionsChack(Mathf.Abs(distanceX), 'X');
                maxPosX = pos.localPosition.x;

                //���炷
                catPlayMove.PlayBellSeSound(maxVel);
                maxVel = 0;
            }
            resetTimer = 0;
        }
        else if (locVel.x < -deadZone)//���ɐU���Ă���
        {
            if (minPosX > pos.localPosition.x)
            {
                //�ŏ��|�W�V�����L�^
                minPosX = pos.localPosition.x;
            }
            //�ō����x�L�^
            if (maxVel < Vector3.SqrMagnitude(locVel))
            {
                maxVel = Vector3.SqrMagnitude(locVel);
            }
            if (!isLeft)//�܂�Ԃ�1F��
            {
                isRight = false;
                isLeft = true;
                distanceX = minPosX - maxPosX;
                ConditionsChack(Mathf.Abs(distanceX), 'X');
                minPosX = pos.localPosition.x;

                //���炷
                catPlayMove.PlayBellSeSound(maxVel);
                maxVel = 0;
            }
            resetTimer = 0;
        }
        else
        {
            resetTimer += Time.deltaTime;
            if (resetTimer > disResetTime)
            {
                isRight = false;
                isLeft = false;
                maxPosX = pos.localPosition.x;
                minPosX = pos.localPosition.x;
            }
        }

        //y�̑��x�ƃ|�W�V�������m
        if (locVel.y > deadZone)//��ɐU���Ă���
        {
            if (maxPosY < pos.localPosition.y)
            {
                //�ő�|�W�V�����L�^
                maxPosY = pos.localPosition.y;
            }
            if (!isUp)//�܂�Ԃ�1F��
            {
                isUp = true;
                isDown = false;
                distanceY = maxPosY - minPosY;
                ConditionsChack(Mathf.Abs(distanceY), 'Y');
                maxPosY = pos.localPosition.y;
            }
            resetTimer = 0;
        }
        else if (locVel.y < -deadZone)//���ɐU���Ă���
        {
            if (minPosY > pos.localPosition.y)
            {
                //�ŏ��|�W�V�����L�^
                minPosY = pos.localPosition.y;
            }
            if (!isDown)//�܂�Ԃ�1F��
            {
                isUp = false;
                isDown = true;
                distanceY = minPosY - maxPosY;
                ConditionsChack(Mathf.Abs(distanceY), 'Y');
                minPosY = pos.localPosition.y;
            }
            resetTimer = 0;
        }
        else
        {
            resetTimer += Time.deltaTime;
            if (resetTimer > disResetTime)
            {
                isUp = false;
                isDown = false;
                maxPosY = pos.localPosition.y;
                minPosY = pos.localPosition.y;
            }
        }

        if (gameManager.isRunPhase)
        {
            //�x�X�g�|�W�V�����ɂ͂����Ă��邩
            if (pos.localPosition.x < funPositionArea_start.x && pos.localPosition.y < funPositionArea_start.y
                && pos.localPosition.x > funPositionArea_end.x && pos.localPosition.y > funPositionArea_end.y
                 && isMove)
            {
                //�����{�[�i�X
                catMove.catMoveSpeedRate = catMove.accelerationRate;
                if (!bestPositionPrticle.isPlaying)
                {
                    //�G�t�F�N�g�Đ�
                    bestPositionPrticle.Play();
                }
                if (!isMeowsCoolDown)//�����̃N�[���^�C���t���O���~��Ă�������Ă��炤
                {
                    catVoiceAs[Random.Range(0, catVoiceAs.Length)].Play();
                    isMeowsCoolDown = true;
                }
            }
            else
            {
                //���Ƃɖ߂�
                catMove.catMoveSpeedRate = 1;
                if (bestPositionPrticle.isPlaying)
                {
                    //�G�t�F�N�g��~
                    bestPositionPrticle.Stop();
                }
            }
        }
    }

    void ConditionsChack(float dis, char xy)//�L����炵�̓�������
    {
        if (xy == 'X')
        {
            if (dis >= catMinDistanceX)
            {
                isConditionX = true;
                conditionsResetTime_counterX = 0;
            }
        }
        if (xy == 'Y')
        {
            if (dis >= catMinDistanceY)
            {
                isConditionY = true;
                conditionsResetTime_counterY = 0;
            }
        }
    }

    void CatJudge()//�L����炵�̓����̌`����
    {
        if (isConditionX && isConditionY)
        {
            catPlayMoveType = 1;
        }
        else if (isConditionX)
        {
            catPlayMoveType = 0;
        }
        else if (isConditionY)
        {
            //���ɂȂ�
        }
        else
        {
            catPlayMoveType = -1;
        }
    }

    void CatConditions()//���̓����������ɍ��������m������
    {
        if (!gameManager.isLastSpurt && !gameManager.isFailureGame)
        {
            if (nowCatsPreferenceType == catPlayMoveType)
            {
                if (gameManager.isWaitPhase)//�҂��t�F�[�Y
                {
                    //��莞�Ԉȏ�����������Ύ��̃t�F�[�Y��
                    waitTime_counter += Time.deltaTime;
                    if(waitTime_counter > waitTime)
                    {
                        gameManager.SetIsWaitPhase(false);
                        catMove.CatMoveToSofa();
                    }
                }
                if (gameManager.isRunPhase)//�i�ރt�F�[�Y
                {
                    //�˂��i��
                    catMove.CatMoveAnim();
                }
                
                //�G�t�F�N�g����
                if (!heartPrticle.isPlaying) heartPrticle.Play();
                missTime_counter = 0;
            }
            else
            {
                if (gameManager.isWaitPhase)//�҂��t�F�[�Y
                {
                    if (isInvincible)
                    {
                        //���G��..
                    }
                    else
                    {
                        missTime_counter += Time.deltaTime;
                        if (missTime_counter >= missTime)
                        {
                            missCounter++;
                            missTime_counter = 0;
                            MissCountCheck();
                        }
                    }
                }
                if (gameManager.isRunPhase)//�i�ރt�F�[�Y
                {
                    //�˂��~�܂�
                    if (catMove.isRun)
                    {
                        catMove.CatIdleAnim();
                    }

                    if (isInvincible)
                    {
                        //���G��..
                    }
                    else
                    {
                        missTime_counter += Time.deltaTime;
                        if (missTime_counter >= missTime)
                        {
                            missCounter++;
                            missTime_counter = 0;
                            MissCountCheck();
                        }
                    }
                }
                    
                //�G�t�F�N�g����
                if (heartPrticle.isPlaying) heartPrticle.Stop();
                
            }
        }
        //���X�g�X�p�[�g���̏���
        else if(gameManager.isLastSpurt)
        {
            if (isMove && cameraAnimEvents.isReadyCamera)
            {
                if (!heartPrticle_LastPart.isPlaying) heartPrticle_LastPart.Play();
            }
            else
            {
                if (heartPrticle_LastPart.isPlaying) heartPrticle_LastPart.Stop();
            }
        }
        //���s���̏���
        else if (gameManager.isFailureGame)
        {
            //���ɂȂ�
        }
    }

    void MissCountCheck()//�Q�[���I�[�o�[�ɂȂ邩����
    {
        if(missCounter > maxMissAllowedCount)
        {
            dissPrticle_miss.Play();
            gameManager.FailureGame();
            catMove.CatLookAtTargetMoveCenter();
        }
        else
        {
            dissPrticle.Play();
        }
    }

    public void CatMotionStop()//�L�̓����S��~
    {
        // �˂��~�܂�
        catMove.CatIdleAnim();
        //�G�t�F�N�g����
        if (heartPrticle.isPlaying) heartPrticle.Stop();
    }

    void TimersCount()//�^�C�}�[�J�E���g����
    {
        if (gameManager.isGameStart)
        {
            //�L����炵��XY���m�^�C�}�[
            if (isConditionX)
            {
                conditionsResetTime_counterX += Time.deltaTime;
                if (conditionsResetTime_counterX > conditionsResetTime)
                {
                    isConditionX = false;
                    conditionsResetTime_counterX = 0;
                }
            }
            if (isConditionY)
            {
                conditionsResetTime_counterY += Time.deltaTime;
                if (conditionsResetTime_counterY > conditionsResetTime)
                {
                    isConditionY = false;
                    conditionsResetTime_counterY = 0;
                }
            }

            //�L�̋C�����߃^�C�}�[
            if (!gameManager.isLastSpurt)
            {
                changeCatsTypeTime_counter += Time.deltaTime;
                if (changeCatsTypeTime_counter > changeCatsTypeTime)
                {
                    RandomSetCatsPreferenceType();//�˂��̋C����ς���
                    RandomSetChangeCatsTypeTime();//���̋C���ւ��܂ł̎��Ԃ��Z�b�g
                    changeCatsTypeTime_counter = 0;
                }
            }

            //���G���ԃ^�C�}�[
            if (isInvincible)//���G���L�����Ɏ��s
            {
                invincibleTime_counter += Time.deltaTime;
                if (invincibleTime_counter > invincibleTime)
                {
                    isInvincible = false;
                    invincibleTime_counter = 0;
                }
            }

            //���X�g�X�p�[�g�^�C�}�[
            if (gameManager.isLastSpurt)
            {
                lastSpartTime_counter += Time.deltaTime;
                if (lastSpartTime_counter > lastSpartTime)
                {
                    catMove.CatJumpAnim();
                    lastSpartTime_counter = -100f;
                }
            }
        }

        //�Q�[�����X�^�[�g����O�̒i�K
        if (gameManager.isReady && !gameManager.isGameStart)
        {
            if (isMove)//�˂�����炵�������Ă���Ƃ�
            {
                if(catCallTime_counter >= 0)
                {
                    catCallTime_counter += Time.deltaTime;
                    if (catCallTime_counter > catCallTime)
                    {
                        gameManager.CatEnterAnim();
                        catCallTime_counter = -1;
                    }
                }
            }
        }

        if (gameManager.isRunPhase)
        {
            if (isMeowsCoolDown)//�˂��̖������N�[���^�C��
            {
                catMeowsCoolTime_counter += Time.deltaTime;
                if(catMeowsCoolTime_counter > catMeowsCoolTime)
                {
                    isMeowsCoolDown = false;
                    catMeowsCoolTime_counter = 0;
                }
            }
        }
    }
}