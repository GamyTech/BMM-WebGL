﻿using GT.Websocket;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForLoginWidget : Widget
{
    public Text ErrorText;
    public SmoothResize errorResizer;

    public override void EnableWidget()
    {
        base.EnableWidget();

        WebSocketKit.Instance.AckEvents[RequestId.Login] += OnLoginToServer;
        WebSocketKit.Instance.SendRequest(RequestId.Login);
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.Login] -= OnLoginToServer;

        LoadingController.Instance.HidePageLoading();
        base.DisableWidget();
    }

    #region Events
    private void OnLoginToServer(Ack ack)
    {
        LoginAck loginAck = ack as LoginAck;
        LoadingController.Instance.HidePageLoading();
        switch (loginAck.Code)
        {
            case WSResponseCode.OK:
                if (!UserController.Instance.HandelReconnectInLogin(loginAck))
                    MenuSceneController.GoToStartingPage();
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
        if (string.IsNullOrEmpty(error))
            errorResizer.targetHeight = 0;
        else
        {
            errorResizer.targetHeight = ErrorText.rectTransform.sizeDelta.y;
            ErrorText.text = Utils.LocalizeTerm(error);
        }
    }
    #endregion Aid Functions 
}
