using UnityEngine;
using UnityEngine.UI;

public class TourneyWinView : AbstractWinView
{
    #region Public Members
    public Text PlayerPointsWonText;
    public Text OpponentPointsWonText;
    public Text PlayerPointsLostText;
    public Text OpponentPointsLostText;

    public GameObject PlayerWonScores;
    public GameObject PlayerLostScores;
    public GameObject OpponentWonScores;
    public GameObject OpponentLostScores;
    #endregion Public Members

    #region Public Functions
    public void ShowTourneyView(int userOldScores, int userNewScores, int opponentOldScores, int opponentNewScores)
    {
        if (userOldScores <= userNewScores)
        {
            PlayerWonScores.SetActive(true);
            PlayerLostScores.SetActive(false);
            PlayerPointsWonText.text = "+" + (userNewScores - userOldScores);
        }
        else
        {
            PlayerWonScores.SetActive(false);
            PlayerLostScores.SetActive(true);
            PlayerPointsLostText.text = (userNewScores - userOldScores).ToString();
        }
        if (opponentOldScores <= opponentNewScores)
        {
            OpponentWonScores.SetActive(true);
            OpponentLostScores.SetActive(false);
            OpponentPointsWonText.text = "+" + (opponentNewScores - opponentOldScores);
        }
        else
        {
            OpponentWonScores.SetActive(false);
            OpponentLostScores.SetActive(true);
            OpponentPointsLostText.text = (opponentNewScores - opponentOldScores).ToString();
        }        
    }
    #endregion Public Functions
}
