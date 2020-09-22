using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GT.Backgammon.Player;
using GT.Assets;
using GT.Websocket;
using System.Collections.Generic;

public class GameMenuView : MonoBehaviour
{
    #region Public Members

    public GameStoreView StoreView;
    public ChatView chatView;
    public GameSettingView settingView;

    public GameObject UserTextBubble;
    public GameObject UserImageBubble;

    public GameObject OpponentTextBubble;
    public GameObject OpponentImageBubble;

    public GameObject OpponentBankTime;
    public GameObject PlayerBankTime;

    public GameObject PlayerImageLoading;
    public GameObject OpponentImageLoading;

    public Text OpponentBankTimeText;
    public Text PlayerBankTimeText;

    public Image OpponentTimeFill;
    public Image PlayerTimeFill;

    public GameObject GameInfo;
    public Text CurrentBetText;
    public Text MaxBetText;

    public GameObject TourneyInfo;
    public TourneyTimer OpponentTourneyTimer;
    public TourneyTimer UserTourneyTimer;

    public Text PlayerNameText;
    public Text OpponentNameText;

    public Image PlayerSprite;
    public Image OpponentSprite;

    public Image PlayerLuckyItem;
    public Image OpponentLuckyItem;

    public Button ChatButton;

    public float imageMsgDuration;
    public float textMsgDuration;
    #endregion Public Members

    #region Private Members

    private bool isChatMute = false;

    private PlayerData playerData;
    private PlayerData opponentData;

    private Color greenStart = new Color(0.5725f, 0.9137f, 0.3882f);
    private Color greenEnd = new Color(0.3372f, 0.7960f, 0.0862f);
    private Color redStart = new Color(0.9137f, 0.3882f, 0.3882f);
    private Color redEnd = new Color(0.7960f, 0.0862f, 0.0862f);

    private bool isTourney;
    private float tourneySecondsLeft;
    #endregion Private Members

    private void Start()
    {
        isTourney = TourneyController.Instance.IsInNotFinishedTourney();
        SwitchUITourneyState(isTourney);
    }

    public void SetTourneyTimers(string userName, float maxUserTime, string opponentName, float maxOpponentTime)
    {
        OpponentTourneyTimer.SetUser(opponentName, maxOpponentTime); 
        UserTourneyTimer.SetUser(userName, maxUserTime);
    }

    private void SwitchUITourneyState(bool isTourney)
    {
        GameInfo.SetActive(!isTourney);
        TourneyInfo.SetActive(isTourney);
    }

    #region Public Functions
    public void InitPlayersData(PlayerData player, PlayerData opponent)
    {
        WebSocketKit.Instance.ServiceEvents[ServiceId.SendChat] += WebSocketKit_SendChat;
        WebSocketKit.Instance.ServiceEvents[ServiceId.SendTourneyChat] += WebSocketKit_SendChat;

        playerData = player;
        opponentData = opponent;
        playerData.OnSelectedItemsUpdate += PlayerData_OnPlayerSelectedItemsUpdate;
        opponentData.OnSelectedItemsUpdate += PlayerData_OnOpponentSelectedItemsUpdate;

        PlayerNameText.text = playerData.UserName;
        SetProfilePicture(playerData, PlayerSprite, PlayerImageLoading);
        SetLuckyItem(playerData, PlayerLuckyItem);

        OpponentNameText.text = opponentData.UserName;
        SetProfilePicture(opponentData, OpponentSprite, OpponentImageLoading);
        SetLuckyItem(opponentData, OpponentLuckyItem);

        SetTourneyTimers(player.UserName, player.TotalTime, opponent.UserName, opponent.TotalTime);
    }

    private void OnDestroy()
    {
        WebSocketKit.Instance.ServiceEvents[ServiceId.SendChat] -= WebSocketKit_SendChat;
        WebSocketKit.Instance.ServiceEvents[ServiceId.SendTourneyChat] -= WebSocketKit_SendChat;

        if (playerData != null) playerData.OnSelectedItemsUpdate -= PlayerData_OnPlayerSelectedItemsUpdate;
        if (opponentData != null) opponentData.OnSelectedItemsUpdate -= PlayerData_OnOpponentSelectedItemsUpdate;
    }

    public void DefaultInit()
    {
        PlayerNameText.text = "";
        OpponentNameText.text = "";
    }

    public void InitStakeData(string currentBet, string currentFee, string maxBet)
    {
        CurrentBetText.text = currentBet;
        MaxBetText.text = maxBet;
    }

    private void SetProfilePicture(PlayerData Data, Image image, GameObject imageLoading)
    {
        if (Data == null || Data.Avatar == null)
        {
            image.sprite = AssetController.Instance.DefaultAvatar;
            StopImageLoadAnimation(imageLoading);
            Debug.LogError("SetImage PlayerData is Null");
            return;
        }

        Data.Avatar.LoadImage(this, s => {image.sprite = s; StopImageLoadAnimation(imageLoading); }, AssetController.Instance.DefaultAvatar);
    }

    private void SetLuckyItem(PlayerData Data, Image image)
    {
        if (Data == null || Data.SelectedItems == null)
        {
            Debug.LogError("SelectedItems PlayerData is Null");
            return;
        }
        ShowLuckyItem(Data.SelectedItems, image);
    }

    private void ShowLuckyItem(Dictionary<Enums.StoreType, string[]> selectedItems, Image image)
    {
        string[] luckyItems;
        if (selectedItems.TryGetValue(Enums.StoreType.LuckyItems, out luckyItems) && luckyItems != null && luckyItems.Length > 0)
            UserController.Instance.gtUser.StoresData.GetItem(Enums.StoreType.LuckyItems, luckyItems[0]).LocalSpriteData.LoadImage(this, s => { image.sprite = s; });
    }

    private void PlayerData_OnPlayerSelectedItemsUpdate(Dictionary<Enums.StoreType, string[]> selectedItems)
    {
        ShowLuckyItem(selectedItems, PlayerLuckyItem);
    }

    private void PlayerData_OnOpponentSelectedItemsUpdate(Dictionary<Enums.StoreType, string[]> selectedItems)
    {
        ShowLuckyItem(selectedItems, OpponentLuckyItem);
    }

    private void StopImageLoadAnimation(GameObject imageLoading)
    {
        imageLoading.SetActive(false);
    }

    public void SetChatMute(bool isMute)
    {
        isChatMute = isMute;
    }

    public void UpdateBetAmounts(string currentBetAmount)
    {
        CurrentBetText.text = currentBetAmount;
    }

    public void OpenSettingsView()
    {
        settingView.Show();
        if (StoreView != null)
            StoreView.SetActive(false);
        if (chatView != null)
            chatView.Hide();
    }

    public void OpenStoreView()
    {
        settingView.Hide();
        if (StoreView != null)
            StoreView.ToggleActive();
        if (chatView != null)
            chatView.Hide();
    }

    public void OpenChatView()
    {
        settingView.Hide();
        if (StoreView != null)
            StoreView.SetActive(false);
        if (chatView != null)
            chatView.Show();
    }

    public void RequestStatus()
    {
        RequestInGameController.Instance.GetCurrentMoveState(
            RemoteGameController.Instance.GetCurrentTurnIndex(), RemoteGameController.Instance.IsDoubleCube());
    }

    public void ActivateChatButton(bool b)
    {
        if (ChatButton != null)
            ChatButton.interactable = b;
    }

    public void DimUserPic(IPlayer player, bool isMainPlayer)
    {
        if (isMainPlayer)
            PlayerSprite.color = Color.grey;
        else
            OpponentSprite.color = Color.grey;
    }

    private void ResetUserImagesDims()
    {
        PlayerSprite.color = Color.white;
        OpponentSprite.color = Color.white;
    }


    public void WebSocketKit_SendChat(Service service)
    {
        ChatReceivedService chatReceived = service as ChatReceivedService;
        Debug.Log("ChatMessageRecieved : " + chatReceived.Message);

        if(chatReceived.SenderId != UserController.Instance.gtUser.Id && !isChatMute)
            DisplayPlayerChatMessage(chatReceived.Message, false);
    }

    public void DisplayPlayerChatMessage(string Id, bool localPlayer)
    {
        Debug.Log("DisplayPlayerChatMessage : " + Id);

        if (Id[0].Equals('#'))
        {
            GameObject Textbubble = localPlayer ? UserTextBubble : OpponentTextBubble;
            StartCoroutine(DisplayTextMessage(Id.Substring(1), Textbubble, localPlayer));
        }
        else
        {
            ChatItemData chatItem = AssetController.Instance.GetStoreAsset(Id) as ChatItemData;
            if (chatItem != null)
            {
                if(localPlayer)
                    ChatButton.interactable = false;

                if (string.IsNullOrEmpty(chatItem.text))
                {
                    GameObject ImageBubble = localPlayer ? UserImageBubble : OpponentImageBubble;

                    if (ImageBubble.transform.childCount > 0)
                        Destroy(ImageBubble.transform.GetChild(0).gameObject);

                    GameObject go = Instantiate(chatItem.prefab);
                    StartCoroutine(DisplayImageMessage(go, ImageBubble, localPlayer));
                }
                else
                {
                    GameObject Textbubble = localPlayer ? UserTextBubble : OpponentTextBubble;
                    StartCoroutine(DisplayTextMessage(chatItem.text, Textbubble, localPlayer));
                }
            }
        }
    }

    private IEnumerator DisplayTextMessage(string text, GameObject go, bool isLocalPlayer)
    {
        string s = "";
        int lines = 1;
        if (text.Length > 12)
        {
            string[] words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 12)
                {
                    int count = 0;
                    int start = 0;
                    string newWord = "";

                    foreach (var c in words[i])
                    {
                        count++;
                        if (count == 12)
                        {
                            newWord += words[i].Substring(start, count) + "\n";
                            start += count;
                            count = 0;
                        }
                    }
                    s += newWord;
                }
                else
                    s += words[i] + " ";

                if (s.Length >= 12 * lines)
                {
                    s += "\n";
                    lines++;
                }
            }

            if (s.Length > 48) s = s.Substring(0, 48) + "...";
            go.GetComponent<HorizontalLayoutGroup>().padding.bottom = lines > 1 ? 50 : 0;
        }
        else
            s = text;

        go.GetComponentInChildren<Text>().text = s;
        go.SetActive(true);
        yield return new WaitForSeconds(textMsgDuration);

        go.SetActive(false);
        if (isLocalPlayer)
            ChatButton.interactable = true;
    }

    public IEnumerator ShowNewLuckyItem(string itemID, bool UserIsPurchaser, bool isAGift)
    {

        Image luckyItemChanged;
        Image PurchaseDisplay;

        yield return new WaitUntil(() => { return !PopupController.Instance.isShowing; });

        if (UserIsPurchaser)
        {
            PurchaseDisplay = PlayerSprite;
            luckyItemChanged = isAGift ? OpponentLuckyItem : PlayerLuckyItem;
        }

        else
        {
            PurchaseDisplay = OpponentSprite;
            luckyItemChanged = isAGift ? PlayerLuckyItem : OpponentLuckyItem;
        }

        PurchaseDisplay.color = Color.grey;
        yield return new WaitForSeconds(1.0f);
        PurchaseDisplay.color = Color.white;

        bool imageSet = false;
        UserController.Instance.gtUser.StoresData.GetItem(Enums.StoreType.LuckyItems, itemID).LocalSpriteData.LoadImage(this, s => { imageSet = true; luckyItemChanged.sprite = s; });

        yield return new WaitUntil(() => { return imageSet; });
    }

    private IEnumerator DisplayImageMessage(GameObject go, GameObject ParentGo, bool isLocalPlayer)
    {
        go.InitGameObjectAfterInstantiation(ParentGo.transform);
        ParentGo.SetActive(true);

        yield return new WaitForSeconds(imageMsgDuration);
        ParentGo.GetComponent<Animator>().SetBool("IsOpen", false);
        yield return new WaitForSeconds(1);

        ParentGo.SetActive(false);
        if (isLocalPlayer)
            ChatButton.interactable = true;
    }

    public void PlayerTimer(PlayerData data)
    {
        if (data.CurrentTurnTime <= 0)
        {
            PlayerBankTime.SetActive(true);
            PlayerBankTimeText.text = ((int)data.CurrentBankTime).ToString();
            PlayerTimeFill.GetComponent<UnityEngine.UI.Gradient>().startColor = redStart;
            PlayerTimeFill.GetComponent<UnityEngine.UI.Gradient>().endColor = redEnd;
            float turnValue = data.CurrentBankTime / data.TotalBankTime;
            PlayerTimeFill.fillAmount = turnValue;
        }
        else
        {
            PlayerTimeFill.GetComponent<UnityEngine.UI.Gradient>().startColor = greenStart;
            PlayerTimeFill.GetComponent<UnityEngine.UI.Gradient>().endColor = greenEnd;
            float turnValue = data.CurrentTurnTime / data.TotalTurnTime;
            PlayerTimeFill.fillAmount = turnValue;
        }
    }

    public void OpponentTimer(PlayerData data)
    {
        if (data.CurrentTurnTime <= 0)
        {
            OpponentBankTime.SetActive(true);
            OpponentBankTimeText.text = ((int)data.CurrentBankTime).ToString();
            OpponentTimeFill.GetComponent<UnityEngine.UI.Gradient>().startColor = redStart;
            OpponentTimeFill.GetComponent<UnityEngine.UI.Gradient>().endColor = redEnd;
            float turnValue = data.CurrentBankTime / data.TotalBankTime;
            OpponentTimeFill.fillAmount = turnValue;
        }
        else
        {
            OpponentTimeFill.GetComponent<UnityEngine.UI.Gradient>().startColor = greenStart;
            OpponentTimeFill.GetComponent<UnityEngine.UI.Gradient>().endColor = greenEnd;
            float turnValue = data.CurrentTurnTime / data.TotalTurnTime;
            OpponentTimeFill.fillAmount = turnValue;
        }
    }

    public void PlayerTotalTimer(PlayerData data)
    {
        SwitchTourneyTimers(true);
        UserTourneyTimer.SetSeconds(data.TotalTime);
    }

    public void OpponentTotalTimer(PlayerData data)
    {
        SwitchTourneyTimers(false);
        OpponentTourneyTimer.SetSeconds(data.TotalTime);
    }

    public void StopWaitForPlayer()
    {
        PlayerTimeFill.gameObject.SetActive(false);
    }

    public void HideBankTime()
    {
        OpponentBankTime.SetActive(false);
        PlayerBankTime.SetActive(false);
    }

    public void ActivatePlayersTime(bool isLocalPlayer)
    {
        OpponentTimeFill.gameObject.SetActive(!isLocalPlayer);
        PlayerTimeFill.gameObject.SetActive(isLocalPlayer);
        ResetUserImagesDims();
        HideBankTime();

        SwitchTourneyTimers(isLocalPlayer);
    }

    private void SwitchTourneyTimers(bool isLocalPlayer)
    {
        if (isTourney)
        {
            OpponentTourneyTimer.Switch(!isLocalPlayer);
            UserTourneyTimer.Switch(isLocalPlayer);
        }
    }

    public void Reset()
    {
        ActivateChatButton(true);
    }

#endregion Public Functions
}
