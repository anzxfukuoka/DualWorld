using IAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartPanel : MonoBehaviour
{
    public Text mToNextLVL;
    public Slider timeSlider;

    public AdsManager adsManager;

    public GameObject errPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mToNextLVL.text = (int)Player.metersToNextLVL() + "m to next level";

        timeSlider.value = (Game.instance.waitForContinue - Game.timeToRestart)/ Game.instance.waitForContinue;
    }

    public void OnWatchAdButtonClick() 
    {
        Debug.Log("ad button");

        //Game.isContinue = true;

        Game.stopContinueTimer = true;        
        //adsManager.ShowImNonSkipable(gameObject);
        adsManager.UserChoseToWatchAd(gameObject);
        //adsManager.ShowInterstitial(gameObject);
    }

    public void OnShowAdd(bool success) 
    {
        Game.stopContinueTimer = false;

        if (success)
        {
            Game.isContinue = true;
        }
        else
        {
            //Game.timeToRestart = 0.5f;
            errPanel.SetActive(true);
        }
    }
}
