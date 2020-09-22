using UnityEngine;
using System.Collections.Generic;
using GT.Notifications;
using GT.User;

public class NotificationSystemController : MonoBehaviour
{
    private static NotificationSystemController m_instance;
    public static NotificationSystemController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<NotificationSystemController>()); }
        private set { m_instance = value; }
    }

    private Dictionary<Enums.NotificationId, Notification> Notifications = new Dictionary<Enums.NotificationId, Notification>();

    void OnEnable()
    {
        Instance = this;
        InitNotifications();
        InitUserRelatedNotifications(UserController.Instance.gtUser);
        UserController.OnGTUserChanged += UserController_OnGTUserChanged;
    }

    void OnDisable()
    {
        Notifications.Clear();
        Instance = null;
        UserController.OnGTUserChanged -= UserController_OnGTUserChanged;
    }

    #region User Functions
    public void RegisterForNotification(Enums.NotificationId notification, NotificationCountChanged notificationEvent)
    {
        if (Notifications.ContainsKey(notification))
            Notifications[notification].OnNotificationCountChanged += notificationEvent;
    }

    public void RegisterForNotification(Enums.NotificationId[] notification, NotificationCountChanged notificationEvent)
    {
        for(int x = 0; x < notification.Length; ++x)
            Notifications[notification[x]].OnNotificationCountChanged += notificationEvent;
    }

    public void UnregisterForNotification(Enums.NotificationId notification, NotificationCountChanged notificationEvent)
    {
        if (Notifications.ContainsKey(notification))
            Notifications[notification].OnNotificationCountChanged -= notificationEvent;
    }

    public void UnregisterForNotification(Enums.NotificationId[] notification, NotificationCountChanged notificationEvent)
    {
        for (int x = 0; x < notification.Length; ++x)
            Notifications[notification[x]].OnNotificationCountChanged -= notificationEvent;
    }

    public int GetNotificationCount(Enums.NotificationId id)
    {
        return Notifications[id].notificationCount;
    }

    public int GetNotificationCount(Enums.NotificationId[] id)
    {
        int total = 0;
        for (int x = 0; x < id.Length; ++x)
            total += Notifications[id[x]].notificationCount;
        return total;
    }

    public void ForceUpdate(Enums.NotificationId id)
    {
        Notifications[id].ForceUpdate();
    }
    #endregion User Functions

    #region Aid Functions
    private void InitNotifications()
    {
        Notifications.Add(Enums.NotificationId.Cashin, new CashinNotification(this));
        Notifications.Add(Enums.NotificationId.Bonus, new BonusNotification(this));
        Notifications.Add(Enums.NotificationId.History, new HistoryNotification());
    }

    private void InitUserRelatedNotifications(GTUser user)
    {
        foreach (var notification in Notifications)
        {
            if(notification.Value is IUserNotification)
                ((IUserNotification)(notification.Value)).SetUser(user);
        }
    }
    #endregion Aid Functions

    #region Events
    private void UserController_OnGTUserChanged(GTUser newValue)
    {
        InitUserRelatedNotifications(newValue);
    }
    #endregion Events
}
