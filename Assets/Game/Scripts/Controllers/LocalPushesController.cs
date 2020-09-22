using System;
using UnityEngine;

public class LocalPushesController : MonoBehaviour
{
    public int BeginHour = 8;
    public int EndHour = 24;

    private static bool isOn = false;

    PushNotificationKit.LocalPushType pushType;
    private int waitingTime;

    public static void Init()
    {
        isOn = true;
        PushNotificationKit.CancelAllLocalPushes();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!isOn)
            return;

        if (pause)
            ScheduleNextLocalPush();
        else
            PushNotificationKit.CancelAllLocalPushes();
    }

    private void OnApplicationQuit()
    {
        if (!isOn)
            return;
        ScheduleNextLocalPush();
    }

    private void ScheduleNextLocalPush()
    {
        PushNotificationKit.CancelAllLocalPushes();

        waitingTime = int.MaxValue;
        if (TourneyController.Instance.IsInTourney() && TourneyController.Instance.ongoingTourney.IsInPreTourney)
        {
            waitingTime = (int)Mathf.Max(0, TourneyController.Instance.GetSecondsLeft()); 
            pushType = PushNotificationKit.LocalPushType.TourneyReminder;
            ScheduleIfPossible(pushType, GetMessageForType(pushType), waitingTime);
        }
    }

    private void ScheduleIfPossible(PushNotificationKit.LocalPushType type, string message, float seconds)
    {
        DateTime pushTime = DateTime.Now.AddSeconds(seconds);
        if (!string.IsNullOrEmpty(message) && pushTime.Hour >= BeginHour && pushTime.Hour < EndHour)
        { 
            try
            {
                PushNotificationKit.ScheduleLocalPush(type, message, (int)seconds);
            }
            catch (Exception e)
            {
                Debug.LogError("Push scheduling error! " + e);
            }
        }
    }

    private string GetMessageForType(PushNotificationKit.LocalPushType pushType)
    {
        string message = "";
        switch (pushType)
        {
            case PushNotificationKit.LocalPushType.TourneyReminder:
                message = Utils.LocalizeTerm("The tourney you are registered to has started!");
                break;
        }
        return message;
    }
}
