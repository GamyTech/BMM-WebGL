using System;
using System.Collections.Generic;
using System.Linq;

public class OngoingTourneyDetails
{
    public string TourneyId { get; private set; }
    public string TourneyDbId { get; private set; }
    public bool isRegisteredOnly { get; private set; }
    public DateTime PreStartDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public List<TourneyScores> HighScore { get; private set; }
    public int MaxTime { get; private set; }
    public float Fee { get; private set; }
    public float BuyInAmount { get; private set; }
    public Dictionary<string, float> Rewards { get; private set; }

    public bool IsInPreTourney { get; private set; }
    public bool IsFinished { get; private set; }
    public int TimeLeftToStartTourney { get; private set; }
    public int TimeToStartTourney { get; private set; }

    public OngoingTourneyDetails(Dictionary<string, object> data, bool isFromPretourney = false, bool isFinished = false)
    {
        IsFinished = isFinished;

        object o;

        if (data.TryGetValue("TourneyId", out o))
            TourneyId = o.ToString();

        isRegisteredOnly = true;
        if (data.TryGetValue("StartDate", out o))
        {
            PreStartDate = DateTime.Parse(o.ToString());
            isRegisteredOnly = false;
        }

        if (data.TryGetValue("TourneyDbId", out o))
        {
            TourneyDbId = o.ToString();
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TourneyId, TourneyDbId);
        }
        else
            TourneyDbId = TourneyId;

        if (data.TryGetValue("MaxTime", out o))
            MaxTime = o.ParseInt();

        if (data.TryGetValue("IsInPreTourney", out o))
            IsInPreTourney = o.ParseBool();
        else 
            IsInPreTourney = isFromPretourney;

        TimeToStartTourney = 30;
        if (data.TryGetValue("TimeToStartTourney", out o))
            TimeToStartTourney = o.ParseInt();

        DateTime currentDate = DateTime.UtcNow;
        if (data.TryGetValue("CurrentDate", out o))
            currentDate = DateTime.Parse(o.ToString());

        if (data.TryGetValue("TimeLeftToStartTourney", out o))
        {
            TimeLeftToStartTourney = o.ParseInt();
            TimeLeftToStartTourney -= (int)(DateTime.UtcNow - currentDate).TotalSeconds;
        }

        StartDate = PreStartDate.AddSeconds(TimeToStartTourney);

        if (data.TryGetValue("BuyInAmount", out o))
            BuyInAmount = o.ParseFloat();
        if (data.TryGetValue("Fee", out o))
            Fee = o.ParseFloat();

        if (data.TryGetValue("Rewards", out o))
        {
            Rewards = new Dictionary<string, float>();
            Dictionary<string, object> rewardsData = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> rewardDataPair in rewardsData)
            {
                Rewards.Add(rewardDataPair.Key, rewardDataPair.Value.ParseFloat());
            }
        }

        Update(data);
    }

    public void Start()
    {
        TimeLeftToStartTourney = 0;
        IsInPreTourney = false;
    }

    public void Update(Dictionary<string, object> data, bool isFinished = false)
    {
        if (isFinished)
            IsFinished = isFinished;

        object o;

        if (data.TryGetValue("TourneyDbId", out o))
        {
            TourneyDbId = o.ToString();
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TourneyId, TourneyDbId);
        }

        if (data.TryGetValue("HighScore", out o))
        {
            HighScore = new List<TourneyScores>();
            List<object> scoresData;
            if (o is List<object>)
                scoresData = o as List<object>;
            else if (!string.IsNullOrEmpty(o.ToString()))
                scoresData = MiniJSON.Json.Deserialize(o.ToString()) as List<object>;
            else
                scoresData = new List<object>();

            for (int i = 0; i < scoresData.Count; i++)
            {
                HighScore.Add(new TourneyScores(scoresData[i] as Dictionary<string, object>));
            }
            HighScore = SortHighScores(HighScore);
        }
    }

    private List<TourneyScores> SortHighScores(List<TourneyScores> highScore)
    {
        return highScore.OrderByDescending(o => o.Score).ToList();
    }

    public void CopyMoneyDataFrom(Tourney tourney)
    {
        if (tourney == null) return;
        Fee = tourney.Fee;
        if (tourney.Rewards != null && tourney.Rewards.Count > 0)
            Rewards = tourney.Rewards;
        BuyInAmount = tourney.BuyIn;
    }
}

public class TourneyScores
{
    public string UserName { get; private set; }
    public string UserId { get; private set; }
    public int Score { get; private set; }
    public string Country { get; private set; }

    public TourneyScores(Dictionary<string, object> data)
    {
        object o;
        if (data.TryGetValue("UserName", out o))
            UserName = o.ToString();
        if (data.TryGetValue("UserId", out o))
            UserId = o.ToString();
        if (data.TryGetValue("Score", out o))
            Score = o.ParseInt();
        if (data.TryGetValue("Country", out o))
            Country = o.ToString();
    }
}