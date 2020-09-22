using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PushNotificationKit
{
    public static bool HasPendingPage { get { return OpeningPage != Enums.PageId.InitApp; } }
    public static string LaunchedPushID;

    static Enums.PageId OpeningPage = Enums.PageId.InitApp;
    static Dictionary<string, object> OpeningData = new Dictionary<string, object>();
    static private bool Registered = false;

    public static void Init()
    {
        string PushwooshAppCode = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.PushwooshAppCode);
        string GcmProjectNumber = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.GcmProjectNumber, "137852696295");

        if (string.IsNullOrEmpty(PushwooshAppCode))
        {
            switch (AppInformation.GAME_ID)
            {
                case Enums.GameID.Backgammon4Money:
                    PushwooshAppCode = "29112-6C2F4";
                    break;
                case Enums.GameID.BackgammonRoyale:
                    PushwooshAppCode = "96A71-06670";
                    break;
                case Enums.GameID.BackgammonForFriends:
                    PushwooshAppCode = "3C6E0-C6AD2";
                    break;
                default:
                    Debug.Log("No PushwooshAppCode Initiation ( good programmation <3)");
                    return;
            }
        }

        Pushwoosh.ApplicationCode = PushwooshAppCode;
        Pushwoosh.GcmProjectNumber = GcmProjectNumber;
        Pushwoosh.Instance.OnRegisteredForPushNotifications += OnRegisteredForPushNotifications;
        Pushwoosh.Instance.OnFailedToRegisteredForPushNotifications += OnFailedToRegisteredForPushNotifications;

        Register();
    }

    private static void Register()
    {
        Pushwoosh.Instance.RegisterForPushNotifications();
    }

    private static void OnRegisteredForPushNotifications(string token)
    {
        Debug.Log("OnRegisteredForPushNotifications " + token);
        Registered = true;
        GetLaunchNotification();
    }

    private static void OnFailedToRegisteredForPushNotifications(string error)
    {
        Debug.LogError("Failed To Registered For Push Notifications " + error);
    }

    private static void OnPushNotificationsOpened(string payload)
    {
        Debug.Log("OnPushNotificationsOpened : " + payload);
        HandleNotificationAction(payload);
    }

    public static void GetLaunchNotification()
    {
        if (!Registered)
            return;

        Debug.Log("GetLaunchPushNotification");
        LaunchedPushID = string.Empty;
        string payload = null;
#if UNITY_ANDROID && !UNITY_EDITOR
        payload = PushNotificationsAndroid.Instance.GetLaunchNotification();
        PushNotificationsAndroid.Instance.ClearLaunchNotification();
#elif UNITY_IPHONE && !UNITY_EDITOR
        payload = PushNotificationsIOS.Instance.GetLaunchNotification();
        PushNotificationsIOS.Instance.ClearLaunchNotification();
#endif
        LaunchedPushID = HandleNotificationAction(payload);
    }

    private static string HandleNotificationAction(string payload)
    {
        ResetOpeningPage();
        if (string.IsNullOrEmpty(payload))
            return null;

        Debug.Log("HandlePushNotificationAction + " + payload);

        Dictionary<string, object> dict = MiniJSON.Json.Deserialize(payload) as Dictionary<string, object>;
        string CampaignID = null;
        object o;
        if (dict.TryGetValue("pwcid", out o))
        {
            CampaignID = o.ToString();
            Debug.LogError("Push Campaign ID :" + CampaignID);
        }
        else
            Debug.Log("No CAMPAIGN_CODE");

        if (dict.TryGetValue("u", out o))
        {
            Dictionary<string, object> data = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
            Enums.ActionType action;
            if (data.TryGetValue("ActionType", out o) && Utils.TryParseEnum(o, out action))
            {
                switch (action)
                {
                    case Enums.ActionType.OpenPage:
                        OpenPageThroughPayload(data);
                        break;
                    case Enums.ActionType.OpenMessage:
                        OpenMessageThroughPayload(data);
                        break;
                    case Enums.ActionType.OpenUrl:
                        OpenURLThroughPayload(data);
                        break;
                    default:
                        Debug.Log("ActionType is not valid");
                        break;
                }
            }
            else
                Debug.Log("No push ActionType in payload");
        }
        else
            Debug.Log("No userdata");

        return CampaignID;
    }

    private static void ResetOpeningPage()
    {
        OpeningPage = Enums.PageId.InitApp;
        OpeningData.Clear();
    }

    public static void OpenPendingPage()
    {
        if(HasPendingPage && PageController.Instance != null && UserController.Instance.gtUser != null)
        {
            Debug.Log("Open Page throug Notification");
            PageController.Instance.ChangePage(OpeningPage);
            ResetOpeningPage();
        }
    }

    private static void OpenPageThroughPayload(Dictionary<string, object> data)
    {
        object o;
        Enums.PageId pageId;
        if (data.TryGetValue("ActionString", out o) && Utils.TryParseEnum(o, out pageId))
        {
            OpeningPage = pageId;
            if (data.TryGetValue("ActionData", out o))
                OpeningData = o as Dictionary<string, object>;
            OpenPendingPage();
        }
        else
            Debug.Log("No ActionString in payload");
    }

    private static void OpenMessageThroughPayload(Dictionary<string, object> data)
    {
        object o;
        if (data.TryGetValue("ActionString", out o))
            PopupController.Instance.ShowPopup(o as Dictionary<string, object>);
    }

    private static void OpenURLThroughPayload(Dictionary<string, object> data)
    {
        object o;
        if (data.TryGetValue("ActionString", out o))
            Utils.OpenURL(o.ToString());
    }

    #region Local pushes
    public enum LocalPushType
    {
        TourneyReminder
    }
#if UNITY_ANDROID && !UNITY_EDITOR
    private static Dictionary<LocalPushType, List<int>> scheduledPushes = new Dictionary<LocalPushType, List<int>>();
#endif

    public static void ScheduleLocalPush(LocalPushType type, string message, int seconds)
    {
        Debug.Log("Send push: " + type + ": " + message + " in " + seconds + " seconds");
#if UNITY_ANDROID && !UNITY_EDITOR
        List<int> ids;
        if (!scheduledPushes.TryGetValue(type, out ids))
        {
            ids = new List<int>();
            scheduledPushes.Add(type, ids);
        }
        ids.Add(Pushwoosh.Instance.ScheduleLocalNotification(message, seconds, type.ToString()));
#elif UNITY_IOS && !UNITY_EDITOR
        UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
        notification.alertBody = message;
        notification.fireDate = System.DateTime.Now.AddSeconds(seconds);
        notification.applicationIconBadgeNumber = 1;
        notification.userInfo = new Dictionary<string, string>();
        notification.userInfo.Add("type", type.ToString());
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notification);
#endif
    }
    public static void ScheduleLocalPush(LocalPushType type, string message, System.DateTime date)
    {
        ScheduleLocalPush(type, message, (int)date.Subtract(System.DateTime.Now).TotalSeconds);
    }

    public static void CancelLocalPushesOfType(LocalPushType type)
    {
        Debug.Log("Clearing local pushes of type " + type);
#if UNITY_ANDROID && !UNITY_EDITOR
        List<int> ids;
        if (scheduledPushes.TryGetValue(type, out ids))
        {
            for (int i = 0; i < ids.Count; i++)
                Pushwoosh.Instance.ClearLocalNotification(ids[i]);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        UnityEngine.iOS.LocalNotification[] notifications = UnityEngine.iOS.NotificationServices.localNotifications;
        int pushesCount = notifications.Length;
        for (int i = 0; i < pushesCount; i++)
        {
            if ((notifications[i].userInfo as Dictionary<string, string>)["type"] == type.ToString())
                UnityEngine.iOS.NotificationServices.CancelLocalNotification(notifications[i]);
        }
#endif
    }

    public static void CancelAllLocalPushes()
    {
        Debug.Log("Clearing all local pushes");
#if UNITY_ANDROID && !UNITY_EDITOR
        Pushwoosh.Instance.ClearLocalNotifications();
        scheduledPushes = new Dictionary<LocalPushType, List<int>>();
#elif UNITY_IOS && !UNITY_EDITOR
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
    }
    #endregion Local pushes
}
