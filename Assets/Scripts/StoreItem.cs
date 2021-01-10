using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StoreItem : MonoBehaviour
{
    
    public StoreItemSettings storeItemSettings;

    [Header("Perfabs")]
    
    public Image playerImageDay;
    public Image backgroundImageDay;
    public Image accentImageDay;

    [Space(10)]

    public Image playerImageNight;
    public Image backgroundImageNight;
    public Image accentImageNight;

    [Space(10)]

    public Image starImage;
    public Image gemsImage1;
    public Image gemsImage2;

    [Space(10)]

    public Text starPriceText;
    public Text gemsPriceText;
    public Text lvlLocked;

    [Space(10)]

    public GameObject valuesConteiner;
    public GameObject value;
    public GameObject specialValue;
    public GameObject locked;
    

    // 
    public bool purshased
    {
        get
        {
            if (storeItemSettings.itemName == "default")
                return true;

            return Convert.ToBoolean(PlayerPrefs.GetInt(storeItemSettings.itemID, 0));
        }
    }

    public bool isLocked
    {
        get
        {
            bool unlockAll = Convert.ToBoolean(PlayerPrefs.GetInt(Statics.UNLOCKALL, 0));
            
            if (unlockAll)
                return false;

            return storeItemSettings.lvl > PlayerPrefs.GetInt(Statics.HIGHTLVL, 1);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //
        if (storeItemSettings.special)
        {
            specialValue.SetActive(true);
            value.SetActive(false);
        }
        else
        {
            specialValue.SetActive(false);
            value.SetActive(true);
        }


        if (isLocked && !purshased)
        {
            locked.SetActive(true);
            valuesConteiner.SetActive(false);
            lvlLocked.text = "lvl " + storeItemSettings.lvl;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked && !purshased)
        {
            locked.SetActive(true);
            valuesConteiner.SetActive(false);
            lvlLocked.text = "lvl " + storeItemSettings.lvl;
        }

        if (!isLocked) {
            locked.SetActive(false);
            valuesConteiner.SetActive(true);
            if (storeItemSettings.special)
            {
                specialValue.SetActive(true);
                value.SetActive(false);
            }
            else
            {
                specialValue.SetActive(false);
                value.SetActive(true);
            }
        }

        if (purshased)
        {
            valuesConteiner.SetActive(false);
        }
    }

    public void SetStoreItemSettings(StoreItemSettings settings)
    {
        storeItemSettings = settings;

        playerImageDay.color = settings.colorTheme.playerDayColor;
        backgroundImageDay.color = settings.colorTheme.backgroundNightColor;

        playerImageNight.color = settings.colorTheme.playerNightColor;
        backgroundImageNight.color = settings.colorTheme.backgroundDayColor;

        accentImageDay.color = settings.colorTheme.accentColor;
        accentImageNight.color = settings.colorTheme.accentColor;

        starImage.color = settings.colorTheme.accentColor;
        gemsImage1.color = settings.colorTheme.accentColor;
        gemsImage2.color = settings.colorTheme.accentColor;

        starPriceText.text = settings.starsPrice + "";
        gemsPriceText.text = settings.gemsPrice + "";

        starPriceText.color = settings.colorTheme.backgroundDayColor;
        gemsPriceText.color = settings.colorTheme.backgroundDayColor;
    }

}


[Serializable]
public class StoreItemSettings {

    public string itemName;

    public string itemID {
        get 
        {
            return "theme_" + itemName;
        }
    }

    public string appStoreID {
        get {
            return "app_" + itemID;
        }
    }

    public string gPlayID {
        get {
            return "gp_" + itemID;
        }
    }
    
    public ColorTheme colorTheme;

    public bool special = false;

    [Header("Price")]
    public int lvl;
    public int starsPrice = 0;
    public int gemsPrice = 0;
}


[Serializable]
public class PointsItemSettings
{

    [Header("Points")]
    public int starsPrice = 0;
    public int gemsPrice = 0;

    [Header("Price")]
    public float realCurrency = 0.99f;

    public string itemID
    {
        get
        {
            return "points_stars" + starsPrice + "_gems" + gemsPrice;
        }
    }

    public string appStoreID
    {
        get
        {
            return "app_" + itemID;
        }
    }

    public string gPlayID
    {
        get
        {
            return "gp_" + itemID;
        }
    }
}
