using UnityEngine;
using UnityEngine.UI;
using GT.Database;
using GT.Websocket;
using GT.User;

public class ForgotPasswordWidget : Widget
{
    public SmoothLayoutElement element;
    public Text errorText;
    public InputField emailField;
    public RectTransform loadingRectTransform;

    private void SetErrorText(string error, Color color)
    {
        if (string.IsNullOrEmpty(error))
            element.SetVerticalStateUsingPhysicalSize(0);
        else
        {
            errorText.color = color;
            element.SetVerticalStateUsingPhysicalSize(1);
            errorText.text = Utils.LocalizeTerm(error);
        }
    }

    public override void EnableWidget()
    {
        base.EnableWidget();

        WebSocketKit.Instance.AckEvents[RequestId.ForgotPassword] += OnForgotPassword;
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.ForgotPassword] -= OnForgotPassword;

        base.DisableWidget();
    }

    #region Inputs
    public void RegisterButton()
    {
        PageController.Instance.ChangePage(Enums.PageId.Register);
    }

    public void SignInButton()
    {
        PageController.Instance.ChangePage(Enums.PageId.SignIn);
    }

    public void SendButton()
    {
        SetErrorText(string.Empty, Utils.Color_Red);

        string error;
        string email = emailField.text;
        if (Utils.IsEmailValid(email, out error))
        {
            LoadingController.Instance.ShowPageLoading(loadingRectTransform);
            UserController.Instance.SendForgotPassword(email);
        }
        else
            SetErrorText(error, Utils.Color_Red);
    }
    #endregion Inputs

    #region Callbacks
    private void OnForgotPassword(Ack ack)
    {
        LoadingController.Instance.HidePageLoading();
        switch (ack.Code)
        {
            case WSResponseCode.OK:
                SetErrorText("Email Sent. Check Your Email.", Utils.Color_Green);
                break;
            case WSResponseCode.UserNotExist:
                SetErrorText("User Doesn't Exist", Utils.Color_Red);
                break;
            default:
                SetErrorText("Unexpected Error " + ack.Code, Utils.Color_Red);
                break;
        }
    }
    #endregion Callbacks
}
