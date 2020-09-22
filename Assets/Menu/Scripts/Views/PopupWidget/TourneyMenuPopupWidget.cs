using GT.Websocket;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TourneyMenuPopupWidget : TourneyInfoPopupWidget
{
    public Text TourneyTitle;
    
    public Animator PlayButtonAnimator;
    public Text PlayButtonLabel;
    public Button PlayButton;

    public Text TimerText;
    
    private bool isRegistered;

    private bool isOngoing;
    private float secondsLeft;
    private int maxPlayers;

    public override void EnableWidget()
    {
        base.EnableWidget();

        TourneyController.Instance.OnOngoingTourneyChanged += OnOngoingTourneyChanged;
    }

    public override void DisableWidget()
    {
        TourneyController.Instance.OnOngoingTourneyChanged -= OnOngoingTourneyChanged;

        base.DisableWidget();
    }

    protected override void OnEnable()
    {
        UpdatePlayButtonView();
        base.OnEnable();
    }

    private void Update()
    {
        if (isOngoing)
        {
            secondsLeft = Mathf.Max(0, secondsLeft - Time.deltaTime);
            UpdateTimerText();
        }
    }

    protected override void UpdateInfoFrom(Tourney tourney)
    {
        isRegistered = tourney.RegisteredUsers.IndexOf(UserController.Instance.gtUser.Id) >= 0;
        isOngoing = false;
        maxPlayers = tourney.MaxPlayers;
        SetTitle(tourney.MaxPlayers);
        UpdateTourneyInfo(tourney.DurationSeconds, tourney.Fee);
        SetPlayersText(tourney.CurrentPlayersAmount, tourney.MaxPlayers);
        SetTimer();
        UpdatePlayButtonView();
    }

    protected override void UpdateInfoFrom(OngoingTourneyDetails tourney)
    {
        isRegistered = true;
        isOngoing = true;
        secondsLeft = Mathf.Max(0, (float)(tourney.StartDate - DateTime.UtcNow).TotalSeconds);
        maxPlayers = tourney.HighScore.Count;
        SetTitle(tourney.HighScore.Count);
        UpdateTourneyInfo(tourney.MaxTime, tourney.Fee);
        SetPlayersText(tourney.HighScore.Count, tourney.HighScore.Count);
        SetTimer();
        UpdatePlayButtonView();
    }

    private void SetTimer()
    {
        TimerText.gameObject.SetActive(isOngoing);
        if (isOngoing) UpdateTimerText();
    }

    private void SetTitle(int maxPlayers)
    {
        TourneyTitle.text = Utils.LocalizeTerm("{0} Players Tournament", maxPlayers);
    }

    private void UpdateTimerText()
    {
        TimerText.text =
            Utils.LocalizeTerm("{0} Players Tournament", maxPlayers) + " ";
        if (secondsLeft > 0)
            TimerText.text +=
                Utils.LocalizeTerm("will start in") + ": " +
                Utils.SecondsToShortTimeString((int)Mathf.Ceil(secondsLeft));
        else
            TimerText.text += Utils.LocalizeTerm("will start soon");
    }

    private void UpdatePlayButtonView()
    {
        PlayButtonLabel.text = Utils.LocalizeTerm(isRegistered ? "Unregister" : "Register");
        PlayButtonAnimator.SetTrigger(isOngoing ? "Disabled" : ("Highlight" + (isRegistered ? "Off" : "On")));
        PlayButton.interactable = !isOngoing;
        CloseButton.gameObject.SetActive(!isRegistered);
        IsClosable = !isRegistered;
    }

    private void Register()
    {
        Tourney tourney = TourneyController.Instance.GetTourneyById(tourneyId);
        if (tourney == null)
        {
            HidePopup();
            PopupController.Instance.ShowSmallPopup("Not registered", new string[] { "The tourney no longer exists" });
            return;
        }
        if (!UserController.Instance.wallet.HaveEnoughMoney(tourney))
        {
            PopupController.Instance.ShowSmallPopup("You dont have enough money to play on this tourney");
            return;
        }

        UserController.Instance.RegisterToTourney(tourneyId);
    }

    private void Unregister()
    {
        UserController.Instance.UnregisterToTourney(tourneyId);
    }

    #region Input 
    public void Play()
    {
        LoadingController.Instance.ShowPageLoading();
        if (isRegistered)
            Unregister();
        else
            Register();
    }
    #endregion Input

    #region Events
    private void OnOngoingTourneyChanged(OngoingTourneyDetails newValue)
    {
        if (newValue == null)
        {
            HidePopup();
            return;
        }
        if (newValue.IsInPreTourney)
        {
            secondsLeft = Mathf.Max(0, (float)(newValue.StartDate - DateTime.UtcNow).TotalSeconds);
            Tourney tourney = TourneyController.Instance.GetTourneyById(tourneyId);
            if (tourney != null) newValue.CopyMoneyDataFrom(tourney);
            UpdateInfoFrom(newValue);
        }
    }
    #endregion Events
}
