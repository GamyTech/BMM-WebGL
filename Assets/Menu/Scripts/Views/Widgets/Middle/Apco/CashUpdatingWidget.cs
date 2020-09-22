using UnityEngine.UI;


public class CashUpdatingWidget : PopupWidget
{
    public Button ReloadButton;

    public override void EnableWidget()
    {
#if UNITY_WEBGL
        ReloadButton.RegisterCallbackOnPressedDown();
#endif
        base.EnableWidget();
    }

    public override void DisableWidget()
    {
#if UNITY_WEBGL
        ReloadButton.UnregisterCallbackOnPressedDown();
#endif
        base.DisableWidget();
    }

    #region Buttons
    public void ContactSupport()
    {
        if (UserController.Instance.gtUser.cashInData.method != CashInData.TransactionMethod.Apco)
        {
            PageController.Instance.ChangePage(Enums.PageId.ContactSupport);
            WidgetController.Instance.HideWidgetPopups();
        }
        else
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.ChangePaymentGatewayPopup, UnityEngine.TextAnchor.UpperCenter);
    }

    public void ReloadPage()
    {
        Utils.OpenURL(UserController.Instance.DepositURL, "Payment");
    }
    #endregion Buttons

}
