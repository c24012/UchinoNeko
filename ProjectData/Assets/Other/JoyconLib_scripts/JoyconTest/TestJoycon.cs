using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJoycon : MonoBehaviour
{
    private List<Joycon> joycons;

    public int jc_ind = 0;
    public Quaternion orientation;

    void Start()
    {
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // make sure the Joycon only gets checked if attached
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            // Bボタンでセンター位置のリセット
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                j.Recenter();
            }

            orientation = j.GetVector();
            gameObject.transform.rotation = orientation;
        }
    }
}
