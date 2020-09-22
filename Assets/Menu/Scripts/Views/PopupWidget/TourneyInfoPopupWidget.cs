using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourneyInfoPopupWidget : PopupWidget
{
    public Text TourneyId;
    public Text DurationText;
    public Text FeeText;
    public Text PlayersText;

    public ScrollRect PrizeList;
    public Text PrizeLine;

    public Animator CopiedBubble;

    protected string tourneyId;

    public override void DisableWidget()
    {
        if (TourneyController.Instance != null)
            TourneyController.Instance.OnTourneyChanged -= GTUser_OnTourneyChanged;
        base.DisableWidget();
    }

    public override void EnableWidget()
    {
        base.EnableWidget();
        TourneyController.Instance.OnTourneyChanged += GTUser_OnTourneyChanged;
        
        if (TourneyController.Instance.ongoingTourney == null)
        {
            tourneyId = TourneyController.Instance.activeTourneyId;
            UpdateInfoFrom(TourneyController.Instance.GetTourneyById(tourneyId));
        }
        else
        {
            tourneyId = TourneyController.Instance.ongoingTourney.TourneyId;
            UpdateInfoFrom(TourneyController.Instance.ongoingTourney);
        }
        
        ShowPrizeList();
    }

    private void ShowPrizeList()
    {
        Dictionary<string, float> rewards = null;
        if (TourneyController.Instance.ongoingTourney != null)
            rewards = TourneyController.Instance.ongoingTourney.Rewards;
        if (rewards == null)
        {
            Tourney tourney = TourneyController.Instance.GetTourneyById(TourneyController.Instance.activeTourneyId);
            if (tourney != null)
                rewards = tourney.Rewards;
        }
        if (rewards == null)
            rewards = new Dictionary<string, float>();

        RemoveAllPrizes();
        foreach (KeyValuePair<string, float> rewardInfo in rewards)
        {
            Text prizeListLine = Instantiate(PrizeLine, PrizeList.content);
            prizeListLine.text = rewardInfo.Key + Utils.LocalizeTerm(Utils.GetNumberPostfix(rewardInfo.Key) + " " + Utils.LocalizeTerm("Prize")) + ": " +
                Wallet.CashPostfix + Wallet.AmountToString(rewardInfo.Value, 2);
        }
    }

    private void RemoveAllPrizes()
    {
        for (int i = PrizeList.content.childCount - 1; i >= 0; i--)
            Destroy(PrizeList.content.GetChild(i).gameObject);
    }

    protected virtual void UpdateInfoFrom(Tourney tourney)
    {
        UpdateTourneyInfo(tourney.DurationSeconds, tourney.Fee);
        SetPlayersText(tourney.CurrentPlayersAmount, tourney.MaxPlayers);
    }

    protected virtual void UpdateInfoFrom(OngoingTourneyDetails tourney)
    {
        UpdateTourneyInfo(tourney.MaxTime, tourney.Fee);
        SetPlayersText(tourney.HighScore.Count, tourney.HighScore.Count);
    }

    protected virtual void UpdateTourneyInfo(int duration, float fee)
    {
        if (TourneyId != null) TourneyId.text = "#" + TourneyController.Instance.ongoingTourney.TourneyDbId;
        DurationText.text = Utils.LocalizeTerm("Tournament Duration {0}", Utils.SecondsToTimeFormat(duration));
        FeeText.text = Utils.LocalizeTerm("Fee") + " " + Wallet.CashPostfix + Wallet.AmountToString(fee, 2);
    }

    protected void SetPlayersText(int current, int max)
    {
        PlayersText.text = Utils.LocalizeTerm("Tournament will start when {0} players will be registered (registered {1}/{2}).",
                new object[] { max, current, max });
    }
    
    #region Input 
    public void ShowInfo()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyRulesPopup, null, true, false);
    }

    public void CopyID()
    {
        GUIUtility.systemCopyBuffer = tourneyId;
        CopiedBubble.SetTrigger("Show");
    }
    #endregion Input

    #region Events
    private void GTUser_OnTourneyChanged(Tourney updated)
    {
        if (tourneyId == updated.TourneyId)
        {
            UpdateInfoFrom(updated);
        }
    }
    #endregion Events
}
