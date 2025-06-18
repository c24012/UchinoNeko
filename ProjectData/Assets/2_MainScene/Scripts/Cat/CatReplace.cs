using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatReplace : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] GameObject catAnimOnlyObj;
    [SerializeField] Transform catAnimOnlyObjPivot;
    [SerializeField] Animator catAnimOnlyAnim;
    [SerializeField] GameObject catObj;
    [SerializeField] SkinnedMeshRenderer catSkinMesh;
    [SerializeField] GameObject catLookAtObj;
    [SerializeField] Vector3 headDefuoltEuler;

    private void Awake()
    {
        catAnimOnlyObj.SetActive(false);
    }

    public void ReplaceCat()
    {
        catSkinMesh.enabled = false;
        catAnimOnlyObj.SetActive(true);
        catAnimOnlyAnim.SetTrigger("RunTrigger");
    }

    public void ReturnCat()
    {
        catObj.transform.position = catAnimOnlyObjPivot.transform.position;

        Vector3 euler = catAnimOnlyObj.transform.eulerAngles;
        catObj.transform.eulerAngles = new Vector3(euler.x, euler.y - 180, euler.z);
        catLookAtObj.transform.localEulerAngles = headDefuoltEuler;

        catSkinMesh.enabled = true;
        catAnimOnlyObj.SetActive(false);

        gameManager.SetIsRunPhase(true);
    }
}
