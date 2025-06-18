using System.Collections;
using UnityEngine;

public class CatMove : MonoBehaviour
{
    #region 変数宣言

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
    [SerializeField, Tooltip("x:最小値 y:最大値")] Vector2 catIdleToSitTime_RandomRange = Vector2.zero;

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
        RondamSetIdleToSitTime();//座るまでの時間をランダムに代入
    }

    private void Update()
    {
        if (gameManager.isGameStart)
        {
            if (isRun && isReadyRun)
            {
                //ねこ移動
                MoveVelocityToCat();
            }
        }

        //注目フラグが立っている場合、頭を猫じゃらしに向いてもらう
        if (isFollow)
        {
            //ねこの注目(頭の回転)
            CatHeadLookAtTarget();
        }

        if (!gameManager.isFailureGame)
        {
            //アイドリングアニメーション時に座るまでのタイマーカウント
            if (isIdel)
            {
                CountIdleToSitTime();
            }

            if (isJump)
            {
                JumpChangeSpeedAnimation();
            }

            //ねこの注目地点を猫じゃらしに固定させる
            lookAtObj.transform.position = catPlay.transform.position;
        }
    }

    public void CatMoveToSofa()//ねこソファーまでの移動アニメーション再生
    {
        catMoveAnim.SetTrigger("SofaTrigger");
    }

    public void SetCatMoveToSofaAnimSpeed()//ねこソファーまでの移動アニメーション再生速度指定
    {
        catAnim.SetFloat("WalkingSpeed", catRunToSofaAnimPlaySpeed);
        catAnim.SetInteger("CatState", 2);
    }

    public void SetDefaultInCatMoveSpeed()//移動アニメーション速度を元に戻す
    {
        catAnim.SetFloat("WalkingSpeed", catRunAnimPlaySpeed);
    }

    public void CatMoveAnim()//ねこ移動アニメーション遷移
    {
        isRun = true;
        catAnim.SetInteger("CatState", 2);
        catAnim.SetFloat("WalkingSpeed", catRunAnimPlaySpeed * catMoveSpeedRate);
    }

    public void CatSitAnim()//ねこ座るアニメーション遷移
    {
        if (!gameManager.isLastSpurt)
        {
            catAnim.SetInteger("CatState", 1);
        }
    }
    
    public void CatIdleAnim()//ねこ停止アニメーション遷移
    {
        isRun = false;
        catAnim.SetInteger("CatState", 0);
    }

    public void CatJumpAnim()//ねこラストジャンプアニメーション再生
    {
        isJump = true;
        catAnim.SetTrigger("JumpTrigger");
        fadeInPanel.SetActive(true);
    }

    void JumpChangeSpeedAnimation()//ねこジャンプアニメーションの再生速度と猫じゃらしの移動速度を徐々に減速
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

    public void SetIsRunFromAnimator(bool isWitch)//走るアニメーションを再生中か
    {
        if (gameManager.isRunPhase)
        {
            isReadyRun = isWitch;
        }
    }

    public void SetIsIdleFromAnimator(bool isWitch)//アイドリングアニメーションを再生中か
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
    
    void RondamSetIdleToSitTime()//座るまでの時間をランダムに代入
    {
        catIdleToSitTime = Random.Range(catIdleToSitTime_RandomRange.x, catIdleToSitTime_RandomRange.y);
    }

    void CountIdleToSitTime()//座るまでの時間をカウント
    {
        if (isIdel && gameManager.isRunPhase)
        {
            catIdleToSitTime_counter += Time.deltaTime;
            if (catIdleToSitTime_counter > catIdleToSitTime)
            {
                CatSitAnim();
                catIdleToSitTime_counter = 0;
                RondamSetIdleToSitTime();//座るまでの時間をランダムで再代入
            }
        }
    }

    //public void AccelerationCatSpeed()//ねこ速度加速
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

    //public void ResetCatSpeed()//ねこ速度減速
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

    void MoveVelocityToCat()//ねこ移動処理
    {
        switch (carAi.catMoveRoute)
        {
            case 0:
                routeMove_sofa.Move(basicCatMoveSpeed * 0.01f * catMoveSpeedRate/*べスポジ加速*/);
                break;
            default:Debug.LogError("[int:catMoveRoute] 想定外の値です");
                break;
        }
        
    }

    void CatHeadLookAtTarget()//ねこの注目(頭の回転)処理
    {
        //ターゲットまでのベクトル取得
        Vector3 relativePos = lookAtObj.position - headTf.position;
        //ベクトルを回転に変換
        Quaternion lookRotation = Quaternion.LookRotation(relativePos);
        //回転を少しずつ補完する
        headTf.rotation = Quaternion.Slerp(headTf.rotation, lookRotation, lookAtSpeed);
    }

    public void CatLookAtTargetMoveCenter()//ねこの目線を中央に移動
    {
        //ねこの注目地点を中央にする
        lookAtObj.transform.position = Camera.main.transform.position ;
    }
}
