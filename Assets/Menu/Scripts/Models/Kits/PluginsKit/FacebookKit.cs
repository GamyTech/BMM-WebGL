using UnityEngine;
using System;
using System.Collections.Generic;
#if !UNITY_STANDALONE
using Facebook.Unity;
#endif

public class FacebookKit
{

#region Enums
    public enum FacebookResult
    {
        Success,
        Cancelled,
        Failed,
        AlreadyLoggedIn,
    }
#endregion Enums

#region Variables
    private static List<string> loginPermission = new List<string> { "public_profile", "email", "user_friends" };
    const string initialAPIRequest = "me?fields=id,name,first_name,picture.height(256).width(256),email,friends.limit(2000).fields(name,picture.height(100).width(100))";
#endregion Variables

    public static void Init()
    {
#if !UNITY_STANDALONE
        if (FB.IsInitialized)
            FB.ActivateApp();
        else
            FB.Init(AppInformation.FACEBOOK_APP_ID, onInitComplete: OnInitComplete, onHideUnity: OnHideUnity);
#endif
    }

#region User Functions
    public static void GetFBUser(Action<FBUser> callback)
    {
#if !UNITY_STANDALONE
        if (FB.IsLoggedIn)
        {
            Debug.Log("TryGetFBUser : IsLoggedIn");
            InitialAPIRequest(r => OnGetFBUser(r, callback));
        }
        else
        {
            if (AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
                Login(l => InitialAPIRequest(r => OnGetFBUser(r, callback)));
            else if(callback != null)
                callback(null);
        }
#endif
    }

    public static void Login(Action<FBResponse> callback)
    {
#if !UNITY_STANDALONE
        if (FB.IsInitialized)
        {
            Debug.Log("TryToLoginFB : isIntialized + " + AppInformation.FACEBOOK_APP_ID);
            LoginWithPermissions(callback);
        }
        else if (!FB.IsLoggedIn)
        {
            Debug.Log("TryToLoginFB : Is!LoggedIn + " + AppInformation.FACEBOOK_APP_ID);
            FB.Init(AppInformation.FACEBOOK_APP_ID,clientToken: FB.ClientToken, onInitComplete: () => LoginWithPermissions(callback), onHideUnity: OnHideUnity);
        }
        else
        {
            Debug.Log("TryToLoginFB : AlreadyLoggedIn");
            if (callback != null)
                callback(new FBResponse(FacebookResult.AlreadyLoggedIn));
        }
#endif
    }

    public static void LoginWithPermissions(Action<FBResponse> callback)
    {
#if !UNITY_STANDALONE
        FB.LogOut();
        FB.LogInWithReadPermissions(loginPermission, r => HandleResult(r, callback));
#endif
    }

    public static void Logout()
    {
#if !UNITY_STANDALONE
        Debug.Log("FBToLogOut");
        if (FB.IsLoggedIn)
            FB.LogOut();
#endif
    }

    public static void InitialAPIRequest(Action<FBResponse> callback)
    {
#if !UNITY_STANDALONE
        FB.API(initialAPIRequest, HttpMethod.GET, r => HandleResult(r, callback));
#endif
    }

    public static void ShareTourney(string place, int score, float prize)
    {
        string title = Utils.LocalizeTerm("I got {0} place in Backgammon For Money Tournament!", place);
        string descr = Utils.LocalizeTerm("I scored {0} points and got {1}! Join me in the next Backgammon Tournament", score, Wallet.CashPostfix + prize);
        string link = "http://backgammon4money.com/Tourneys/shareb4m.php?title=" + title + "&descr=" + descr;
#if !UNITY_STANDALONE
        FB.ShareLink(
            new Uri(link),
            callback: (i) => {}
        );
#else
        Application.OpenURL("https://www.facebook.com/sharer/sharer.php?u=" + Uri.EscapeUriString(link));
#endif
    }
    #endregion User Functions

    #region Events
#if !UNITY_STANDALONE
    private static void OnGetFBUser(FBResponse response, Action<FBUser> callback)
    {
        Debug.Log("OnGetFBUser " + response.result + response.data.Display());
        FBUser fbUser = null;
        if (response.result == FacebookResult.Success)
        {
            string id = null;
            response.data.TryGetValue("id", out id);

            string name = null;
            response.data.TryGetValue("name", out name);

            string firstName = null;
            response.data.TryGetValue("first_name", out firstName);

            string email = null;
            response.data.TryGetValue("email", out email);

            object o;
            string PictureURL = null;
            if(GetDataDictFromDict(response.data, "picture", out o))
                if((o as Dictionary<string, object>).TryGetValue("url", out o))
                    PictureURL = o.ToString();

            List<ChallengeableFriend> installedFriends = new List<ChallengeableFriend>();
            if (GetDataDictFromDict(response.data, "friends", out o))
            {
                List<object> fList = (List<object>)o;
                for (int i = 0; i < fList.Count; i++)
                {
                    Dictionary<string, object> friendDict = (Dictionary<string, object>)fList[i];
                    string friendId, friendName, friendPic = null;
                    friendDict.TryGetValue("id", out friendId);
                    friendDict.TryGetValue("name", out friendName);

                    object picObj;
                    if (GetDataDictFromDict(friendDict, "picture", out picObj))
                        (picObj as Dictionary<string, object>).TryGetValue("url", out friendPic);
                    
                    installedFriends.Add(new ChallengeableFriend(friendId, friendName, friendPic));
                }
            }

            if (id != null)
                fbUser = new FBUser(id, name, firstName, email, PictureURL, installedFriends);

            Debug.Log("send FB user " + id + " " + name + " " + email + " " + PictureURL);
        }
        else
            Debug.Log("OnGetFBUser error :" + response.error);

        if (callback != null)
            callback(fbUser);
    }

    private static bool GetDataDictFromDict(Dictionary<string, object> sourceDict, string key, out object data)
    {
        data = null;
        object o;
        if (sourceDict.TryGetValue(key, out o) && o is Dictionary<string, object> && (o as Dictionary<string, object>).TryGetValue("data", out data))
            return true;

        Debug.LogError(key + " doesnt conatin data");
        return false;
    }

    private static void OnAutoInitLogin(Action<FBResponse> callback)
    {
        if (FB.IsInitialized)
            LoginWithPermissions(callback);
        else
        {
            //callback failed to initialize
            if (callback != null)
                callback(new FBResponse(FacebookResult.Failed));
        }
    }

    private static void OnInitComplete()
    {
        FB.ActivateApp();
        Debug.Log(string.Format("OnFacebookInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'", FB.IsLoggedIn, FB.IsInitialized));
    }

    private static void OnHideUnity(bool isGameShown)
    {
        Debug.Log("Is game shown: " + isGameShown);
    }

    /// <summary>
    /// Result Handling
    /// </summary>
    /// <param name="result"></param>
    /// <param name="callback"></param>
    /// 
    private static void HandleResult(IResult result, Action<FBResponse> callback)
    {
        if (result == null)
        {
            Debug.Log("Null Response\n");
            if (callback != null)
                callback(new FBResponse(FacebookResult.Failed));
            return;
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error Response:\n" + result.Error + result.RawResult);
            if (callback != null)
                callback(new FBResponse(FacebookResult.Failed, result.Error));
        }
        else if (result.Cancelled)
        {
            Debug.Log("Cancelled Response:\n" + result.RawResult);
            if (callback != null)
                callback(new FBResponse(FacebookResult.Cancelled));
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("Success Response: " + result.RawResult);
            if (callback != null)
                callback(new FBResponse(FacebookResult.Success, new Dictionary<string, object>(result.ResultDictionary)));
        }
        else
        {
            Debug.Log("Empty Response\n");
            if (callback != null)
                callback(new FBResponse(FacebookResult.Failed));
        }
    }
#endif
    #endregion Events

    #region Response Class
    public class FBResponse
    {
        public FacebookResult result;
        public string error;
        public Dictionary<string, object> data;

        public FBResponse(FacebookResult response)
        {
            this.result = response;
        }

        public FBResponse(FacebookResult response, string error) : this(response)
        {
            this.error = error;
        }

        public FBResponse(FacebookResult response, Dictionary<string, object> dict) : this(response)
        {
            this.data = dict;
        }
    }
#endregion Response Class
}
