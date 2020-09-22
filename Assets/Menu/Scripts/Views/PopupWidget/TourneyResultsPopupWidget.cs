using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourneyResultsPopupWidget : PopupWidget
{
    public Text TopText;
    public GameObject ContinueButton;
    public GameObject CollectButton;
    public GameObject ShareButton;
    public ObjectPool HighScorePool;
    public ScrollRect HighScoreContainer;

    private string place;
    private float reward;
    private int score;
    private List<HighScoresLineView> scoreLines;

    public override void EnableWidget()
    {
        base.EnableWidget();

        ContinueButton.SetActive(false);
        CollectButton.SetActive(false);
        ShareButton.SetActive(false);

        scoreLines = new List<HighScoresLineView>();

        UpdateView(TourneyController.Instance.ongoingTourney);
        TourneyController.Instance.OnOngoingTourneyChanged += OnOngoingTourneyChanged;
    }

    public override void DisableWidget()
    {
        TourneyController.Instance.OnOngoingTourneyChanged -= OnOngoingTourneyChanged;
        base.DisableWidget();
    }

    private void UpdateView(OngoingTourneyDetails ongoingTourney)
    {
        if (ongoingTourney == null) return;

        RemoveHighScores();

        for (int i = 0; i < ongoingTourney.HighScore.Count; i++)
        {
            if (ongoingTourney.HighScore[i].UserId == UserController.Instance.gtUser.Id)
            {
                TourneyScores scores = ongoingTourney.HighScore[i];
                score = scores.Score;
                string placeName = (i + 1).ToString();
                place = placeName + Utils.LocalizeTerm(Utils.GetNumberPostfix(placeName) + " Place");
                if (ongoingTourney.Rewards.TryGetValue(placeName, out reward))
                {
                    TopText.text = Utils.LocalizeTerm("Congratulations") + " " + Utils.LocalizeTerm("You got") + " " + place + "! " +
                        Utils.LocalizeTerm("You won") + ": " + Wallet.CashPostfix + reward + ".";
                    CollectButton.SetActive(true);
                    ShareButton.SetActive(true);
                }
                else
                {
                    TopText.text = Utils.LocalizeTerm("You got") + " " + place + ".";
                    ContinueButton.SetActive(true);
                }
            }
            HighScoresLineView scoreLine = HighScorePool.GetObjectFromPool().GetComponent<HighScoresLineView>();
            scoreLine.gameObject.InitGameObjectAfterInstantiation(HighScoreContainer.content);
            scoreLines.Add(scoreLine);
            
            if (ongoingTourney.Rewards.TryGetValue((i + 1).ToString(), out reward))
                scoreLine.Init(ongoingTourney.HighScore[i], i + 1, reward);
            else
                scoreLine.Init(ongoingTourney.HighScore[i], i + 1, 0);
        }
    }

    private void RemoveHighScores()
    {
        for (int i = 0; i < scoreLines.Count; i++)
            HighScorePool.PoolObject(scoreLines[i].gameObject);
        scoreLines.Clear();
    }

    public void Continue()
    {
        GoHome();
    }

    public void Collect()
    {
        UserController.Instance.GetUserVarsFromServer(GT.Websocket.APIGetVariable.Wallet);
        GoHome();
    }

    private void GoHome()
    {
        TourneyController.Instance.ClearTourney();
        if (PageController.Instance.CurrentPageId != Enums.PageId.Home)
            PageController.Instance.ChangePage(Enums.PageId.Home);
        IsClosable = true;
        HidePopup();
    }

    public void Share()
    {
        FacebookKit.ShareTourney(place, score, reward);
    }

    private void OnOngoingTourneyChanged(OngoingTourneyDetails tourney)
    {
        UpdateView(tourney);
    }
}
