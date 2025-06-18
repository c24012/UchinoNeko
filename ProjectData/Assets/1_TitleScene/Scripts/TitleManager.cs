using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Animator[] buttonsAnimation;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] Animator fadeAnim;
    [SerializeField] GameObject buttonsPanel;
    [SerializeField] GameObject startButton;
    [SerializeField,Tooltip("ボタンを押してからシーン遷移するまでの待機時間")] float sceneTransitionTime;

    private void Awake()
    {
        //マウス非表示位置を固定
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Application.targetFrameRate = 60;

        tutorialPanel.SetActive(false);
        buttonsPanel.SetActive(false);
    }

    void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            if (tutorialPanel.activeSelf)
            {
                tutorialPanel.SetActive(false);
            }
        }
    }

    public void StartButton()
    {
        buttonsPanel.SetActive(true);
        startButton.SetActive(false);
    }

    public void NextSceneButton()
    {
        fadeAnim.SetTrigger("FadeInTrigger");
        Invoke(((Action)ToMainScene).Method.Name, sceneTransitionTime);
    }

    public void TutorialButton()
    {
        tutorialPanel.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    void ToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
