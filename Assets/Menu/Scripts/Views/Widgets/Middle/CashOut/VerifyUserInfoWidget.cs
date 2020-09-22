using GT.Websocket;
using UnityEngine.UI;

public class VerifyUserInfoWidget : Widget
{
    public VerificationFormView userForm;

    public InputField PayPal;
    public InputField IBAN;
    public InputField SWIFT;
    
    private string paypal = string.Empty;
    private string iban = string.Empty;
    private string swift = string.Empty;

    public override void EnableWidget()
    {
        base.EnableWidget();
        WebSocketKit.Instance.AckEvents[RequestId.CashOutRequest] += OnCashOutRequest;

        CashOutData data = UserController.Instance.gtUser.cashOutData;
        userForm.InitFields(data);
        userForm.OnImageReady += UpdateServer;

        PayPal.text = data.PayPal;
        IBAN.text = data.IBAN;
        SWIFT.text = data.SWIFT;
    }

    public override void DisableWidget()
    {
        userForm.OnImageReady -= UpdateServer;

        WebSocketKit.Instance.AckEvents[RequestId.CashOutRequest] -= OnCashOutRequest;
        base.DisableWidget();
    }

    #region Buttons
    public void NextPage()
    {
        if (ValidityCheck())
            userForm.NextPage();
    }

    public void PayPalEndEdit(string s)
    {
        paypal = s;
        UserController.Instance.gtUser.cashOutData.PayPal = paypal;
    }

    public void IBANEndEdit(string s)
    {
        iban = s;
        UserController.Instance.gtUser.cashOutData.IBAN = iban;
    }

    public void SWIFTEndEdit(string s)
    {
        swift = s;
        UserController.Instance.gtUser.cashOutData.SWIFT = swift;
    }

    public void InfoIBANButton()
    {
        string[] contentTexts = new string[]
        {
            Utils.LocalizeTerm("*IBAN: stands for International Bank Account Number,"),
            Utils.LocalizeTerm("which you can use when receiving international payments."),
        };
        PopupController.Instance.ShowSmallPopup("Available Cash Out", contentTexts);
    }

    #endregion Buttons

    private bool ValidityCheck()
    {
        string error = null;
        int page = userForm.GetCurrentPage();
        if (page == 0)
        {
            if (userForm.ValidityCheckWithoutAddress())
                return true;
        }
        else if (page == 1)
        {
            if (userForm.FullValidityCheck())
                return true;
        }
        else if (page == 2)
        {
            if (userForm.FullValidityCheck() &&
                Utils.IsEmailValid(paypal, "PayPal Account", out error) &&
                Utils.IsStringValid(iban, "IBAN", out error))
                return true;
        }
        if (!string.IsNullOrEmpty(error))
            userForm.SetErrorText(error);

        return false;
    }

    #region Events
    private void UpdateServer()
    {
        UserController.Instance.SendCashOutToServer();
    }

    private void OnCashOutRequest(Ack ack)
    {
        LoadingController.Instance.HidePageLoading();
        switch (ack.Code)
        {
            case WSResponseCode.OK:
                TrackingKit.CashOutTracker(UserController.Instance.gtUser.cashOutData.Amount);
                UserController.Instance.gtUser.CashOutPending = true;
                break;
            case WSResponseCode.CashOutAlreadyPending:
                PopupController.Instance.ShowSmallPopup("Failed to cash out", new string[] { "Cash out already pending" });
                UserController.Instance.gtUser.CashOutPending = true;
                break;
            default:
                PopupController.Instance.ShowSmallPopup("Failed to cash out", new string[] { "Error code: " + ack.Code.ToString() },
                    new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)));
                break;
        }
        PageController.Instance.ChangePage(Enums.PageId.Cashout);
    }
    #endregion Events
}
