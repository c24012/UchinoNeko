using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] CatBlinkAnim catBlinkAnim;
    [SerializeField] CubismLookTarget cubismLookTarget;

    [SerializeField] Texture2D cursorTx_def;
    [SerializeField] Texture2D cursorTx_teary;
    [SerializeField] Texture2D cursorTx_bone;
    [SerializeField] Vector2 hotSpot;

    [SerializeField] GameObject eatParticle;

    bool isBone = false;

    [SerializeField] int satietyLevel; //満腹度
    [SerializeField] float satietyLevelReset;
    float resetTime;

    //前フレームでヒットしたヘルプが必要なオブジェクト
    //GameObject _preFrameAssistanceObject = null;
    //uiにRayがあたったか結果格納リスト
    //readonly List<RaycastResult> rayResult = new();

    private void Awake()
    {
        //起動時にデフォルトのマウスアイコンに変更
        Cursor.SetCursor(cursorTx_def, hotSpot, CursorMode.ForceSoftware);
    }

    void FixedUpdate()
    {
        //rayResult.Clear();

        //var currentPointData = new PointerEventData(EventSystem.current);
        //currentPointData.position = Input.mousePosition;
        //EventSystem.current.RaycastAll(currentPointData, rayResult);

        //if (rayResult.Count != 0)
        //{
        //    foreach (RaycastResult raycastResult in rayResult)
        //    {
        //        GameObject hitObj = raycastResult.gameObject;
        //        if (hitObj != null)
        //        {
        //            if (_preFrameAssistanceObject == null && hitObj.CompareTag("TitleCat"))
        //            {
        //                //ねこちゃんに当たっていればマウスアイコンを涙目に変更
        //                _preFrameAssistanceObject = hitObj;
        //                Cursor.SetCursor(cursorTx_teary, hotSpot, CursorMode.ForceSoftware);
        //                break;
        //            }

        //            //前フレームにあたったオブジェクトがヒットしていなければuiから離れたとみなしてデフォルトにする
        //            else if (rayResult.All(ray => ray.gameObject != _preFrameAssistanceObject))
        //            {
        //                _preFrameAssistanceObject = null;
        //                Cursor.SetCursor(cursorTx_def, hotSpot, CursorMode.ForceSoftware);
        //                break;
        //            }
        //        }
        //        else if (_preFrameAssistanceObject != null)
        //        {
        //            _preFrameAssistanceObject = null;
        //            Cursor.SetCursor(cursorTx_def, hotSpot, CursorMode.ForceSoftware);
        //        }
        //    }
        //}
        //else if (_preFrameAssistanceObject != null)
        //{
        //    _preFrameAssistanceObject = null;
        //}
    }

    private void Update()
    {
        Vector3 targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(targetPosition.x, targetPosition.y, 10f));
        transform.position = targetPosition;
        if (satietyLevel == 5)
        {
            resetTime = Random.Range(5, 15);
            satietyLevelReset += Time.deltaTime;
            if (satietyLevelReset >= resetTime)
            {
                satietyLevel = 0;
                satietyLevelReset = 0;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isBone)
        {
            if (collision.CompareTag("TitleCat"))
            {
                Cursor.SetCursor(cursorTx_teary, hotSpot, CursorMode.ForceSoftware);
            }
            else if (satietyLevel != 5)
            {
                Vector3 mouPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                Instantiate(eatParticle, mouPos, Quaternion.identity);
                Cursor.SetCursor(cursorTx_bone, hotSpot, CursorMode.ForceSoftware);
                cubismLookTarget.OnBorn();
                isBone = true;
                satietyLevel += 1;
                Debug.Log("現在値" + satietyLevel);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isBone)
        {
            if (collision.CompareTag("TitleCat"))
            {
                Cursor.SetCursor(cursorTx_def, hotSpot, CursorMode.ForceSoftware);
            }
        }
    }
    //マウスカーソルリセット
    public void FReset()
    {
        Cursor.SetCursor(cursorTx_def, hotSpot, CursorMode.ForceSoftware);
        isBone = false;
    }
}

