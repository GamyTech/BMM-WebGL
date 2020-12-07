using System.Collections.Generic;
using UnityEngine;

public class MenuSceneController : MonoBehaviour
{
    private static MenuSceneController m_instance;
    public static MenuSceneController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<MenuSceneController>()); }
        private set { m_instance = value; }
    }

    void OnDisable()
    {
        if (TourneyController.Instance != null)
            TourneyController.Instance.OnTourneyListChanged -= GTUser_OnTourneyListChanged;
        m_instance = null;
    }

    public static void InitializeScene()
    {
        StartScene();
    }

    private static void StartScene()
    {
        GoToStartingPage();

        LoadingController.Instance.HideSceneLoading();
        LoadingController.Instance.HidePageLoading();
    }

    public static void GoToStartingPage()
    {
        if (UserController.Instance.gtUser == null)
        {
            if (NetworkController.Instance.IsClientGame())
                PageController.Instance.ChangePage(Enums.PageId.WaitingForLogin);
            else 
                PageController.Instance.ChangePage(Enums.PageId.Register);
        }
        else if (UserController.Instance.WatchedHistoryMatch != null)
        {
            PageController.OnPageChanged += OpenViewedMatchData;
            PageController.Instance.ChangePage(Enums.PageId.PreviewMatchHistory);
        }
        else
        {
            if (PushNotificationKit.HasPendingPage)
                PushNotificationKit.OpenPendingPage();
            else if (!TutorialController.Instance.StartTutorial())
            {
                PageController.OnPageChanged += OnPageLoaded;
                PageController.Instance.ChangePage(Enums.PageId.Home);
                PageController.Instance.ForceReloading();
            }
        }
    }

    private static void OpenViewedMatchData(Page page)
    {
        PageController.OnPageChanged -= OpenViewedMatchData;
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.MatchDataPopup);
    }
     
    private static void OnPageLoaded(Page page)
    {
        PageController.OnPageChanged -= OnPageLoaded;
        CheckAndShowStartPagePopup();
    }

    private static void CheckAndShowStartPagePopup()
    {
        if (!TourneyController.Instance.CheckAndShowTourneyMenu())
        {
            if (TourneyController.Instance.tourneysList == null)
                TourneyController.Instance.OnTourneyListChanged += GTUser_OnTourneyListChanged;
            else
                UserController.Instance.CheckAndShowUserVerification();
        }
    }

    private static void GTUser_OnTourneyListChanged(List<Tourney> newValue)
    {
        TourneyController.Instance.OnTourneyListChanged -= GTUser_OnTourneyListChanged;
        CheckAndShowStartPagePopup();
    }
}
