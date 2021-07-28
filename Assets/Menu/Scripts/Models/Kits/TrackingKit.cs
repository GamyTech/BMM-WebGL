using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;


public class TrackingKit
{
    private const int TOTAL_MATCHES_TO_NOTIFY = 3;

    public static void InitTracking()
    {
        PushNotificationKit.Init();
        AppsFlyerKit.InitAppFlyer();
        OpenAppTracker();
        Debug.Log("<color=blue> InitTracking </color>");
    }

    #region Tracking Functions

    public static void OpenAppTracker()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.OpenAppTracker();
        Pushwoosh.Instance.SetStringTag("DeviceId", NetworkController.DeviceId);
#endif
        ContentController.Instance.StartCoroutine(GT.Database.DatabaseKit.CheckAppsFlyer());
    }

    public static void LoginTracker()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.LoginTracker();
        Pushwoosh.Instance.SetUserId(UserController.Instance.gtUser.Id);
        Pushwoosh.Instance.SetStringTag("UserId", UserController.Instance.gtUser.Id);
#endif
        SendCustomEvent("LogIn");
    }

    public static void SetUserNameTracker(string UserName)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        Pushwoosh.Instance.SetStringTag("Alias", UserName);
#endif
    }

    public static void SetLoyaltiesAmount(int loyalties)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        Pushwoosh.Instance.SetIntTag("LoyaltyPoints", loyalties);
#endif
    }

    public static void SetCashAmount(int cash)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        Pushwoosh.Instance.SetIntTag("Balance", cash);
#endif
    }

    public static void RegisterTracker()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.RegisterTracker();

        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        Pushwoosh.Instance.SetIntTag("LastRegistration", cur_time);
        Pushwoosh.Instance.SetUserId(UserController.Instance.gtUser.Id);
        Pushwoosh.Instance.SetStringTag("UserId", UserController.Instance.gtUser.Id);
        Pushwoosh.Instance.PostEvent("Register", new Dictionary<string, object>());
#endif
        SendCustomEvent("Register");
    }

    public static void FacebookSignUpTracker()
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.FacebookSignUpTracker();
#endif
        SendCustomEvent("facebook_LogIn");
    }

    public static void IAPTracker(string currency, string amount)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.IAPTracker(currency, amount);
#endif
        SendCustomEvent("IAP", new Dictionary<string, object>(){ {"currency", currency }, { "amount", amount} });
    }

    
    public static void CashInRequestTracker(string error = null)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            AppsFlyerKit.CashInRequestTracker(error);
            Pushwoosh.Instance.PostEvent("CashinRequested", new Dictionary<string, object>());
#endif
        Dictionary<string, object> eventData = null;
        if (!string.IsNullOrEmpty(error))
            eventData = new Dictionary<string, object> { { "error", error } };

        SendCustomEvent("CashInRequest", eventData);
    }

    public static void CashInTracker(bool FirstCashin, float amount)
    {
        if(FirstCashin)
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            AppsFlyerKit.FirstCashInTracker(amount.ToString());
#endif
            SendCustomEvent("FirstCashIn", new Dictionary<string, object>() { { "amount", amount } });
        }
        else
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            AppsFlyerKit.CashInTracker(amount.ToString());
#endif
            SendCustomEvent("CashIn", new Dictionary<string, object>() { { "amount", amount } });
        }
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        Pushwoosh.Instance.SendPurchase(amount.ToString(), (double)amount, "USD");
        Pushwoosh.Instance.PostEvent("CashIn", new Dictionary<string, object>() { { "__amount", (int)amount }, { "__currency", "USD" } });
        if (amount >= 500f)
            Pushwoosh.Instance.PostEvent("CashIn500", new Dictionary<string, object>());
        else if (amount >= 200f)
            Pushwoosh.Instance.PostEvent("CashIn200", new Dictionary<string, object>());
        else if (amount >= 100f)
            Pushwoosh.Instance.PostEvent("CashIn100", new Dictionary<string, object>());
#endif
    }

    public static void CashInFailed(float amount, string error)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.CashInTracker(amount.ToString());
        Pushwoosh.Instance.SetStringTag("FailedDeposit", error);
#endif
        SendCustomEvent("Failed_CashIn", new Dictionary<string, object>() { { "error", error } });
    }

    public static void CashOutTracker(string amount)
    {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        AppsFlyerKit.CashOutTracker(amount);
#endif
        SendCustomEvent("CashOut", new Dictionary<string, object>() { { "amount", amount } });
    }

    public static void OnTotalMatchesChanged(int amount)
    {
        if (amount == TOTAL_MATCHES_TO_NOTIFY)
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            AppsFlyerKit.NotifyTotalMatches();
#endif
            SendCustomEvent("NotifyTotalMatches", new Dictionary<string, object>() {});
        }
    }

    public static int VictoryStreak = 0;
    public static void SetVictory(float womAmount)
    {
        ++VictoryStreak;
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        if(womAmount >= 100f)
            Pushwoosh.Instance.PostEvent("VictoryHighPrice100", new Dictionary<string, object>());
        else if (womAmount >= 20f)
                Pushwoosh.Instance.PostEvent("VictoryHighPrice20", new Dictionary<string, object>());

        if(VictoryStreak == 4)
            Pushwoosh.Instance.PostEvent("VictoryStreak4", new Dictionary<string, object>());
        else if (VictoryStreak == 8)
            Pushwoosh.Instance.PostEvent("VictoryStreak8", new Dictionary<string, object>());
#endif
        SendCustomEvent("Victory", new Dictionary<string, object>() { { "WonAmount", (int)womAmount }, { "Streak", VictoryStreak } });
    }

    public static void SendCustomEvent(string name, Dictionary<string,object> dict = null)
    {
        AnalyticsResult reslut;
        if (dict == null)
            reslut = Analytics.CustomEvent(name);
        else                               
            reslut = Analytics.CustomEvent(name, dict);

        if (reslut != AnalyticsResult.Ok)
            Debug.LogError(name +" event was not sent correctly " + reslut.ToString());
    }

#endregion Tracking Functions
}
