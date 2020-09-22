using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class SmallPopupView : MonoBehaviour
{
    private FadingElement m_fadingElement;
    public FadingElement fadingElement
    {
        get
        {
            if (m_fadingElement == null)
                m_fadingElement = GetComponent<FadingElement>();
            return m_fadingElement;
        }
    }

    public Font BoldFont;
    public Font MediumFont;
    public GameObject PopupPanel;
    public ObjectPool textsPool;
    public ObjectPool buttonsPool;
    public Transform HorizontalButtonsPanel;
    public Transform VerticalButtonsPanel;
    public Text headlineText;

    private List<GameObject> activeTexts = new List<GameObject>();
    private List<GameObject> activeButtons = new List<GameObject>();

    public void ShowPopup(SmallPopup smallpopup)
    {
        ClearAll();
        CreateTexts(smallpopup.Headline, smallpopup.ContentText);
        CreateButtons(smallpopup.Buttons, smallpopup.BoldIndex, smallpopup.CloseCallBack);
        fadingElement.FadeIn();
    }

    public void HidePopup(bool instant)
    {
        DisableButtons();
        fadingElement.FadeOut(instant);
    }

    public void HidePopup()
    {
        DisableButtons();
        fadingElement.FadeOut(false);
    }

#region Aid Functions
    private void CreateButtons(SmallPopupButton[] buttons, int boldIndex, UnityAction closeCallback)
    {
        boldIndex = Mathf.Clamp(boldIndex, 0, buttons.Length);
        Transform buttonsContent;
        bool multiLine = buttons.Length > 2;
        HorizontalButtonsPanel.gameObject.SetActive(!multiLine);
        VerticalButtonsPanel.gameObject.SetActive(multiLine);
        buttonsContent = multiLine ? VerticalButtonsPanel : HorizontalButtonsPanel;

        if (buttons.Length == 0)
            buttons = new SmallPopupButton[1] { new SmallPopupButton("OK") };

        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject go = buttonsPool.GetObjectFromPool();
            activeButtons.Add(go);
            go.InitGameObjectAfterInstantiation(buttonsContent);
            if(!multiLine)
                go.transform.SetAsFirstSibling();

            SetButton(go.GetComponent<Button>(), buttons[i], boldIndex == i, closeCallback);
        }
    }

    private void SetButton(Button button, SmallPopupButton buttonData, bool isBold, UnityAction closeCallback)
    {
        if (button == null)
            return;

#if UNITY_WEBGL
        if (buttonData.IsInstantOnWebGL)
            button.RegisterCallbackOnPressedDown();
#endif

        button.GetComponentInChildren<Text>().font = isBold ? BoldFont : MediumFont;
        button.GetComponentInChildren<Text>().text = buttonData.ButtonText;
        button.interactable = true;
        button.onClick.RemoveAllListeners();
        if(closeCallback!= null)
            button.onClick.AddListener(closeCallback);
        button.onClick.AddListener(HidePopup);
        button.onClick.AddListener(buttonData.ButtonAction);
    }

    private void CreateTexts(string headline, string[] texts)
    {
        bool hastitle = !string.IsNullOrEmpty(headline);
        headlineText.gameObject.SetActive(hastitle);

        if (hastitle)
            headlineText.text = Utils.LocalizeTerm(headline);

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

#if UNITY_WEBGL
        for (int i = 0; i < activeButtons.Count; i++)
            activeButtons[i].GetComponent<Button>().UnregisterCallbackOnPressedDown();
#endif

        buttonsPool.PoolObjects(activeButtons);
        activeButtons.Clear();

        for (int i = 0; i < VerticalButtonsPanel.childCount; i++)
            Destroy(VerticalButtonsPanel.GetChild(i).gameObject);

        for (int i = 0; i < HorizontalButtonsPanel.childCount; i++)
            Destroy(HorizontalButtonsPanel.GetChild(i).gameObject);
    }

    private void DisableButtons()
    {
        for (int i = 0; i < activeButtons.Count; i++)
            activeButtons[i].GetComponent<Button>().interactable = false;
    }
#endregion Aid Functions
}

public struct SmallPopupButton
{
    public string ButtonText { get; private set; }
    public UnityAction ButtonAction { get; private set; }
    public bool IsInstantOnWebGL { get; private set; }


    public SmallPopupButton(string text, UnityAction action = null, bool instantOnWebGL = false)
    {
        ButtonText = Utils.LocalizeTerm(text);
        ButtonAction = action ?? new UnityAction(() => { });
        IsInstantOnWebGL = instantOnWebGL;
    }

}
