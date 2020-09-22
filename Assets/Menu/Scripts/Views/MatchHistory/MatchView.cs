using UnityEngine;
using UnityEngine.UI;
using GT.User;

public class MatchView : MonoBehaviour
{
    public Image PictureImage;
    public Text PendingIconText;
    public Text StatusText;
    public Text NameText;
    public Text AmountText;
    public GameObject VS;
    public Text DateText;
    public Text FeeText;

    private MatchHistoryData m_match;

    public void Populate(MatchHistoryData match)
    {
        this.m_match = match;

        NameText.text = match.OpponentName;
        DateText.text = Utils.LocalizeTerm("Date") + ": " + m_match.StartDate.ToString("dd/MM/yyyy");
        FeeText.text = Utils.LocalizeTerm("Entry Fee") + ": " + Wallet.MatchKindToPrefix(match.Kind) + Wallet.AmountToString(m_match.Fee, 2);
        match.OpponentAvatar.LoadImage(this, s => PictureImage.sprite = s, AssetController.Instance.DefaultAvatar);
        AmountText.text = Wallet.MatchKindToPrefix(m_match.Kind) + Wallet.AmountToString(m_match.Win, 2);

        AmountText.gameObject.SetActive(true);
        VS.SetActive(match.Status != MatchHistoryData.MatchSatus.Lost);

        if (match.Status == MatchHistoryData.MatchSatus.Won)
        {
            StatusText.transform.SetAsFirstSibling();
            StatusText.text = Utils.LocalizeTerm("You Won").ToUpper();
        }
        else if (match.Status == MatchHistoryData.MatchSatus.Lost)
        {
            StatusText.transform.SetAsLastSibling();
            StatusText.text = Utils.LocalizeTerm("WON").ToUpper();
        }
        else
        {
            StatusText.transform.SetAsFirstSibling();
            StatusText.text = Utils.LocalizeTerm(match.Status.ToString()).ToUpper();
            AmountText.gameObject.SetActive(false);
        }
    }

    #region Buttons
    public void InfoButtonClick()
    {
        UserController.Instance.WatchedHistoryMatch = m_match;
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.MatchDataPopup);
    }
    #endregion Buttons
}
