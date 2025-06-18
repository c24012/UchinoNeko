using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoinerManager : MonoBehaviour
{
    private List<Joycon> joycons;
    public Vector3 gyro;
    public Vector3 accel;
    public int jc_ind = 0;
    public Quaternion orientation;


    /*
     * Recenter();回転リセット
     * SetRumble(160, 320, 0.6f, 200);Joycon振動
     */

    void Start()
    {
        gyro = new Vector3(0, 0, 0);
        accel = new Vector3(0, 0, 0);
        joycons = JoyconManager.Instance.j;
        if (joycons.Count < jc_ind + 1)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (joycons.Count > 0)
        {
            Joycon j = joycons[jc_ind];

            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                //Bボタンで回転リセット
                transform.localPosition = new Vector3(0,0,3f);
            }

            gyro = j.GetGyro();

            transform.localPosition += new Vector3(gyro.z, gyro.y, 0) * 0.05f;
            //transform.localPosition += new Vector3(
            //    Screen.width / 2 - Camera.main.WorldToScreenPoint(transform.position).x,
            //    Screen.height / 4 - Camera.main.WorldToScreenPoint(transform.position).y,
            //    0
            //    ) * 0.00005f;
        }
    }
}
