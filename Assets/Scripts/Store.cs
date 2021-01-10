using IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public static Store instance;

    public InAppManager inAppManager;
    public AdsManager adsManager;

    public StoreItemSettings[] items;

    public static StoreItemSettings selectedItem;

    public PointsItemSettings[] pointsItems;

    public static PointsItemSettings selectedPointsItem;

    [Header("Perfabs")]

    public GameObject storeItemButton;
    public Transform content;

    [Space(10)]
    public Text starsPointsLabel;
    public Text gemsPointsLabel;

    [Space(10)]
    public Text lvlLabel;

    [Space(10)]
    public GameObject buyThemePanel;
    public GameObject openAllThemesPanel;
    public GameObject errorPanel;
    public GameObject successPanel;
    public GameObject notEnoughStarsPanel;
    public GameObject notEnoughPointsPanel;


    [Header("Preview Mats")]

    public Material playerMaterial;
    public Material backgroundMaterial;
    public Material wallMaterial;
    public Material blockNightMaterial;
    public Material blockDayMaterial;

    [Space(10)]
    public Material accentMaterial;

    [Space(10)]
    public Material numsTextMaterial;
    public Material wordsTextMaterial;

    [Space(10)]
    public Texture2D gradTex;
    public Image gradImage;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            GameObject a = Instantiate(storeItemButton, content);
            StoreItem sti = a.GetComponent<StoreItem>();
            sti.SetStoreItemSettings(items[i]);
        }
    }

    private void OnEnable()
    {
        instance = this;

        UpdatePointsLabels();

        lvlLabel.text = PlayerPrefs.GetInt(Statics.HIGHTLVL, 1) + "";

        SetPreviewColorTheme(Game.GetColorTheme());
    }

    public void OnCloseStorePanel() 
    {
        SetPreviewColorTheme(Game.GetColorTheme());
    }

    public static void UpdatePointsLabels() {

        float starsPoints = PlayerPrefs.GetInt(Statics.STARSPOINTS, 0);
        float gemsPoints = PlayerPrefs.GetInt(Statics.GEMSPOINTS, 0);

        instance.starsPointsLabel.text = starsPoints + "";
        instance.gemsPointsLabel.text = gemsPoints + "";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStoreItemClick(StoreItem storeItem)
    {
        selectedItem = storeItem.storeItemSettings;
        SetPreviewColorTheme(storeItem.storeItemSettings.colorTheme);

        //Debug.Log(storeItem.purshased);

        if (!storeItem.isLocked) //if unlocked
        {
            if (!storeItem.purshased)
            {
                buyThemePanel.SetActive(true);
            }
            else
            {
                Game.SetColorTheme(storeItem.storeItemSettings.colorTheme.themeName);
            }

        }
        else 
        {
            openAllThemesPanel.SetActive(true);
        }

        UpdatePointsLabels();
    }

    public static StoreItemSettings[] GetThemeItems() {
        return instance.items;
    }

    public static PointsItemSettings[] GetPointsItens() {
        return instance.pointsItems;
    }

    public static void BuySelectedTheme() 
    {
        if (selectedItem.special)
        {
            instance.inAppManager.BuyProductID(selectedItem.itemID); //selectedItem.itemID
        }
        else
        {
            int starsPrice = selectedItem.starsPrice; 
            int gemsPrice = selectedItem.gemsPrice;

            if (HasEnoughStarsPoints(starsPrice) && HasEnoughGemsPoints(gemsPrice)) //хватает ли
            {
                WithdrawStarsPoints(starsPrice);
                WithdrawGemsPoints(gemsPrice);

                SetThemeItemPurchased(selectedItem);
            }
            else 
            {
                instance.notEnoughPointsPanel.SetActive(true);
            }
        }

        instance.buyThemePanel.SetActive(false);

        UpdatePointsLabels();

    }

    public static void OnThemePurchased(string themeItemID)
    {
        for (int i = 0; i < instance.items.Length; i++) 
        {
            if (instance.items[i].itemID == themeItemID) 
            {
                SetThemeItemPurchased(instance.items[i]);
            }
        }

        UpdatePointsLabels();
    }

    public static void OnThemePurchaseFail() {
        instance.errorPanel.SetActive(true);
    }

    public static void SetThemeItemPurchased(StoreItemSettings item) 
    {
        PlayerPrefs.SetInt(item.itemID, Convert.ToInt32(true));
        Game.SetColorTheme(item.colorTheme.themeName);

        instance.successPanel.SetActive(true);
        
        UpdatePointsLabels();
    }

    public static void Exchange() {

        int stars = 640;
        int gems = 10;

        if (HasEnoughStarsPoints(stars))
        {
            WithdrawStarsPoints(stars);
            AddGemsPoints(gems);

            instance.successPanel.SetActive(true);
        }
        else 
        {
            instance.notEnoughStarsPanel.SetActive(true);
        }

        UpdatePointsLabels();
    }

    public void BuyPoints(PointsItem pointsItem) 
    {
        PointsItemSettings pointsItemSettings = pointsItem.pointsItemSettings;
        selectedPointsItem = pointsItemSettings;
        inAppManager.BuyProductID(pointsItemSettings.itemID);
    }

    public static void OnPointsPurchased() 
    {
        int stars = selectedPointsItem.starsPrice;
        int gems = selectedPointsItem.gemsPrice;

        AddStarsPoints(stars);
        AddGemsPoints(gems);
        UpdatePointsLabels();
        instance.successPanel.SetActive(true);
    }

    public void OpenAll() 
    {
        inAppManager.BuyProductID(InAppManager.pOpenAllThemes);
    }

    public static void OnOpenAllThemesPurchased() 
    {
        PlayerPrefs.SetInt(Statics.UNLOCKALL, Convert.ToInt32(true));
        instance.successPanel.SetActive(true);
    }

    public void BuyNoAds() 
    {
        instance.inAppManager.BuyProductID(InAppManager.pNoAds);
    }

    public static void OnNoAdsPurchased() 
    {
        instance.adsManager.SetNoAds(true);
        instance.successPanel.SetActive(true);
    }

    public static void SetPreviewColorTheme(ColorTheme theme) 
    {
        instance.wallMaterial.color = theme.borderNightColor;
        instance.backgroundMaterial.color = theme.backgroundNightColor;

        instance.blockDayMaterial.color = theme.backgroundDayColor;
        instance.blockNightMaterial.color = theme.backgroundNightColor;

        instance.playerMaterial.color = theme.playerDayColor;

        instance.accentMaterial.color = theme.accentColor;

        instance.numsTextMaterial.color = theme.backgroundDayColor;
        instance.wordsTextMaterial.color = theme.playerDayColor;

        if (theme.specialTheme)
        {
            instance.gradImage.material = null;
            Texture2D tex = ApplyGradientToTexture(instance.gradTex, theme.trailGrad);
            instance.gradImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        else {
            instance.gradImage.material = instance.accentMaterial;
            instance.gradImage.sprite = Sprite.Create(instance.gradTex, new Rect(0.0f, 0.0f, instance.gradTex.width, instance.gradTex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public static Texture2D ApplyGradientToTexture(Texture2D tex, Gradient grad)
    {
        Texture2D re = new Texture2D(tex.width, tex.height);

        for (int y = 0; y < tex.height; y++) 
        {
            Color pixColor = grad.Evaluate((float)y / tex.height);

            for (int x = 0; x < tex.width; x++) 
            {    
                re.SetPixel(x, y, pixColor);
            }

            //Debug.Log(pixColor.r + " " + pixColor.g + " " + pixColor.b + " " + pixColor.a);
        }

        re.Apply();
        return re;
    }


    public static bool HasEnoughStarsPoints(int stars) {

        int generalSP = PlayerPrefs.GetInt(Statics.STARSPOINTS, 0);

        return (generalSP - stars >= 0);
    }

    public static bool HasEnoughGemsPoints(int gems)
    {
        int generalGP = PlayerPrefs.GetInt(Statics.GEMSPOINTS, 0);

        return (generalGP - gems >= 0);
    }

    public static void AddStarsPoints(int sp) 
    {
        int generalSP = PlayerPrefs.GetInt(Statics.STARSPOINTS, 0);
        generalSP += sp;

        PlayerPrefs.SetInt(Statics.STARSPOINTS, generalSP);
    }

    public static void AddGemsPoints(int gp)
    {
        int generalGP = PlayerPrefs.GetInt(Statics.GEMSPOINTS, 0);
        generalGP += gp;

        PlayerPrefs.SetInt(Statics.GEMSPOINTS, generalGP);
    }

    public static void WithdrawStarsPoints(int sp)
    {
        int generalSP = PlayerPrefs.GetInt(Statics.STARSPOINTS, 0);

        generalSP -= sp;

        PlayerPrefs.SetInt(Statics.STARSPOINTS, generalSP);
    }

    public static void WithdrawGemsPoints(int gp)
    {
        int generalGP = PlayerPrefs.GetInt(Statics.GEMSPOINTS, 0);

        generalGP -= gp;

        PlayerPrefs.SetInt(Statics.GEMSPOINTS, generalGP);
    }
}