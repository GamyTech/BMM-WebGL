using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GT.Encryption;
using GT.Websocket;
using GT.User;
using GT.Backgammon.Player;
using UnityEngine.Events;

public class UserController : MonoBehaviour
{
    private static UserController m_instance;
    public static UserController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<UserController>()); }
        private set { m_instance = value; }
    }

    #region Delegates And Events
    public delegate void VoidDelegate();
    public delegate void Changed<T>(T newValue);

    public static event Changed<GTUser> OnGTUserChanged;

    public event VoidDelegate OnFacebookLoginFailed;
    public event VoidDelegate OnFacebookLoginCancelled;
    public event VoidDelegate OnFacebookLoginAlreadyLoggedIn;

    public event VoidDelegate OnFacebookLoggedIn;
    public event VoidDelegate OnFacebookLoggedOut;
    #endregion Delegates And Events

    public FBUser fbUser { get; private set; }
    public GTUser gtUser { get; private set; }
    public Wallet wallet { get { return gtUser != null ? gtUser.wallet : null; } }

    public int PhoneValidationBonus;

    #region Temp Variables
    [HideInInspector]
    public int TempMatchId;
    [HideInInspector]
    public ReconnectData reconnectData;
    [HideInInspector]
    public GTUser WatchedUser;
    [HideInInspector]
    public string searchingBets;
    [HideInInspector]
    public string DepositURL;
    [HideInInspector]
    public MatchHistoryData WatchedHistoryMatch;
    [HideInInspector]
    public TourneyHistoryData WatchedHistoryTourney;
    [HideInInspector]
    public GT.Backgammon.ReplayMatchData WatchedReplayMatch;
    [HideInInspector]
    public PlayerFoundData opponentFoundData;
    #endregion Temp Variables

    void OnEnable()
    {
        Debug.Log("UserController OnEnable");

        Instance = this;
        WebSocketKit.Instance.OnOpenEvent += WebSocketKit_OnOpenEvent;
        WebSocketKit.Instance.OnCloseEvent += WebSocketKit_OnCloseEvent;

        WebSocketKit.Instance.ServiceEvents[ServiceId.NotifyCashInSuccess] += WebSocketKit_NotifyCashInSuccess;
        WebSocketKit.Instance.ServiceEvents[ServiceId.NotifyCashInFailed] += WebSocketKit_NotifyCashInFailed;
        WebSocketKit.Instance.ServiceEvents[ServiceId.FriendInviteToPlay] += WebSocketKit_OnFriendInviteToPlay;
        WebSocketKit.Instance.ServiceEvents[ServiceId.IsReady] += WebSocketKit_IsReady;
        WebSocketKit.Instance.ServiceEvents[ServiceId.IsTourneyReady] += WebSocketKit_IsReady;
    }

    void OnDisable()
    {
        WebSocketKit.Instance.OnOpenEvent -= WebSocketKit_OnOpenEvent;
        WebSocketKit.Instance.OnCloseEvent -= WebSocketKit_OnCloseEvent;

        WebSocketKit.Instance.ServiceEvents[ServiceId.NotifyCashInSuccess] -= WebSocketKit_NotifyCashInSuccess;
        WebSocketKit.Instance.ServiceEvents[ServiceId.NotifyCashInFailed] -= WebSocketKit_NotifyCashInFailed;
        WebSocketKit.Instance.ServiceEvents[ServiceId.FriendInviteToPlay] -= WebSocketKit_OnFriendInviteToPlay;
        WebSocketKit.Instance.ServiceEvents[ServiceId.IsReady] -= WebSocketKit_IsReady;
        WebSocketKit.Instance.ServiceEvents[ServiceId.IsTourneyReady] -= WebSocketKit_IsReady;
        Instance = null;
    }

    #region Set Users

    public void SetGTUser(GTUser user)
    {
        gtUser = user;

        if (gtUser != null)
        {
            gtUser.InitStores();
            gtUser.MatchHistoryData.InitMatches(gtUser.Id);
            ContentController.InitBetRanges();

            GetUserVarsFromServer(APIGetVariable.PurchasedItems);

            SavedUser savedUser = SavedUsers.LoadOrCreateUserFromFile(gtUser.Id);
            if (savedUser.pendingInAppPurchase.Count > 0)
            {
                for (int x = 0; x < savedUser.pendingInAppPurchase.Count; ++x)
                {
                    SendPurchaseToServer(savedUser.pendingInAppPurchase[x]);
                    WebSocketKit.Instance.AckEvents[RequestId.PurchaseItem] += OnPurchaseItem;
                }
            }

            FacebookKit.GetFBUser(SetFBUser);
            Instance.GetTourneysInfo();
        }
        else
        {
            GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.LastGTUserEmail);
            GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.LastGTUserPass);
        }

        if (OnGTUserChanged != null)
            OnGTUserChanged(gtUser);
    }

    public void SetFBUser(FBUser user)
    {
        fbUser = user;

        if (user != null)
        {
            Debug.Log("SetFBUser " + user.Id + user.Name + user.Email);
            UpdateFacebookUser(user.Id, user.Name, user.Email);

            if (OnFacebookLoggedIn != null)
                OnFacebookLoggedIn();

#if UNITY_WEBGL
            if(AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
                UpdateAvatar(user.PictureURL);
#endif
        }
        else
        {
            Debug.Log("LogOutFBUser");
            if (OnFacebookLoggedOut != null)
                OnFacebookLoggedOut();
        }
    }
    #endregion Set Users

    #region User Functions
    public bool IsLoggedToFB()
    {
        return fbUser != null;
    }

    public bool HaveEnoughMoney(Enums.MatchKind kind, float money)
    {
        switch (kind)
        {
            case Enums.MatchKind.Cash:
                return money <= wallet.TotalCash;
            case Enums.MatchKind.Virtual:
                return money <= (float)wallet.VirtualCoins;
            default:
                Debug.LogError("HaveEnoughMoney Unrecognized MatchKind " + kind);
                break;
        }
        return false;
    }

    public bool CheckAndShowUserVerification()
    {
        if (gtUser.NeedsVerification)
        {
            PopupController.Instance.ShowSmallPopup("Account Verification", new string[] 
                { "Your account has been flagged because of unusual activity, confirm your account information to continue" }, 0,
                new SmallPopupButton[] {
                    new SmallPopupButton("OK", () => { WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.VerifyUserPopup); }),
                    new SmallPopupButton("Not now")
                });
            return true;
        }
        return false;
    }
    #endregion User Functions

    #region Application Start User Initialization

    public void InitializeUser()
    {
        string email, pass;

        if (NetworkController.Instance.IsClientGame())
        {
            Debug.Log("InitializeUser");
            WebSocketKit.Instance.AckEvents[RequestId.Login] += OnAutoSignedIn;
            WebSocketKit.Instance.SendRequest(RequestId.Login);

            LoadingController.Instance.SetCurrentSceneLoadingProgress(.5f, "Signing In");
        } 
        else if (GetSavedGTUser(out email, out pass))
        {
            Debug.Log("email : " + email + " pass : " + pass);

            WebSocketKit.Instance.AckEvents[RequestId.Login] += OnAutoSignedIn;
            string lastMatchId = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.LastMatchId);
            WebSocketKit.Instance.Login(email, pass, lastMatchId);

            LoadingController.Instance.SetCurrentSceneLoadingProgress(.5f, "Signing In");
        }
        else
            FinishSceneLoading();
    }

    private bool GetSavedGTUser(out string email, out string pass)
    {
        pass = null;
        email = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.LastGTUserEmail);
        if (string.IsNullOrEmpty(email))
            return false;
        string passCypher = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.LastGTUserPass);
        if (string.IsNullOrEmpty(passCypher))
            return false;

        try
        {
            pass = AesBase64Wrapper.Decrypt(passCypher);
        }
        catch (Exception e)
        {
            Debug.LogError("Decrypt error " + e.Message);
            GTDataManagementKit.RemoveFromPrefs(Enums.PlayerPrefsVariable.LastGTUserPass);
            return false;
        }

        return !string.IsNullOrEmpty(pass);
    }
    #endregion Application Start User Initialization

    #region Facebook User Functions
    public void FacebookLogin()
    {
        FacebookKit.Login(OnFBLogin);
    }

    private void FacebookLogout()
    {
        FacebookKit.Logout();
        SetFBUser(null);
    }

    void OnFBLogin(FacebookKit.FBResponse response)
    {
        Debug.Log("OnLoggedin " + response.result.ToString());

        switch(response.result)
        {
            case FacebookKit.FacebookResult.Success:
                TrackingKit.FacebookSignUpTracker();
                FacebookKit.GetFBUser(SetFBUser);

            if (AppInformation.GAME_ID == Enums.GameID.BackgammonForFriends)
                    GT.InAppPurchase.FacebookPurchaser.ConsumeAllPendingProducts();
                break;
            case FacebookKit.FacebookResult.Failed:
                if (OnFacebookLoginFailed != null)
                    OnFacebookLoginFailed();
                break;
            case FacebookKit.FacebookResult.Cancelled:
                if (OnFacebookLoginCancelled != null)
                    OnFacebookLoginCancelled();
                break;
            case FacebookKit.FacebookResult.AlreadyLoggedIn:
                if (OnFacebookLoginAlreadyLoggedIn != null)
                    OnFacebookLoginAlreadyLoggedIn();
                break;
        }
    }

    #endregion Facebook User Functions

    #region Gamytech User Functions
    public void GamytechSignedIn(GTUser user)
    {
        SetGTUser(user);
    }

    private void GamytechLogout()
    {
        SetGTUser(null);
    }

    public void Logout()
    {
        if (PageController.Instance != null)
        {
            PageController.Instance.ChangePage(Enums.PageId.SignIn);
            PageController.Instance.ForceReloading();
        }
        MobileNavigation nav = FindObjectOfType<MobileNavigation>();
        if (nav != null)
            nav.CloseMenu();

        if (gtUser != null)
            WebSocketKit.Instance.SendRequest(RequestId.Logout);

        FacebookLogout();
        GamytechLogout();

        searchingBets = string.Empty;
        WatchedHistoryMatch = null;
        WatchedHistoryTourney = null;
        opponentFoundData = null;
    }

    public void GetUserVarsFromServer(APIGetVariable vars, Dictionary<PassableVariable, object> variablesToPass = null)
    {
        WebSocketKit.Instance.SendAPIRequest(vars, variablesToPass);
    }

    public void UpdateUserVarsToServer(APIUpdateVariable vars, Dictionary<PassableVariable, object> variablesToPass = null)
    {
        WebSocketKit.Instance.SendAPIUpdate(vars, variablesToPass);
    }

    public void UpdateFacebookUser(string facebookId, string facebookName, string email)
    {
        Dictionary<PassableVariable, object> dict = new Dictionary<PassableVariable, object>();
        dict.Add(PassableVariable.FacebookId, facebookId);
        dict.Add(PassableVariable.FacebookName, facebookName ?? "");
        dict.Add(PassableVariable.FacebookAddtionalId, email ?? "");
        UpdateUserVarsToServer(APIUpdateVariable.UpdateFacebook, dict);
    }

    public void SetPlayerFoundData(List<Dictionary<string, object>> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            object o;
            if (!users[i].TryGetValue("UserId", out o) || gtUser.Id.Equals(o.ToString()))
                continue;
            opponentFoundData = new PlayerFoundData(users[i]);
        }
    }

    public void GetFriendsFromServer()
    {
        List<string> friends = fbUser.GetChallengeableFriends().ToListOfStrings();
        GetUserVarsFromServer(APIGetVariable.GamyTechFriends, new Dictionary<PassableVariable, object> { { PassableVariable.GamyTechFriends, friends } });
    }

    public void GetMissingMatchHistory()
    {
        GetMatchHistory(gtUser.MatchHistoryData == null ? 0 : gtUser.MatchHistoryData.GetNextUnupdatedIndex());
    }

    public void GetReplayInfo(string MatchId)
    {
        WebSocketKit.Instance.SendRequest(RequestId.GetMatchHistoryMoves, new Dictionary<string, object>() { { "MatchId", MatchId } });
    }

    public void GetLastMatchHistory()
    {
        GetMatchHistory(0);
    }

    private void GetMatchHistory(int startIndex)
    {
        GetUserVarsFromServer(APIGetVariable.HistoryMatches, new Dictionary<PassableVariable, object> { { PassableVariable.StartIndex, startIndex } });
    }

    public void GetNextTransactionHistoryPage()
    {
        GetTransactionHistory(gtUser.TransactionHistoryData.LastPage + 1);
    }

    public void GetLastTransactionHistory()
    {
        GetTransactionHistory(1);
    }

    private void GetTransactionHistory(int startIndex)
    {
        WebSocketKit.Instance.SendRequest(RequestId.GetWalletChangeLogs, new Dictionary<string, object> { { "Page", startIndex } });
    }

    public void RegisterToServer(string userName, string email, string pass)
    {
        WebSocketKit.Instance.Register(userName, email, pass);
    }

    public void SignInToServer(string email, string pass)
    {
        WebSocketKit.Instance.Login(email, pass);
    }

    public void UpdateAvatar(string picurl)
    {
        Dictionary<PassableVariable, object> dictToPass = new Dictionary<PassableVariable, object>()
        {
            { PassableVariable.UserId, gtUser.Id },
            { PassableVariable.PictureUrl, picurl }
        };

        UpdateUserVarsToServer(APIUpdateVariable.UpdateProfilePicture, dictToPass);
    }

    public void SendPurchaseToServer(Dictionary<string, object> varsToPass)
    {
        WebSocketKit.Instance.SendRequest(RequestId.PurchaseItem, varsToPass);
    }

    public void SendConsumeItem(string purchaseId)
    {
        Dictionary<PassableVariable, object> dictToPass = new Dictionary<PassableVariable, object>() { { PassableVariable.PurchaseId, purchaseId } };
        UpdateUserVarsToServer(APIUpdateVariable.ConsumeItem, dictToPass);
    }

    public void GetVerification(Enums.VerificationRank requiredRank)
    {
        if(gtUser.Verification >= requiredRank)
        {
            Debug.Log("User has already the required rank of verification");
            return;
        }
        switch (requiredRank)
        {
            case Enums.VerificationRank.IdentityVerified:
                PopupController.Instance.ShowSmallPopup("Verification required", new string[] { "To continue you have to provide a proof of your identity." },
                    new SmallPopupButton("Continue", () => PageController.Instance.ChangePage(Enums.PageId.VerifyIdentity)), new SmallPopupButton("Cancel"));
                break;
            case Enums.VerificationRank.AddressVerified:
                PopupController.Instance.ShowSmallPopup("Verification required", new string[] { "To continue you have to provide a proof of your official location." },
                    new SmallPopupButton("Continue", () => PageController.Instance.ChangePage(Enums.PageId.VerifyAddress)), new SmallPopupButton("Cancel"));
                break;
            default:
                break;
        }
    }

    public void ChallengeFriend(string userId, string bets, string data = "")
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("UserId", userId);
        dict.Add("Bet", bets);
        dict.Add("Data", data);
        WebSocketKit.Instance.SendRequest(RequestId.PlayFriend, dict);
    }

    public void ChallengeFriendAnswer(bool answer, string userId, string bet = "", string data = "")
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("UserId", userId);
        dict.Add("Bet", bet);
        dict.Add("Answer", answer);
        dict.Add("Data", data);
        WebSocketKit.Instance.SendRequest(RequestId.PlayFriendAnswer, dict);
    }

    public void SendIdentityProofToServer(Enums.VerificationRank NextRank, string picUrl)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("RequestedRank", (int)NextRank);
        dict.Add("ImageUrl", picUrl);
        WebSocketKit.Instance.SendRequest(RequestId.AddPendingVerification, dict);
    }

    public void GetSmsValidation(string phoneNumber)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("PhoneNumber", phoneNumber);
        WebSocketKit.Instance.SendRequest(RequestId.GetSmsValidation, dict);
    }

    public void ValidateSmsByUser(string key)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("UniqueKey", key);
        WebSocketKit.Instance.SendRequest(RequestId.ValidateSmsByUser, dict);
    }

    public void SendCollectTimelyBonus()
    {
        WebSocketKit.Instance.SendRequest(RequestId.CollectTimelyBonus);
    }

    public void SendStartSearchingMatch(List<BetRoom> selectedRooms)
    {
        reconnectData = null;

        Dictionary<float, float> betDict = new Dictionary<float, float>();
        searchingBets = string.Empty;
        for (int i = 0; i < selectedRooms.Count; i++)
        {
            betDict.Add(selectedRooms[i].BetAmount, selectedRooms[i].FeeAmount);

            searchingBets += Wallet.AmountToString(selectedRooms[i].BetAmount, selectedRooms[i].Kind);
            if (i != selectedRooms.Count - 1)
                searchingBets += ", ";
        }

        Dictionary<Enums.StoreType, string[]> OnGameItem = new Dictionary<Enums.StoreType, string[]>();
        foreach(var item in SavedUsers.LoadOrCreateUserFromFile(gtUser.Id).selectedStoreItems)
            if (item.Key == Enums.StoreType.Mascots || item.Key == Enums.StoreType.LuckyItems)
                OnGameItem.Add(item.Key, item.Value);

        WebSocketKit.Instance.SendRequest(RequestId.SearchMatch, new Dictionary<string, object>()
        {
            { "BetAmounts", MiniJSON.Json.Serialize(betDict) },
            { "MatchKind", selectedRooms[0].Kind },
            { "Data", MiniJSON.Json.Serialize(OnGameItem) }
        });
    }

    public void SendStartSearchingTourneyMatch(string tourneyId)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("TourneyId", tourneyId);
        WebSocketKit.Instance.SendRequest(RequestId.SearchTourneyhMatch, dict);
    }

    public void CancelTourneySearch(string tourneyId)
    {
        WebSocketKit.Instance.SendRequest(RequestId.CancelTourneySearch);
    }

    public void ReadyToPlayTourneyMatch(string matchId)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("TempMatchId", matchId);
        WebSocketKit.Instance.SendRequest(RequestId.ReadyToPlayTourneyMatch, dict);
    }

    public void StopTourneyMatch(string winnerId)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("Winner", winnerId);
        WebSocketKit.Instance.SendRequest(RequestId.StopTourneyMatch, dict);
    }

    public void GetSpecificTourneyDetails(string tourneyId)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("TourneyId", tourneyId);
        WebSocketKit.Instance.SendRequest(RequestId.GetSpecificTourneyDetails, dict);
    }

    public void SendCashOutToServer()
    {
        WebSocketKit.Instance.SendRequest(RequestId.CashOutRequest, gtUser.cashOutData.ToStringDict(gtUser.Verified));
    }

    public void SendContactSupport(string subject, string message, string phone, string firstName, string lastName)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(PassableVariable.Complaint.ToString(), subject);
        dict.Add(PassableVariable.Message.ToString(), message);
        dict.Add(PassableVariable.Email.ToString(), gtUser.Email);
        dict.Add(PassableVariable.Phone.ToString(), phone);
        dict.Add(PassableVariable.FirstName.ToString(), firstName);
        dict.Add(PassableVariable.LastName.ToString(), lastName);
        WebSocketKit.Instance.SendRequest(RequestId.ContactGamytech, dict);
    }

    public void ChangeGamytechPassword(string oldPass, string newPass)
    {
        Dictionary<PassableVariable, object> variableToPass = new Dictionary<PassableVariable, object>()
        {
            { PassableVariable.UserId, gtUser.Id },
            { PassableVariable.OldPassword, oldPass },
            { PassableVariable.NewPassword, newPass },
        };

        UpdateUserVarsToServer(APIUpdateVariable.SetPassword, variableToPass);
    }

    public void SendForgotPassword(string email)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(ServerRequestKey.Email.ToString(), email);
        WebSocketKit.Instance.SendRequest(RequestId.ForgotPassword, dict);
    }

    public void GetTourneysInfo()
    {
        WebSocketKit.Instance.SendRequest(RequestId.GetTourneysInfo);
    }

    public void RegisterToTourney(string id)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("TourneyId", id);
        WebSocketKit.Instance.SendRequest(RequestId.RegisterTourney, dict);
    }

    public void UnregisterToTourney(string id)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("TourneyId", id);
        WebSocketKit.Instance.SendRequest(RequestId.UnRegisterTourney, dict);
    }

    public void ListenToTourneysEvents()
    {
        WebSocketKit.Instance.SendRequest(RequestId.ListenToTourneysEvents);
    }

    public void StopListenToTourneysEvents()
    {
        WebSocketKit.Instance.SendRequest(RequestId.StopListenToTourneysEvents);
    }
    #endregion Gamytech User Functions

    #region Apco User Functions
    public void DeleteCreditCard(GTUser.SavedCreditCard card)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("CardDetailsId", card.id);
        
        WebSocketKit.Instance.SendRequest(RequestId.DeleteCard, dict);
    }

    public void SubmitCard(NewCreditCard creditCard, string phoneNumber = "")
    {
        if (creditCard == null)
            return;

        gtUser.cashInData.SetNewCard(creditCard);

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("CardNumber", creditCard.CardNumber);
        dict.Add("Cvv2", creditCard.SecurityCode.ToString());
        dict.Add("ExpMonth", creditCard.expirationMonth);
        dict.Add("ExpYear", creditCard.expirationYear);
        dict.Add("HolderName", creditCard.FullName);
        dict.Add("CashToDeposit", gtUser.cashInData.depositAmount.amount.ToString());
        dict.Add("CardType", creditCard.CardType.ToString());
        dict.Add("ShouldSave", true);
        if (!string.IsNullOrEmpty(phoneNumber))
            dict.Add("Phone", phoneNumber);

        if(gtUser.cashInData.method == CashInData.TransactionMethod.Apco)
            WebSocketKit.Instance.SendRequest(RequestId.CashIn, dict);
        else if(gtUser.cashInData.method == CashInData.TransactionMethod.EasyTransac)
            WebSocketKit.Instance.SendRequest(RequestId.CashInEasyTransact, dict);
    }

    public void UseCard(GTUser.SavedCreditCard creditCard)
    {
        gtUser.cashInData.RemoveNewCard();
        gtUser.cashInData.SetSavedCard(creditCard);

        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("CardDetailsId", creditCard.id);
        dict.Add("CashToDeposit", gtUser.cashInData.depositAmount.amount.ToString());
        dict.Add("ShouldSave", false);

        if (gtUser.cashInData.method == CashInData.TransactionMethod.Apco)
            WebSocketKit.Instance.SendRequest(RequestId.CashIn, dict);
        else if (gtUser.cashInData.method == CashInData.TransactionMethod.EasyTransac)
            WebSocketKit.Instance.SendRequest(RequestId.CashInEasyTransact, dict);
    }

    public void SendVerification()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("FirstName", gtUser.verificationData.FirstName);
        dict.Add("LastName", gtUser.verificationData.LastName);
        dict.Add("SecondaryEmail", gtUser.verificationData.Email);
        dict.Add("SubmittedCountry", gtUser.verificationData.Country);
        dict.Add("City", gtUser.verificationData.City);
        dict.Add("Street", gtUser.verificationData.Street + " " + gtUser.verificationData.Number);
        dict.Add("PostalCode", gtUser.verificationData.PostalCode);
        dict.Add("PhoneNumber", gtUser.verificationData.PhoneNumber);
        dict.Add("IdImage", gtUser.verificationData.IdImageLink);
        
        WebSocketKit.Instance.SendRequest(RequestId.UpdateVerificationDetails, dict);
    }

    public void RequestPaySafeCardCashin()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>() { { "Amount", gtUser.cashInData.depositAmount.amount.ToString() } };
        WebSocketKit.Instance.SendRequest(RequestId.CashInPaySafe, dict);
    }

    public void RequestSkrillCashin()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>() { { "Amount", gtUser.cashInData.depositAmount.amount.ToString() } };
        WebSocketKit.Instance.SendRequest(RequestId.CashInSkrill, dict);
    }
    #endregion Apco User Functions

    #region Websocket Events
    private void WebSocketKit_OnOpenEvent()
    {
        Debug.Log("UserController WebSocketKit_OnOpenEvent");
        InitializeUser();
    }

    private void WebSocketKit_OnCloseEvent(ushort code, string message)
    {
        WSResponseCode errorCode;
        Utils.TryParseEnum(code, out errorCode);
        if (errorCode == WSResponseCode.MultipleConnection)
        {
            PopupController.Instance.ShowSmallPopup("Disconnected", new string[] { "Other device is using this account" },
                new SmallPopupButton[] { new SmallPopupButton("Try Again", NetworkController.Instance.Connect),
                    new SmallPopupButton("Logout", () => { Logout(); NetworkController.Instance.Connect(); }) });
            LoadingController.Instance.ShowPageLoading();
        }
    }

    private void WebSocketKit_NotifyCashInSuccess(Service service)
    {
        Debug.Log("Get Cashin Success : " + service.RawData);

        if (gtUser == null)
            return;

        NotifyCashInSuccessService Nservice = service as NotifyCashInSuccessService;
        TrackingKit.CashInTracker(Nservice.IsFirstCashIn, Nservice.TotalCash);

        UnityAction buttonAction = () => { };

        if (MenuSceneController.Instance != null && 
            (PageController.Instance.CurrentPageId == Enums.PageId.DepositInfo ||
            PageController.Instance.CurrentPageId == Enums.PageId.CreditCard))
            buttonAction = () => { PageController.Instance.ChangePage(Enums.PageId.Home); };

        PopupData popup = PopupController.Instance.GetPopup(Enums.PopupId.CashinSuccess);
        popup.textElements[1].text = Wallet.CashPostfix + Nservice.TotalCash.ToString();
        popup.textElements[3].text = Wallet.CashPostfix + Nservice.NewTotalCash;
        popup.buttonElements[0].action = buttonAction;
        PopupController.Instance.ShowPopup(popup);
#if UNITY_ANDROID || UNITY_IOS
        InAppBrowser.CloseBrowser();
#endif
    }

    private void WebSocketKit_NotifyCashInFailed(Service service)
    {
        NotifyCashInFailedService failure = service as NotifyCashInFailedService;
        Debug.Log("Get Cashin Failed : " + service.RawData);

        GTUser user = gtUser;
        if (user == null)
            return;

        float cashinAmount = user.cashInData.depositAmount != null ? user.cashInData.depositAmount.amountWithBonus : 0.0f;
        TrackingKit.CashInFailed(cashinAmount, "Unkown Error");

        if (PageController.Instance != null && PageController.Instance.CurrentPageId == Enums.PageId.DepositInfo)
        {
            if (failure.Canceled)
                WidgetController.Instance.HideWidgetPopups();
            else
            {
                string reason = string.IsNullOrEmpty(failure.Reason) ? "We've encountered an error while processing your payment" : failure.Reason;
            
                PopupController.Instance.ShowSmallPopup("Transaction Failed", new string[] { reason, "You were not charged." },
                    new SmallPopupButton[] { new SmallPopupButton("Contact Support", () => PageController.Instance.ChangePage(Enums.PageId.ContactSupport)), new SmallPopupButton("OK", WidgetController.Instance.HideWidgetPopups) });
            }
        }
#if UNITY_ANDROID || UNITY_IOS
        InAppBrowser.CloseBrowser();
#endif
    }

    private void WebSocketKit_OnFriendInviteToPlay(Service service)
    {
        Debug.Log("Get Invitation from a friend : " + service.RawData);

        if (MenuSceneController.Instance == null || PageController.Instance.CurrentPage.NavState == Enums.NavigationState.Blocked)
            return;

        MenuSoundController.Instance.Play(Enums.MenuSound.Invite);
        FriendInviteService fs = service as FriendInviteService;
        fbUser.ChallengedFriend = new ChallengeableFriend(fs.senderId, fs.name, fs.picUr, fs.bets);
        WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.ChallengePopup);
    }


    bool gotReady = false;
    private void WebSocketKit_IsReady(Service service)
    {
        if (gotReady)
            return;
        gotReady = true;
        WidgetController.Instance.HideWidgetPopups();
        OpponentFoundService foundService = service as OpponentFoundService;
        TempMatchId = foundService.matchId;
        StartCoroutine(Utils.Wait(1f, () => { SceneController.Instance.ChangeScene(SceneController.SceneName.Game); gotReady = false; }));
    }

    void OnPurchaseItem(Ack ack)
    {
        WebSocketKit.Instance.AckEvents[RequestId.PurchaseItem] -= OnPurchaseItem;

        Debug.Log("GTPurchaseCallback " + ack);
        LoadingController.Instance.HidePageLoading();
        if (ack.Code == WSResponseCode.OK)
        {
                PopupController.Instance.ShowSmallPopup("Your purchase was successful");
                SavedUser user = SavedUsers.LoadOrCreateUserFromFile(gtUser.Id);
                user.pendingInAppPurchase.Clear();
                SavedUsers.SaveUserToFile(user);
        }
        else
            Debug.LogError("Purchase Error in pending purchase. Vladyoni");
    }
    #endregion Websocket Events

    public bool HandelReconnectInLogin(LoginAck loginAck)
    {
        reconnectData = null;
        bool goToGameScene = loginAck != null && loginAck.Code == WSResponseCode.OK && loginAck.IsReconnect;
        if (goToGameScene)
        {
            reconnectData = loginAck.ReconnectData;
            TempMatchId = loginAck.ReconnectData.MatchId;
            SceneController.Instance.ChangeScene(SceneController.SceneName.Game);
        }
        else
            RemoteGameController.CleanMatchData();

        return goToGameScene;

    }

    private void OnAutoSignedIn(Ack ack)
    {
        LoginAck loginAck = ack as LoginAck;
        Debug.Log("OnSignedIn " + loginAck.Code + " " + loginAck.ToString(true));
        switch (loginAck.Code)
        {
            case WSResponseCode.OK:
                GTUser user = loginAck.UserResponse.CreateGTUser();
                Debug.Log("OnAutoGetGTUser " + user ?? "null");
                GamytechSignedIn(user);
                FinishSceneLoading(loginAck);
                break;
            case WSResponseCode.WrongPassword:
            case WSResponseCode.UserNotExist:
                Debug.LogError("Invalid user name or password");
                FinishSceneLoading(loginAck);
                break;
            case WSResponseCode.CantLogFromCountry:
                Debug.LogError("Your country is locked for real money play");
                FinishSceneLoading(loginAck);
                break;
            case WSResponseCode.Banned:
                PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount);
                break;
            default:
                FinishSceneLoading(loginAck);
                break;
        }
    }

    private void FinishSceneLoading(LoginAck loginAck = null)
    {
        Debug.Log("load scene on login");
        StartCoroutine(Utils.WaitForBool(
        () => { return AssetController.Instance.Initialized; },
        () => {
            if (!HandelReconnectInLogin(loginAck))
                SceneController.Instance.ChangeScene(SceneController.SceneName.Menu);//Dima Support
        }));
    }
}
