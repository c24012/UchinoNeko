using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test0 : MonoBehaviour
{
    public GameObject StartPoint;
    public GameObject Plane;

    // Update is called once per frame
    void Update()
    {
        var n = Plane.transform.up;
        var x = Plane.transform.position;
        var x0 = StartPoint.transform.position;
        var m = StartPoint.transform.forward;
        var h = Vector3.Dot(n, x);

        //Dot(n,m)Ç™0ÇÃèÍçáÅAë„ÇÌÇËÇ…0.0001Çë„ì¸
        float dtartPointDot = Vector3.Dot(n, m) != 0 ? Vector3.Dot(n, m) : 0.0001f;
        var intersectPoint = x0 + ((h - Vector3.Dot(n, x0)) / dtartPointDot) * m;

        transform.position = intersectPoint;
    }
}
