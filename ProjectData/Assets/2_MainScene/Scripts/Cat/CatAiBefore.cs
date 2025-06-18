using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class CatAiBefore : MonoBehaviour
{
    [SerializeField] CatPlayMove catPlayMove;
    [SerializeField] GameObject catPlay;
    [SerializeField] ParticleSystem heartPrticle;
    ParticleSystem.EmissionModule heartPrticleEmi;
    [SerializeField] ParticleSystem dissPrticle;

    [SerializeField] float deadZone;
    [SerializeField] float speedLimit;  //�X�s�[�h����
    [SerializeField] float distanceLimit;�@//�ӂ蕝����
    [SerializeField] float resetTime;
    float maxPosX = 0;
    float minPosX = 0;
    float maxPosY = 0;
    float minPosY = 0;
    [SerializeField] bool isRight = false;
    [SerializeField] bool isLeft = false;
    [SerializeField] bool isUp = false;
    [SerializeField] bool isDown = false;
    float horiTimer = 0;
    float vertTimer = 0;
    float resetTimer = 0;
    float distanceX = 0;
    float distanceY = 0;

    [SerializeField] float animResetTimeX = 0;
    [SerializeField] float animResetTimeY = 0;
    float animResetTimerX = 0;
    float animResetTimerY = 0;
    [SerializeField] bool animX = false;
    [SerializeField] bool animY = false;
    [SerializeField] float addScoreTime = 0;
    float addScoreTimer = 0;
    [SerializeField] float dissPrticleTime = 0;
    float dissPrticleTimer = 0;

    [SerializeField] float catMaxSpeedX;
    [SerializeField] float catMinDistanceX;
    [SerializeField] float catMaxSpeedY;
    [SerializeField] float catMinDistanceY;

    int score;

    //Debug
    [SerializeField] Text debugText;
    [SerializeField] Text debugText2;
    [SerializeField] UnityEngine.UI.Toggle toggleX;
    [SerializeField] UnityEngine.UI.Toggle toggleY;

    private void Awake()
    {
        catMinDistanceX = Random.Range(0.5f, 2f);
        catMinDistanceY = Random.Range(0.2f, 1f);
        SwingSpeed();
        heartPrticleEmi = heartPrticle.emission;
    }

    private void Update()
    {
        //�˂������̋@�����m
        CatConditions();

        //Debug
        debugText.text = $"{score}";

        if (animX) toggleX.isOn = true;
        else toggleX.isOn = false;

        if (animY) toggleY.isOn = true;
        else toggleY.isOn = false;
    }

    public void CatPlayVelocity(Vector3 locVel, Transform pos)//�L����炵�̓������m
    {
        //x�̉����x�ƃ|�W�V�������m
        if (locVel.x > deadZone)
        {
            if (maxPosX < pos.localPosition.x)
            {
                maxPosX = pos.localPosition.x;
            }
            if (!isRight)
            {
                isRight = true;
                isLeft = false;
                distanceX = maxPosX - minPosX;
                CatJudge(horiTimer, Mathf.Abs(distanceX), 'X');
                horiTimer = 0;
                maxPosX = pos.localPosition.x;
            }
            horiTimer += Time.deltaTime;
            resetTimer = 0;
        }
        else if (locVel.x < -deadZone)
        {
            if (minPosX > pos.localPosition.x)
            {
                minPosX = pos.localPosition.x;
            }
            if (!isLeft)
            {
                isRight = false;
                isLeft = true;
                distanceX = minPosX - maxPosX;
                CatJudge(horiTimer, Mathf.Abs(distanceX), 'X');
                horiTimer = 0;
                minPosX = pos.localPosition.x;
            }
            horiTimer += Time.deltaTime;
            resetTimer = 0;
        }
        else
        {
            resetTimer += Time.deltaTime;
            if (resetTimer > resetTime)
            {
                isRight = false;
                isLeft = false;
                maxPosX = pos.localPosition.x;
                minPosX = pos.localPosition.x;
            }
        }
        //y�̉����x�ƃ|�W�V�������m
        if (locVel.y > deadZone)
        {
            if (maxPosY < pos.localPosition.y)
            {
                maxPosY = pos.localPosition.y;
            }
            if (!isUp)
            {
                isUp = true;
                isDown = false;
                distanceY = maxPosY - minPosY;
                CatJudge(horiTimer, Mathf.Abs(distanceY), 'Y');
                vertTimer = 0;
                maxPosY = pos.localPosition.y;
            }
            vertTimer += Time.deltaTime;
            resetTimer = 0;
        }
        else if (locVel.y < -deadZone)
        {
            if (minPosY > pos.localPosition.y)
            {
                minPosY = pos.localPosition.y;
            }
            if (!isDown)
            {
                isUp = false;
                isDown = true;
                distanceY = minPosY - maxPosY;
                CatJudge(horiTimer, Mathf.Abs(distanceY), 'Y');
                vertTimer = 0;
                minPosY = pos.localPosition.y;
            }
            vertTimer += Time.deltaTime;
            resetTimer = 0;
        }
        else
        {
            resetTimer += Time.deltaTime;
            if (resetTimer > resetTime)
            {
                isUp = false;
                isDown = false;
                maxPosY = pos.localPosition.y;
                minPosY = pos.localPosition.y;
            }
        }
    }

    void CatJudge(float time, float dis, char xy)
    {
        if (xy == 'X')
        {
            //speedLimit = �X�s�[�h����
            if (time >= catMaxSpeedX - speedLimit && time <= catMaxSpeedX)
            {
                if (dis >= catMinDistanceX && dis <= catMinDistanceX + distanceLimit)
                {
                    //�n�[�g�A�j���[�V�����E�D���x�㏸
                    animX = true;
                    animResetTimerX = 0;
                }
            }
        }
        if (xy == 'Y')
        {
            if (time >= catMaxSpeedY - speedLimit && time <= catMaxSpeedY)
            {
                if (dis >= catMinDistanceY && dis <= catMinDistanceY + distanceLimit)
                {
                    //�n�[�g�A�j���[�V�����E�D���x�㏸
                    animY = true;
                    animResetTimerY = 0;
                }
            }
        }
    }

    //�ӂ蕝�ɂ���ĐU��X�s�[�h�̃����_���l��ς���
    void SwingSpeed()
    {
        if (catMinDistanceX >= 0 && catMinDistanceX <= 1f)
        {
            catMaxSpeedX = Random.Range(0.1f, 0.2f);
        }
        else
        {
            catMaxSpeedX = Random.Range(0.2f, 0.3f);
        }
        //0�ȏ�P�ȉ�
        if (catMinDistanceY >= 0 && catMinDistanceY <= 0.5f)
        {
            catMaxSpeedY = Random.Range(0.1f, 0.2f);
        }
        else
        {
            catMaxSpeedY = Random.Range(0.2f, 0.3f);
        }
        //Debug.Log("�U�蕝X" + catMinDistanceX + "�U�蕝Y" + catMinDistanceY);
        //Debug.Log("���xX" + catMaxSpeedX + "���xY" + catMaxSpeedY);

        debugText2.text = $"X:��{catMinDistanceX:F2}�`{catMinDistanceX + distanceLimit:F2} ���x{((catMaxSpeedX - speedLimit) > 0 ? (catMaxSpeedX - speedLimit) : 0):F2}�`{catMaxSpeedX:F2}\n" +
            $"Y:��{catMinDistanceY:F2}�`{catMinDistanceY + distanceLimit:F2} ���x{((catMaxSpeedY - speedLimit) > 0 ? (catMaxSpeedY - speedLimit) : 0):F2}�`{catMaxSpeedY:F2}";
    }

    void CatConditions()
    {
        //�n�[�g�A�j���[�V�����̗ʎw�菈��
        if (animX || animY)
        {
            if (animX && animY)
            {
                HeartPrticleRot(2);
                AddScore(2);
            }
            else if (animX)
            {
                HeartPrticleRot(0.8f);
                AddScore(1);
                
            }
            else if (animY)
            {
                HeartPrticleRot(0.8f);
                AddScore(1);
            }
        }
        else
        {
            HeartPrticleRot(0);
        }


        //�^�C�}�[�J�E���g
        if (animX)
        {
            animResetTimerX += Time.deltaTime;

            if (animResetTimerX >= animResetTimeX)
            {
                animX = false;
            }
        }
        if (animY)
        {
            animResetTimerY += Time.deltaTime;
            if (animResetTimerY >= animResetTimeY)
            {
                animY = false;
            }
        }

        if(!(animX || animY))
        {
            dissPrticleTimer += Time.deltaTime;
            if(dissPrticleTimer >= dissPrticleTime)
            {
                dissPrticle.Play();
                dissPrticleTimer = 0;
            }
        }
        else
        {
            dissPrticleTimer = 0;
        }
    }

    /// <param name="ratio">�{��</param>
    void AddScore(int ratio)
    {
        addScoreTimer += Time.deltaTime * ratio;
        if (addScoreTimer >= addScoreTime)
        {
            score++;
            addScoreTimer = 0;
        }
    }

    //�n�[�g�p�[�e�B�N���̗ʎw��
    void HeartPrticleRot(float rot)
    {
        if (heartPrticleEmi.rateOverTimeMultiplier != rot)
        {
            heartPrticleEmi.rateOverTime = rot;
        }
    }

    public void ChangeState()
    {
        catMinDistanceX = Random.Range(0.5f, 2f);
        catMinDistanceY = Random.Range(0.2f, 1f);
        SwingSpeed();
    }
}
