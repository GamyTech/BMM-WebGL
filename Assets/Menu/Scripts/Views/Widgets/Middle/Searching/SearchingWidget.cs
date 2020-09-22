using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;
using GT.User;

public class SearchingWidget : Widget
{
    public SearchingProfilePictureView localPlayer;
    public RemoteSearchingProfileView remotePlayer;

    public Text searchingText;
    public Text betsText;
    public Button cancelButton;
    public GameObject tipsObject;
    public Text tipsText;

    private bool isTourney;

    public override void EnableWidget()
    {
        base.EnableWidget();

        cancelButton.interactable = true;

        isTourney = TourneyController.Instance.IsInNotFinishedTourney();

        WebSocketKit.Instance.ServiceEvents[ServiceId.IsReady] += WebSocketKit_IsReady;
        WebSocketKit.Instance.AckEvents[RequestId.SearchMatch] += WebSocketKit_SearchMatch;
        WebSocketKit.Instance.ServiceEvents[ServiceId.WasRemovedFromMatchSearch] += WebSocketKit_OnSearchCanceled;

        WebSocketKit.Instance.ServiceEvents[ServiceId.IsTourneyReady] += WebSocketKit_IsReady;
        WebSocketKit.Instance.AckEvents[RequestId.SearchTourneyhMatch] += WebSocketKit_SearchMatch;
        WebSocketKit.Instance.AckEvents[RequestId.CancelTourneySearch] += WebSocketKit_OnTourneySearchCanceled;

        SetPlayerProfile(UserController.Instance.gtUser);
        searchingText.text = Utils.LocalizeTerm("Searching for a challenger");
        
        if (isTourney)
            betsText.gameObject.SetActive(false);
        else
            betsText.text = Utils.LocalizeTerm("Bets") + ": " + UserController.Instance.searchingBets;

        tipsObject.SetActive(false);
        remotePlayer.StartSpining();
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.ServiceEvents[ServiceId.IsReady] -= WebSocketKit_IsReady;
        WebSocketKit.Instance.AckEvents[RequestId.SearchMatch] -= WebSocketKit_SearchMatch;
        WebSocketKit.Instance.ServiceEvents[ServiceId.WasRemovedFromMatchSearch] -= WebSocketKit_OnSearchCanceled;

        WebSocketKit.Instance.ServiceEvents[ServiceId.IsTourneyReady] -= WebSocketKit_IsReady;
        WebSocketKit.Instance.AckEvents[RequestId.SearchTourneyhMatch] -= WebSocketKit_SearchMatch;
        WebSocketKit.Instance.AckEvents[RequestId.CancelTourneySearch] -= WebSocketKit_OnTourneySearchCanceled;

        base.DisableWidget();
    }

    private void Update()
    {
        if (TourneyController.Instance.IsInTourney() && cancelButton.interactable)
            if (TourneyController.Instance.GetSecondsLeft() <= TourneyController.Instance.matchesClosedTime)
                CancelButton();
    }

    private void SetPlayerProfile(GTUser user)
    {
        localPlayer.SetProfilePic(user.Avatar);
        localPlayer.SetLuckyItem(user.LuckyItemSprite);
        localPlayer.SetTexts(user.UserName);
    }

    private void WebSocketKit_IsReady(Service service)
    {
        cancelButton.interactable = false;
        OpponentFoundService foundService = service as OpponentFoundService;
        searchingText.text = Utils.LocalizeTerm("Starting Game");

        if (!isTourney)
            betsText.text = Utils.LocalizeTerm("Bet") + ": " + Wallet.MatchKindToPrefix(foundService.MatchKind) + foundService.bet;

        remotePlayer.StopSpining("", null, null);

        UserController.Instance.gtUser.MatchHistoryData.SetNeedUpdate();
        MenuSoundController.Instance.Play(Enums.MenuSound.Matched);
    }

    private void WebSocketKit_OnSearchCanceled(Service service)
    {
        MenuSoundController.Instance.Stop();
        PageController.Instance.ChangePage(Enums.PageId.Home);
    }

    private void WebSocketKit_OnTourneySearchCanceled(Ack ack)
    {
        if (ack.Code == WSResponseCode.OK)
        {
            MenuSoundController.Instance.Stop();
            PageController.Instance.ChangePage(Enums.PageId.TourneyProgress);
        }
    }

    private void WebSocketKit_SearchMatch(Ack ack)
    {
        if (ack.Code == WSResponseCode.InMaintenance)
            PopupController.Instance.ShowPopup(Enums.PopupId.Maintenance);
    }

    #region Input
    public void CancelButton()
    {
        cancelButton.interactable = false;
        searchingText.text = Utils.LocalizeTerm("Canceling");        
        WebSocketKit.Instance.SendRequest(isTourney ? RequestId.CancelTourneySearch : RequestId.CancelSearch);
    }
    #endregion Input
}
