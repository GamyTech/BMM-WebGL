using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using GT.Websocket;

namespace GT.User
{
    public class MatchHistoryData : FragmentedListDynamicElement
    {
        public enum MatchSatus
        {
            Won,
            Lost,
            Canceled,
            Pending,
        }

        public string OpponentName { get; private set; }
        public string Country { get; private set; }
        public string WinnerID { get; private set; }
        public float Bet { get; private set; }
        public float Fee { get; private set; }
        public DateTime StartDate { get; private set; }
        public Enums.MatchKind Kind { get; private set; }
        public int Loyalty { get; private set; }
        public MatchSatus Status { get; private set; }
        public float Win { get; private set; }
        public SpriteData OpponentAvatar { get; private set; }

        private int m_id;
        public override int Id { get { return m_id; } }
        public override bool IsFiller { get { return false; } }
        public override bool NeedToUpdate { get { return Status == MatchSatus.Pending; } }

        public MatchHistoryData(Dictionary<string, object> dict, string currentUserId)
        {
            object o;
            if (dict.TryGetValue("Id", out o))
                m_id = o.ParseInt();
            else
                Debug.LogError("Id missing");

            if (dict.TryGetValue("UserName", out o))
                OpponentName = o.ToString();
            else
                Debug.LogError("UserName missing");

            if (dict.TryGetValue("PictureUrl", out o))
                OpponentAvatar = new SpriteData(o.ToString());
            else
            {
                Debug.LogError("PictureUrl missing");
                OpponentAvatar = new SpriteData(AssetController.Instance.EmptySprite);
            }

            if (dict.TryGetValue("Country", out o))
                Country = o.ToString();
            else
                Debug.LogError("Country missing");

            if (dict.TryGetValue("Winner", out o))
                WinnerID = o.ToString();
            else
                Debug.LogError("Winner missing");

            if (WinnerID == currentUserId)
                Status = MatchSatus.Won;
            else if (WinnerID == "Canceled")
                Status = MatchSatus.Canceled;
            else if (WinnerID == "Pending")
                Status = MatchSatus.Pending;
            else
                Status = MatchSatus.Lost;

            float doubleBet = 0.0f;
            if (dict.TryGetValue("DoubleBet", out o))
                doubleBet = o.ParseFloat();
            else
                Debug.LogError("DoubleBet missing");

            if (doubleBet == 0.0f)
            {
                if (dict.TryGetValue("InitialBet", out o))
                    Bet = o.ParseFloat();
                else
                    Debug.LogError("InitialBet missing");

                if (dict.TryGetValue("InitialFee", out o))
                    Fee = o.ParseFloat();
                else
                    Debug.LogError("InitialFee missing");
            }
            else
            {
                Bet = doubleBet;

                if (dict.TryGetValue("DoubleFee", out o))
                    Fee = o.ParseFloat();
                else
                    Debug.LogError("DoubleFee missing");
            }
            Win = Status == MatchSatus.Won? Bet * 2 : Bet;

            if (dict.TryGetValue("LoyaltyPoints", out o))
                Loyalty = o.ParseInt();
            else
                Debug.LogError("LoyaltyPoints missing");

            DateTime time;
            if (dict.TryGetValue("StartDate", out o) && DateTime.TryParse(o.ToString(), out time))
                StartDate = time;
            else
                Debug.LogError("StartDate missing");

            Enums.MatchKind kind;
            if (dict.TryGetValue("MatchKind", out o) && Utils.TryParseEnum(o, out kind))
                Kind = kind;
            else
                Kind = AppInformation.MATCH_KIND;
        }

        protected override void Populate(RectTransform activeObject)
        {
            activeObject.GetComponent<MatchView>().Populate(this);
        }
    }
}