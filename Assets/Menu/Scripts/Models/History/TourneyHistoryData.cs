using System;
using System.Collections.Generic;

public class TourneyHistoryData
{
    public string TourneyId { get; private set; }
    public int Duration { get; private set; }
    public float BuyIn { get; private set; }
    public float Fee { get; private set; }
    public int MaxPlayers { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime ClosedDate { get; private set; }
    public string Status { get; private set; }
    public Dictionary<string, float> Rewards { get; private set; }
    public TourneyScores[] HighScore { get; private set; }

    public TourneyHistoryData(Dictionary<string, object> data)
    {
        object o;

        if (data.TryGetValue("TourneyId", out o))
            TourneyId = o.ToString();
        if (data.TryGetValue("Duration", out o))
            Duration = o.ParseInt();
        if (data.TryGetValue("BuyIn", out o))
            BuyIn = o.ParseFloat();
        if (data.TryGetValue("Fee", out o))
            Fee = o.ParseFloat();
        if (data.TryGetValue("MaxPlayers", out o))
            MaxPlayers = o.ParseInt();
        if (data.TryGetValue("StartDate", out o))
            StartDate = DateTime.Parse(o.ToString());
        if (data.TryGetValue("ClosedDate", out o))
            ClosedDate = DateTime.Parse(o.ToString());
        if (data.TryGetValue("Status", out o))
            Status = o.ToString();

        if (data.TryGetValue("Rewards", out o))
        {
            Rewards = new Dictionary<string, float>();
            Dictionary<string, object> rewardsData = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> rewardDataPair in rewardsData)
            {
                Rewards.Add(rewardDataPair.Key, rewardDataPair.Value.ParseFloat());
            }
        }

        if (data.TryGetValue("Results", out o) && !string.IsNullOrEmpty(o.ToString()))
        {
            List<object> results = MiniJSON.Json.Deserialize(o.ToString()) as List<object>;
            HighScore = new TourneyScores[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                Dictionary<string, object> result = results[i] as Dictionary<string, object>;
                int rank;
                if (result.TryGetValue("Rank", out o))
                {
                    rank = o.ParseInt();
                    HighScore[rank - 1] = new TourneyScores(result);
                }
            }
        }
        else
            HighScore = new TourneyScores[0];
    }

    public int GetPlyaerPlace(string id)
    {
        for (int i = 0; i < HighScore.Length; i++)
        {
            if (HighScore[i].UserId == id) return i + 1;
        }
        return -1;
    }
}