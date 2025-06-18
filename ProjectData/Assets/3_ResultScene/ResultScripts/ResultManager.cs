using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{

    //[SerializeField] Animator[] buttonsAnimation;
    //[SerializeField] Animation fadeAnim;
    

   

    void Start()
    {
        Application.targetFrameRate = 60;    
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    Debug.Log($"{Input.mousePosition.x}:{Input.mousePosition.y}");
        //    fadeAnim.Play();
        //}

       
    }

    public void StartButton()
    {
       
        
    }

    public void NextSceneButton()
    {
        Invoke("ChangeScene", 0.2f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    
}
