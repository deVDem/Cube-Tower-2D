using System;
using Game_Scene;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager : MonoBehaviour
{
    public RewardedAd rewardedAd;
    public static AdMobManager Instance;

    private string adId = "ca-app-pub-7389415060915567/5417167078";

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        rewardedAd = new RewardedAd(adId);
        rewardedAd.OnUserEarnedReward += AdRewind;
        AdRequest request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("3A7E5906D9D0159B").Build();
        rewardedAd.LoadAd(request);
        Instance = this;
    }

    public static void showAd()
    {
        Instance.rewardedAd.Show();
    }

    private void AdRewind(object sender, EventArgs args)
    {
        GameController.getReward();
    }
}