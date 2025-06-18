using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimOnlyAnimConEvents : MonoBehaviour
{
    [SerializeField] CatReplace catReplace;

    public void FinishAnimation()
    {
        catReplace.ReturnCat();
    }
}
