using GT.Websocket;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TourneyController : MonoBehaviour
{
    private static TourneyController m_instance;
    public static TourneyController Instance { get { return m_instance ?? (m_instance = FindObjectOfType<TourneyController>()); } }

    #region Delegates And Events
    public delegate void Changed<T>(T newValue);
    public event Changed<List<Tourney>> OnTourneyListChanged = (i) => { };
    public event Changed<OngoingTourneyDetails> OnOngoingTourneyChanged = (i) => { };
    public event Changed<Tourney> OnTourneyChanged = (i) => { };
    #endregion Delegates And Events

    #region Editor options
    public float warningTime;
    public float matchesClosedTime;
    public bool showWelocomeMessage;
    #endregion Editor options

    private static List<Tourney> m_tourneysList;
    public List<Tourney> tourneysList
    {
        get { return m_tourneysList; }
        set
        {
            if (Utils.SetProperty(ref m_tourneysList, value) && OnTourneyListChanged != null)
                OnTourneyListChanged(m_tourneysList);
        }
    }

    private static OngoingTourneyDetails m_ongoingTourney;
    public OngoingTourneyDetails ongoingTourney
    {
        get { return m_ongoingTourney; }
        set
        {
            if (Utils.SetProperty(ref m_ongoingTourney, value) && OnOngoingTourneyChanged != null)
            {
                if (value != null)
                {
                    activeTourneyId = value.TourneyId;
                    ongoingTourneyRoom = Tourney.MakeTempInfoFrom(value);
                }
                else
                    GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.TourneyId);
                OnOngoingTourneyChanged(m_ongoingTourney);
            }
        }
    }
    public Tourney ongoingTourneyRoom { get; protected set;}

    public string activeTourneyId;

    void Start()
    {
        m_instance = this;

        WebSocketKit.Instance.AckEvents[RequestId.Login] += SaveInitialTourneyData;
        WebSocketKit.Instance.AckEvents[RequestId.GetTourneysInfo] += OnGetTourneysInfo;
        WebSocketKit.Instance.AckEvents[RequestId.RegisterTourney] += OnRegisterTourney;
        WebSocketKit.Instance.AckEvents[RequestId.UnRegisterTourney] += OnUnregisterTourney;
        WebSocketKit.Instance.AckEvents[RequestId.GetSpecificTourneyDetails] += UpdateOngoingTourney;
        WebSocketKit.Instance.ServiceEvents[ServiceId.TourneysStatus] += UpdateTourneysStatus;
        WebSocketKit.Instance.ServiceEvents[ServiceId.UpdateTourneyDetails] += UpdateTourneyDetails;
        WebSocketKit.Instance.ServiceEvents[ServiceId.StartPreTourney] += StartPreTourney;
        WebSocketKit.Instance.ServiceEvents[ServiceId.StartTourney] += StartTourney;
        WebSocketKit.Instance.ServiceEvents[ServiceId.CloseTourney] += CloseTourney;
    }

    private void OnDestroy()
    {
        m_instance = null;
        WebSocketKit.Instance.AckEvents[RequestId.Login] -= SaveInitialTourneyData;
        WebSocketKit.Instance.AckEvents[RequestId.GetTourneysInfo] -= OnGetTourneysInfo;
        WebSocketKit.Instance.AckEvents[RequestId.RegisterTourney] -= OnRegisterTourney;
        WebSocketKit.Instance.AckEvents[RequestId.UnRegisterTourney] -= OnUnregisterTourney;
        WebSocketKit.Instance.AckEvents[RequestId.GetSpecificTourneyDetails] -= UpdateOngoingTourney;
        WebSocketKit.Instance.ServiceEvents[ServiceId.TourneysStatus] -= UpdateTourneysStatus;
        WebSocketKit.Instance.ServiceEvents[ServiceId.UpdateTourneyDetails] -= UpdateTourneyDetails;
        WebSocketKit.Instance.ServiceEvents[ServiceId.StartPreTourney] -= StartPreTourney;
        WebSocketKit.Instance.ServiceEvents[ServiceId.StartTourney] -= StartTourney;
        WebSocketKit.Instance.ServiceEvents[ServiceId.CloseTourney] -= CloseTourney;
    }

    #region Events
    private void SaveInitialTourneyData(Ack ack)
    {
        LoginAck result = ack as LoginAck;
        if (result.OngoingTourney != null)
        {
            if (result.OngoingTourney.isRegisteredOnly)
                activeTourneyId = result.OngoingTourney.TourneyId;
            else
                ongoingTourney = result.OngoingTourney;
        }
    }

    private void OnGetTourneysInfo(Ack ack)
    {
        if (ack.Code == WSResponseCode.OK && ack is GetTourneysInfoAck)
            tourneysList = (ack as GetTourneysInfoAck).Tourneys;
    }

    private void UpdateTourneysStatus(Service service)
    {
        if (service is TourneysStatusService)
            tourneysList = (service as TourneysStatusService).Tourneys;
    }

    private void UpdateOngoingTourney(Ack ack)
    {
        if (ack.Code == WSResponseCode.OK)
        {
            if (ongoingTourney != null)
            {
                ongoingTourney.Update(ack.Data);
                OnOngoingTourneyChanged(ongoingTourney);
            }
        }
    }

    private void UpdateTourneyDetails(Service service)
    {
        if (service.Code == WSResponseCode.OK && service is UpdateTourneyDetailsService)
        {
            UpdateTourneyDetailsService result = service as UpdateTourneyDetailsService;
            for (int i = 0; i < tourneysList.Count; i++)
            {
                if (tourneysList[i].TourneyId == result.TourneyId)
                {
                    tourneysList[i].Update(result.TourneyData);
                    OnTourneyChanged(tourneysList[i]);
                    return;
                }
            }
        }
    }

    private void StartPreTourney(Service service)
    {
        if (service.Code == WSResponseCode.OK && service is StartPreTourneyService)
        {
            SettingsController.Instance.Vibrate();
            StartPreTourneyService result = service as StartPreTourneyService;
            ongoingTourney = result.PreTourney;
            Tourney tourney = GetTourneyById(result.PreTourney.TourneyId);
            ongoingTourney.CopyMoneyDataFrom(tourney);
        }
    }

    private void StartTourney(Service service)
    {
        if (service.Code == WSResponseCode.OK)
        {
            SettingsController.Instance.Vibrate();
            ongoingTourney.Start();
            PageController.Instance.ChangePage(Enums.PageId.TourneyProgress);
        }
    }

    private void CloseTourney(Service service)
    {
        SettingsController.Instance.Vibrate();
        if (ongoingTourney == null) return;

        ongoingTourney.Update(service.Data, true);

        if (SceneController.Instance.GetLoadedSceneName() == SceneController.SceneName.Menu)
            ShowLastTourneyResults();
    }

    private void OnRegisterTourney(Ack ack)
    {
        LoadingController.Instance.HidePageLoading();
        RegisterTourneyAck result = ack as RegisterTourneyAck;
        if (result == null)
        {
            PopupController.Instance.ShowSmallPopup("Error", new string[] { "Please try again" });
            return;
        }
        if (!result.IsSuccess)
        {
            ShowRegisterError(result);
            return;
        }
        UserController.Instance.gtUser.UpdateWallet(result.Wallet);

        Tourney tourney = GetTourneyById(activeTourneyId);
        if (tourney != null && tourney.RegisteredUsers.IndexOf(UserController.Instance.gtUser.Id) < 0)
            tourney.RegisteredUsers.Add(UserController.Instance.gtUser.Id);

        ShowRegisteredPopup(tourney != null ? tourney.BuyIn : (ongoingTourney != null ? ongoingTourney.BuyInAmount : 0));
    }

    private void OnUnregisterTourney(Ack ack)
    {
        LoadingController.Instance.HidePageLoading();
        RegisterTourneyAck result = ack as RegisterTourneyAck;
        if (result == null)
        {
            PopupController.Instance.ShowSmallPopup("Error", new string[] { "Unexpected error" });
            return;
        }
        if (!result.IsSuccess)
        {
            PopupController.Instance.ShowSmallPopup("Unregistration failed");
            return;
        }
        Tourney tourney = GetTourneyById(activeTourneyId);
        ShowUnregisteredPopup(tourney != null ? tourney.BuyIn : (ongoingTourney != null ? ongoingTourney.BuyInAmount : 0));
    }
    #endregion Events

    #region Popups
    private void ShowRegisteredPopup(float buyIn)
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.MessagePopup, new SmallPopup("Registered", new string[]
            {
                Utils.LocalizeTerm("Confirmed Buy-In of") + ": " + Wallet.CashPostfix + Wallet.AmountToString(buyIn, 2),
                "Please wait for more players for the game to begin"
            }, -1,
            new SmallPopupButton("OK", () =>
            {
                WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyMenuPopup, null, true, false);
            })), true, false);
    }

    private void ShowRegisterError(RegisterTourneyAck result)
    {
        if (result.Code == WSResponseCode.TourneyDoesntExist || result.Code == WSResponseCode.MissingTourneyId)
            PopupController.Instance.ShowSmallPopup("Registration failed", new string[] { "Can't register to this tourney" });
        if (result.Code == WSResponseCode.AlreadyInATourney)
            PopupController.Instance.ShowSmallPopup("Registration failed", new string[] { "You are already registered in a tourney" });
        else
            PopupController.Instance.ShowSmallPopup("Registration failed", new string[] { "Please try again" });
    }

    private void ShowUnregisteredPopup(float buyIn)
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.MessagePopup, new SmallPopup("Unregistered", new string[]
            {
                Utils.LocalizeTerm("Buy-In of") + ": " + Wallet.CashPostfix + Wallet.AmountToString(buyIn, 2),
                "Has been refunded to your account"
            }, 0));
    }
    #endregion Popups

    #region Public Methods
    public bool CheckAndShowTourneyMenu()
    {
        GT.User.GTUser user = UserController.Instance.gtUser;
        if (IsInNotFinishedTourney())
        {
            if (IsInStartedTourney())
                PageController.Instance.ChangePage(Enums.PageId.TourneyProgress);
            else
                WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyMenuPopup, null, true, false);
            return true;
        }
        else if (IsInStartedTourney())
        {
            ShowLastTourneyResults();
            return true;
        }

        if (tourneysList != null)
        {
            Tourney tourney = FindTourneyWithUser(user.Id);
            if (tourney != null)
            {
                activeTourneyId = tourney.TourneyId;
                WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyMenuPopup, null, true, false);
                return true;
            }
            else
            {
                if (showWelocomeMessage && !GTDataManagementKit.GetBoolFromPrefs(Enums.PlayerPrefsVariable.SawWelcomeToTourneys, false))
                {
                    GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.SawWelcomeToTourneys, true.ToString());
                    WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.WelcomeToTourneysPopup);
                    return true;
                }
            }
        }
        return false;
    }

    public Tourney GetTourneyById(string id)
    {
        if (tourneysList != null)
        {
            for (int i = 0; i < tourneysList.Count; i++)
            {
                if (tourneysList[i].TourneyId == id)
                {
                    return tourneysList[i];
                }
            }
        }
        return null;
    }

    public bool IsInStartedTourney()
    {
        return ongoingTourney != null && !ongoingTourney.IsInPreTourney && !ongoingTourney.isRegisteredOnly;
    }

    public bool IsFinishedByTime()
    {
        return ongoingTourney != null && DateTime.UtcNow.Subtract(ongoingTourney.StartDate).TotalSeconds >= ongoingTourney.MaxTime;
    }

    public bool IsInNotFinishedTourney()
    {
        return ongoingTourney != null && !ongoingTourney.IsFinished;
    }

    public bool IsInTourney()
    {
        return ongoingTourney != null;
    }

    public int GetSecondsLeft()
    {
        if (ongoingTourney == null) return 0;
        return (int)Mathf.Max(0, (float)(ongoingTourney.StartDate.AddSeconds(ongoingTourney.MaxTime) - DateTime.UtcNow).TotalSeconds);
    }

    public void ClearTourney()
    {
        ongoingTourney = null;
    }

    public int GetUserScore(OngoingTourneyDetails ongoingTourney, string id)
    {
        if (ongoingTourney != null)
        {
            for (int i = 0; i < ongoingTourney.HighScore.Count; i++)
            {
                if (ongoingTourney.HighScore[i].UserId == id)
                    return ongoingTourney.HighScore[i].Score;
            }
        }
        return 0;
    }

    public int GetUserScore(string id)
    {
        if (ongoingTourney != null)
        {
            for (int i = 0; i < ongoingTourney.HighScore.Count; i++)
            {
                if (ongoingTourney.HighScore[i].UserId == id)
                    return ongoingTourney.HighScore[i].Score;
            }
        }
        return 0;
    }

    public Tourney FindTourneyWithUser(string userId)
    {
        if (tourneysList != null)
        {
            for (int i = 0; i < tourneysList.Count; i++)
            {
                if (tourneysList[i].RegisteredUsers.IndexOf(userId) >= 0)
                {
                    return tourneysList[i];
                }
            }
        }
        return null;
    }

    public KeyValuePair<string, float> FindFirstReward(Dictionary<string, float> rewards)
    {
        float maxPrize = 0;
        KeyValuePair<string, float> best = new KeyValuePair<string, float>();
        foreach (KeyValuePair<string, float> reward in rewards)
        {
            if (reward.Key == "1") return reward;
            if (reward.Value > maxPrize)
            {
                best = reward;
                maxPrize = reward.Value;
            }
        }
        return best;
    }

    public void ShowLastTourneyResults()
    {
        if (ongoingTourney.HighScore.Count > 0)
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyResultsPopup, null, true, false);
        else
            ongoingTourney = null;
    }
    #endregion Public Methods
}
