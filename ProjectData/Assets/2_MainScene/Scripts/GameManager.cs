using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    [SerializeField] Animator fadeAnim;
    [SerializeField] Animator camaraAnim;
    [SerializeField] Camera subCamera;
    [SerializeField] Animator subCamaraAnim;
    [SerializeField] Animator catMoveAnim;
    [SerializeField] CatAi catAi;
    [SerializeField] CatMove catMove;
    [SerializeField] ScoreInfo scoreInfo;
    [SerializeField] Animator blackFadeAnim;
    [SerializeField] RectTransform blackFadeCanvasRTf;
    [SerializeField] RectTransform blackFadeTargetRTf;
    [SerializeField] Transform catObj;

    [SerializeField] float gameClearTime_counter = 0;
    [SerializeField] float onIsReadyTime = 0;
    [SerializeField] float gameOverTime = 0;
    [SerializeField] float gameOverTime_counter = 0;
    public bool isReady = false;
    public bool isGameStart = false;
    public bool isWaitPhase = false;
    public bool isRunPhase = false;
    public bool isLastSpurt = false;
    public bool isGameFinish = false;
    public bool isFailureGame = false;

    

    private void Awake()
    {
        //マウス非表示位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        fadeAnim.SetTrigger("FadeOutTrigger");
        Invoke(nameof(SetOnIsReady), onIsReadyTime);
    }

    private void Update()
    {
        if (isGameStart && !isGameFinish)
        {
            gameClearTime_counter += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TitleScene");
        }

        if (isFailureGame)
        {
            if(gameOverTime_counter > -1)
            {
                gameOverTime_counter += Time.deltaTime;
                if (gameOverTime_counter > gameOverTime)
                {
                    blackFadeAnim.SetTrigger("FadeInTrigger");
                    gameOverTime_counter = -1;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            FailureGame();
            blackFadeAnim.SetTrigger("FadeInTrigger");
        }
    }

    public void SetOnIsReady()
    {
        isReady = true;
        //猫じゃらしに注目フラグを起動
        catMove.isFollow = true;
    }

    public void StartGame()
    {
        isGameStart = true;
    }

    public void SetIsWaitPhase(bool isTrue)
    {
        isWaitPhase = isTrue;
        catMove.SetIsIdleFromAnimator(true);
    }

    public void CatEnterAnim()
    {
        catMoveAnim.SetTrigger("EnterTrigger");
        Invoke(nameof(CameraZoomAnim), 2f);
    }

    public void CameraZoomAnim()
    {
        subCamaraAnim.SetTrigger("ZoomTrigger");
    }

    public void SetIsRunPhase(bool isTrue)
    {
        isRunPhase = isTrue;
    }

    public void FinishGame()
    {
        isGameFinish = true;
    }

    public void FailureGame()
    {
        isFailureGame = true;

        //猫の座標(Tranform)をカメラ座標(RectTransform)に変更
        Camera mainCamera = Camera.main;
        Vector2 newPos = Vector2.zero;

        blackFadeTargetRTf.position = RectTransformUtility.WorldToScreenPoint(mainCamera, catObj.position);
    }

    public void LoadResultScene()
    {
        scoreInfo.clearTime = gameClearTime_counter;
        scoreInfo.missCount = catAi.missCounter;
        SceneManager.LoadScene("ResultScene");
    }
}
