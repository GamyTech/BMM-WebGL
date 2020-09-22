using GT.Store;
using System.Collections.Generic;

public class GameStoreItemView : GeneralStoreItemView
{
    List<string> m_opponentUsedItem = new List<string>();

    public void UpdateOpponentUsedItems(GT.Backgammon.Player.PlayerData opponentData)
    {
        m_opponentUsedItem = opponentData.SelectedItems.Values.ToListOfStrings();
    }

    protected override void SetButton(StoreItem item)
    {
        if (item.IsSelectable(1))
        {
            if (item.CanBeGivenToOpponent && !m_opponentUsedItem.Contains(item.Id))
                PurchasedItemButtonHandler(item.SelectedString, item.Selected, () => item.PurchaseInGame());
            else
                PurchasedItemButtonHandler(item.SelectedString, item.Selected, item.Select);
        }

        else
        {
            if (item.IsLocked())
                LockedItemButtonHandler(item.UnlockRank);
            else
            {
                if (item.CanBeGivenToOpponent)
                    PrePurchasedItemButtonHandler(item.Cost.GetCostsString(), () => item.PurchaseInGame(m_opponentUsedItem.Contains(item.Id)));
                else
                    PrePurchasedItemButtonHandler(item.Cost.GetCostsString(), item.Purchase);
            }
        }
    }
}
