using GT.Websocket;
using UnityEngine.UI;

public class VerificationApprovedPopupWidget : PopupWidget
{
    public Text LoyaltyCount;

    private ValidateSmsByUserAck ack;

    public override void Init(bool closable = true, object data = null)
    {
        base.Init(closable, data);
        ack = data as ValidateSmsByUserAck;
    }

    public override void EnableWidget()
    {
        base.EnableWidget();
        LoyaltyCount.text = string.Format(LoyaltyCount.text, Wallet.AmountToString(UserController.Instance.PhoneValidationBonus));
    }

    public void Collect()
    {
        if (ack != null)
            UserController.Instance.gtUser.UpdateWallet(ack.Data);
        HidePopup();
    }
}