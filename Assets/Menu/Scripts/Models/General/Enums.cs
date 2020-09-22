using System;

public static class Enums
{

    #region GameID
    /// <summary>
    /// Different versions of backgammon
    /// </summary>
    public enum GameID
    {
        Backgammon4Money,
        BackgammonRoyale,
        BackgammonForFriends,
        BackgammonBlockChain,
        BackgammonOrg,
    }
    #endregion GameID

    #region PageGroups
    public enum PageGroups
    {
        None,
        QuickAccessPages,
        CashierPages,
        FriendsPages,
        WalletPages,
        NavigationPages,
        StoresGroup,
    }
    #endregion PageGroups

    #region Navigation Button States
    /// <summary>
    /// State of title bar navigation button
    /// Controls if the button on top title widget
    /// will open navigation panel or back or cancel
    /// </summary>
    public enum NavigationState
    {
        Menu,
        Back,
        Cancel,
        Blocked,
    }

    public enum NavigationCategory
    {
        None,
        Home,
        History,
        Cashier,
        Balance,
        Withdraw,
        Store,
        Challenge,
        Settings,
        Support,
        Profile,

    }
    #endregion Navigation Button States

    #region Verification rank for Users
    public enum VerificationRank
    {
        EmailVerified = 1,
        IdentityVerified = 2,
        AddressVerified = 3,
    }
    #endregion Verification rank for Users

    #region Store types
    public enum StoreType
    {
        Loyalty,
        ShoutOuts,
        LuckyItems,
        BallsStore,
        Chat,
        Boards,
        Dices,
        Cups,
        Mascots,
        Blessings,
        InAppCoins,
        None
    }
    #endregion

    #region Pages
    /// <summary>
    /// Pages enum. Starts with id 0.
    /// All possible pages in the app
    /// </summary>
    public enum PageId
    {
        InitApp = 0,     
        FirstTimeProfile = 1,
        Home = 2,            
        Profile = 3,        
        Cashin = 4,          
        Cashout = 5,         
        Affiliate = 6,
        Challenge = 7,
        Achivements = 8,   
        Settings = 9,        
        QuickAccessCashin = 10,

        PreviewMatchHistory = 11,
        FullMatchHistory = 12,

        Balance = 13,

        Tournament = 14,
        Season = 15,           

        LoyaltyStoreInfo = 16,
        LoyaltyStore = 17,
        ShoutOuts = 18,
        LuckyItems = 19,
        ChatWords = 20,
        ChatEditor = 21,
        Boards = 22,
        Dice = 23,
        Mascots = 24,
        Blessings = 25,

        InAppPurchase = 26,

        FacebookLogin = 27,

        ChangeProfile = 28,

        SignIn = 29,
        Register = 30,
        ForgotPassword = 31,

        DepositInfo = 32,
        PCPayPalConnecting = 33,
        PCPayPalSupport = 34,

        Searching = 35,
        Rank = 36,
        ContactSupport = 37,
        Languages = 38,
        Resolution = 39,

        VerifyAccount = 41,
        VerifyIdentity = 42,
        VerifyAddress = 43,

        CreditCard = 44,

        TourneyProgress = 45,
    }
    #endregion Pages

    #region Widgets 
    /// <summary>
    /// Main Widgets
    /// </summary>
    public enum WidgetId
    {
        Default = 0,

        // Lobby
        Play = 1,
        PCPlay = 2,
        Advertisement = 3,
        // Lobby

        // Match History
        PreviewMatchHistory = 4,
        FullMatchHistory = 5,
        // Match History

        // Friends
        FacebookLogin = 6,
        FriendsChallenge = 7,
        // Friends

        // QuickAccessCashin
        EarnMoreBonus = 8,
        // QuickAccessCashin

        // Profile
        Profile = 9,
        Rank = 10,
        ChangeProfile = 11,
        TournamentRank = 12,
        SeasonPosition = 13,
        TrophiesWon = 14,
        MeVSFriends = 15,
        // Profile

        // Cashin
        Deposit = 16,
        ContactUs = 17,
        DepositInfo = 18,
        PCPayPalConnecting = 19,
        SecurityImages = 20,
        CreditCard = 51,
        // Cashin

        // Balance
        Balance = 21,
        FullTransactionHistory = 22,
        PreviewTransactionHistory = 23,
        // Balance

        // Cashout
        VerifyUserInfo = 24,
        UploadIdPicture = 25,
        CashoutYourWinnings = 26,
        // Cashout

        // User Verification
        IdentityProofWidget = 27,
        AddressProofWidget = 28,
        // User Verification

        // Specials
        Specials = 29,
        // Specials

        // Searching
        Searching = 30,
        // Searching

        // Achivements
        Achivements = 31,
        // Achivements

        // Settings
        ChangePassword = 32,
        Information = 33,
        Logout = 34,
        Settings = 35,
        LegalInfo = 36,
        // Settings

        // Pre-Login
        SignIn = 37,
        Register = 38,
        ForgotPassword = 39,
        // Pre-Login

        // In App Purchase
        InAppPurchase = 40,
        // In App Purchase

        // Top Widgets
        PCHeader = 41,
        MobileHeader = 42,
        LoyaltyStoreBar = 43,
        ShoutOutsBar = 44,
        ChatWordsBar = 45,   
        PCChatWordsBar = 46,
        LuckyItemsBar = 47,  
        TopEarnMoreBonus = 48,
        // Top Widgets

        // Bottom Widgets
        FriendsBar = 49,
        QuickAccessBar = 50,
        // Bottom Widgets

        // Store
        ShoutOuts = 52,
        ChatWords = 53,
        ChatEditor = 54,
        GeneralStore = 55,
        // Loyalty Store

        // Popups
        ChallengePopup = 56,
        CustomBetsPopup = 57,
        RatePopup = 58,
        LanguagePopup = 59,
        ResolutionPopup = 60,
        MatchDataPopup = 61,
        CashUpdating = 62,
        VerifyWithSMSPopup = 69,
        VerificationApprovedPopup = 70,
        ChangePaymentGatewayPopup = 71,
        CvvHintPopup = 72,
        TourneyMenuPopup = 73,
        MessagePopup = 74,
        TourneyRulesPopup = 76,
        TourneyInfoPopup = 77,
        TourneyResultsPopup = 78,
        WelcomeToTourneysPopup = 79,
        TourneyHistoryPopup = 80,
        VerifyUserPopup = 81,
        // Popups

        ComingSoon = 63,
        GameTitle = 64,
        Bonus = 65,
        SupportMessage = 66,

        //SMS
        VerifyPhonePromo = 68,
        //SMS

        TourneyProgress = 75,
    }
    #endregion Widgets 

    #region Widget Position
    /// <summary>
    /// Position of a widget on the page
    /// </summary>
    public enum WidgetPosition
    {
        Top,
        Middle,
        Bottom,
        FloatingTop,
        FloatingBottom
    }
    #endregion Widget Position

    #region Notifications
    public enum NotificationId
    {
        Cashin,
        Bonus,
        History,
        Game,
    }
    #endregion Notifications

    #region Actions 
    public enum ActionType
    {
        None = 0,
        OpenUrl,
        OpenPage,
        CloseApp,
        ChangePref,
        OpenUpdatePage,
        OpenTermsOfUse,
        OpenPrivacyPolicy,
        OpenMessage,
        ShowPopup,
    }
    #endregion Actions

    #region Player Prefs
    public enum PlayerPrefsVariable
    {
        TutoStep,
        LastGTUserEmail,
        LastGTUserPass,
        Music,
        Resolution,
        Sfx,
        Vibration,
        SpecialOfferID,
        FirstTimeOnPageData,
        LastMatchId,
        LastMatchBotDoublesCount,
        Mails,
        WebGLDeviceId,

        //Versionned info
        Versions,
        AppVersion,
        IsMaintenance,
        Bets,
        Fees,
        Loyatly,
        Category,
        CashInAmounts,
        MinCashout,
        Ads,
        LocalizationVer,
        Stores,
        SmallGlobalData,
        AppsFlyerIOSID,
        AppFlyerDevKit,
        PushwooshAppCode,
        GcmProjectNumber,
        GameWebSite,
        IosAppSite,
        IosAppRateSite,
        AndroidAppSite,
        AssetBundleURL,
        StorageAccount,
        StorageKey,
        TermsAndConditionsLink,
        PrivacyPolicyLink,
        DepositInfo,

        SMSSentCode,
        SMSSentPhoneNumber,
        SMSCountryISO,

        SawWelcomeToTourneys,
        SawTourneyRules,
        TourneyId,
    }
    #endregion Player Prefs

    #region Tutorial - First Time In App
    public enum TutorialProgress
    {
        Home_Welcome,
        Home_Wallet,
        Home_Bet,
        Home_Cashin,
        Deposit_Welcome,
        Deposit_Bonus,
        Deposit_Store,
        Store_Redeem,
        Store_Home,
        Home_Play,
        Finished,
    }

    public enum Position
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
        None = 5,
    }
    #endregion Tutorial - First Time In App

    #region Popup
    public enum PopupId
    {
        Custom = 0,
        Error = 1,
        BannedAccount = 2,
        DeletedAccount = 3,
        FrozenAccount = 4,
        DailyBonus1 = 5,
        DailyBonus2 = 6,
        DailyBonus3 = 7,
        DailyBonus4 = 8,
        DailyBonus5 = 9,
        IdentityRequest = 10,
        IdentityConfirm = 11,
        Maintenance = 12,
        Update = 13,
        Voucher = 14,
        CashinSuccess = 15,
        AutoDeposit = 16,
    }
    #endregion Popup

    #region currency
    public enum MatchKind
    {
        Cash,
        Virtual,
    }
    #endregion currency

    #region Credit Cards
    public enum CreditCardType
    {
        Unknown,
        Visa,
        Mastercard,
        AmericanExpress,
        DinersClub,
        Discover,
        JCB,
    };
    #endregion Credit Cards

    #region sounds
    public enum GameSound
    {
        BeforeRollToggle,
        BeforeRollBarHideShow,
        Eat,
        Move,
        CheckerOut,
        BankTime,
        YourTurn,
        ViewShow,
        ViewHide,
    }

    public enum MenuSound
    {
        None,
        Click,
        Matched,
        Invite,
        CollectBonus,
        Spinning,
        LoyaltyBought,
        BonusCashBought,
    }
    #endregion currency
}
