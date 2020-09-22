using UnityEngine;
using UnityEngine.UI;
using GT.User;
using GT.Websocket;

public class MatchDataPopupWidget : PopupWidget
{    
    public Text WonAmountText;
    public Text LoyaltyAmountText;

    public Text MiddleStatusText;

    public Image UserImage;
    public Text UserNameText;
    public GameObject UserWin;

    public Image OpponentImage;
    public Text OpponentNameText;
    public GameObject OpponentWin;

    public Text EntryFeeText;
    public Text DateText;
    public Text MatchIDText;

    public Text PlayerMoveCount;
    public Text PlayerDoubleDice;
    public Text PlayerHitCheckers;
    public Text PlayerSixFives;

    public Text OpponentMoveCount;
    public Text OpponentDoubleDice;
    public Text OpponentHitCheckers;
    public Text OpponentSixFives;

    public GameObject MatchStatsView;
    public GameObject WatchReplayView;

    public override void EnableWidget()
    {
        base.EnableWidget();

        MatchHistoryData match = UserController.Instance.WatchedHistoryMatch;
        if (match == null)
            return;
        
        WonAmountText.text = Wallet.MatchKindToPrefix(match.Kind) + Wallet.AmountToString(match.Win, 2);
        Wallet.SetGradientForMatchKind(match.Kind, WonAmountText.GetComponent<UnityEngine.UI.Gradient>());
        LoyaltyAmountText.text = Wallet.LoyaltyPointsPostfix + match.Loyalty;

        UserImage.sprite = UserController.Instance.gtUser.Avatar;
        UserNameText.text = UserController.Instance.gtUser.UserName;
        UserWin.SetActive(match.Status == MatchHistoryData.MatchSatus.Won);

        OpponentNameText.text = match.OpponentName;
        OpponentWin.SetActive(match.Status == MatchHistoryData.MatchSatus.Lost);
        
        SetMiddleTextIfNeeded(match.Status);

        EntryFeeText.text = Wallet.CashPostfix + " " + Wallet.AmountToString(match.Fee, 2);
        DateText.text = match.StartDate.ToString("dd/MM/yyyy");
        MatchIDText.text = match.Id.ToString();

        ShowMatchStats(match);
        WatchReplayView.SetActive(false);
        WebSocketKit.Instance.AckEvents[RequestId.GetMatchHistoryMoves] += WebSocketKit_GetMatchHistoryMovesAck;
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
        WebSocketKit.Instance.AckEvents[RequestId.GetMatchHistoryMoves] -= WebSocketKit_GetMatchHistoryMovesAck;
    }

    protected override void OnDestroy()
    {
        WebSocketKit.Instance.AckEvents[RequestId.GetMatchHistoryMoves] -= WebSocketKit_GetMatchHistoryMovesAck;
    }

    public override void OnPopupWidgetShown()
    {
        base.OnPopupWidgetShown();
        UserController.Instance.WatchedHistoryMatch.OpponentAvatar.LoadImage(this, s => OpponentImage.sprite = s, AssetController.Instance.DefaultAvatar);
    }

    private void SetMiddleTextIfNeeded(MatchHistoryData.MatchSatus status)
    {
        if (status == MatchHistoryData.MatchSatus.Lost || status == MatchHistoryData.MatchSatus.Won)
            MiddleStatusText.gameObject.SetActive(false);
        else
            MiddleStatusText.text = Utils.LocalizeTerm(status.ToString());
    }

    private void ShowMatchStats(MatchHistoryData match)
    {
        MatchStatsView.SetActive(false);
    }

    #region Inputs
    protected override void HidePopup()
    {
        UserController.Instance.WatchedHistoryMatch = null;
        UserController.Instance.WatchedReplayMatch = null;
        base.HidePopup();
    }

    public void ReplayGame()
    {
        UserController.Instance.GetReplayInfo(MatchIDText.text);
        LoadingController.Instance.ShowPageLoading(WatchReplayView.transform as RectTransform);
    }

    private void WebSocketKit_GetMatchHistoryMovesAck(Ack service)
    {
        GetMatchHistoryMovesAck getMovesAck = service as GetMatchHistoryMovesAck;
        LoadingController.Instance.HidePageLoading();

        if (service.Code == WSResponseCode.OK && !string.IsNullOrEmpty(getMovesAck.MatchData.Winner) && getMovesAck.MatchData.GameLogs.Count > 0)
        {
            UserController.Instance.WatchedReplayMatch = getMovesAck.MatchData;
            SceneController.Instance.ChangeScene(SceneController.SceneName.Replay);
        }
        else
            PopupController.Instance.ShowSmallPopup("this match is not visisble yet.");
    }
    #endregion Inputs
}
