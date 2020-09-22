using GT.Store;

public class ChatWordView : GeneralStoreItemView
{
    public override void Populate(StoreItem item)
    {
        FillInfo(item);
        SetButton(item);
    }

    protected void FillInfo(StoreItem item)
    {
        if (item.LocalSpriteData == null || string.IsNullOrEmpty(item.LocalSpriteData.PictureUrl))
        {
            Name.text = item.Name;
            Image.sprite = null;
        }
        else
        {
            Name.text = "";
            SetImage(item.LocalSpriteData);
        }
    }

    protected override void SetButton(StoreItem item)
    {
        if (item.IsSelectable(1))
        {
            PurchasedItemButtonHandler(item.SelectedString, item.Selected, item.Select);
        }
        else
        {
            if (item.IsLocked())
            {
                LockedItemButtonHandler(item.UnlockRank);
            }
            else
            {
                PrePurchasedItemButtonHandler(item.Cost.GetCostsString(), () => item.Purchase());
            }
        }
    }
}
