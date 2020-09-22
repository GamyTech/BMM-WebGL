using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourneyRoomCategoryView : BetRoomCategoryView
{
    public Text BuyIn;
    public Text Prize;
    public Text DurationText;
    public Text FeeText;
    public Text EnrolledText;

    public Animator PlayButtonAnimator;
    public Text PlayButtonLabel;

    private Tourney tourney;
    private bool isRegistered;

    private void OnDestroy()
    {
        if (TourneyController.Instance != null)
            TourneyController.Instance.OnTourneyChanged -= GTUser_OnTourneyChanged;
    }

    private void Start()
    {
        TourneyController.Instance.OnTourneyChanged += GTUser_OnTourneyChanged;
    }

    public void Init(Tourney tourney)
    {
        this.tourney = tourney;

        GameNameText.text = Utils.LocalizeTerm("{0} Players Tournament", tourney.MaxPlayers);
        BuyIn.text = "— " + Utils.LocalizeTerm("Buy in {0}", Wallet.CashPostfix + Wallet.AmountToString(tourney.BuyIn, 2)) + " —";
        KeyValuePair<string, float> firstPrize = TourneyController.Instance.FindFirstReward(tourney.Rewards);
        Prize.text = firstPrize.Key + Utils.LocalizeTerm(Utils.GetNumberPostfix(firstPrize.Key) + " " + Utils.LocalizeTerm("Prize")) + ": " + 
            Wallet.CashPostfix + Wallet.AmountToString(firstPrize.Value, 2);
        DurationText.text = Utils.LocalizeTerm("Tournament Duration {0}", Utils.SecondsToTimeFormat(tourney.DurationSeconds));
        FeeText.text = Utils.LocalizeTerm("Fee") + " " + Wallet.CashPostfix + tourney.Fee + ",";
        EnrolledText.text = Utils.LocalizeTerm("Enrolled") + " " + tourney.CurrentPlayersAmount + "/" + tourney.MaxPlayers;

        PlayButtonAnimator.SetTrigger("HighlightOn");
        PlayButtonLabel.text = Utils.LocalizeTerm("Register");
    }

    private void UpdatePlayButtonView()
    {
    }

    private void Register()
    {
        if (!UserController.Instance.wallet.HaveEnoughMoney(tourney))
        {
            PopupController.Instance.ShowSmallPopup("You dont have enough money to play on this tourney");
            LoadingController.Instance.HidePageLoading();
            return;
        }
        if (UserController.Instance.CheckAndShowUserVerification())
            return;

        UserController.Instance.RegisterToTourney(tourney.TourneyId);
    }

    private void Unregister()
    {
        UserController.Instance.UnregisterToTourney(tourney.TourneyId);
    }

    protected override void InitRooms() { }

    #region Buttons
    public void ShowTourneyInfo()
    {
        if (UserController.Instance.CheckAndShowUserVerification())
            return;
        TourneyController.Instance.activeTourneyId = tourney.TourneyId;
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyMenuPopup, null, true, false);
    }

    public override void Play()
    {
        if (UserController.Instance.CheckAndShowUserVerification())
            return;
        LoadingController.Instance.ShowPageLoading();
        TourneyController.Instance.activeTourneyId = tourney.TourneyId;
        Register();
    }
    #endregion Buttons

    #region Events
    private void updated(Tourney updated)
    {
        if (tourney.TourneyId == updated.TourneyId)
            Init(updated);
    }

    private void GTUser_OnTourneyChanged(Tourney updated)
    {
        if (tourney.TourneyId == updated.TourneyId)
            Init(updated);
    }
    #endregion Events
}
