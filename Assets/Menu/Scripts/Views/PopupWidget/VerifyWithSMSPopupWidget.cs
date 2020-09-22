using GT.Websocket;
using SP.Dto.ProcessBreezeRequests;

public class VerifyWithSMSPopupWidget : PopupWidget
{
    public PhoneInputView PhoneInputPanel;
    public VerifyCodeView VerifyCodePanel;

    private ISO3166Country country;
    private int countryCode;
    private string number;

    #region Overrides
    public override void EnableWidget()
    {
        base.EnableWidget();

        string sentCode = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.SMSSentCode);
        string sentNumber = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.SMSSentPhoneNumber);
        if (string.IsNullOrEmpty(sentNumber))
            SwitchView(true);
        else
        {
            SwitchView(false);
            VerifyCodePanel.SetPhoneNumber("+" + sentCode + " " + sentNumber);
        }

        PhoneInputPanel.OnWaitForCode += SwitchToWaitForCode;
        VerifyCodePanel.OnGoBack += SwitchToPhoneInput;
        WebSocketKit.Instance.AckEvents[RequestId.ValidateSmsByUser] += OnCodeVerification;
    }

    public override void DisableWidget()
    {
        PhoneInputPanel.OnWaitForCode -= SwitchToWaitForCode;
        VerifyCodePanel.OnGoBack -= SwitchToPhoneInput;
        WebSocketKit.Instance.AckEvents[RequestId.ValidateSmsByUser] -= OnCodeVerification;
        base.DisableWidget();
    }
    #endregion Overrides

    private void SwitchView(bool isPhoneInput)
    {
        VerifyCodePanel.gameObject.SetActive(!isPhoneInput);
        PhoneInputPanel.gameObject.SetActive(isPhoneInput);
    }

    #region Input
    public void Close()
    {
        HidePopup();
    }
    #endregion Input

    #region Events 
    private void SwitchToWaitForCode(ISO3166Country country, int codeId, string number)
    {
        this.country = country;
        countryCode = codeId;
        this.number = number;

        string formatted = "+" + country.DialCodes[codeId] + " " + number;
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.SMSSentCode, country.DialCodes[codeId]);
        GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.SMSSentPhoneNumber, number);
        SwitchView(false);
        VerifyCodePanel.SetPhoneNumber(formatted);
    }

    private void SwitchToPhoneInput()
    {
        SwitchView(true);
        PhoneInputPanel.SetSelected(country, countryCode, number);
    }

    private void OnCodeVerification(Ack ack)
    {
        GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.SMSSentCode);
        GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.SMSSentPhoneNumber);
        ValidateSmsByUserAck validation = ack as ValidateSmsByUserAck;
        if (ack.Code == WSResponseCode.OK && validation != null && validation.Result)
        {
            UserController.Instance.gtUser.IsPhoneVerified = true;
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.VerificationApprovedPopup, ack);
        }
        else
        {
            PopupController.Instance.ShowSmallPopup("Error", new string[]
            {
                "We couldn't verify your phone number.",
                "Please try again."
            }, new SmallPopupButton("OK"));
        }
    }
    #endregion Events
}
