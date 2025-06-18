using System;
using Unity.VisualScripting;
using UnityEngine;

public class RouteMoveAnimation : MonoBehaviour
{
    [SerializeField] GameObject cat;

    public Transform catTf;
    [SerializeField] GameObject moveMarkerParentObj;
    [SerializeField] Transform[] moveMarkers;
    [SerializeField] float[] pointsAngleDiff;
    [SerializeField] int nowPoint = 0;
    [SerializeField] float rotateSpeed;

    private void Start()
    {
        foreach(Transform child in moveMarkerParentObj.GetComponentsInChildren<Transform>())
        {
            if(child != moveMarkerParentObj.transform)
            {
                Array.Resize(ref moveMarkers, moveMarkers.Length + 1);
                moveMarkers[moveMarkers.Length - 1] = child;
            }
        }

        for (int i = 0; i < moveMarkers.Length; i++)
        {
            if (i == 0)
            {
                Vector3 vecFrom = Vector3.ProjectOnPlane(moveMarkers[0].position - catTf.position, Vector3.up);
                Vector3 vecTo = Vector3.ProjectOnPlane(-catTf.forward, Vector3.up);

                Array.Resize(ref pointsAngleDiff, pointsAngleDiff.Length + 1);
                pointsAngleDiff[0] = catTf.eulerAngles.y - Vector3.SignedAngle(vecFrom, vecTo, Vector3.up);
            }
            else if (i == 1)
            {
                Vector3 vecFrom = Vector3.ProjectOnPlane(moveMarkers[i].position - moveMarkers[i - 1].position, Vector3.up);
                Vector3 vecTo = Vector3.ProjectOnPlane(moveMarkers[i - 1].position - catTf.position, Vector3.up);

                Array.Resize(ref pointsAngleDiff, pointsAngleDiff.Length + 1);
                pointsAngleDiff[i] = pointsAngleDiff[i - 1] - Vector3.SignedAngle(vecFrom, vecTo, Vector3.up);
            }
            else
            {
                Vector3 vecFrom = Vector3.ProjectOnPlane(moveMarkers[i].position - moveMarkers[i - 1].position, Vector3.up);
                Vector3 vecTo = Vector3.ProjectOnPlane(moveMarkers[i - 1].position - moveMarkers[i - 2].position, Vector3.up);

                Array.Resize(ref pointsAngleDiff, pointsAngleDiff.Length + 1);
                pointsAngleDiff[i] = pointsAngleDiff[i - 1] - Vector3.SignedAngle(vecFrom, vecTo, Vector3.up);
            }
        }
    }

    public void Move(float moveSpeed)
    {
        for (int i = 0; i < moveMarkers.Length; i++)
        {
            if (nowPoint == i)
            {
                cat.transform.position = Vector3.MoveTowards(catTf.position, moveMarkers[i].position, moveSpeed * Time.deltaTime * 60);
                catTf.rotation = Quaternion.RotateTowards(
                    catTf.rotation, Quaternion.Euler(catTf.eulerAngles.x, pointsAngleDiff[i], catTf.eulerAngles.z), rotateSpeed
                    );
                if (cat.transform.position == moveMarkers[i].position)
                {
                    nowPoint++;
                }
            }
        }
    }

    //インスペクタから実行
    [ContextMenu("マーカー表示")]
    public void InvisibleMarker()//マーカー非表示
    {
        foreach (MeshRenderer meshRndr in moveMarkerParentObj.GetComponentsInChildren<MeshRenderer>())
        {
            meshRndr.enabled = !meshRndr.enabled;
        }
    }
}

