using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField] AudioClip bgm1;
    [SerializeField] AudioClip bgm2;

    [SerializeField] AudioSource resultBgm;
    [SerializeField] ScoreManager scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BgmStart()
    {
        Debug.Log(scoreManager.score);
        if (scoreManager.score == 6)
        {
            resultBgm.clip = bgm2;
        }
        else
        {
            resultBgm.clip = bgm1;
        }
        resultBgm.Play();
    }
}
