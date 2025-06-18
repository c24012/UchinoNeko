using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGyro : MonoBehaviour
{

    private List<Joycon> joycons;
    private Joycon joycon;
    // Sensitivity �����p�i�ړ��̑�����ς�����j
    public float sensitivity = 0.1f;
    // �����ʒu���L�^
    private Vector3 initialPosition;
    void Start()
    {
        // Joy-Con���X�g���擾
        joycons = JoyconManager.Instance.j;
        if (joycons == null || joycons.Count == 0)
        {
            Debug.LogError("No Joy-Con connected!");
            return;
        }
        // �ŏ���Joy-Con���g�p
        joycon = joycons[0];
        // �����ʒu���L�^
        initialPosition = transform.position;
    }
    void Update()
    {
        if (joycon == null) return;
        // �W���C���f�[�^�̎擾
        Vector3 gyro = joycon.GetGyro();
        // �W���C���f�[�^���ʒu�ɕϊ����Ĉړ�
        // X����Z���𗘗p���ĕ��ʈړ�
        Vector3 movement = new Vector3(gyro.z, gyro.y, 0) * sensitivity;
        // �I�u�W�F�N�g�̈ʒu���X�V
        transform.position += movement * Time.deltaTime;
        // �K�v�Ȃ�A�����ʒu����̃I�t�Z�b�g���v�Z���Đ���\
        // transform.position = initialPosition + movement;
    }
}
