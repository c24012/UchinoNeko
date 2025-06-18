using Live2D.Cubism.Framework.LookAt;
using UnityEngine;
public class CubismLookTarget : MonoBehaviour, ICubismLookTarget
{
    [SerializeField] Transform lookTargetTf;
    bool isFreeEye = false;
    public Vector3 GetPosition()
    {
        if (!isFreeEye)
        {
            Vector3 targetPosition = Input.mousePosition;
            targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(targetPosition.x, targetPosition.y, 10f));
            transform.position = targetPosition;
            return targetPosition;
        }
        else
        {
            return lookTargetTf.position;
        }
    }

    public bool IsActive()
    {
        return true;
    }

    public void OnBorn()
    {
        //isFreeEye = true;
    }
}