using System.Collections.Generic;
using GT.Websocket;
using UnityEngine;
using GT.Database;

public class FBUser
{
    #region Delegates And Events
    public delegate void ChallengableFriendsChanged(List<ChallengeableFriend> friends);

    public event ChallengableFriendsChanged OnChallengableFriendsChanged;
    #endregion Delegates And Events

    #region Static Variables
    public static int maxInvites = 25;

    public static Dictionary<ChallengeableFriend.FriendStatus, Color> StatusToColorDict = new Dictionary<ChallengeableFriend.FriendStatus, Color>()
    {
        { ChallengeableFriend.FriendStatus.Online, new Color(.218f,.554f,.234f) },
        { ChallengeableFriend.FriendStatus.Busy, new Color(.964f,.66f,.042f) },
        { ChallengeableFriend.FriendStatus.Offline, new Color(.824f,.183f,.183f) },
    };

    public static Dictionary<ChallengeableFriend.FriendOrigin, char> OriginToCharDict = new Dictionary<ChallengeableFriend.FriendOrigin, char>()
    {
        { ChallengeableFriend.FriendOrigin.Facebook, 'r' },
        { ChallengeableFriend.FriendOrigin.GamyTech, '(' },
    };
    #endregion Static Variables

    public string Id { get; protected set; }
    public string Email { get; protected set; }
    public string Name { get; protected set; }
    public string FirstName { get; protected set; }
    public string PictureURL { get; protected set; }

    public List<ChallengeableFriend> InstalledFriendsList { get; protected set; }

    public ChallengeableFriend ChallengedFriend;

    #region Constractors
    public FBUser(string id, string name, string firstName, string email, string pictureURL, List<ChallengeableFriend> installedFriends)
    {
        WebSocketKit.Instance.RegisterForAPIVariableEvent(APIResponseVariable.GamyTechFriends, OnFriendsStatusChanged);
        Id = id;
        Name = name;
        FirstName = firstName;
        Email = email;
        PictureURL = pictureURL;
        InstalledFriendsList = installedFriends;
        if (OnChallengableFriendsChanged != null)
            OnChallengableFriendsChanged(InstalledFriendsList);
    }

    ~FBUser()
    {
        WebSocketKit.Instance.UnregisterForAPIVariableEvent(APIResponseVariable.GamyTechFriends, OnFriendsStatusChanged);
    }
    #endregion Constractors

    public List<ChallengeableFriend> GetChallengeableWithSearch(string str)
    {
        List<ChallengeableFriend> matchedList = new List<ChallengeableFriend>();
        for (int i = 0; i < InstalledFriendsList.Count; i++)
            if (InstalledFriendsList[i].Name.ToLower().Contains(str.ToLower()))
                matchedList.Add(InstalledFriendsList[i]);

        return matchedList;
    }

    public List<string> GetChallengeableFriends()
    {
        List<string> list = new List<string>();
        for (int i = 0; i < InstalledFriendsList.Count; i++)
            list.Add(InstalledFriendsList[i].FacebookId);
        
        return list;
    }

    #region Overrides
    public override string ToString()
    {
        return "id:" + Id + " , name:" + Name + " , email:" + Email + " , friends:"
            + InstalledFriendsList.Display();
    }
    #endregion Overrides

    #region Events
    private void OnFriendsStatusChanged(object o)
    {
        Dictionary<string, object> dict = (Dictionary<string, object>)o;
        object data;
        for (int i = 0; i < InstalledFriendsList.Count; i++)
            if (dict.TryGetValue(InstalledFriendsList[i].FacebookId, out data))
                InstalledFriendsList[i].UpdateStatus((Dictionary<string, object>)data);

        if (OnChallengableFriendsChanged != null)
            OnChallengableFriendsChanged(InstalledFriendsList);
    }
    #endregion Events
}
