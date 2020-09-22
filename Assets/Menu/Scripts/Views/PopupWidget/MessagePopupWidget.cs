using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class MessagePopupWidget : PopupWidget
{
    public Text headlineText;
    public ObjectPool normalButtonsPool;
    public ObjectPool activeButtonsPool;
    public ObjectPool textsPool;
    public Transform buttonsContent;

    private List<GameObject> activeButtons;
    private List<GameObject> activeTexts;

    private SmallPopup smallpopup;

    public override void Init(bool closable = true, object data = null)
    {
        base.Init(closable, data);

        CloseButton.gameObject.SetActive(closable);
        smallpopup = data as SmallPopup;

        activeTexts = new List<GameObject>();
        activeButtons = new List<GameObject>();

        ClearAll();
        CreateTexts(smallpopup.Headline, smallpopup.ContentText);
        CreateButtons(smallpopup.Buttons, smallpopup.BoldIndex, smallpopup.CloseCallBack);
    }

    #region Aid Functions
    private void CreateButtons(SmallPopupButton[] buttons, int boldIndex, UnityAction closeCallback)
    {
        bool multiLine = buttons.Length > 2;

        if (buttons.Length == 0)
            buttons = new SmallPopupButton[1] { new SmallPopupButton("OK") };

        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject go = (boldIndex == i ? activeButtonsPool : normalButtonsPool).GetObjectFromPool();
            activeButtons.Add(go);
            go.InitGameObjectAfterInstantiation(buttonsContent);
            if (!multiLine)
                go.transform.SetAsFirstSibling();

            SetButton(go.GetComponent<Button>(), buttons[i], closeCallback);
        }
    }

    private void SetButton(Button button, SmallPopupButton buttonData, UnityAction closeCallback)
    {
        if (button == null)
            return;

#if UNITY_WEBGL
        if (buttonData.IsInstantOnWebGL)
            button.RegisterCallbackOnPressedDown();
#endif

        button.GetComponentInChildren<Text>().text = buttonData.ButtonText;
        button.interactable = true;
        button.onClick.RemoveAllListeners();
        if (closeCallback != null)
            button.onClick.AddListener(closeCallback);
        button.onClick.AddListener(HidePopup);
        button.onClick.AddListener(buttonData.ButtonAction);
    }

    private void CreateTexts(string headline, string[] texts)
    {
        bool hastitle = !string.IsNullOrEmpty(headline);
        headlineText.gameObject.SetActive(hastitle);

        if (hastitle)
            headlineText.text = Utils.LocalizeTerm(headline).ToUpper();

        if (texts == null)
            return;

        for (int i = 0; i < texts.Length; i++)
        {
            GameObject go = textsPool.GetObjectFromPool();
            activeTexts.Add(go);
            go.InitGameObjectAfterInstantiation(textsPool.transform);
            go.GetComponent<Text>().text = Utils.LocalizeTerm(texts[i]);
        }
    }

    private void ClearAll()
    {
        textsPool.PoolObjects(activeTexts);
        activeTexts.Clear();

        for (int i = 0; i < activeButtons.Count; i++)
        {
#if UNITY_WEBGL
            activeButtons[i].GetComponent<Button>().UnregisterCallbackOnPressedDown();
#endif
            (smallpopup.BoldIndex == i ? activeButtonsPool : normalButtonsPool).PoolObject(activeButtons[i]);
        }
        activeButtons.Clear();

        for (int i = 0; i < buttonsContent.childCount; i++)
            Destroy(buttonsContent.GetChild(i).gameObject);
    }

    private void DisableButtons()
    {
        for (int i = 0; i < activeButtons.Count; i++)
            activeButtons[i].GetComponent<Button>().interactable = false;
    }
    #endregion Aid Functions
}