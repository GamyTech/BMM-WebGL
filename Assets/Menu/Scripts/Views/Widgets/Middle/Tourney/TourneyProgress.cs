using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourneyProgress : Widget
{
    public float updateScoresSeconds = 5;

    public Text Title;
    public Text TournamentId;
    public Text FirstPrizeText;
    public Slider TimeProgressBar;
    public Text EndsInText;
    public ObjectPool HighScorePool;
    public ScrollRect HighScoreContainer;

    public GameObject ButtonWarning;
    public Text ButtonWarningSeconds;
    public GameObject MatchesWarning;
    public Text MatchesWarningSeconds;

    public Animator PlayButtonAnimator;

    private OngoingTourneyDetails tourneyDetails;
    private float secondsLeft;
    private List<HighScoresLineView> scoreLines;

    public override void EnableWidget()
    {
        base.EnableWidget();
        TourneyController.Instance.OnOngoingTourneyChanged += GTUser_OnOngoingTourneyChanged;

        scoreLines = new List<HighScoresLineView>();

        tourneyDetails = TourneyController.Instance.ongoingTourney;
        if (tourneyDetails != null)
        {
            ShowTourneyProgress(tourneyDetails);

            StartCoroutine(UpdateTourneyDetails());
            UserController.Instance.GetSpecificTourneyDetails(tourneyDetails.TourneyId);
        }

        UserController.Instance.GetTourneysInfo();
    }

    public override void DisableWidget()
    {
        base.DisableWidget();
        if (TourneyController.Instance != null)
        {
            TourneyController.Instance.OnOngoingTourneyChanged -= GTUser_OnOngoingTourneyChanged;
        }

        StopAllCoroutines();
    }

    private void Update()
    {
        if (secondsLeft > 0)
            UpdateTimeView(Time.deltaTime);
    }

    private void ShowTourneyProgress(OngoingTourneyDetails tourneyDetails)
    {
        Title.text = Utils.LocalizeTerm("{0} Players Tournament", tourneyDetails.HighScore.Count).ToUpper();
        TournamentId.text = Utils.LocalizeTerm("Tournament ID") + " #" + tourneyDetails.TourneyDbId;

        KeyValuePair<string, float> rewardInfo = TourneyController.Instance.FindFirstReward(tourneyDetails.Rewards);
        FirstPrizeText.text = rewardInfo.Key + Utils.LocalizeTerm(Utils.GetNumberPostfix(rewardInfo.Key) + " " + Utils.LocalizeTerm("Prize")) + ": " +
                Wallet.CashPostfix + Wallet.AmountToString(rewardInfo.Value, 2);

        DateTime endDate = tourneyDetails.StartDate.AddSeconds(tourneyDetails.MaxTime);
        secondsLeft = Mathf.Max(0, (float)(endDate - DateTime.UtcNow).TotalSeconds);        
        PlayButtonAnimator.SetTrigger(secondsLeft > TourneyController.Instance.matchesClosedTime ? "HighlightOn" : "Disabled");
        UpdateTimeView(0);

        UpdateHighScoreList();
    }

    private void UpdateTimeView(float deltaTime)
    {
        float left = Mathf.Max(0, secondsLeft - deltaTime);

        TimeProgressBar.value = secondsLeft / tourneyDetails.MaxTime;
        EndsInText.text = Utils.LocalizeTerm("Ends in") + ": " + Utils.SecondsToTimeFormat((int)secondsLeft);

        if (secondsLeft > TourneyController.Instance.matchesClosedTime && left <= TourneyController.Instance.matchesClosedTime)
            PlayButtonAnimator.SetTrigger("Disabled");

        ButtonWarning.SetActive(left <= TourneyController.Instance.matchesClosedTime + TourneyController.Instance.warningTime && left > TourneyController.Instance.matchesClosedTime);
        if (ButtonWarning.activeSelf)
            ButtonWarningSeconds.text = Utils.SecondsToShortTimeString((int)Mathf.Ceil(left - TourneyController.Instance.matchesClosedTime));
        MatchesWarning.SetActive(left <= TourneyController.Instance.matchesClosedTime);
        if (MatchesWarning.activeSelf)
            MatchesWarningSeconds.text = Utils.SecondsToShortTimeString((int)Mathf.Ceil(left));

        secondsLeft = left;
    }

    private void UpdateHighScoreList()
    {
        RemoveHighScores();

        List<TourneyScores> highScore = tourneyDetails.HighScore;
        for (int i = 0; i < highScore.Count; i++)
        {
            HighScoresLineView scoreLine = HighScorePool.GetObjectFromPool().GetComponent<HighScoresLineView>();
            scoreLine.gameObject.InitGameObjectAfterInstantiation(HighScoreContainer.content);
            scoreLines.Add(scoreLine);
            scoreLine.Init(highScore[i], i + 1);
        }
    }

    private void RemoveHighScores()
    {
        for (int i = 0; i < scoreLines.Count; i++)
            HighScorePool.PoolObject(scoreLines[i].gameObject);
        scoreLines.Clear();
    }

    private IEnumerator UpdateTourneyDetails()
    {
        while (secondsLeft > 0)
        {
            yield return new WaitForSeconds(updateScoresSeconds);
            UserController.Instance.GetSpecificTourneyDetails(tourneyDetails.TourneyId);
        }
    }

    #region Input
    public void ShowInfo()
    {
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.TourneyInfoPopup);
    }

    public void Play()
    {
        if (secondsLeft > TourneyController.Instance.matchesClosedTime)
        {
            UserController.Instance.SendStartSearchingTourneyMatch(tourneyDetails.TourneyId);
            PageController.Instance.ChangePage(Enums.PageId.Searching);
        }
    }
    #endregion Input

    #region Events
    private void GTUser_OnOngoingTourneyChanged(OngoingTourneyDetails newValue)
    {
        if (newValue == null)
        {
            StopAllCoroutines();
            return;
        }
        if (newValue.TourneyId == tourneyDetails.TourneyId)
        {
            tourneyDetails = newValue;
            ShowTourneyProgress(tourneyDetails);
            UpdateHighScoreList();
        }
    }
    #endregion Events
}
