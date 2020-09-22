using UnityEngine;
using UnityEngine.UI;

public class TourneyView : MonoBehaviour
{
    public Text PlaceText;
    public Text TourneyName;
    public Text WinAmount;
    public Text DateText;
    public Text BuyInText;

    private TourneyHistoryData tourney;

    public void Populate(TourneyHistoryData tourney)
    {
        this.tourney = tourney;

        int place = tourney.GetPlyaerPlace(UserController.Instance.gtUser.Id);
        if (place < 1)
            PlaceText.text = "";
        else
            PlaceText.text = place.ToString() + Utils.LocalizeTerm(Utils.GetNumberPostfix(place.ToString()) + " " + "Place");

        TourneyName.text = Utils.LocalizeTerm("{0} Players Tournament", tourney.MaxPlayers);
        float reward;
        if (tourney.Rewards.TryGetValue(place.ToString(), out reward))
            WinAmount.text = Wallet.CashPostfix + Wallet.AmountToString(reward, 2);
        else
            WinAmount.text = "";
        DateText.text = tourney.StartDate.ToShortDateString();
        BuyInText.text = Utils.LocalizeTerm("Buy-In of") + ": " + Wallet.CashPostfix + Wallet.AmountToString(tourney.BuyIn, 2);
    }

    #region Buttons
    public void InfoButtonClick()
    {
        UserController.Instance.WatchedHistoryTourney = tourney;
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyHistoryPopup);
    }
    #endregion Buttons
}
