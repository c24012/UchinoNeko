using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ScoreManager : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] GameObject poor;
    [SerializeField] GameObject fair;
    [SerializeField] GameObject good;
    [SerializeField] GameObject verygood;
    [SerializeField] GameObject excellent;
    [SerializeField] GameObject perfect;
    [SerializeField] GameObject hurtImage;
    [SerializeField] GameObject hand;
    [SerializeField] GameObject cursor;
    [SerializeField] GameObject catHand;


    [SerializeField] ScoreInfo scoreInfo;
    [SerializeField] BgmManager bgmManager;
    [SerializeField] HurtAnimationControll hurt;
    [SerializeField] Vector2 hotSpot;
    [SerializeField] Texture2D cursorHand;
    static int perfectClear = 0;

   
    int cheat;
    public int score;

    //Debug
    [SerializeField] GameObject debugPanel;
    [SerializeField] Text debugText;

    #endregion
    private void Awake()
    {
              
        Cursor.SetCursor(cursorHand, hotSpot, CursorMode.ForceSoftware);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log(scoreInfo.missCount);
        Debug.Log(scoreInfo.clearTime);

        //Debug
        debugPanel.SetActive(false);
    }

    void Start()
    {
        poor.SetActive(false);
        fair.SetActive(false);
        good.SetActive(false);
        verygood.SetActive(false);
        excellent.SetActive(false);
        perfect.SetActive(false);
        hand.SetActive(false);
        hurtImage.SetActive(true);
        catHand.SetActive(false);
        cursor.SetActive(false);


        if (scoreInfo.missCount >= 4 ||
            scoreInfo.missCount == 0 && scoreInfo.clearTime >= 90f ||
            scoreInfo.missCount == 1 && scoreInfo.clearTime >= 80f ||
            scoreInfo.missCount == 2 && scoreInfo.clearTime >= 70f ||
            scoreInfo.missCount == 3 && scoreInfo.clearTime >= 60f)
        {
            score = 1;
            perfectClear = 0;
        }

        else if (
            scoreInfo.missCount == 0 && scoreInfo.clearTime >= 70f ||
            scoreInfo.missCount == 1 && scoreInfo.clearTime >= 60f ||
            scoreInfo.missCount == 2 && scoreInfo.clearTime >= 50f ||
            scoreInfo.missCount == 3)
        {
            score = 2;
            perfectClear = 0;
        }

        else if(
            scoreInfo.missCount == 0 && scoreInfo.clearTime >= 50f ||
            scoreInfo.missCount == 1 && scoreInfo.clearTime >= 40f ||
            scoreInfo.missCount == 2)
        {
            score = 3;
            perfectClear = 0;
        }

        else if(
            scoreInfo.missCount == 0 && scoreInfo.clearTime >= 40f ||
            scoreInfo.missCount == 1)
        {
            score = 4;
            perfectClear = 0;
        }

        else if(scoreInfo.missCount == 0 &&  scoreInfo.clearTime < 40f && perfectClear < 3)
        {
            score = 5;
            perfectClear += 1;
            Debug.Log("nowclear" + perfectClear);
        }

        else if(scoreInfo.missCount == 0 && scoreInfo.clearTime < 40f)
        {
            score = 6;
            perfectClear = 0;
        }

        else
        {
            Debug.LogError("scoreの条件にひっかかりませんでした");
        }

        poor.SetActive(score == 1);
        fair.SetActive(score == 2);
        good.SetActive(score == 3);
        verygood.SetActive(score == 4);
        excellent.SetActive(score == 5);
        catHand.SetActive(score == 6);
        perfect.SetActive(score == 6);
        hand.SetActive(score == 6);
        cursor.SetActive(score < 6);

        if (score <= 5)
        {
            hurtImage.SetActive(true);
            hurt.StartCoroutine("HurtRepeat", score);
        }
        else
        {
            hurtImage.SetActive(false);
            //マウス非表示位置を固定 
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

        bgmManager.BgmStart();
    }


    void Update()
    {
        #region デバッグ用
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        score = 1;
        //        hurt.StartCoroutine("HurtRepeat", score);
        //        Debug.Log(score);
        //    }

        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        score = 2;
        //        hurt.StartCoroutine("HurtRepeat", score);
        //        Debug.Log(score);
        //    }

        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        score = 3;
        //        hurt.StartCoroutine("HurtRepeat", score);
        //        Debug.Log(score);
        //    }

        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        score = 4;
        //        hurt.StartCoroutine("HurtRepeat", score);
        //        Debug.Log(score);
        //    }

        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        score = 5;
        //        hurt.StartCoroutine("HurtRepeat", score);
        //        Debug.Log(score);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Y))
        //    {
        //        score = 6;
        //        Debug.Log(score);
        //    }


        //poor.SetActive(score == 1);
        //fair.SetActive(score == 2);
        //good.SetActive(score == 3);
        //verygood.SetActive(score == 4);
        //excellent.SetActive(score == 5);
        //perfect.SetActive(score == 6);
        //hand.SetActive(score == 6);

        //    hurt.StartCoroutine("HurtRepeat", score);
        #endregion

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            debugText.text = $"クリアタイム \n{scoreInfo.clearTime}\nミス回数 {scoreInfo.missCount}回";
            debugPanel.SetActive(true);
        } 
    }

}
