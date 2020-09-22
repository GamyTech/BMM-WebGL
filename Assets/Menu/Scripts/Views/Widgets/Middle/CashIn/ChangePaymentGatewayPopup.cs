using UnityEngine;
using UnityEngine.UI;
using GT.User;
using GT.Websocket;

public class ChangePaymentGatewayPopup : PopupWidget
{
    public GameObject NextButtonView;
    public GameObject SelectMethodView;
    public Image ApcoSelection;
    public Image ETSelection;

    public override void EnableWidget()
    {
        base.EnableWidget();
        WebSocketKit.Instance.AckEvents[RequestId.CashIn] += OnCashIn;
        WebSocketKit.Instance.AckEvents[RequestId.CashInEasyTransact] += OnCashIn;

        UpdateSelected();
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.CashIn] -= OnCashIn;
        WebSocketKit.Instance.AckEvents[RequestId.CashInEasyTransact] -= OnCashIn;
        base.DisableWidget();
    }

    private void UpdateSelected()
    {
        ApcoSelection.enabled = UserController.Instance.gtUser.cashInData.method == CashInData.TransactionMethod.Apco;
        ETSelection.enabled = UserController.Instance.gtUser.cashInData.method == CashInData.TransactionMethod.EasyTransac;
    }

    #region Input
    public void OnNext()
    {
        NextButtonView.SetActive(false);
        SelectMethodView.SetActive(true);
    }

    public void SelectApco()
    { 
        UserController.Instance.gtUser.cashInData.method = CashInData.TransactionMethod.Apco;
        UpdateSelected();
    }

    public void SelectEasyTransac()
    {
        UserController.Instance.gtUser.cashInData.method = CashInData.TransactionMethod.EasyTransac;
        UpdateSelected();
    }

    public void OnContactSupport()
    {
        PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
        HidePopup();
    }

    public void OnTryAgain()
    {
        LoadingController.Instance.ShowPageLoading();

        if (UserController.Instance.gtUser.cashInData.newCardData != null)
            UserController.Instance.SubmitCard(UserController.Instance.gtUser.cashInData.newCardData);
        else
            UserController.Instance.UseCard(UserController.Instance.gtUser.cashInData.savedCardData);
    }

    private void OnCashIn(Ack ack)
    {
        LoadingController.Instance.HidePageLoading();

        CashInAck response = ack as CashInAck;

        LoadingController.Instance.HidePageLoading();
        switch (response.Code)
        {
            case WSResponseCode.OK:
                bool gotURL = !string.IsNullOrEmpty(response.VerifyUrl);
                if (gotURL)
                {
                    WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.CashUpdating, TextAnchor.MiddleCenter);
                    UserController.Instance.DepositURL = response.VerifyUrl;
#if UNITY_WEBGL
                    PopupController.Instance.ShowSmallPopup("Payment Processing", new SmallPopupButton("Open new tab", () =>
                    {
                        Utils.OpenURL(response.VerifyUrl, "Payment");
                        TrackingKit.CashInRequestTracker();
                    }, true));
#else
                    Utils.OpenURL(response.VerifyUrl, "Payment");
#endif
                }
                break;
            case WSResponseCode.ApcoFailed:
            case WSResponseCode.ETFail:
                PopupController.Instance.ShowSmallPopup("Card information not valid", new string[] { "Please make sure that the information you entered is correct and try again" },
                    new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)),
                    new SmallPopupButton("OK"));
                TrackingKit.CashInRequestTracker("apco_failed");
                break;
            case WSResponseCode.UserNotExist:
            case WSResponseCode.Banned:
            default:
                PopupController.Instance.ShowSmallPopup(Utils.LocalizeTerm("Unexpected error, Try again or contact support. code: {0}", (int)response.Code),
                    new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)),
                    new SmallPopupButton("OK"));
                TrackingKit.CashInRequestTracker("user_banned");
                break;
        }
    }
    #endregion Input
}
