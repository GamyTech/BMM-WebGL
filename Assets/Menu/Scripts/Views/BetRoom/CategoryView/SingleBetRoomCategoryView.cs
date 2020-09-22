using UnityEngine.UI;

public class SingleBetRoomCategoryView : BetRoomCategoryView
{
    public Text WinAmountText;
    public Text AmountsText;

    protected override void InitRooms()
    {
        Rooms[0].Selected = true;
        AmountsText.text = "-- " + Utils.LocalizeTerm("Bet") + ": " + Wallet.AmountToString(Rooms[0].BetAmount, Rooms[0].Kind) +
            " +" + Utils.LocalizeTerm("Fee") + ": " + Wallet.AmountToString(Rooms[0].FeeAmount, Rooms[0].Kind) + " --";

        WinAmountText.text = Utils.LocalizeTerm("Win") + ": " + Wallet.AmountToString(Rooms[0].WinAmount, Rooms[0].Kind);
    }
}
