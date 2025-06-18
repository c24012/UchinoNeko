using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTracking : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y,10f));
        Debug.Log(pos);
        transform.position = pos;
    }
}
