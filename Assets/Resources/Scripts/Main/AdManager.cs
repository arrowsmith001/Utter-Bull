using UnityEngine;

using System;

using GoogleMobileAds.Api;
using System.Runtime.Serialization.Formatters;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    public bool TestAdsOn = true;

    private string appID = "";
    public string appIDAndroid = "ca-app-pub-5234994484736807~4997593194";
    public string appIDIOS = "ca-app-pub-5234994484736807~7079711499";

    public BannerView bannerView;
    private string bannerID = "";

    public string legitBannerAndroid = "ca-app-pub-5234994484736807~4997593194";
    public string legitBannerIOS = "ca-app-pub-5234994484736807~7079711499";

    public string testBannerAndroid = "ca-app-pub-3940256099942544/6300978111";
    public string testBannerIOS = "ca-app-pub-3940256099942544/2934735716";

    public MainScript main;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appID = appIDAndroid;

            if (TestAdsOn)
            {
                bannerID = testBannerAndroid;
            }
            else
            {
                bannerID = legitBannerAndroid;

            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            appID = appIDIOS;

            if (TestAdsOn)
            {
                bannerID = testBannerIOS;
            }
            else
            {
                bannerID = legitBannerIOS;

            }
        }

        if(appID != "" && bannerID != "")
        {
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("ADMOB INITIALISED");
            }
            );
        }
    }

    public void RequestBanner()
    {
        bannerView = new BannerView(bannerID, AdSize.Banner,AdPosition.Top);

        AdRequest request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
        bannerView.Show();

        main.MakeTopSpace(bannerView.GetHeightInPixels());
       
    }

    internal void RemoveAds()
    {
        if(bannerView!=null) bannerView.Destroy();
        main.UndoAdsSpace();
    }
}

