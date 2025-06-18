using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVeocity : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;
    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(rb2d.velocity);
    }
}
