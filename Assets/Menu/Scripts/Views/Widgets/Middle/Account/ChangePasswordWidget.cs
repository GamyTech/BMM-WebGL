using UnityEngine;
using UnityEngine.UI;
using GT.Database;
using GT.Encryption;
using GT.Websocket;
using GT.User;
using System.Collections.Generic;
using System;

public class ChangePasswordWidget : Widget
{
    public Text ErrorText;
    public Button UpdateButton;
    public InputField CurrentInput;
    public InputField NewInput;
    public InputField RetypeInput;

    private string pass;

    public SmoothResize errorResizer;

    public RectTransform rectTransform
    {
        get { return transform.GetChild(0).transform as RectTransform; }
    }

    public override void EnableWidget()
    {
        base.EnableWidget();
        pass = string.Empty;
        SetErrorText();
        ResetFields();
        WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.SetPassword, SetPassword);
    }

    public override void DisableWidget()
    {
        Debug.Log("DisableWidget ChangePasswordWidget");
        WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.SetPassword, SetPassword);

        base.DisableWidget();
    }

    private void ResetFields()
    {
        CurrentInput.text = string.Empty;
        NewInput.text = string.Empty;
        RetypeInput.text = string.Empty;
    }

    #region Buttons & Events
    public void UpdatePasswordButton()
    {
        string oldPassword = CurrentInput.text;
        string newPassword = NewInput.text;
        string retypedPassword = RetypeInput.text;
        if(isValid(oldPassword, newPassword, retypedPassword))
        {
            LoadingController.Instance.ShowPageLoading(this.rectTransform);
            pass = newPassword;
            UserController.Instance.ChangeGamytechPassword(oldPassword, newPassword);
        }
    }

    private void SetPassword(object o)
    {
        if (!gameObject.activeInHierarchy)
            return;

        LoadingController.Instance.HidePageLoading();
        ResetFields();

        Dictionary<string, object> responseDict = o as Dictionary<string, object>;
        object error = null;
        if (responseDict != null && responseDict.TryGetValue("ErrorCode", out error))
            SetErrorText("changing password failed", true);
        else
        {
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastGTUserPass, AesBase64Wrapper.Encrypt(pass));
            pass = string.Empty;
            SetErrorText("Password Changed!", false);
        }
    }

    public void SelectInput()
    {
        SetErrorText();
    }
    #endregion Buttons & Events

    private bool isValid(string oldPass, string newPass, string retypedPass)
    {
        string error;
        if (!Utils.IsPasswordValid(newPass, out error))
            SetErrorText(error);

        else if (retypedPass.Equals(newPass) == false)
            SetErrorText("new passwords do not match");

        else if (AesBase64Wrapper.Encrypt(oldPass) != GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.LastGTUserPass))
            SetErrorText("Invalid password");

        else
            return true;

        return false;
    }

    private void SetErrorText(string error = null, bool isRed = true)
    {
        ErrorText.color = isRed ? Utils.Color_Red : Utils.Color_Green;
        if (string.IsNullOrEmpty(error))
            errorResizer.targetHeight = 0;
        else
        {
            errorResizer.targetHeight = ErrorText.rectTransform.sizeDelta.y;
            ErrorText.text = Utils.LocalizeTerm(error);
        }
    }
}
