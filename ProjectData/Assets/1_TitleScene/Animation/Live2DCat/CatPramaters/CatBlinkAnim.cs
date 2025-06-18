using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBlinkAnim : MonoBehaviour
{
    [SerializeField] CubismModel catModel;
    [SerializeField] float breathingSpeed = 0.1f;
    [SerializeField] float breathNowValue = 0;

    bool isOpen = false;

    [SerializeField]bool isBreathing = false;

    private void LateUpdate()
    {
        if (isOpen)
        {
            catModel.Parameters[3].Value = 1;
        }
        else
        {
            catModel.Parameters[3].Value = 0;
        }

        if (isBreathing)
        {
            if((breathNowValue + Time.deltaTime * breathingSpeed) < 1)
            {
                breathNowValue += Time.deltaTime * breathingSpeed;
                catModel.Parameters[20].Value = breathNowValue;
            }
            else
            {
                catModel.Parameters[20].Value = 1;
                breathNowValue = 1;
                isBreathing = false;
            }
        }
        else
        {
            if ((breathNowValue - Time.deltaTime * breathingSpeed) > 0)
            {
                breathNowValue -= Time.deltaTime * breathingSpeed;
                catModel.Parameters[20].Value = breathNowValue;
            }
            else
            {
                catModel.Parameters[20].Value = 0;
                breathNowValue = 0;
                isBreathing = true;
            }
        }
    }

    public void OpenBrow()
    {
        isOpen = true;
    }

    public void CloseBrow()
    {
        isOpen = false;
    }

    public void StartBreathing()
    {
        isBreathing = true;
    }
}