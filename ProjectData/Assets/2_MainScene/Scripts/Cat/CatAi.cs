using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CatAi : MonoBehaviour
{
    #region 変数宣言

    [SerializeField, Header("コンポーネント")] GameManager gameManager;
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

    //猫じゃらし検知用変数
    [SerializeField,Header("変数")] float deadZone;
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

    //条件変数&タイマー
    bool isConditionX = false;
    bool isConditionY = false;
    [SerializeField] float conditionsResetTime = 0;
    float conditionsResetTime_counterX = 0;
    float conditionsResetTime_counterY = 0;

    //ベストポジション変数
    [SerializeField] float maxScale_X;
    [SerializeField] float maxScale_Y;
    Vector2 funPositionArea_start = Vector2.zero;
    Vector2 funPositionArea_end = Vector2.zero;

    //ミス回数&タイマー
    [SerializeField] float missTime = 0;
    float missTime_counter = 0;
    public int missCounter = 0;
    public float maxMissAllowedCount;

    //WaitPhaseタイマー
    [SerializeField] float waitTime = 0;
    float waitTime_counter = 0;

    //ねこちゃん呼び出しタイマー
    [SerializeField] float catCallTime = 0;
    float catCallTime_counter = 0;

    //無敵(不機嫌にならない)タイマー
    [SerializeField] float invincibleTime;
    float invincibleTime_counter;
    [SerializeField] bool isInvincible;

    //気分転換タイマー
    [SerializeField,Tooltip("x:最小値 y:最大値")] Vector2 ChangeCatsTypeTime_RandomRange = Vector2.zero;
    [SerializeField] float changeCatsTypeTime = 0;
    float changeCatsTypeTime_counter = 0;

    //ラストスパートタイマー
    [SerializeField] float lastSpartTime = 0;
    float lastSpartTime_counter = 0;

    //猫じゃらしの動きId
    [Tooltip("-1:該当なし 0:横 1:円状"),SerializeField] int catPlayMoveType = -1;
    [SerializeField] int catPlayMoveTypeTotalCount;
    [SerializeField] int nowCatsPreferenceType = -1;
    
    //猫の道のりId
    [Tooltip("-1:該当なし 0:ソファー")] public int catMoveRoute = -1;
    [SerializeField] int catMoveRouteTotalCount;

    //ふり幅最低条件
    [SerializeField] float catMinDistanceX;
    [SerializeField] float catMinDistanceY;

    //猫の鳴き声
    [SerializeField,Header("効果音")] AudioSource[] catVoiceAs;
    [SerializeField] float catMeowsCoolTime;
    float catMeowsCoolTime_counter;
    bool isMeowsCoolDown = false;
    #endregion

    private void Awake()
    {
        //ねこの気分決め
        RandomSetCatsPreferenceType();

        //次の気分決めまでの時間をランダムで指定
        RandomSetChangeCatsTypeTime();

        //ねこの近づくルートをランダムで決める
        RandomSetCatsMoveRoute();
    }

    private void Update()
    {
        if(gameManager.isGameStart)
        {
            //猫じゃらしの動きの形判定
            CatJudge();

            //ねこちゃんの機嫌(エフェクトアニメーション再生)
            CatConditions();
        }

        //タイマーカウント処理
        TimersCount();
    }

    void RandomSetCatsPreferenceType()//ねこの気分決め
    {
        nowCatsPreferenceType = Random.Range(0, catPlayMoveTypeTotalCount);

        //ベストポジションの更新も同時に行う
        RandomSetPosCatsFunPositionObj();

        //無敵付与
        isInvincible = true;
    }

    void RandomSetChangeCatsTypeTime()//次の気分決めまでの時間をランダムで指定
    {
        changeCatsTypeTime = Random.Range(ChangeCatsTypeTime_RandomRange.x, ChangeCatsTypeTime_RandomRange.y);
    }

    void RandomSetCatsMoveRoute()//ねこの近づくルートをランダムで決める
    {
       catMoveRoute = Random.Range(0, catMoveRouteTotalCount);
    }

    void RandomSetPosCatsFunPositionObj()//ベストポジションのランダム配置
    {
        Vector2 startPoint = new(
            Random.Range(catPlayMove.max_X, catPlayMove.min_X + distanceX + 0.5f/*ふり幅余白最低保証*/),
            Random.Range(catPlayMove.max_Y, catPlayMove.min_Y + distanceY + 0.5f/*ふり幅余白最低保証*/)
            );

        Vector2 areaScale = new(
            Random.Range(catMinDistanceX + 0.5f/*ふり幅余白最低保証*/, maxScale_X),
            Random.Range(catMinDistanceY + 0.5f/*ふり幅余白最低保証*/, maxScale_Y)
            );

        Vector2 position = startPoint - (areaScale / 2);

        funPositionTf.localPosition = new Vector3(position.x, position.y, 3);
        funPositionTf.localScale = new Vector3(areaScale.x, areaScale.y, 0.01f);

        //範囲を変数に代入取得
        CalculationForFunPositionArea(startPoint);
    }

    void CalculationForFunPositionArea(Vector2 startPos)//ベストポジションの範囲を取得
    {
        //範囲の開始地点(最大点)をvec2で保存
        funPositionArea_start = startPos;

        //範囲の終了地点(最小点)をvec2で保存
        funPositionArea_end = new Vector2(
            (funPositionTf.localPosition.x - (funPositionTf.localScale.x / 2)),
            (funPositionTf.localPosition.y - (funPositionTf.localScale.y / 2))
            );
    }

    public void CatPlayVelocity(Vector3 locVel, Transform pos)//猫じゃらしの動き検知
    {
        //猫じゃらしが動いているか
        isMove = (locVel != Vector3.zero);

        //xの速度とポジション検知
        if (locVel.x > deadZone)//右に振っている
        {
            //最大ポジション記録
            if (maxPosX < pos.localPosition.x)
            {
                maxPosX = pos.localPosition.x;
            }
            //最高速度記録
            if (maxVel < Vector3.SqrMagnitude(locVel))
            {
                maxVel = Vector3.SqrMagnitude(locVel);
            }
            if (!isRight)//折り返し1F目
            {
                isRight = true;
                isLeft = false;
                distanceX = maxPosX - minPosX;
                ConditionsChack(Mathf.Abs(distanceX), 'X');
                maxPosX = pos.localPosition.x;

                //鈴を鳴らす
                catPlayMove.PlayBellSeSound(maxVel);
                maxVel = 0;
            }
            resetTimer = 0;
        }
        else if (locVel.x < -deadZone)//左に振っている
        {
            if (minPosX > pos.localPosition.x)
            {
                //最小ポジション記録
                minPosX = pos.localPosition.x;
            }
            //最高速度記録
            if (maxVel < Vector3.SqrMagnitude(locVel))
            {
                maxVel = Vector3.SqrMagnitude(locVel);
            }
            if (!isLeft)//折り返し1F目
            {
                isRight = false;
                isLeft = true;
                distanceX = minPosX - maxPosX;
                ConditionsChack(Mathf.Abs(distanceX), 'X');
                minPosX = pos.localPosition.x;

                //鈴を鳴らす
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

        //yの速度とポジション検知
        if (locVel.y > deadZone)//上に振っている
        {
            if (maxPosY < pos.localPosition.y)
            {
                //最大ポジション記録
                maxPosY = pos.localPosition.y;
            }
            if (!isUp)//折り返し1F目
            {
                isUp = true;
                isDown = false;
                distanceY = maxPosY - minPosY;
                ConditionsChack(Mathf.Abs(distanceY), 'Y');
                maxPosY = pos.localPosition.y;
            }
            resetTimer = 0;
        }
        else if (locVel.y < -deadZone)//下に振っている
        {
            if (minPosY > pos.localPosition.y)
            {
                //最小ポジション記録
                minPosY = pos.localPosition.y;
            }
            if (!isDown)//折り返し1F目
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
            //ベストポジションにはいっているか
            if (pos.localPosition.x < funPositionArea_start.x && pos.localPosition.y < funPositionArea_start.y
                && pos.localPosition.x > funPositionArea_end.x && pos.localPosition.y > funPositionArea_end.y
                 && isMove)
            {
                //加速ボーナス
                catMove.catMoveSpeedRate = catMove.accelerationRate;
                if (!bestPositionPrticle.isPlaying)
                {
                    //エフェクト再生
                    bestPositionPrticle.Play();
                }
                if (!isMeowsCoolDown)//鳴き声のクールタイムフラグが降りていたら鳴いてもらう
                {
                    catVoiceAs[Random.Range(0, catVoiceAs.Length)].Play();
                    isMeowsCoolDown = true;
                }
            }
            else
            {
                //もとに戻す
                catMove.catMoveSpeedRate = 1;
                if (bestPositionPrticle.isPlaying)
                {
                    //エフェクト停止
                    bestPositionPrticle.Stop();
                }
            }
        }
    }

    void ConditionsChack(float dis, char xy)//猫じゃらしの動き判定
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

    void CatJudge()//猫じゃらしの動きの形判定
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
            //特になし
        }
        else
        {
            catPlayMoveType = -1;
        }
    }

    void CatConditions()//今の動きが条件に合うか検知＆処理
    {
        if (!gameManager.isLastSpurt && !gameManager.isFailureGame)
        {
            if (nowCatsPreferenceType == catPlayMoveType)
            {
                if (gameManager.isWaitPhase)//待ちフェーズ
                {
                    //一定時間以上条件が合えば次のフェーズへ
                    waitTime_counter += Time.deltaTime;
                    if(waitTime_counter > waitTime)
                    {
                        gameManager.SetIsWaitPhase(false);
                        catMove.CatMoveToSofa();
                    }
                }
                if (gameManager.isRunPhase)//進むフェーズ
                {
                    //ねこ進む
                    catMove.CatMoveAnim();
                }
                
                //エフェクト処理
                if (!heartPrticle.isPlaying) heartPrticle.Play();
                missTime_counter = 0;
            }
            else
            {
                if (gameManager.isWaitPhase)//待ちフェーズ
                {
                    if (isInvincible)
                    {
                        //無敵中..
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
                if (gameManager.isRunPhase)//進むフェーズ
                {
                    //ねこ止まる
                    if (catMove.isRun)
                    {
                        catMove.CatIdleAnim();
                    }

                    if (isInvincible)
                    {
                        //無敵中..
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
                    
                //エフェクト処理
                if (heartPrticle.isPlaying) heartPrticle.Stop();
                
            }
        }
        //ラストスパート時の処理
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
        //失敗時の処理
        else if (gameManager.isFailureGame)
        {
            //特になし
        }
    }

    void MissCountCheck()//ゲームオーバーになるか判定
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

    public void CatMotionStop()//猫の動き全停止
    {
        // ねこ止まる
        catMove.CatIdleAnim();
        //エフェクト処理
        if (heartPrticle.isPlaying) heartPrticle.Stop();
    }

    void TimersCount()//タイマーカウント処理
    {
        if (gameManager.isGameStart)
        {
            //猫じゃらしのXY検知タイマー
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

            //猫の気分決めタイマー
            if (!gameManager.isLastSpurt)
            {
                changeCatsTypeTime_counter += Time.deltaTime;
                if (changeCatsTypeTime_counter > changeCatsTypeTime)
                {
                    RandomSetCatsPreferenceType();//ねこの気分を変える
                    RandomSetChangeCatsTypeTime();//次の気分替えまでの時間をセット
                    changeCatsTypeTime_counter = 0;
                }
            }

            //無敵時間タイマー
            if (isInvincible)//無敵が有効時に実行
            {
                invincibleTime_counter += Time.deltaTime;
                if (invincibleTime_counter > invincibleTime)
                {
                    isInvincible = false;
                    invincibleTime_counter = 0;
                }
            }

            //ラストスパートタイマー
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

        //ゲームがスタートする前の段階
        if (gameManager.isReady && !gameManager.isGameStart)
        {
            if (isMove)//ねこじゃらしが動いているとき
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
            if (isMeowsCoolDown)//ねこの鳴きごえクールタイム
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