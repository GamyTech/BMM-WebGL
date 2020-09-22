using UnityEngine.UI;

public class VerifyPhonePromoWidget : Widget
{
    public Text LoyaltyCount;

    #region Overrides
    public override void EnableWidget()
    {
        base.EnableWidget();

        LoyaltyCount.text = string.Format(LoyaltyCount.text, Wallet.AmountToString(UserController.Instance.PhoneValidationBonus));

        UserController.Instance.gtUser.OnPhoneVerifiedChanged += UpdatePromoState;

        if (UserController.Instance.gtUser.IsPhoneVerified)
            gameObject.SetActive(false);
    }

    public override void DisableWidget()
    {
        if (UserController.Instance != null && UserController.Instance.gtUser != null)
            UserController.Instance.gtUser.OnPhoneVerifiedChanged -= UpdatePromoState;
        base.DisableWidget();
    }
    #endregion Overrides

    #region Events
    private void UpdatePromoState(bool isVerified)
    {
        gameObject.SetActive(!isVerified);
    }
    #endregion Events

    #region Input
    public void OpenVerification()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.VerifyWithSMSPopup);
    }
    #endregion Input
}
