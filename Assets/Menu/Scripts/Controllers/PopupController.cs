using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PopupController : MonoBehaviour
{
    public const string POPUP_RESOURCES_PATH = "Popups/";
    public bool isShowing { get; private set; }

    private static PopupController m_instance;
    public static PopupController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<PopupController>()); }
        private set { m_instance = value; }
    }

    public PopupView PopupView;
    public SmallPopupView PortraitSmallPopupView;
    public SmallPopupView LandscapeSmallPopupView;

    public SmallPopupView SmallPopupView
    {
        get
        {
#if UNITY_IOS || UNITY_ANDROID
            if (RemoteGameController.Instance != null) return LandscapeSmallPopupView;
#endif
            return PortraitSmallPopupView;
        }
    }

    private Dictionary<Enums.PopupId, PopupData> readyPopups;
    private Queue<PopupData> bigPopupsQueue = new Queue<PopupData>();
    private Queue<SmallPopup> smallPopupsQueue = new Queue<SmallPopup>();
    private PopupData currentBigPopup;
    private SmallPopup currentSmallPopup;

    void Awake()
    {
        readyPopups = InitializePopupsFromAssets();
    }

    void OnDisable()
    {
        Instance = null;
    }

    void Start()
    {
        PortraitSmallPopupView.HidePopup(true);
        LandscapeSmallPopupView.HidePopup(true);
        PopupView.HidePopup(true);
        PopupView.Init(HideBigPopupCallback);
    }

    void Update()
    {
        isShowing = currentSmallPopup == null && smallPopupsQueue.Count > 0;
        if (isShowing)
        {
            currentSmallPopup = smallPopupsQueue.Peek();
            SmallPopupView.ShowPopup(currentSmallPopup);
        }

        isShowing = currentBigPopup == null && bigPopupsQueue.Count > 0;
        if (isShowing)
        {
            currentBigPopup = bigPopupsQueue.Peek();
            PopupView.ShowPopup(currentBigPopup);
        }
    }

    #region User Functions
    public PopupData GetPopup(Enums.PopupId popupType)
    {
        PopupData popupData = null;
        if(readyPopups.TryGetValue(popupType, out popupData))
            return popupData;
        return null;
    }

    public void ShowPopup(PopupData popup)
    {
        bigPopupsQueue.Enqueue(popup);
    }

    public void ShowPopupFromData(object o)
    {
        List<object> listData = o as List<object>;
        for (int x = 0; x < listData.Count; ++x)
            ShowPopup(listData[x] as Dictionary<string, object>);
    }

    public void ShowPopup(Dictionary<string, object> data)
    {
        string type = null;
        Dictionary<string, object> parameters = null;

        object o;
        if (data.TryGetValue("MessageType", out o))
            type = o.ToString();
        else
        {
            Debug.LogError("MessageType is missing from dictionnary");
            return;
        }

        if (data.TryGetValue("Parameters", out o))
            parameters = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;

        switch (type)
        {
            case "PayPalCashIn":
                ShowPayPalCashInPopup(parameters);
                break;
            case "SmallMessage":
                ShowSmallMessagePopup(parameters);
                break;
            case "CustomMessage":
                ShowCustomPopup(parameters);
                break;
            case "StandartPopup":
                ShowStandartPopup(parameters);
                break;
            case "adPopup":
                break;
            default:
                break;
        }
    }

    public void ShowPopup(Enums.PopupId popupType, bool forceCloseable = false, params UnityAction[] overrideButtonActions)
    {
        PopupData popup;
        if (readyPopups.TryGetValue(popupType, out popup))
        {
            if (forceCloseable)
                popup.isCloseable = true;

            for (int i = 0; i < Mathf.Min(popup.buttonElements.Count, overrideButtonActions.Length); i++)
                popup.buttonElements[i].action = overrideButtonActions[i];

            ShowPopup(popup);
        }
        else
            Debug.LogError("Popup asset for " + popupType + " is missing");
    }

    public void HidePopup()
    {
        PopupView.HidePopup();
    }

    public void ShowSmallPopup(string headline, string[] contentTexts, int boldIndex = 0, params SmallPopupButton[] buttons)
    {
        SmallPopupView.ShowPopup(new SmallPopup(headline, contentTexts, boldIndex, buttons));
    }

    public void ShowSmallPopup(string headline, int boldIndex = 0, params SmallPopupButton[] buttons)
    {
        SmallPopupView.ShowPopup(new SmallPopup(headline, null, boldIndex, buttons));
    }

    public void ShowSmallPopup(string headline, string[] contentTexts, params SmallPopupButton[] buttons)
    {
        SmallPopupView.ShowPopup(new SmallPopup(headline, contentTexts, 0, buttons));
    }

    public void ShowSmallPopup(string headline, params SmallPopupButton[] buttons)
    {
        SmallPopupView.ShowPopup(new SmallPopup(headline, null, 0, buttons));
    }

    public void EnqueueSmallPopup(string headline, string[] contentTexts, int boldIndex = 0, params SmallPopupButton[] buttons)
    {
        smallPopupsQueue.Enqueue(new SmallPopup(headline, contentTexts, boldIndex, HideSmallPopup, buttons));
    }

    public void HideSmallPopup()
    {
        SmallPopupView.HidePopup();
    }
    #endregion User Functions


    private void HideBigPopupCallback()
    {
        currentBigPopup = null;
        if (bigPopupsQueue.Count > 0)
            bigPopupsQueue.Dequeue();
    }

    public void HideSmallPopupCallback()
    {
        currentSmallPopup = null;
        if (smallPopupsQueue.Count > 0)
            smallPopupsQueue.Dequeue();
    }

    private void ShowPayPalCashInPopup(Dictionary<string, object> parameters)
    {
        object o;
        float amount = 0.0f;
        float bonus = 0.0f;

        if(parameters.TryGetValue("Amount", out o))
            amount = o.ParseFloat();
        if(parameters.TryGetValue("BonusCash", out o))
            bonus = o.ParseFloat();

        PopupData p = GetPopup(Enums.PopupId.CashinSuccess);
        p.textElements[1].text = Wallet.CashPostfix + (amount + bonus).ToString("0.00");
        p.textElements[3].text = Wallet.CashPostfix + UserController.Instance.wallet.TotalCash.ToString("0.00");
        ShowPopup(p);
    }

    private void ShowSmallMessagePopup(Dictionary<string, object> parameters)
    {
        object o;
        string title = "Welcome";
        List<string> texts = new List<string>();
        string buttonText = "OK";
        string actionString = null;
        UnityAction buttonAction = () => { };

        if (parameters.TryGetValue("Title", out o))
            title = o.ToString();
        if (parameters.TryGetValue("Texts", out o))
            texts = (o as List<object>).ConvertAll( a => a.ToString());
        if (parameters.TryGetValue("ButtonText", out o))
            buttonText = o.ToString();
        if (parameters.TryGetValue("ActionString", out o))
            actionString = o.ToString();

        bool openPage = false;
        if (parameters.TryGetValue("ActionType", out o))
            buttonAction = ActionKit.CreateAction(o.ToString(), actionString, out openPage);

        EnqueueSmallPopup(title, texts.ToArray(), 0, new SmallPopupButton(buttonText, buttonAction, instantOnWebGL: openPage));
    }

    private void ShowCustomPopup(Dictionary<string, object> parameters)
    {

        object o;
        bool isCloseable = parameters.TryGetValue("Closeable", out o) ? o.ParseBool() : false;

        bool isStatic = parameters.TryGetValue("Static", out o) ? o.ParseBool() : false;

        TextData headline = null;
        if (parameters.TryGetValue("Headline", out o))
            headline = new TextData(o as Dictionary<string, object>, 47);

        ImageData picture = null;
        if (parameters.TryGetValue("Picture", out o))
            picture = new ImageData(o as Dictionary<string, object>);

        List<TextData> textElements = new List<TextData>();
        if (parameters.TryGetValue("TextElements", out o))
        {
            List<object> textsList = o as List<object>;
            for (int x = 0; x < textsList.Count; ++x)
                textElements.Add(new TextData(textsList[x] as Dictionary<string, object>));
        }

        List<ButtonData> buttonElements = new List<ButtonData>();
        if (parameters.TryGetValue("ButtonElements", out o))
        {
            List<object> textsList = o as List<object>;
            for (int x = 0; x < textsList.Count; ++x)
                buttonElements.Add(new ButtonData(textsList[x] as Dictionary<string, object>));
        }

        ShowPopup(new PopupData(Enums.PopupId.Custom, headline, picture, isCloseable, textElements, buttonElements, isStatic));
    }

    private void ShowStandartPopup(Dictionary<string, object> parameters)
    {
        object o;
        Enums.PopupId id = Enums.PopupId.Custom;
        if (parameters.TryGetValue("ID", out o) && Utils.TryParseEnum(o.ToString(), out id))
            ShowPopup(GetPopup(id));
    }

    private void ShowWidgetPopup(Dictionary<string, object> parameters)
    {
        object o;
        Enums.WidgetId id = Enums.WidgetId.RatePopup;
        if (parameters.TryGetValue("ID", out o) && Utils.TryParseEnum(o.ToString(), out id))
            WidgetController.Instance.ShowWidgetPopup(id);
    }

    #region Asset Initialization 
    /// <summary>
    /// Initialize popup assets
    /// </summary>
    /// <returns></returns>
    private Dictionary<Enums.PopupId, PopupData> InitializePopupsFromAssets()
    {
        Dictionary<Enums.PopupId, PopupData> popups = new Dictionary<Enums.PopupId, PopupData>();

        PopupData[] popupList = Resources.LoadAll<PopupData>(POPUP_RESOURCES_PATH);
        for (int i = 0; i < popupList.Length; i++)
        {
            Enums.PopupId id;
            if (Utils.TryParseEnum(popupList[i].popupId, out id))
                popups.AddOrOverrideValue(id, popupList[i]);
        }
        return popups;
    }
    #endregion Asset Initialization
}