using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GT.PayPal;
using GT.Websocket;

public class DepositInfoWidget : Widget
{
    public RectTransform depositItemAchor;
    public Text depositAmountText;
    public Text bonusAmountText;
    public Text totalAmountText;
    public DepositItemView depositDataView;
    private bool m_waitBeforeNewURL = false;

    private Canvas m_canvas;
    protected Canvas canvas
    {
        get
        {
            if (m_canvas == null)
                m_canvas = GetComponentInParent<Canvas>();
            return m_canvas;
        }
    }

    public override void EnableWidget()
    {
        base.EnableWidget();

        FillAmounts(UserController.Instance.gtUser.cashInData);

        WebSocketKit.Instance.AckEvents[RequestId.CashInPaySafe] += OnCashIn;
        WebSocketKit.Instance.AckEvents[RequestId.CashInSkrill] += OnCashIn;
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.CashInPaySafe] -= OnCashIn;
        WebSocketKit.Instance.AckEvents[RequestId.CashInSkrill] -= OnCashIn;

        base.DisableWidget();
    }

    private void MoveOutside(Image collider, Transform bottomElement)
    {
        collider.rectTransform.sizeDelta = new Vector2(Screen.width / canvas.scaleFactor, Screen.height / canvas.scaleFactor);
        collider.transform.SetParent(bottomElement.parent, false);
        collider.transform.SetSiblingIndex(bottomElement.GetSiblingIndex());
    }

    private void MoveBackInside(Image collider)
    {
        collider.transform.SetParent(transform, false);
    }

    private void Update()
    {
        depositDataView.transform.position = depositItemAchor.transform.position;
    }

    private IEnumerator WaitBeforeNewURL()
    {
        m_waitBeforeNewURL = true;
        yield return new WaitForSeconds(1.0f);
        m_waitBeforeNewURL = false;
    }

    #region Input
    public void GoBack()
    {
        PageController.Instance.BackPage();
    }
    #endregion Input

    #region Buttons

    public void BonusCashInfoButton()
    {
        PopupController.Instance.ShowSmallPopup("Bonus Cash",
            new string[] { "Bonus cash can be used in real money games", "Bonus cash cannot be withdrawn" }, 1,
            new SmallPopupButton("Learn More", () => Utils.OpenURL(ContentController.GameWebsite), true), new SmallPopupButton("OK"));
    }

    public void SkrillButtonClick()
    {
        if (!m_waitBeforeNewURL)
        {
            StartCoroutine(WaitBeforeNewURL());
            UserController.Instance.gtUser.cashInData.method = CashInData.TransactionMethod.Skrill;
            UserController.Instance.RequestSkrillCashin();
        }
    }

    public void CreditCardButtonClick()
    {
        UserController.Instance.gtUser.cashInData.method = CashInData.TransactionMethod.Apco;
        PageController.Instance.ChangePage(Enums.PageId.CreditCard);
    }

    public void PaypalButtonClick()
    {
        if(!m_waitBeforeNewURL)
        {
            StartCoroutine(WaitBeforeNewURL());
            UserController.Instance.gtUser.cashInData.method = CashInData.TransactionMethod.PayPal;
            float amount = UserController.Instance.gtUser.cashInData.depositAmount.amount;
            PayPalKit.OpenPayPal(PayPalCallback, amount.ToString() + " Dollars", UserController.Instance.gtUser.Email, amount);
        }
    }

    public void PaySafeCardButtonClick()
    {
        if (!m_waitBeforeNewURL)
        {
            StartCoroutine(WaitBeforeNewURL());
            UserController.Instance.gtUser.cashInData.method = CashInData.TransactionMethod.PaySafeCard;
            UserController.Instance.RequestPaySafeCardCashin();
        }
    }
    #endregion Buttons

    #region Callbacks
    private void PayPalCallback(PayPalResponse response)
    {
        Debug.Log("PayPalCallback " + response);

        switch (response.responseType)
        {
            case PayPalResponseType.OK:
                AppsFlyerKit.CashInTracker(response.price.ToString());
#if UNITY_WEBGL
                PopupController.Instance.ShowSmallPopup("Verification needed", new SmallPopupButton("Ok", () =>
                {
                    Utils.OpenURL(response.adress);
                    PageController.Instance.ChangePage(Enums.PageId.PCPayPalConnecting);
                }, true));
#else
                Utils.OpenURL(response.adress);
                PageController.Instance.ChangePage(Enums.PageId.PCPayPalConnecting);
#endif
                break;
            case PayPalResponseType.Cancel:
                break;
            case PayPalResponseType.NotSupported:

                PopupController.Instance.ShowSmallPopup("Not supported.");
                break;
        }
    }

    private void OnCashIn(Ack ack)
    {
        CashInAck response = ack as CashInAck;        
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
            case WSResponseCode.PaySafeCardDepositError:
                PopupController.Instance.ShowSmallPopup("Server error", new string[] { "Please try another payment method or contact support." },
                    new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)),
                    new SmallPopupButton("OK"));
                TrackingKit.CashInRequestTracker("no_url");
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
#endregion Callbacks

    #region Aid Functions
    private void FillAmounts(CashInData data)
    {
        depositDataView.PopulateItem(data.depositAmount);
        depositAmountText.text = Wallet.CashPostfix + data.depositAmount.amount.ToString("0.00");
        bonusAmountText.text = Wallet.CashPostfix + data.depositAmount.bonusCash.ToString("0.00");
        totalAmountText.text = "+" + Wallet.CashPostfix + data.depositAmount.amountWithBonus.ToString("0.00");
    }
#endregion Aid Functions
}
