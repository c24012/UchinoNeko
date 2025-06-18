using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CameraAnimEvents : MonoBehaviour
{
    public bool isReadyCamera = false;

    [SerializeField] GameManager gameManager;
    [SerializeField] Animator mainCameraAnim;
    public void ReadyLastSartAnim()
    {
        isReadyCamera = true;
    }

    [SerializeField] Camera subCamera;

    public void SetToOnDepth()
    {
        subCamera.depth = 1;
    }

    public void SetToOffDepth()
    {
        subCamera.depth = -1;
    }

    public void PlayFirstMainCameraAnim()
    {
        mainCameraAnim.SetTrigger("FirstAngleTrigger");
    }

    public void FinishFirstCameraAnim()
    {
        gameManager.StartGame();
        gameManager.SetIsWaitPhase(true);
    }
}
