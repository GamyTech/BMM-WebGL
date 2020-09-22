using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class PopupView : MonoBehaviour
{
    public FadingElement FadingElement;
    public PopupText headline;
    public Button closeButton;
    public Button closeBackground;
    public PopupImage picture;
    public ObjectPool textsPool;
    public ObjectPool buttonsPool;

    private List<PopupText> activeDescriptionTexts = new List<PopupText>();
    private List<PopupButton> activeButtons = new List<PopupButton>();
    private Action closeAction;

    #region User Functions
    public void Init(Action closeAction)
    {
        this.closeAction = closeAction;
    }

    public void ShowPopup(PopupData popup, bool instant = false)
    {
        headline.SetText(popup.headline);
        SetCloseable(popup.isCloseable);
        SetDescription(popup.textElements);
        SetButtons(popup.buttonElements, popup.isStatic);

        picture.LoadImageData(popup.picture, this, () =>
        {
            FadingElement.FadeIn(instant);
            if (MenuSoundController.Instance != null)
                MenuSoundController.Instance.Play(popup.startSound);
        });
    }

    public void ShowPopupImmediate(PopupData popup)
    {
        headline.SetText(popup.headline);
        SetCloseable(popup.isCloseable);
        SetDescription(popup.textElements);
        SetButtons(popup.buttonElements, popup.isStatic);
        picture.LoadImageData(popup.picture, this);

        FadingElement.FadeIn(true);
        if (MenuSoundController.Instance != null)
            MenuSoundController.Instance.Play(popup.startSound);
    }

    public void HidePopup(bool instant)
    {
        DisableButtons();
        FadingElement.FadeOut(instant, closeAction);
    }

    public void HidePopup()
    {
        DisableButtons();
        FadingElement.FadeOut(false, closeAction);
    }

    public void SetToCurrentConfiguration(Enums.PopupId type, PopupData source)
    {
        TextData headlineData = new TextData(headline);
        bool isClosable = closeButton.interactable;
        ImageData pictureData = new ImageData(picture);

        List<TextData> descriptionTextsData = new List<TextData>();
        PopupText[] descriptionTexts = textsPool.GetComponentsInChildren<PopupText>();
        for (int i = 0; i < descriptionTexts.Length; i++)
            descriptionTextsData.Add(new TextData(descriptionTexts[i]));

        List<ButtonData> buttonsData = new List<ButtonData>();
        PopupButton[] buttons = buttonsPool.GetComponentsInChildren<PopupButton>();
        bool isStatic = false;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (!buttons[i].ClosePopup)
                isStatic = true;
            buttonsData.Add(new ButtonData(buttons[i]));
        }

        source.SetData(type, headlineData, pictureData, isClosable, descriptionTextsData, buttonsData, isStatic);
    }

    public void Reset()
    {
        headline.Reset();
        picture.Reset();

        PopupText[] descriptionTexts = textsPool.GetComponentsInChildren<PopupText>();
        for (int i = 0; i < descriptionTexts.Length; i++)
        {
            DestroyImmediate(descriptionTexts[i].gameObject);
        }

        PopupButton[] buttons = buttonsPool.GetComponentsInChildren<PopupButton>();
        for (int i = 0; i < buttons.Length; i++)
        {
            DestroyImmediate(buttons[i].gameObject);
        }

        activeDescriptionTexts.Clear();
        activeButtons.Clear();
    }
    #endregion User Functions

    #region Aid Functions
    private void DisableButtons()
    {
        foreach (var button in activeButtons)
        {
            button.button.interactable = false;
        }
    }
    #endregion Aid Functions

    #region Populate Popup
    private void SetCloseable(bool isCloseable)
    {
        closeButton.interactable = isCloseable;
        closeBackground.interactable = isCloseable;
    }

    private void SetDescription(List<TextData> texts)
    {
        while (activeDescriptionTexts.Count > texts.Count)
        {
            textsPool.PoolObject(activeDescriptionTexts[0].gameObject);
            activeDescriptionTexts.RemoveAt(0);
        }
        for (int i = 0; i < texts.Count; i++)
        {
            PopupText text;
            if(i >= activeDescriptionTexts.Count)
            {
                GameObject obj = textsPool.GetObjectFromPool();
                text = obj.GetComponent<PopupText>();
                activeDescriptionTexts.Add(text);
            }
            else
            {
                text = activeDescriptionTexts[i];
            }
            text.transform.SetParent(textsPool.transform);
            text.gameObject.ResetGameObject();
            text.SetText(texts[i]);
        }
    }

    private void SetButtons(List<ButtonData> buttons, bool iStatic)
    {
        while (activeButtons.Count > buttons.Count)
        {
            buttonsPool.PoolObject(activeButtons[0].gameObject);
            activeButtons.RemoveAt(0);
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            PopupButton button;
            if (i >= activeButtons.Count)
            {
                GameObject obj = buttonsPool.GetObjectFromPool();
                button = obj.GetComponent<PopupButton>();
                activeButtons.Add(button);
            }
            else
                button = activeButtons[i];

            button.button.interactable = true;
            button.transform.SetParent(buttonsPool.transform);
            button.gameObject.ResetGameObject();
            button.SetButton(buttons[i], this);
            if(!iStatic)
                button.button.onClick.AddListener(HidePopup);
        }
    }
    #endregion Populate Popup
}
