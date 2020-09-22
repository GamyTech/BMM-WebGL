using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Working together with ChallengeFriendView
/// </summary>
public class ChallengeableFriend : DynamicElement
{
    public enum FriendOrigin { Facebook, GamyTech }
    public enum FriendStatus { Online, Busy, Offline }

    public delegate void StatusChanged(FriendStatus status);
    public event StatusChanged OnFriendStatusChanged;

    public string GamytechId { get; private set; }
    public string FacebookId { get; private set; }
    public string Name { get; private set; }
    public SpriteData Picture { get; private set; }

    public FriendStatus Status { get; private set; }
    public FriendOrigin Origin { get; private set; }
    public int MaxBet { get; private set; }
    public bool Invite { get; private set; }
    public List<BetRoom> ValidRooms { get; private set; }

    private string noBetReason;

    public ChallengeableFriend(string facebookId, string name, string picUrl)
    {
        FacebookId = facebookId;
        Name = name;
        Invite = false;
        Picture = new SpriteData(picUrl);

        Status = FriendStatus.Offline;
        Origin = FriendOrigin.Facebook;
        MaxBet = 0;
    }

    public ChallengeableFriend(string gamytechId, string name, string picUrl, List<float> rooms)
    {
        ChallengeableFriend facebookFriend = UserController.Instance.fbUser.InstalledFriendsList.Find(f => gamytechId.Equals(f.GamytechId));
        if(facebookFriend != null)
        {
            Name = name + " (" + facebookFriend.Name + ")";
            Picture = facebookFriend.Picture;
        }
        else
        {
            Name = name;
            Picture = new SpriteData(picUrl);
        }

        GamytechId = gamytechId;
        Invite = true;
        MaxBet = int.MaxValue;

        Status = FriendStatus.Online;
        Origin = FriendOrigin.Facebook;

        // handle rooms
        ValidRooms = new List<BetRoom>();
        List<BetRoom> customrooms;
        ContentController.GetByCategory(AppInformation.MATCH_KIND).TryGetValue(ContentController.CustomCatId, out customrooms);

        for (int i = 0; i < customrooms.Count; i++)
            if (!ValidRooms.Contains(customrooms[i].BetAmount) && rooms.Contains(customrooms[i].BetAmount))
                ValidRooms.Add(new BetRoom(customrooms[i]));

        ValidRooms.Sort((a, b) => a.BetAmount.CompareTo(b.BetAmount));
    }

    protected override void Populate(RectTransform activeObject)
    {
        ChallengeableFriendView view = activeObject.GetComponent<ChallengeableFriendView>();
        OnFriendStatusChanged += view.OnStatusChanged;
        view.PopulateData(this);
    }

    public override void DeactivateObject()
    {
        ChallengeableFriendView view = activeObject.GetComponent<ChallengeableFriendView>();
        OnFriendStatusChanged -= view.OnStatusChanged;

        base.DeactivateObject();
    }

    public void UpdateStatus(Dictionary<string, object> dict)
    {
        if (dict == null) return;

        object o;
        if(dict.TryGetValue("State", out o))
        {
            switch (o.ToString())
            {
                case "ReadyToPlay":
                    Status = FriendStatus.Online;

                    if (dict.TryGetValue("UserId", out o))
                        GamytechId = o.ToString();

                    if (dict.TryGetValue("Bet", out o))
                    {
                        MaxBet = o.ParseInt();
                        ValidRooms = new List<BetRoom>();
                        noBetReason = null;

                        if (MaxBet == 0.0f)
                            noBetReason = "Opponent doesn't have money to play";
                        else
                        {
                            List<BetRoom> customRooms;
                            ContentController.GetByCategory(AppInformation.MATCH_KIND).TryGetValue(ContentController.CustomCatId, out customRooms);
                            int i = 0;
                            while (i < customRooms.Count && customRooms[i].TotalAmount <= UserController.Instance.wallet.Cash  && customRooms[i].BetAmount <= MaxBet)
                            {
                               ValidRooms.Add(new BetRoom(customRooms[i]));
                                ++i;
                            }

                            if (ValidRooms.Count == 0)
                                noBetReason = "You don't have money to play.";
                        }
                    }
                    break;
                case "Playing":
                    Status = FriendStatus.Busy;
                    break;
                default:
                    Status = FriendStatus.Offline;
                    break;
            }
        }

        if (OnFriendStatusChanged != null)
            OnFriendStatusChanged(Status);
    }

    public void Play()
    {
        if (Status != FriendStatus.Online)
            return;

        if (!string.IsNullOrEmpty(noBetReason))
            PopupController.Instance.ShowSmallPopup("No Availabe Bets", new string[] { noBetReason });
        else
        {
            UserController.Instance.fbUser.ChallengedFriend = this;
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.ChallengePopup);
        }

    }
}
