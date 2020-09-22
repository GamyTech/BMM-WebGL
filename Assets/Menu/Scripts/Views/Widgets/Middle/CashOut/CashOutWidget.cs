using UnityEngine;
using UnityEngine.UI;
using GT.Websocket;

public class CashOutWidget : Widget {

    public Text AcountBalance;
    public Text AvailableCashOut;
    public Text ErrorText;
    public InputField CashOutAmount;
    public Button CashOutButton;
    public GameObject[] DontShowOnPending;
    public SmoothResize errorResizer;

    private RectTransform m_loadingTransform;
    protected RectTransform loadingTransform
    {
        get
        {
            if (m_loadingTransform == null)
                m_loadingTransform = transform.GetChild(0).transform as RectTransform;
            return m_loadingTransform;
        }
    }

    private float availableCash;

    public override void EnableWidget()
    {
        base.EnableWidget();

        UserController.Instance.gtUser.cashOutData.Reset();
        SetErrorText();
        WebSocketKit.Instance.AckEvents[RequestId.CashOutRequest] += OnCashOutRequest;
        availableCash = UserController.Instance.wallet.Cash;
        AcountBalance.text = Wallet.CashPostfix + UserController.Instance.wallet.TotalCash.ToString("#,##0.00");
        AvailableCashOut.text = Wallet.CashPostfix + availableCash.ToString("#,##0.00");
        CashOutAmount.text = string.Empty;
        UserController.Instance.wallet.OnCashChanged += WalletOnCashChanged;
        SetCashOutPendingStatus(UserController.Instance.gtUser.CashOutPending);
        UserController.Instance.gtUser.OnCashOutPendingChanged += GtUser_OnCashOutPendingChanged;
    }

    public override void DisableWidget()
    {
        WebSocketKit.Instance.AckEvents[RequestId.CashOutRequest] -= OnCashOutRequest;
        UserController.Instance.wallet.OnCashChanged -= WalletOnCashChanged;
        UserController.Instance.gtUser.OnCashOutPendingChanged -= GtUser_OnCashOutPendingChanged;

        base.DisableWidget();
    }

    private void GtUser_OnCashOutPendingChanged(bool newValue)
    {
        SetCashOutPendingStatus(newValue);
    }

    private void SetCashOutPendingStatus(bool isPending)
    {
        if (isPending)
            CashOutButton.GetComponentInChildren<Text>().text = Utils.LocalizeTerm("Cash Out Pending");
        else
            CashOutButton.GetComponentInChildren<Text>().text = Utils.LocalizeTerm("Cash Out Plain");

        CashOutButton.interactable = !isPending;
        CashOutAmount.interactable = !isPending;

        foreach (GameObject item in DontShowOnPending)
        {
            item.SetActive(!isPending);
        }
    }

    private void WalletOnCashChanged(float newMoney)
    {
        availableCash = newMoney;
    }

    private void OnCashOutRequest(Ack ack)
    {
        switch (ack.Code)
        {
            case WSResponseCode.OK:
                TrackingKit.CashOutTracker(UserController.Instance.gtUser.cashOutData.Amount);
                break;
            default:
                SetErrorText("Cash Out Request Failed");
                break;
        }
        LoadingController.Instance.HidePageLoading();
    }

    private bool ValidateAmount(string s, out string error)
    {
        if (string.IsNullOrEmpty(s))
        {
            error = "Invalid Amount";
            return false;
        }
        int amount;
        if (int.TryParse(s, out amount) == false)
        {
            error = "Invalid Amount";
            return false;
        }
        if(amount > availableCash)
        {
            error = "Selected Amount is More Than Available";
            return false;
        }
        if(amount < ContentController.MinCashOut)
        {
            error = "Selected Amount is Less Than Minimum Cash Out Amount";
            return false;
        }
        if (amount > ContentController.MaxCashOut)
        {
            error = "Selected Amount is More Than Maximum Cash Out Amount";
            return false;
        }
        error = null;
        return true;
    }

    #region Inputs
    public void CreditCardButton()
    {

        //if (UserController.Instance.gtUser.VerificationRank < Enums.VerificationRank.AddressVerified)
        //{
        //    UserController.Instance.GetVerification(Enums.VerificationRank.AddressVerified);
        //    return;
        //}

        string error;
        if (ValidateAmount(CashOutAmount.text, out error))
        {
            GT.User.GTUser user = UserController.Instance.gtUser;
            user.cashOutData.Amount = CashOutAmount.text;

            string[] contentTexts = new string[]
            {
                Utils.LocalizeTerm("Are you sure you want to withdraw {0}", Wallet.CashPostfix + Wallet.AmountToString(user.cashOutData.Amount.ParseFloat(), 2)),
                Utils.LocalizeTerm("You will lose {0} bonus cash", Wallet.CashPostfix + Wallet.AmountToString(UserController.Instance.wallet.BonusCash, 2)),
            };

            PopupController.Instance.ShowSmallPopup("Withdrawal Confirmation", contentTexts
                ,new SmallPopupButton("OK", () => CheckIfUserVerified(user)), new SmallPopupButton("Cancel"));
        }
        else
            SetErrorText(error);
    }

    private void CheckIfUserVerified(GT.User.GTUser user)
    {
        if (user.Verified)
        {
            UserController.Instance.gtUser.CashOutPending = true;
            UserController.Instance.SendCashOutToServer();
        }
        else
        {
            PageController.Instance.ChangePage(Enums.PageId.VerifyAccount);
        }
    }

    public void InfoButton()
    {
        string[] contentTexts = new string[] 
        {
            Utils.LocalizeTerm("Enter an amount to withdraw up to {0}", Wallet.CashPostfix + Wallet.AmountToString(UserController.Instance.wallet.Cash, 2)),
            Utils.LocalizeTerm("The minimum amount is {0}", Wallet.CashPostfix + ContentController.MinCashOut),
            Utils.LocalizeTerm("You have {0} Bonus Cash", Wallet.CashPostfix + Wallet.AmountToString(UserController.Instance.wallet.BonusCash, 2)),
            Utils.LocalizeTerm("Bonus cash cannot be withdrawn and will be forfeited upon withdrawal")
        };
        PopupController.Instance.ShowSmallPopup("Available Cash Out", contentTexts);
    }
    #endregion Inputs

    private void SetErrorText(string error = null)
    {
        if (string.IsNullOrEmpty(error))
            errorResizer.targetHeight = 0;
        else
        {
            Debug.Log("SetErrorText: " + error);
            errorResizer.targetHeight = ErrorText.rectTransform.sizeDelta.y;
            ErrorText.text = Utils.LocalizeTerm(error);
        }
    }
}
