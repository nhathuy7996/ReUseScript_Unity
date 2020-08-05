using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    string AppID = "ca-app-pub-9056640292640134~3154610018",//"ca-app-pub-3940256099942544~3347511713",
   BannerID = "ca-app-pub-3940256099942544/6300978111",//"ca-app-pub-3940256099942544/6300978111",
   InterID = "ca-app-pub-3940256099942544/1033173712",//"ca-app-pub-3940256099942544/1033173712", 
   VideoID = "ca-app-pub-3940256099942544/5224354917";//"ca-app-pub-3940256099942544/5224354917";

    [Header("AdUnit")]
    [SerializeField] private InterstitialAd interstitial = null;
    private List<RewardedAd> RewardAds = new List<RewardedAd>();
    [SerializeField] int MaxReward = 3;

    public UnityEvent RewardCallBack = null, InterCallBack = null;
    private BannerView bannerAd = null;

    private bool _Banner_active = false, _InterShowed = false;
    float TimeInter = 0;
    [SerializeField] float MaxTimerInter = 60;
    public bool Banner_active => _Banner_active;
    public bool InterShowed => _InterShowed;

    float DelayReloadInter = 1, DelayReloadReward = 1, DelayReloadBanner = 1;

    void Start() {
        MobileAds.Initialize(this.AppID);

        this.RequestInterstitial();
        //this.RequestBanner();

        for (int i = 0; i < MaxReward; i++) {
            RewardAds.Add(this.RequestReward());
        }
    }

    void Update() {
        if (_InterShowed) {
            TimeInter += Time.deltaTime;
            if (TimeInter >= MaxTimerInter)
            {
                TimeInter = 0;
                _InterShowed = false;
            }
        }
    }

    #region Banner
    public void RequestBanner()
    {
        Debug.LogError("Banner request!!");
        // Create a 320x50 banner at the top of the screen.
        bannerAd = new BannerView(this.BannerID, AdSize.SmartBanner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        bannerAd.OnAdLoaded += Banner_HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerAd.OnAdFailedToLoad += Banner_HandleOnAdFailedToLoad;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerAd.LoadAd(request);
    }
    #endregion

    #region Handle Banner
    public void Banner_HandleOnAdLoaded(object sender, System.EventArgs args)
    {
        Debug.LogError("Load banner success!");
        _Banner_active = true;
    }

    public void Banner_HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.LogError("Load banner faild");
        Invoke("RequestBanner", DelayReloadBanner);
        DelayReloadBanner *= 5;
    }
    #endregion

    #region Inter
    public void RequestInterstitial()
    {
        if (this.interstitial != null) {
            this.interstitial.Destroy();
        }
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(InterID);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += Inter_HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += Inter_HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += Inter_HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += Inter_HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += Inter_HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);

        Debug.LogError("Inter requested");
        StartCoroutine(WaitLoadInter());
    }

    IEnumerator WaitLoadInter() {
        yield return new WaitUntil(() => this.interstitial.IsLoaded());
        Debug.LogError("Inter load done!");
    }

    public void ShowInter()
    {
        if (this.interstitial.IsLoaded()) {

            if (_InterShowed)
                return;
            _InterShowed = true;
            TimeInter = 0;
            this.interstitial.Show();
        }
    }
    #endregion

    #region HandleInter

    public void Inter_HandleOnAdLoaded(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("Inter HandleAdLoaded event received");

    }

    public void Inter_HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("inter HandleFailedToReceiveAd event received with message: ");
        Invoke("RequestInterstitial", DelayReloadInter);
        DelayReloadInter *= 2;
    }

    public void Inter_HandleOnAdOpened(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("Inter HandleAdOpened event received");

    }

    public void Inter_HandleOnAdClosed(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        RequestInterstitial();
        if (InterCallBack != null) {
            InterCallBack.Invoke();
            InterCallBack.RemoveAllListeners();
        }

    }

    public void Inter_HandleOnAdLeavingApplication(object sender, System.EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
        RequestInterstitial();
    }
    #endregion

    #region Reward
    public RewardedAd RequestReward()
    {

        RewardedAd rewardedAd = new RewardedAd(VideoID);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);

        Debug.Log("Reward requested");

        return rewardedAd;
    }

    public void ShowReward()
    {
#if UNITY_EDITOR
        if (RewardCallBack != null)
        {
            RewardCallBack.Invoke();
            RewardCallBack.RemoveAllListeners();
        }
        return;
#endif
        RewardedAd R = CheckAvailableReward();
        if (R != null) {
            R.Show();
            return;
        }
        if (RewardCallBack != null)
        {
            RewardCallBack.RemoveAllListeners();
        }
        Debug.LogError("Not avaiable reward!");
    }

    public RewardedAd CheckAvailableReward() {
        foreach (RewardedAd R in this.RewardAds) {
            if (R.IsLoaded()) {
                return R;
            }
        }
        if (this.RewardAds.Count < MaxReward)
            RewardAds.Add(this.RequestReward());
        return null;
    }

    #endregion

    #region HandleReward

    public void HandleRewardedAdLoaded(object sender, System.EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
        this.RewardAds.Remove((RewardedAd)sender);
        Invoke("CheckAvailableReward", DelayReloadReward);
        DelayReloadReward *= 2;
    }

    public void HandleRewardedAdOpening(object sender, System.EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToShow event received with message: ");
        if (RewardCallBack != null)
        {
            RewardCallBack.RemoveAllListeners();
        }
    }

    public void HandleRewardedAdClosed(object sender, System.EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed event received");
        this.RewardAds.Remove((RewardedAd)sender);
        CheckAvailableReward();

    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {

        Debug.Log("HandleRewardedAdRewarded event received for ");
        if (RewardCallBack != null) {
            StartCoroutine(RunRewardCallBack());
        }
        this.RewardAds.Remove((RewardedAd)sender);
        CheckAvailableReward();
    }
    private IEnumerator RunRewardCallBack()
    {
        yield return null;
        RewardCallBack.Invoke();
        RewardCallBack.RemoveAllListeners();
    }
    #endregion

}
