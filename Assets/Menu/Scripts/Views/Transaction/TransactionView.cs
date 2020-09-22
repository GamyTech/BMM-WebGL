using UnityEngine;
using UnityEngine.UI;

public class TransactionView : MonoBehaviour
{
    public Text Date;
    public Text Reason;
    public Text Amount;
    public Text MatchId;
    public Text Currency;

    public void Populate(TransactionData transaction)
    {
        Date.text = transaction.Date.ToString("dd/MM/yy");

        Amount.text = (transaction.Amount < 0f ? "- " : "+ ") + Wallet.MatchKindToPrefix(AppInformation.MATCH_KIND) + Mathf.Abs(transaction.Amount);

        Reason.text = transaction.Reason;

        if (transaction.Reason != null)
        {
            bool isInMatch = transaction.Reason.ToLower().Contains("match");
            MatchId.gameObject.SetActive(isInMatch);

            if (isInMatch)
                MatchId.text = transaction.Id.ToString();
        }
        else
            MatchId.gameObject.SetActive(false);

        Currency.text = Wallet.MatchKindToPrefix(AppInformation.MATCH_KIND) + transaction.Currency.ToString("F");
    }
}
