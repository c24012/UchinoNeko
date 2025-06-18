using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using UnityEngine;

public class CatAnimControllerEvents : MonoBehaviour
{
    [SerializeField] CatMove catMove;
    [SerializeField] GameManager gameManager;

    public void ReadyToRun()
    {
        catMove.SetIsRunFromAnimator(true);
    }

    public void NotReadyToRun()
    {
        catMove.SetIsRunFromAnimator(false);
    }

    public void IsIdleAnimation()
    {
        catMove.SetIsIdleFromAnimator(true);
    }

    public void NotIdleAnimation()
    {
        catMove.SetIsIdleFromAnimator(false);
    }

    public void FinishMainGame()
    {
        gameManager.FinishGame();
    }
}
