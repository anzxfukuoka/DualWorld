using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using GoogleMobileAds.Common;

public class AdsManager : MonoBehaviour
{
    public static bool NoAds = false;

    // test rewarded ads ids
    // android ca-app-pub-3940256099942544/5224354917
    // ios ca-app-pub-3940256099942544/1712485313


    //rewarded ads
    private const string adUnitRewardedIdAndroid = "ca-app-pub-1044796649859765/4821695656"; //ca-app-pub-1044796649859765/4821695656
    private const string adUnitRewardedIdIOS = "ca-app-pub-1044796649859765/7019966661"; //ca-app-pub-1044796649859765/7019966661

    // test Interstitial ads ids
    // android ca-app-pub-3940256099942544/1033173712
    // ios ca-app-pub-3940256099942544/4411468910

    //Interstitial ads
    private const string adUnitInterstitialIdAndroid = "ca-app-pub-1044796649859765/6761463280";
    private const string adUnitInterstitialIdIOS = "ca-app-pub-1044796649859765/1959211674";

    private RewardedAd rewardedAd;
    private InterstitialAd interstitial;

    static GameObject curr;
    static string status = ":/";

    private void Start()
    {
        NoAds = Convert.ToBoolean(PlayerPrefs.GetInt(Statics.NOADS, 0));

        InitializeAddMob();
        //InitializeAppodeal(false);
    }

    public void SetNoAds(bool val) 
    {
        NoAds = val;
        PlayerPrefs.SetInt(Statics.NOADS, Convert.ToInt32(NoAds));
    }

    //private void InitializeAppodeal(bool isTesting)
    //{
    //    Appodeal.setNonSkippableVideoCallbacks(this);
    //    Appodeal.setInterstitialCallbacks(this);

    //    Appodeal.setTesting(isTesting);
    //    Appodeal.muteVideosIfCallsMuted(true);
    //    Appodeal.initialize(APP_KEY, Appodeal.INTERSTITIAL | Appodeal.NON_SKIPPABLE_VIDEO);

    //    StartCoroutine(loader());
    //}

    void InitializeAddMob() 
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
        // Add some test device IDs (replace with your own device IDs).
        //deviceIds.Add("");

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds).build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);

        RequestInterstitial();
        CreateAndLoadRewardedAd();
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // main thread.
        // In this example we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() => {
            status = "Initialization complete";
            CreateAndLoadRewardedAd();
        });
    }

    public void CreateAndLoadRewardedAd()
    {
        string adUnitId;

        #if UNITY_ANDROID
            adUnitId = adUnitRewardedIdAndroid;
        #elif UNITY_IPHONE
            adUnitId = adUnitRewardedIdIOS;
        #else
            adUnitId = "unexpected_platform";
        #endif

        //Rewarded Ads
        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
        //StartCoroutine(googleLoader(request));
    }

    private void RequestInterstitial()
    {
        #if UNITY_ANDROID
            string adUnitId = adUnitInterstitialIdAndroid;
        #elif UNITY_IPHONE
            string adUnitId =  adUnitInterstitialIdIOS;
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    IEnumerator googleLoader(AdRequest request) 
    {
        while (!rewardedAd.IsLoaded())
        {
            status = "loading...";
            rewardedAd.LoadAd(request);
            yield return null;
        }

        status = "loaded";
    }

    public void UserChoseToWatchAd(GameObject curr)
    {
        if (this.rewardedAd != null && this.rewardedAd.IsLoaded())
        {
            AdsManager.curr = curr;
            this.rewardedAd.Show();
            status = "rewardedAd loaded";
        }
        else 
        {
            curr.SendMessage("OnShowAdd", false);
            status = "rewardedAd not loaded";
        }
    }

    public void ShowInterstitial(GameObject curr)
    {
        if (NoAds)
            return;

        if (this.interstitial.IsLoaded())
        {
            AdsManager.curr = curr;
            this.interstitial.Show();
            status = "interstitial loaded";
        }
        else 
        {
            curr.SendMessage("OnShowAdd", false);
            status = "interstitial not loaded";
        }
    }

    #region AddMobRewardedHandles

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);

        curr.SendMessage("OnShowAdd", false);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");

        curr.SendMessage("OnShowAdd", true);

        CreateAndLoadRewardedAd(); //загружает новую рекламу
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }

    #endregion

    #region AddMobInterstitialHandles

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);

        curr.SendMessage("OnShowAdd", false);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");

        curr.SendMessage("OnShowAdd", true);
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    #endregion

    //IEnumerator loader() {
    //    while (!Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO)) 
    //    {
    //        status = "trying to load Appodeal...";
    //        yield return null;
    //    }

    //    status = "Appodeal loaded";
    //}

    //public void ShowImInterstitial()
    //{
    //    if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
    //        Appodeal.show(Appodeal.INTERSTITIAL);
    //}

    //public void ShowImNonSkipable(GameObject curr)
    //{
    //    if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
    //    {
    //        Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
    //        AdsManager.curr = curr;   
    //        status = "Appodeal loaded, curr = " + curr.name;
    //    }
    //    else 
    //    {
    //        curr.SendMessage("OnShowAdd", false);
    //        status = "Appodeal not loaded";
    //        //Initialize(false);
    //    }
    //}

    #region Interstittial
    public void onInterstitialLoaded(bool isPrecache)
    {
        //загружена межстраничная реклама
    }

    public void onInterstitialFailedToLoad()
    {
        //ошибка при загрузке межстр рекламы
    }

    public void onInterstitialShowFailed()
    {
        //ошибка показа межстраничной рекламы
        curr.SendMessage("OnShowAdd", false);
    }

    public void onInterstitialShown()
    {
        //меж страничная реклама показана
    }

    public void onInterstitialClosed()
    {
        //межстраничная реклама закрыта
        curr.SendMessage("OnShowAdd", true);
    }

    public void onInterstitialClicked()
    {
        //клик по межстраничной рекламе
    }

    public void onInterstitialExpired()
    {
        //межстраничная реклама закончилась
    }
    #endregion
    #region Reward
    public void onNonSkippableVideoLoaded(bool isPrecache)
    {
        Debug.Log("onNonSkippableVideoLoaded");
        status = "NonSkippableVideoLoaded";
    }

    public void onNonSkippableVideoFailedToLoad()
    {
        Debug.Log("onNonSkippableVideoFailedToLoad");
        status = "NonSkippableVideoFailedToLoad";
        //curr.SendMessage("OnShowAdd", false);
    }

    public void onNonSkippableVideoShowFailed()
    {
        Debug.Log("onNonSkippableVideoShowFailed");
        status = "NonSkippableVideoShowFailed";
        curr.SendMessage("OnShowAdd", false);
    }

    public void onNonSkippableVideoShown()
    {
        Debug.Log("onNonSkippableVideoShown");
        status = "NonSkippableVideoShown";
        //curr.SendMessage("OnShowAdd", true);
    }

    public void onNonSkippableVideoFinished()
    {
        Debug.Log("onNonSkippableVideoFinished");
        status = "NonSkippableVideoFinished";
        //curr.SendMessage("OnShowAdd", true);
    }

    public void onNonSkippableVideoClosed(bool finished)
    {
        Debug.Log("onNonSkippableVideoClosed");
        status = "NonSkippableVideoClosed";

        curr.SendMessage("OnShowAdd", finished);
    }

    public void onNonSkippableVideoExpired()
    {
        Debug.Log("onNonSkippableVideoExpired");
        status = "NonSkippableVideoExpired";
    }
    #endregion

    //  AdsManager.NoAds = true;

    //void OnGUI()
    //{
    //    GUI.contentColor = Color.magenta;
    //    GUI.Label(new Rect(10, 100, 400, 20), status);
    //}

    //private void OnGUI()
    //{
    //    GUIStyle guiStyle = new GUIStyle();
    //    guiStyle.fontSize = 20;
    //    GUI.color = Color.magenta;
    //    GUI.Label(new Rect(10, 100, 100, 20), status, guiStyle);
    //}
}
