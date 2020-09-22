using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppsFlyerKit
{
#if UNITY_IOS || UNITY_ANDROID
    private static bool isDebugMode = false;
#endif
#if UNITY_IOS
    private static bool isSendboxMode = false;
#endif


    public static void InitAppFlyer()
    {
        string appsFlyerDevKey = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.AppFlyerDevKit);
        string appsFlyerIOSID = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.AppsFlyerIOSID);

        if(string.IsNullOrEmpty(appsFlyerDevKey) || string.IsNullOrEmpty(appsFlyerIOSID))
        {
            switch(AppInformation.GAME_ID)
            {
                case Enums.GameID.Backgammon4Money:
                    appsFlyerDevKey = "9qq7ZHjCYDkEyaCPi5Mir6";
                    appsFlyerIOSID = "975475057";
                    break;
                case Enums.GameID.BackgammonRoyale:
                    appsFlyerDevKey = "9qq7ZHjCYDkEyaCPi5Mir6";
                    appsFlyerIOSID = "1229622706";
                    break;
                case Enums.GameID.BackgammonForFriends:
                    appsFlyerDevKey = "9qq7ZHjCYDkEyaCPi5Mir6";
                    appsFlyerIOSID = "1229622706";
                    break;
                default:
                    Debug.Log("No AppsFlyer Initiation ( good programming <3)");
                    return;
            }
        }
#if UNITY_IOS
        Debug.Log("InitAppFlyer DevKey: " + appsFlyerDevKey + " IOSID: " + appsFlyerIOSID);

        AppsFlyer.setAppsFlyerKey (appsFlyerDevKey);
        AppsFlyer.setAppID (appsFlyerIOSID);
        
        // For detailed logging
        AppsFlyer.setIsDebug (isDebugMode); 
        
        // For getting the conversion data will be triggered on AppsFlyerTrackerCallbacks.cs file
        AppsFlyer.getConversionData (); 
        
        // For testing validate in app purchase (test against Apple's sandbox environment
        AppsFlyer.setIsSandbox(isSendboxMode);         
    
        AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
        AppsFlyer.init(appsFlyerDevKey, "GTPluginBridge");
        AppsFlyer.setAppID(AppInformation.BUNDLE_ID);

        AppsFlyer.setIsDebug (isDebugMode);
        AppsFlyer.createValidateInAppListener ("AppsFlyerTrackerCallbacks", "onInAppBillingSuccess", "onInAppBillingFailure");
        AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks");
#else
        Debug.Log("AppsFlyer disabled in unity editor or non mobile platform");
#endif
    }

#region Tracking Functions
    public static void OpenAppTracker()
    {
        Dictionary<string, string> eventData = new Dictionary<string, string>
        {
            {"af_deviceid", SystemInfo.deviceUniqueIdentifier},
        };
        AppsFlyer.trackRichEvent("af_session", eventData);
    }

    public static void FacebookSignUpTracker()
    {
        AppsFlyer.trackRichEvent("af_login", new Dictionary<string, string>());
    }

    public static void RegisterTracker()
    {
        AppsFlyer.trackRichEvent("af_complete_registration", new Dictionary<string, string>());
    }

    public static void LoginTracker()
    {
        AppsFlyer.trackRichEvent("af_gamytech_login", new Dictionary<string, string>());
    }

    public static void CashInRequestTracker(string error = null)
    {
        Dictionary<string, string> eventData = new Dictionary<string, string> { { "af_OS", Application.platform.ToString() }, };
        if (!string.IsNullOrEmpty(error))
            eventData.Add("af_error", error);

        AppsFlyer.trackRichEvent("af_CashInRequest", eventData);
    }

    public static void IAPTracker(string currency, string amount)
    {
        Dictionary<string, string> eventData = new Dictionary<string, string>
        {
            {"af_currency", currency},
            {"af_quantity", amount}
        };
        AppsFlyer.trackRichEvent("af_purchase", eventData);
    }

    public static void FirstCashInTracker(string amount)
    {
        Dictionary<string, string> eventData = new Dictionary<string, string>
        {
            {"af_quantity", amount.ToString()},
            {"af_OS", Application.platform.ToString()},
        };

        if (!string.IsNullOrEmpty(PushNotificationKit.LaunchedPushID))
            eventData.Add("af_PushCampaignID", PushNotificationKit.LaunchedPushID);

        AppsFlyer.trackRichEvent("af_firstCashin", eventData);
    }

    public static void CashInTracker(string amount)
    {
        Dictionary<string, string> eventData = new Dictionary<string, string>
        {
            {"af_quantity", amount.ToString()},
            {"af_OS", Application.platform.ToString()},
        };

        if (!string.IsNullOrEmpty(PushNotificationKit.LaunchedPushID))
            eventData.Add("af_PushCampaignID", PushNotificationKit.LaunchedPushID);
        
        AppsFlyer.trackRichEvent("af_cashin", eventData);
    }

    public static void CashOutTracker(string amount)
    {
        Dictionary<string, string> eventData = new Dictionary<string, string>
        {
            {"af_quantity", amount.ToString()}
        };
        AppsFlyer.trackRichEvent("af_cashout", eventData);
    }

    public static void NotifyTotalMatches()
    {
        Dictionary<string, string> eventData = new Dictionary<string, string> {};
        AppsFlyer.trackRichEvent("af_played_3_matches", eventData);
    }

    #endregion Tracking Functions
}
