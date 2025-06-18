using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerImage : MonoBehaviour
{
    [SerializeField] Transform sphereTf;
    [SerializeField] RectTransform rectTransform;

    //ポインタの速度倍率
    [SerializeField] float scale = 50.0f;

    private void Start()
    {

    }

    private void Update()
    {
        float x = sphereTf.position.x * scale;
        float y = sphereTf.position.z * scale;
        rectTransform.anchoredPosition = new Vector3(x , -y, 0);
    }
}
