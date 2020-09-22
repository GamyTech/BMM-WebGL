using GT.Websocket;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using GT.User;

public class ChallengePopupWidget : PopupWidget
{
    public ObjectPool BetPool;
    public GameObject InfoPanel;
    public RoomCollectionView RoomsView;
    public Image profilePic;
    public Image statusIndicator;
    public Text nameText;
    public Text betHeadline;
    public SmoothLayoutElement betsSection;
    public Button playButton;
    public Text playButtonText;
    public Slider Slider;
    public Text loadingText;

    private const float TIMEOUT_INVITEE_TIME = 35f;
    private const float TIMEOUT_INVITED_TIME = 30f;

    private ChallengeableFriend friend;
    private bool toStartTimer = false;
    private IEnumerator m_loadingRoutine;

    private bool toUpdateBets;
    private bool betsState;
    private bool canceled;
    private bool inviting;

    public override void EnableWidget()
    {
        base.EnableWidget();

        friend = UserController.Instance.fbUser.ChallengedFriend;
        statusIndicator.color = FBUser.StatusToColorDict[friend.Status];
        nameText.text = friend.Name;

        HandleBets(true);
        InfoPanel.SetActive(friend.Invite);
        if(friend.Invite)
            ShowState("Challenge you for a game", true);

        friend.OnFriendStatusChanged += Friend_OnFriendStatusChanged;
        WebSocketKit.Instance.ServiceEvents[ServiceId.FriendInviteToPlayAnswer] += WebSocket_ReceivedDecline;
    }

    public override void OnPopupWidgetShown()
    {
        friend.Picture.LoadImage(this, s => { profilePic.sprite = s; }, AssetController.Instance.DefaultAvatar);
    }

    public override void DisableWidget()
    {
        if (friend != null)
            friend.OnFriendStatusChanged -= Friend_OnFriendStatusChanged;
        
        WebSocketKit.Instance.ServiceEvents[ServiceId.FriendInviteToPlayAnswer] -= WebSocket_ReceivedDecline;

        base.DisableWidget();
    }

    protected override void HidePopup()
    {
        if((inviting || friend.Invite) && !canceled)
            UserController.Instance.ChallengeFriendAnswer(false, friend.GamytechId);

        base.HidePopup();
    }

    void Update()
    {
        if (toUpdateBets)
        {
            toUpdateBets = false;
            betsSection.SetVerticalStateUsingLayoutSize(betsState ? SmoothLayoutElement.State.Max : SmoothLayoutElement.State.Zero);
        }

        if (toStartTimer)
        {
            toStartTimer = false;
            m_loadingRoutine = RunLoadingCoroutine();
            StartCoroutine(m_loadingRoutine);
        }
    }

    private void HandleBets(bool isOpen)
    {
        if (isOpen)
        {
            betHeadline.text = Utils.LocalizeTerm(friend.Invite ? "Select your bet" : "Select one or more bets");

            for (int i = 0; i < friend.ValidRooms.Count; i++)
                friend.ValidRooms[i].Selected = i >= friend.ValidRooms.Count - 4;

            RoomsView.Init(friend.ValidRooms, !friend.Invite);
        }
        betsState = isOpen;
        toUpdateBets = true;
    }

    public void ShowState(string text, bool showLoading = false)
    {
        if (m_loadingRoutine != null)
        {
            StopCoroutine(m_loadingRoutine);
            m_loadingRoutine = null;
        }

        InfoPanel.SetActive(true);
        toStartTimer = showLoading;
        Slider.gameObject.SetActive(showLoading);

        loadingText.text = text;
    }

    private IEnumerator RunLoadingCoroutine()
    {
        float time = friend.Invite ? TIMEOUT_INVITED_TIME : TIMEOUT_INVITEE_TIME;
        float counter = 0;
        while (counter < time)
        {
            yield return null;
            Slider.value = counter / time;
            counter += Time.deltaTime;
        }
        m_loadingRoutine = null;

        canceled = true;
        ShowState(Utils.LocalizeTerm("Challenge Timeout") + "\n" + Utils.LocalizeTerm("Unable to connect with your friend"));
        CloseButton.gameObject.SetActive(false);
        HandleBets(false);
    }

    #region Events
    private void Friend_OnFriendStatusChanged(ChallengeableFriend.FriendStatus status)
    {
        statusIndicator.color = FBUser.StatusToColorDict[status];
    }

    private void WebSocket_ReceivedDecline(Service service)
    {
        Debug.Log("got WebSocket_ReceivedDecline");
        ShowState(Utils.LocalizeTerm(canceled? "Challenge Canceled" : "Challenge Declined"));
        playButtonText.text = Utils.LocalizeTerm("OK");
        canceled = true;
        HandleBets(false);
    }
    #endregion Events

    #region Inputs
    public void PlayButton()
    {
        HandleBets(false);
        CloseButton.gameObject.SetActive(false);

        if(canceled)
            WidgetController.Instance.HideWidgetPopups();
        else if (!inviting)
        {
            string selectedBets = RoomsView.GetCurrentSelectedAmounts();
            Debug.Log("Selected " + selectedBets);
            SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
            string selectedItemsData = MiniJSON.Json.Serialize(user.selectedStoreItems);
            if (friend.Invite)
            {
                UserController.Instance.ChallengeFriendAnswer(true, friend.GamytechId, selectedBets, selectedItemsData);
                ShowState(Utils.LocalizeTerm("Game will start shortly..."));
                playButton.gameObject.SetActive(false);
            }
            else
            {
                inviting = true;
                UserController.Instance.ChallengeFriend(friend.GamytechId, selectedBets, selectedItemsData);
                ShowState("Sending Challenge...", true);
                playButtonText.text = Utils.LocalizeTerm("Cancel");
            }
        }
        else if(!canceled)
        {
            canceled = true;
            ShowState(Utils.LocalizeTerm(friend.Invite? "Challenge Declined" : "Challenge Canceled"));
            playButtonText.text = Utils.LocalizeTerm("OK");
            UserController.Instance.ChallengeFriendAnswer(false, friend.GamytechId);
        }
    }
    #endregion Inputs
}
