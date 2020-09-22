using GT.Store;

public class IAPItemView : GeneralStoreItemView
{
    public override void Populate(StoreItem item)
    {
        base.Populate(item);
    }

    protected override void SetButton(StoreItem item)
    {
        if (item.IsSelectable(1))
            PurchasedItemButtonHandler(item.SelectedString, item.Selected, item.Select);
        else
        {
            if (item.IsLocked())
                LockedItemButtonHandler(item.UnlockRank);
            else
                PrePurchasedItemButtonHandler(
                    Wallet.LoyaltyPointsPostfix + Wallet.AmountToString(item.LoyaltyPoints.ParseFloat()), 
                    () => item.Purchase()
                );
        }
    }
}
