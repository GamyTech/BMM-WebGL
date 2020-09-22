using UnityEngine;
using GT.User;
using UnityEngine.UI;
using GT.Database;
using GT.Encryption;
using GT.Websocket;

public class RegisterWidget : Widget
{
    public InputField NameInputField;
    public InputField EmailInputField;
    public InputField PhoneInputField;
    public InputField PasswordInputField;

    public GameObject TouchIdPanel;
    public GameObject PhonePanel;
    public Toggle TouchIDToggle;
    public Text ErrorText;
    public Toggle AcceptToggle;
    public Button TermsButton;

    public SmoothResize errorResizer;

    //private bool touchActive;

    public override void EnableWidget()
    {
        base.EnableWidget();

        WebSocketKit.Instance.AckEvents[RequestId.RegisterUser] += OnRegister;

        errorResizer.ResetHeight();
        AcceptToggle.isOn = false;
        //TouchIdPanel.SetActive(TouchID.IsSupported);

        PhonePanel.gameObject.SetActive(AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends);

#if UNITY_WEBGL
        TermsButton.RegisterCallbackOnPressedDown();
#endif
    }

    public override void DisableWidget()
    {
        LoadingController.Instance.HidePageLoading();
        WebSocketKit.Instance.AckEvents[RequestId.RegisterUser] -= OnRegister;

#if UNITY_WEBGL
        TermsButton.UnregisterCallbackOnPressedDown();
#endif

        base.DisableWidget();
    }

    #region Buttons
    public void RegisterButton()
    {
        if (PreRegisterValidityCheck())
            SendRegisterRequest();
    }

    public void Select()
    {
        SetErrorText();
    }

    public void ExistingUser()
    {
        PageController.Instance.ChangePage(Enums.PageId.SignIn);
    }

    public void TermsAndConditionButton()
    {
        Utils.OpenURL(ContentController.TermsOfUseSite, "Terms Of Uses");
    }

    #endregion Buttons

    #region Events
    private void OnRegister(Ack ack)
    {
        RegisterAck registerAck = ack as RegisterAck;

        LoadingController.Instance.HidePageLoading();
        switch (registerAck.Code)
        {
            case WSResponseCode.OK:
                RemoteGameController.CleanMatchData();
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastGTUserEmail, EmailInputField.text);
                GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.LastGTUserPass, AesBase64Wrapper.Encrypt(PasswordInputField.text));

                if (PushNotificationKit.HasPendingPage)
                    TutorialController.Instance.SaveTutorialProgressAsFinished();
                else
                    TutorialController.Instance.ResetTutorialProgress();

                UserController.Instance.GamytechSignedIn(registerAck.userResponse.CreateGTUser());
                TrackingKit.RegisterTracker();
                MenuSceneController.GoToStartingPage();
                break;
            case WSResponseCode.UserAlreadyExist:
                SetErrorText("User with this email already exists");
                break;
            case WSResponseCode.CantLogFromCountry:
                SetErrorText("Your country is locked for real money play");
                break;
            case WSResponseCode.SameDevice:
                SetErrorText("There seems to be a problem with the registration. Please contact support for further assistance");
                break;
            default:
                SetErrorText("Unexpected Error: " + registerAck.Code.ToString());
                break;
        }
    }
    #endregion Events,

    #region Aid Functions
    private void SendRegisterRequest()
    {
        LoadingController.Instance.ShowPageLoading(transform as RectTransform);
        UserController.Instance.RegisterToServer(NameInputField.text, EmailInputField.text, PasswordInputField.text);
    }

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

    private bool PreRegisterValidityCheck()
    {
        string error = null;
        if (!AcceptToggle.isOn)
            SetErrorText("You must accept terms of uses to register");

        else if (!Utils.IsUserNameValid(NameInputField.text, out error))
            SetErrorText(error);

        else if (!Utils.IsEmailValid(EmailInputField.text, out error))
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
