using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class CustomRoomCategoryView : BetRoomCategoryView
{
    public static bool ToUpdateSelected;
    public Text SelectedBetsText;

    void Update()
    {
        if(ToUpdateSelected)
        {
            ToUpdateSelected = false;
            UpdateSelected();
        }
    }

    protected override void InitRooms()
    {
        UpdateSelected();
    }

    protected void UpdateSelected()
    {
        List<BetRoom> updatedRooms = null;
        ContentController.CashRoomsByCategory.TryGetValue(ContentController.CustomCatId, out updatedRooms);

        int count = 0;
        string selectedBetsText = string.Empty;

        for (int i = 0; i < Rooms.Count; i++)
        {
            float amount = Rooms[i].BetAmount;
            BetRoom room = updatedRooms.Find(a => a.BetAmount == amount);
            Rooms[i].Selected = room.Selected;

            if (Rooms[i].Selected)
            {
                if (count != 0)
                    selectedBetsText += ", ";
                selectedBetsText += Wallet.AmountToString(Rooms[i].BetAmount, Rooms[i].Kind);
                ++count;
            }
        }

        if (count == 0)
            SelectedBetsText.text = Utils.LocalizeTerm("No Selected Bets");
        if (count == 1)
            SelectedBetsText.text = Utils.LocalizeTerm("Bet") + ": " + selectedBetsText;
        else
            SelectedBetsText.text = Utils.LocalizeTerm("Bets") + ":\n" + selectedBetsText;
    }

    public void SelectCustomBetsButtom()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.CustomBetsPopup);
    }
}
