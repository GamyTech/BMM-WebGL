using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;
using GT.User;
using System.Collections.Generic;

public class CreditCardWidget : Widget
{ 
    public NewCardView NewCardForm;
    public SelectSavedCardView SelectCardForm;
    public Button SubmitButton;
    public GameObject SupportButton;

    private static bool isTransactionTried = false;

    public override void EnableWidget()
    {
        base.EnableWidget();

        NewCardForm.OnFormValidated += UpdateButtonState;

        WebSocketKit.Instance.AckEvents[RequestId.CashIn] += OnCashIn;
        WebSocketKit.Instance.AckEvents[RequestId.CashInEasyTransact] += OnCashIn;

        UserController.Instance.gtUser.OnCreditCardListChanged += OnCreditCardListChanged;

        SupportButton.SetActive(false);

        ShowCardList(UserController.Instance.gtUser.savedCreditCards != null && UserController.Instance.gtUser.savedCreditCards.Count > 0);
    }

    public override void DisableWidget()
    {
        base.DisableWidget();

        NewCardForm.OnFormValidated -= UpdateButtonState;

        WebSocketKit.Instance.AckEvents[RequestId.CashIn] -= OnCashIn;
        WebSocketKit.Instance.AckEvents[RequestId.CashInEasyTransact] -= OnCashIn;

        UserController.Instance.gtUser.OnCreditCardListChanged -= OnCreditCardListChanged;
    }

    private void ShowCardList(bool show)
    {
        NewCardForm.gameObject.SetActive(!show);
        SelectCardForm.gameObject.SetActive(show);
        SubmitButton.interactable = show;
    }

    private void ChangeMethod()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.ChangePaymentGatewayPopup);
    }

    private object GetCardData()
    {
        return NewCardForm.gameObject.activeSelf ? NewCardForm.GetCardData() as object : SelectCardForm.GetSelected();
    }

    #region Buttons
    public void Close()
    {
        if (NewCardForm.gameObject.activeSelf && UserController.Instance.gtUser.savedCreditCards != null && UserController.Instance.gtUser.savedCreditCards.Count > 0)
            ShowCardList(true);
        else
            PageController.Instance.ChangePage(Enums.PageId.DepositInfo);
    }

    public void AddCreditCard()
    {
        ShowCardList(false);
    }

    public void Submit()
    {
        isTransactionTried = true;
        SupportButton.SetActive(true);
        LoadingController.Instance.ShowPageLoading();
        
        if (NewCardForm.gameObject.activeSelf)
            UserController.Instance.SubmitCard(NewCardForm.GetCardData());
        else
            UserController.Instance.UseCard(SelectCardForm.GetSelected());
    }

    public void OnSupportButton()
    {
        if (isTransactionTried && UserController.Instance.gtUser.cashInData.method != CashInData.TransactionMethod.Apco)
        {
            PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
        }
        else
            ChangeMethod();
    }
    #endregion Buttons

    #region Events
    private void OnCreditCardListChanged(List<GTUser.SavedCreditCard> cards)
    {
        ShowCardList(cards.Count > 0);
    }

    private void UpdateButtonState(bool isValid)
    {
        SubmitButton.interactable = isValid;
    }

    private void OnCashIn(Ack ack)
    {
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
                    new SmallPopupButton("Change gateway", ChangeMethod),
                    new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)),
                    new SmallPopupButton("OK"));
                TrackingKit.CashInRequestTracker("apco_failed");
                break;
            case WSResponseCode.UserNotExist:
            case WSResponseCode.Banned:
            default:
                PopupController.Instance.ShowSmallPopup(Utils.LocalizeTerm("Unexpected error, Try again or contact support. code: {0}", (int)response.Code),
                    new SmallPopupButton("Change gateway", ChangeMethod),
                    new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)),
                    new SmallPopupButton("OK"));
                TrackingKit.CashInRequestTracker("user_banned");
                break;
        }
    }
    #endregion Events
}
