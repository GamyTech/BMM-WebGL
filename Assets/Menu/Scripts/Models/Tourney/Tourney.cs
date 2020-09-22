using System.Collections.Generic;

public class Tourney
{
    public bool IsStarted { get; private set; }
    public string TourneyId { get; private set; }
    public int DurationSeconds { get; private set; }
    public float Bet { get; private set; }
    public float BuyIn { get; private set; }
    public float Fee { get; private set; }
    public int Loyalty { get; private set; }
    public int MaxPlayers { get; private set; }
    public Dictionary<string, float> Rewards { get; private set; }
    public int CurrentPlayersAmount { get; private set; }
    public List<string> RegisteredUsers { get; private set; }

    public Tourney()
    {
        Rewards = new Dictionary<string, float>();
    }

    public void Update(Dictionary<string, object> data, bool isFromPretourney = false)
    {
        object o;

        if (data.TryGetValue("IsStarted", out o))
            IsStarted = o.ParseBool();
        if (data.TryGetValue("TourneyId", out o))
            TourneyId = o.ToString();
        if (data.TryGetValue("Duration", out o))
            DurationSeconds = o.ParseInt();
        if (data.TryGetValue("Bet", out o))
            Bet = o.ParseFloat();
        if (data.TryGetValue("BuyIn", out o))
            BuyIn = o.ParseFloat();
        if (data.TryGetValue("Fee", out o))
            Fee = o.ParseFloat();
        if (data.TryGetValue("Loyalty", out o))
            Loyalty = o.ParseInt();
        if (data.TryGetValue("MaxPlayers", out o))
            MaxPlayers = o.ParseInt();
        if (data.TryGetValue("CurrentPlayersAmount", out o))
            CurrentPlayersAmount = o.ParseInt();

        if (data.TryGetValue("Rewards", out o))
        {
            Rewards = new Dictionary<string, float>();
            Dictionary<string, object> rewardsData = MiniJSON.Json.Deserialize(o.ToString()) as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> rewardDataPair in rewardsData)
            {
                Rewards.Add(rewardDataPair.Key, rewardDataPair.Value.ParseFloat());
            }
        }

        RegisteredUsers = new List<string>();
        if (data.TryGetValue("RegisteredUsers", out o))
        {
            List<object> userIds = o as List<object>;
            for (int i = 0; i < userIds.Count; i++)
                RegisteredUsers.Add(userIds[i].ToString());
        }
    }

    public static Tourney MakeTempInfoFrom(OngoingTourneyDetails ongoing)
    {
        Tourney temp = new Tourney();
        temp.TourneyId = ongoing.TourneyId;
        temp.MaxPlayers = ongoing.HighScore != null ? ongoing.HighScore.Count : 0;
        temp.CurrentPlayersAmount = temp.MaxPlayers;
        temp.Fee = ongoing.Fee;
        temp.BuyIn = ongoing.BuyInAmount;
        temp.Rewards = ongoing.Rewards;
        temp.IsStarted = true;
        temp.DurationSeconds = ongoing.MaxTime;
        return temp;
    }

    public void SetRewards(Dictionary<string, float> rewards)
    {
        Rewards = rewards;
    }
}
