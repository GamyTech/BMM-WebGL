using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class PopupButton : MonoBehaviour
{
    public Button button;
    public PopupText popupText;
    public PopupImage popupImage;
    public Enums.ActionType actionType;
    public string actionString;
    public bool ClosePopup;

    private bool OpenWebPage;

    private void OnDisable()
    {
#if UNITY_WEBGL
        if (OpenWebPage)
            button.UnregisterCallbackOnPressedDown();
#endif
    }

    public void SetButton(ButtonData buttonData, MonoBehaviour mono)
    {
        if (buttonData == null)
        {
            Debug.LogError("SetButton - ButtonData null");
            return;
        }

        actionType = buttonData.actionType;
        actionString = buttonData.actionString;
        popupText.SetText(buttonData.textData);
        popupImage.LoadImageData(buttonData.imageData, mono);

        UnityAction action = buttonData.action ?? ActionKit.CreateAction(actionType, actionString, out OpenWebPage);
        if (action != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
#if UNITY_WEBGL
            if (OpenWebPage)
                button.RegisterCallbackOnPressedDown();
#endif
        }
    }
}
