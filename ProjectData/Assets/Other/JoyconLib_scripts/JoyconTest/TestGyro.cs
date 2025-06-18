using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGyro : MonoBehaviour
{

    private List<Joycon> joycons;
    private Joycon joycon;
    // Sensitivity 調整用（移動の速さを変えられる）
    public float sensitivity = 0.1f;
    // 初期位置を記録
    private Vector3 initialPosition;
    void Start()
    {
        // Joy-Conリストを取得
        joycons = JoyconManager.Instance.j;
        if (joycons == null || joycons.Count == 0)
        {
            Debug.LogError("No Joy-Con connected!");
            return;
        }
        // 最初のJoy-Conを使用
        joycon = joycons[0];
        // 初期位置を記録
        initialPosition = transform.position;
    }
    void Update()
    {
        if (joycon == null) return;
        // ジャイロデータの取得
        Vector3 gyro = joycon.GetGyro();
        // ジャイロデータを位置に変換して移動
        // X軸とZ軸を利用して平面移動
        Vector3 movement = new Vector3(gyro.z, gyro.y, 0) * sensitivity;
        // オブジェクトの位置を更新
        transform.position += movement * Time.deltaTime;
        // 必要なら、初期位置からのオフセットを計算して制御可能
        // transform.position = initialPosition + movement;
    }
}
