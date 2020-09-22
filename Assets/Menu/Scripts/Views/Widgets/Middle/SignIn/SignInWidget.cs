using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using GT.Database;
using GT.Encryption;
using GT.Websocket;

public class SignInWidget : Widget
{
    public GameObject TouchIdPanel;
    public InputField EmailInputField;
    public InputField PasswordInputField;
    public Text ErrorText;
    public SmoothResize errorResizer;

    public override void EnableWidget()
    {
        base.EnableWidget();
        WebSocketKit.Instance.AckEvents[RequestId.Login] += OnLoginToServer;

        errorResizer.ResetHeight();
        TouchIdPanel.SetActive(false);

        EmailInputField.text = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.LastGTUserEmail);
        PasswordInputField.text = string.Empty;

        LoadingController.Instance.HidePageLoading();
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.Login] -= OnLoginToServer;

        LoadingController.Instance.HidePageLoading();
        base.DisableWidget();
    }

    #region Buttons
    public void Select()
    {
        SetErrorText();
    }

    public void SignInButton()
    {
        if(PreSignInValidityCheck())
        {
            LoadingController.Instance.ShowPageLoading(transform as RectTransform);
            UserController.Instance.SignInToServer(EmailInputField.text, PasswordInputField.text);
        }
    }

    public void RegisterButton()
    {
        PageController.Instance.ChangePage(Enums.PageId.Register);
    }

    public void ForgotPassword()
    {
        PageController.Instance.ChangePage(Enums.PageId.ForgotPassword);
    }
    #endregion Buttons

    #region Events
    private void OnLoginToServer(Ack ack)
    {
        LoginAck loginAck = ack as LoginAck;

        LoadingController.Instance.HidePageLoading();
        switch (loginAck.Code)
        {
            case WSResponseCode.OK:
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastGTUserEmail, EmailInputField.text);
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastGTUserPass, AesBase64Wrapper.Encrypt(PasswordInputField.text));
                UserController.Instance.GamytechSignedIn(loginAck.UserResponse.CreateGTUser());
                TutorialController.Instance.SaveTutorialProgressAsFinished();
                TrackingKit.LoginTracker();

                if (!UserController.Instance.HandelReconnectInLogin(loginAck))
                    MenuSceneController.GoToStartingPage();
                break;
            case WSResponseCode.WrongPassword:
            case WSResponseCode.UserNotExist:
                SetErrorText("Invalid user name or password");
                break;
            case WSResponseCode.CantLogFromCountry:
                SetErrorText("Your country is locked for real money play");
                break;
            case WSResponseCode.Banned:
                PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
                break;
            default:
                SetErrorText("Unexpected Error: " + loginAck.Code.ToString());
                break;
        }
    }

    #endregion Events

    #region Aid Functions
    private void SetErrorText(string error = null)
    {
        if(string.IsNullOrEmpty(error))
            errorResizer.targetHeight = 0;
        else
        {
            errorResizer.targetHeight = ErrorText.rectTransform.sizeDelta.y;
            ErrorText.text = Utils.LocalizeTerm(error);
        }
    }

    private bool PreSignInValidityCheck()
    {
        string error;
        if (!Utils.IsEmailValid(EmailInputField.text, out error))
            SetErrorText(error);
        else if (!Utils.IsPasswordValid(PasswordInputField.text, out error))
            SetErrorText(error);
        else
        {
            SetErrorText(null);
            return true;
        }

        return false;
    }
    #endregion Aid Functions
}
