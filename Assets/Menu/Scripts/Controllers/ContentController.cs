using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using GT.Database;
using GT.Websocket;
using System.Text;
using GT.User;

/// <summary>
/// Server veriables and app constants
/// </summary>
public class ContentController : MonoBehaviour
{
    private static ContentController m_instance;
    public static ContentController Instance {
        get { return m_instance ?? (m_instance = FindObjectOfType<ContentController>()); }
        private set { m_instance = value; }
    }

    #region Server Variables
    public static string GameWebsite = "http://backgammon4money.com";
    public static string GameSiteApple = "https://itunes.apple.com/us/app/backgammon-for-money/id975475057?ls=1&mt=8";
    public static string GameRateSiteApple = "https://itunes.apple.com/us/app/backgammon-for-money/id975475057?ls=1&mt=8";
    public static string GameSiteGoogle = GameWebsite;
    public static string TermsOfUseSite = "https://bo.gamytechapis.com/Docs/terms.html";
    public static string PrivacyPolicySite = "https://bo.gamytechapis.com/Docs/privacy-policy.html";
    public static float MinCashOut = 10f;
    public static float MaxCashOut = 3000f;

    public static List<AdData> AdsList { get; private set; }

    // play categories
    public static Dictionary<string, List<BetRoom>> CashRoomsByCategory { get; private set; }
    public static Dictionary<string, List<BetRoom>> VirtualRoomsByCategory { get; private set; }

    #region Aid Functions
    // category names
    public const string CustomCatId = "-1";
    public static Dictionary<string, RoomCategory> CategoriesData = new Dictionary<string, RoomCategory>()
    {
        { CustomCatId, new RoomCategory(10,"Custom Bet", "¬") },
        { "0", new RoomCategory(0,"Bronze","S") },
        { "1", new RoomCategory(1,"Silver","S") },
        { "2", new RoomCategory(2,"Gold","S") },
        { "3", new RoomCategory(3,"Platinum","S") },
        { "4", new RoomCategory(4,"Diamond","S") },
        { "5", new RoomCategory(5,"Masters","S") },
        { "6", new RoomCategory(6,"Grand Masters","S") },
        { "7", new RoomCategory(7,"Professionals","S") },
        { "8", new RoomCategory(8,"Practice","") },
        { "9", new RoomCategory(9,"Iron","S") },
    };

    public static Dictionary<string, List<BetRoom>> GetByCategory(Enums.MatchKind kind)
    {
        switch (kind)
        {
            case Enums.MatchKind.Cash:
                return CashRoomsByCategory;
            case Enums.MatchKind.Virtual:
                return VirtualRoomsByCategory;
            default:
                return null;
        }
    }

    public static RoomCategory GetCategoryData(string categoryId)
    {
        RoomCategory category;
        if (!CategoriesData.TryGetValue(categoryId, out category))
        {
            category = CategoriesData[new List<string>(CategoriesData.Keys)[0]];
            Debug.LogWarning("Category id " + categoryId + " not found. Using first one");
        }
        return category;
    }
    #endregion Aid Functions


    #endregion Server Variables

    #region Static Functions
    /// <summary>
    /// Initialize vars from prefs
    /// </summary>
    public static void Initialize()
    {
        string minCashOut = MinCashOut.ToString();
        GameWebsite = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.GameWebSite, GameWebsite);
        GameSiteApple = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.IosAppSite, GameSiteApple);
        GameRateSiteApple = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.IosAppRateSite, GameRateSiteApple);
        GameSiteGoogle = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.AndroidAppSite, GameSiteGoogle);
        TermsOfUseSite = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.TermsAndConditionsLink, TermsOfUseSite);
        PrivacyPolicySite = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.PrivacyPolicyLink, PrivacyPolicySite);
        MinCashOut = float.Parse(GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.MinCashout, minCashOut));
        InitAds();
    }

    private void OnDestroy()
    {
        Instance = null;
        StopAllCoroutines();
    }

    private static void InitAds()
    {
        AdsList = new List<AdData>();
        string savedAds = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Ads);
        List<object> ads = (List<object>)MiniJSON.Json.Deserialize(savedAds);
        for (int i = 0; i < ads.Count; i++)
            AdsList.Add(new AdData(ads[i] as Dictionary<string, object>));
    }

    public static void InitBetRanges()
    {
        CashRoomsByCategory = new Dictionary<string, List<BetRoom>>();
        VirtualRoomsByCategory = new Dictionary<string, List<BetRoom>>();

        string betsString = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Bets);
        List<float> bets = Utils.SplitToFloatList(betsString, ',');

        string feesString = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Fees);
        List<float> fees = Utils.SplitToFloatList(feesString, ',');

        string loyaltiesString = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Loyatly);
        List<int> loyalties = Utils.SplitToIntList(loyaltiesString, ',');

        string categoriesString = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.Category);
        List<string> categories = categoriesString.Split(',').ToListOfStrings();

        if (AppInformation.MATCH_KIND == Enums.MatchKind.Cash)
        {
            CashRoomsByCategory = BetRooms.GetBetRooms(bets, fees, loyalties, categories, Enums.MatchKind.Cash);

            SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
            foreach (var rooms in CashRoomsByCategory)
                BetRooms.RefreshBetSelection(rooms.Key, Enums.MatchKind.Cash, user.GetSavedBetByMatchkind(Enums.MatchKind.Cash));
        }
        else
        {
            VirtualRoomsByCategory = BetRooms.GetBetRooms(bets, fees, loyalties, categories, Enums.MatchKind.Virtual);

            SavedUser user = SavedUsers.LoadOrCreateUserFromFile(UserController.Instance.gtUser.Id);
            foreach (var rooms in CashRoomsByCategory)
                BetRooms.RefreshBetSelection(rooms.Key, Enums.MatchKind.Virtual, user.GetSavedBetByMatchkind(Enums.MatchKind.Virtual));
        }
    }

    #endregion Static Functions

    #region Download Pictures
    public void DownloadPicture(string url, Action<Sprite> callback, Sprite defaultSprite = null)
    {
        StartCoroutine(Utils.DownloadPic(url, callback, defaultSprite));
    }
    public void DownloadPicOrError(string url, Action<Texture2D> callback, Action<string> errorCallback)
    {
        StartCoroutine(Utils.DownloadPicOrError(url, callback, errorCallback));
    }
    
    #endregion Download Pictures
}
