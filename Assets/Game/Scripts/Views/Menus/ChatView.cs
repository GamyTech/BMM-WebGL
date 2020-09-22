using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GT.Store;

public class ChatView : MonoBehaviour {

    public GameMenuView gameMenuView;

    public ObjectPool TextButtonsPool;
    public ObjectPool ImageButtonsPool;

    public GameObject ButtonContainedGameObject;

    public Toggle MuteButton;
    public Button WriteButton;

    public Sprite clickedSprite;
    public Sprite regularSprite;

    public List<Sprite> TestSprites;
    public List<GameObject> HidenObjectOnPC = new List<GameObject>();

    private List<GameObject> activeGameObjects = new List<GameObject>();
    private int maxMessages = 5;
    private int messageCount = 0;

    private TouchScreenKeyboard keyboard;

    void Awake()
    {

#if UNITY_STANDALONE || UNITY_WEBGL
        for (int x = 0; x < HidenObjectOnPC.Capacity; ++x)
            HidenObjectOnPC[x].SetActive(false);
#endif

        Store store = UserController.Instance.gtUser.StoresData.GetStore(Enums.StoreType.Chat);
        InitButtons(new List<StoreItem>(store.selected.selectedItems));

        MuteButton.onValueChanged.RemoveAllListeners();
        WriteButton.onClick.RemoveAllListeners();

        MuteButton.onValueChanged.AddListener(b => Mute(b));
        WriteButton.onClick.AddListener(Write);
    }

    void Update()
    {
        SendWritenChat();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Mute(bool isOn)
    {
        Debug.Log("Mute");
        MuteButton.GetComponentInParent<Image>().sprite = isOn ? clickedSprite : regularSprite;
        gameMenuView.SetChatMute(isOn);
    }

    public void Write()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);    
    }

    public void InitButtons(List<StoreItem> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            StoreItem item = items[i];
            GameObject go;
            if (item == null) continue;
            if (item.LocalSpriteData == null || string.IsNullOrEmpty(item.LocalSpriteData.PictureUrl))
            {
                go = TextButtonsPool.GetObjectFromPool();
                activeGameObjects.Add(go);
                go.InitGameObjectAfterInstantiation(ButtonContainedGameObject.transform);
                go.GetComponent<ChatTextButtonView>().Init(item, ChatButtonClick);
            }
            else
            {
                go = ImageButtonsPool.GetObjectFromPool();
                activeGameObjects.Add(go);
                go.InitGameObjectAfterInstantiation(ButtonContainedGameObject.transform);
                go.GetComponent<ChatImageButtonView>().Init(item, ChatButtonClick);
            }
        }
    }

    private void ChatButtonClick(string Id)
    {
        Debug.Log("ChatButtonClick");
        RequestInGameController.Instance.SendItemChat(Id);
        gameMenuView.DisplayPlayerChatMessage(Id, true);
        Close();
    }

    private bool MaxMessageReached()
    {
        messageCount++;
        bool maxMessageReached = messageCount > maxMessages;
        if (maxMessageReached)
            Mute(true);

        return maxMessageReached;
    }

    private void SendWritenChat()
    {
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            string text = keyboard.text.Length > 48 ? keyboard.text.Substring(0, 48) : keyboard.text;
            if (!text.Equals(""))
            {
                ChatButtonClick("#" + text);
                keyboard = null;
            }
        }
    }
}

public class ChatButtonsData
{
    public Sprite sprite;
    public string text;
    public bool isTextChat;

    public ChatButtonsData(bool isText,string text,Sprite sprite = null)
    {
        isTextChat = isText;
        if (isTextChat)
            this.text = text;
        else
            this.sprite = sprite;
    }

}
