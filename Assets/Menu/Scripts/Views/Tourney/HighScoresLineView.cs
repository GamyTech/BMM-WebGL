using UnityEngine;
using UnityEngine.UI;

public class HighScoresLineView : MonoBehaviour
{
    public Text UserName;
    public Text Scores;
    public Image Background;
    public Text Reward;

    public Color NormalColor;
    public Color UserColor;

    public void Init(TourneyScores tourneyScores, int position)
    {
        Background.color = tourneyScores.UserId == UserController.Instance.gtUser.Id ? UserColor : NormalColor;
        
        UserName.text = position + ". " + tourneyScores.UserName;
        Scores.text = tourneyScores.Score.ToString();
    }

    public void Init(TourneyScores tourneyScores, int position, float reward)
    {
        Init(tourneyScores, position);
        Reward.text = reward == 0 ? "" : Wallet.CashPostfix + Wallet.AmountToString(reward, 2);
    }
}
