using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class TourneyHistoryPopupWidget : PopupWidget
{
    public Text Title;
    public Text TournamentId;
    public Text FirstPrizeText;
    public Text TimeText;
    public ObjectPool HighScorePool;
    public ScrollRect HighScoreContainer;
    public Text DurationText;
    public Text FeeText;
    
    private TourneyHistoryData tourney;
    private List<HighScoresLineView> scoreLines;

    public Animator CopiedBubble;

    public override void EnableWidget()
    {
        base.EnableWidget();
        tourney = UserController.Instance.WatchedHistoryTourney;

        scoreLines = new List<HighScoresLineView>();
        if (tourney != null)
            ShowTourneyFullInfo(tourney);
    }

    public override void DisableWidget()
    {
        UserController.Instance.WatchedHistoryTourney = null;
        base.DisableWidget();
    }

    private void ShowTourneyFullInfo(TourneyHistoryData tourney)
    {
        KeyValuePair<string, float> rewardInfo = TourneyController.Instance.FindFirstReward(tourney.Rewards);
        FirstPrizeText.text = rewardInfo.Key + Utils.LocalizeTerm(Utils.GetNumberPostfix(rewardInfo.Key) + " " + Utils.LocalizeTerm("Prize")) + ": " +
                Wallet.CashPostfix + Wallet.AmountToString(rewardInfo.Value, 2);

        Title.text = Utils.LocalizeTerm("{0} Players Tournament", tourney.HighScore.Length).ToUpper();
        TournamentId.text = Utils.LocalizeTerm("Tournament ID") + " #" + tourney.TourneyId;

        DateTime endDate = tourney.StartDate.AddSeconds(tourney.Duration);
        TimeText.text = Utils.LocalizeTerm("Tournament has ended at") + " " + endDate.ToString();
        DurationText.text = Utils.LocalizeTerm("Tournament Duration {0}", Utils.SecondsToTimeFormat(tourney.Duration));
        FeeText.text = Utils.LocalizeTerm("Fee") + " " + Wallet.CashPostfix + Wallet.AmountToString(tourney.Fee, 2);

        UpdateHighScoreList(tourney);
    }

    private void UpdateHighScoreList(TourneyHistoryData tourney)
    {
        RemoveHighScores();

        TourneyScores[] highScore = tourney.HighScore;
        for (int i = 0; i < highScore.Length; i++)
        {
            HighScoresLineView scoreLine = HighScorePool.GetObjectFromPool().GetComponent<HighScoresLineView>();
            scoreLine.gameObject.InitGameObjectAfterInstantiation(HighScoreContainer.content);
            scoreLines.Add(scoreLine);

            float reward;
            if (tourney.Rewards.TryGetValue((i + 1).ToString(), out reward))
                scoreLine.Init(highScore[i], i + 1, reward);
            else
                scoreLine.Init(highScore[i], i + 1, 0);
        }
    }

    private void RemoveHighScores()
    {
        for (int i = 0; i < scoreLines.Count; i++)
            HighScorePool.PoolObject(scoreLines[i].gameObject);
        scoreLines.Clear();
    }

    private void HideTourneyInfo()
    {
        FirstPrizeText.text = "";
    }

    #region Input
    public void CopyID()
    {
        GUIUtility.systemCopyBuffer = tourney.TourneyId;
        CopiedBubble.SetTrigger("Show");
    }
    #endregion Input
}
