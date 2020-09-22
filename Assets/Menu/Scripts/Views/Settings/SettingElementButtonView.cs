using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[ExecuteInEditMode]
public class SettingElementButtonView : MonoBehaviour {

    public Enums.ActionType _action;
    public string _actionString;

    public Button SettingButton;

    private bool OpenWebPage;

    void OnEnable()
    {
        UnityAction action = ActionKit.CreateAction(_action, _actionString, out OpenWebPage);

        if (action != null)
        {
            SettingButton.onClick.RemoveAllListeners();
            SettingButton.onClick.AddListener(action);
#if UNITY_WEBGL
            if (OpenWebPage)
                SettingButton.RegisterCallbackOnPressedDown();
#endif
        }
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        if (OpenWebPage)
            SettingButton.UnregisterCallbackOnPressedDown();
#endif
    }
}
