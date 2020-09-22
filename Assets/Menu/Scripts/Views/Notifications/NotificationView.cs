using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotificationView : MonoBehaviour
{
    
    public List<Enums.NotificationId> NotificationId = new List<Enums.NotificationId>();
    public List<GT.Notifications.NotificationCountChanged> notificationCountChanged = new List<GT.Notifications.NotificationCountChanged>();
    public FadingElement fadingElement;
    public Text text;

    private List<int> m_notificationCount = new List<int>();

    void OnEnable()
    {
        if (NotificationSystemController.Instance == null)
            return;

        for(int x = 0; x < NotificationId.Count; ++x)
        {
            int index = x;
            m_notificationCount.Add(0);
            notificationCountChanged.Add(i => { OnNotificationChanged(index, i, false); });
            NotificationSystemController.Instance.RegisterForNotification(NotificationId[x], notificationCountChanged[x]);
            OnNotificationChanged(index, NotificationSystemController.Instance.GetNotificationCount(NotificationId[x]), true);
        }
    }

    void OnDisable()
    {
        if (NotificationSystemController.Instance != null)
            for (int x = 0; x < NotificationId.Count; ++x)
                NotificationSystemController.Instance.UnregisterForNotification(NotificationId[x], notificationCountChanged[x]);
        Reset();
    }

    private void Reset()
    {
        text.text = string.Empty;
        fadingElement.FadeOut(true);
    }

    private void OnNotificationChanged(int index, int count, bool instant = false)
    {
        int total = 0;
        m_notificationCount[index] = count;
        for (int x = 0; x < m_notificationCount.Count; ++x)
            total += m_notificationCount[x];

        text.text = total > 9 ? "9+" : total.ToString();
        if (total > 0)
            fadingElement.FadeIn(instant);
        else
            fadingElement.FadeOut(instant);
    }
}
