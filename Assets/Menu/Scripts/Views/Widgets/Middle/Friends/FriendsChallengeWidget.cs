using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FriendsChallengeWidget : Widget
{
    public InputField SearchInputField;
    public int RefreshTime = 10;
    public ScrollRect scrollRect;
    public HorizontalOrVertcalDynamicContentLayoutGroup content;

    public override void EnableWidget()
    {
        base.EnableWidget();

        UserController.Instance.OnFacebookLoggedIn += User_OnFacebookLoggedIn;
        UserController.Instance.OnFacebookLoggedOut += User_OnFacebookLoggedOut;

        if (UserController.Instance.IsLoggedToFB())
        {
            UserController.Instance.fbUser.OnChallengableFriendsChanged += OnChallengeableFriendsChanged;

            StartPopulatingFriends(UserController.Instance.fbUser.InstalledFriendsList);
            StartRefreshingStatus();
#if UNITY_STANDALONE || UNITY_WEBGL
            MiddleWidgetScrollBar.Instance.SetNewScrollRect(scrollRect);
#endif
        }
        else
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public override void DisableWidget()
    {
        StopPopulatingFriends();
        StopResreshingStatus();

        UserController.Instance.OnFacebookLoggedIn -= User_OnFacebookLoggedIn;
        UserController.Instance.OnFacebookLoggedOut -= User_OnFacebookLoggedOut;

        if (UserController.Instance.IsLoggedToFB())
        {
            UserController.Instance.fbUser.OnChallengableFriendsChanged -= OnChallengeableFriendsChanged;
        }

        ClearFriends();
        ClearSearch();

        base.DisableWidget();
    }

    private void StartRefreshingStatus()
    {
        if (refreshFriendsStatusRoutine != null)
        {
            StopCoroutine(refreshFriendsStatusRoutine);
        }
        refreshFriendsStatusRoutine = refreshFriendsStatus(RefreshTime);
        StartCoroutine(refreshFriendsStatusRoutine);
    }

    private void StopResreshingStatus()
    {
        if (refreshFriendsStatusRoutine != null)
        {
            StopCoroutine(refreshFriendsStatusRoutine);
            refreshFriendsStatusRoutine = null;
        }
    }

    IEnumerator refreshFriendsStatusRoutine;
    IEnumerator refreshFriendsStatus(float waitTime)
    {
        while(true)
        {
            UserController.Instance.GetFriendsFromServer();
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void StartPopulatingFriends(List<ChallengeableFriend> friends)
    {
        if(friends == null)
        {
            // TODO show "empty" image
            return;
        }

        List<ChallengeableFriend> sortedList = new List<ChallengeableFriend>();
        for (int x = friends.Count - 1; x >= 0; --x)
        {
            if (friends[x].Status == ChallengeableFriend.FriendStatus.Online)
                sortedList.Insert(0, friends[x]);
            else
                sortedList.Add(friends[x]);
        }

        StopPopulatingFriends();
        content.ClearElements();
        PopulateFriendsRoutine = PopulateFriends(sortedList.ToArray());

        if (gameObject.activeInHierarchy)
            StartCoroutine(PopulateFriendsRoutine);
    }

    private void StopPopulatingFriends()
    {
        if (PopulateFriendsRoutine != null)
        {
            StopCoroutine(PopulateFriendsRoutine);
            PopulateFriendsRoutine = null;
        }
    }

    IEnumerator PopulateFriendsRoutine;
    IEnumerator PopulateFriends(IDynamicElement[] friends)
    {
        SetFriends(new List<IDynamicElement>(friends));
        yield return null;
    }

    void AddFriend(IDynamicElement friend)
    {
        content.AddElement(friend);
    }

    void AddFriends(List<IDynamicElement> friends)
    {
        content.AddElements(friends);
    }

    void SetFriends(List<IDynamicElement> friends)
    {
        content.SetElements(friends);
    }

    void ClearFriends()
    {
        content.ClearElements();
    }

    void ClearSearch()
    {
        SearchInputField.text = "";
    }

    #region Events
    void User_OnFacebookLoggedIn()
    {
        gameObject.SetActive(true);

        UserController.Instance.fbUser.OnChallengableFriendsChanged += OnChallengeableFriendsChanged;

        StartPopulatingFriends(UserController.Instance.fbUser.InstalledFriendsList);
        StartRefreshingStatus();
    }

    void User_OnFacebookLoggedOut()
    {
        gameObject.SetActive(false);
    }

    private void OnChallengeableFriendsChanged(List<ChallengeableFriend> friends)
    {
        StartPopulatingFriends(friends);
    }
    #endregion Events

    #region Buttons
    public void SearchChanged(string str)
    {
        StartPopulatingFriends(UserController.Instance.fbUser.GetChallengeableWithSearch(str));
    }
    #endregion Buttons
}
