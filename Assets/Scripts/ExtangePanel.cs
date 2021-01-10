using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtangePanel : MonoBehaviour
{
    public AdsManager adsManager;
    public GameObject errPanel;

    public void OnOk() 
    {
        //Store.Exchange();
        //adsManager.ShowImNonSkipable(gameObject);
        adsManager.UserChoseToWatchAd(gameObject);
        //adsManager.ShowInterstitial(gameObject);
    }

    public void OnShowAdd(bool success) //extange
    {
        if (success)
        {
            Store.Exchange();
        }
        else
        {
            errPanel.SetActive(true);
        }
    }
}
