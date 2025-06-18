using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearFadeAnim_LoadScene : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public void FinishAnimation()
    {
        gameManager.LoadResultScene();
    }
}
