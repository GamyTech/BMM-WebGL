using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GT.Database;
using GT.Store;
using GT.Assets;
using AssetBundles;

public class TestingController : MonoBehaviour {

    private static TestingController m_instance;
    public static TestingController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<TestingController>()); }
        private set { m_instance = value; }
    }

    public bool EnableDebug = true;
    public bool TestBox = true;

    public Enums.PageId testPage = Enums.PageId.InitApp;
    public Enums.PopupId testPopup = Enums.PopupId.BannedAccount;
    public int testInt = 0;
    public float testFloat = 0f;
    public string testString = "";
    public AnimationClip object1;
    public List<string> languages;

    void OnEnable()
    {
        languages = I2.Loc.LocalizationManager.GetAllLanguages();
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }

    int currentlanguage;
    void Update()
    {
#if UNITY_EDITOR
        if (EnableDebug == false)
            return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            languages = I2.Loc.LocalizationManager.GetAllLanguages();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            I2.Loc.LocalizationManager.CurrentLanguage = testString;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ++currentlanguage;
            if (currentlanguage == languages.Count)
                currentlanguage = 0;
            testString = languages[currentlanguage];
            I2.Loc.LocalizationManager.CurrentLanguage = testString;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PopupController.Instance.ShowSmallPopup("Test", null,
                new SmallPopupButton("OK"),
                new SmallPopupButton("Cancel"));
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PopupController.Instance.ShowSmallPopup("Test", null,
                new SmallPopupButton("OK"),
                new SmallPopupButton("Cancel"),
                new SmallPopupButton("ttt"));
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            FindObjectOfType<QuickAccessBarWidget>().SetGlowingAnim(FindObjectOfType<QuickAccessBarWidget>().HistoryToogle, true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PopupController.Instance.ShowPopup(testPopup);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GTDataManagementKit.SaveToPlayerPrefs(Enums.PlayerPrefsVariable.TutoStep, testInt);
            TutorialController.Instance.StartTutorial();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("User: " + UserController.Instance.gtUser);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            PopupController.Instance.ShowSmallPopup("Maintenance data missing try again please", null,
                new SmallPopupButton("Try Again", SceneController.Instance.RestartApp));
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            WidgetController.Instance.ShowWidgetPopup(Enums.WidgetId.RatePopup);
        }

        if (AssetController.UseAssetsBundle && Input.GetKeyDown(KeyCode.R))
        {
            Shader shader = Shader.Find("Sprites/Default");
            SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
            foreach (SpriteRenderer sprite in sprites)
                sprite.material.shader = shader;

            Image[] Images = FindObjectsOfType<Image>();
            foreach (Image image in Images)
                image.material.shader = Shader.Find(image.material.shader.name); ;
        }
#endif
    }

    #region Amazon Testing
    void S3Response(CloudResponse response)
    {
        Debug.Log("S3Response: " + response);
    }
    #endregion Amazon Testing

    #region Facebook Testing
    void FBLogin()
    {
        FacebookKit.Login(null);
    }

    void FBInitialGet()
    {
        FacebookKit.InitialAPIRequest(null);
    }
    #endregion Facebook Testing

    #region Popup testing
    void buttonTest()
    {
        Debug.Log("Button Clicked!");
    }

    void show()
    {
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.Error, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
        PopupController.Instance.ShowPopup(Enums.PopupId.BannedAccount, true);
    }
    #endregion Popup testing

    #region Store Testing
    private Dictionary<string, object> GetLoyaltyDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                { "Type", "LoyaltyStore" },
                { "Name", "Loyalty Store" },
                { "Icon", '>' },
                { "InGameStore", false },
            };
        List<object> list = new List<object>();
        int start = 1;
        int end = start + 10;
        for (int i = start; i < end; i++)
        {
            list.Add(new Dictionary<string, object>()
            {
                { "ItemId", i },
                { "ItemName", "loyalty " + i },
                { "ImageUrl", "https://image-store.slidesharecdn.com/cdc23be3-67f5-4e65-9411-e0745a60b241-large.jpeg" },
                { "CostData", new Dictionary<string,object>() { { "Loyalty", i } } },
            });
        }
        dict.Add("Items", list);
        return dict;
    }

    private Dictionary<string, object> GetShoutOutDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                { "Type", "ShoutOutStore" },
                { "Name", "Shout Out Store" },
                { "Icon", 'A' },
            };
        List<object> list = new List<object>();
        int start = 10;
        int end = start + 10;
        for (int i = start; i < end; i++)
        {
            Dictionary<string, object> itemDict = new Dictionary<string, object>();
            itemDict.Add("ItemId", i);
            itemDict.Add("ItemName", "shout " + i);
            itemDict.Add("Owned", i % 2 == 0 ? true : false);

            if (i == start + 1)
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i }, { "Cash", i / 10f }, });
            }
            else if (i == start + 2)
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i }, { "IAP", 9.99f }, });
            }
            else if (i == start + 3)
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i }, { "IAP", 9.99f }, { "ItunesItemId", "ItemIDTest" }, });
            }
            else
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i } });
            }
            list.Add(itemDict);
        }
        dict.Add("Items", list);
        return dict;
    }

    private Dictionary<string,object> GetBallsDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                { "Type", "BallsStore" },
                { "Name", "Balls" },
                { "Icon", '�' },
                { "InGameStore", true },
            };
        List<object> list = new List<object>();
        int start = 100;
        int end = start + 10;
        for (int i = start; i < end; i++)
        {
            list.Add(new Dictionary<string, object>()
            {
                { "ItemId", i },
                { "ItemName", "ball " + i },
                { "Owned", i % 3 == 0 ? true : false },
                { "Stats", new List<object>() {
                    new Dictionary<string, object>() { { "Name", "speed" }, { "Value", Random.Range(0, 1f) } },
                    new Dictionary<string, object>() { { "Name", "curve" }, { "Value", Random.Range(0, 1f) } } } },
                { "CostData", new Dictionary<string, object>() { { "Cash", i / 10f } } },
            });
        }
        dict.Add("Items", list);
        return dict;
    }

    private Dictionary<string, object> GetChatWordsDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                { "Type", "ChatWordsStore" },
                { "Name", "Chat Words" },
                { "Icon", '@' },
            };
        List<object> list = new List<object>();
        int start = 1;
        int end = start + 20;
        for (int i = start; i < end; i++)
        {
            Dictionary<string, object> itemDict = new Dictionary<string, object>();
            itemDict.Add("ItemId", (i == 1000 ? "c1" : i.ToString()));
            itemDict.Add("ItemName", "chat " + i);
            itemDict.Add("Owned", true);

            if (i == start + 1)
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", 10000000 }, { "Cash", i / 10f }, });
            }
            else if (i == start + 2)
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i }, { "IAP", 9.99f }, });
            }
            else if (i == start + 3)
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i }, { "IAP", 9.99f }, { "ItunesItemId", "ItemIDTest" }, });
            }
            else
            {
                itemDict.Add("CostData", new Dictionary<string, object>() { { "Loyalty", i }, { "IAP", 9.99f }, { "ItunesItemId", "ItemIDTest" }, });
            }
            list.Add(itemDict);
        }
        dict.Add("Items", list);
        return dict;
    }

    private Dictionary<string, object> GetLuckyItemsDict()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                { "Type", "LuckyItemStore" },
                { "Name", "Lucky Items" },
                { "Icon", '=' },
                { "InGameStore", true },
            };
        List<object> list = new List<object>();
        int start = 10000;
        int end = start + 10;
        for (int i = start; i < end; i++)
        {
            list.Add(new Dictionary<string, object>()
            {
                { "ItemId", i },
                { "ItemName", "lucky " + i },
                { "Locked", i == start ? true : false },
                { "OwnedCount", Random.Range(0, 5) },
                { "CostData", new Dictionary<string, object>() { { "Loyalty", i }, } },
            });
        }
        dict.Add("Items", list);
        return dict;
    }

    public void TestStore()
    {
        Dictionary<string, object> StoresList = new Dictionary<string, object>();
        StoresList.Add("ShoutOutStore",GetShoutOutDict());
        StoresList.Add("LuckyItemStore", GetLuckyItemsDict());
        StoresList.Add("LoyaltyStore", GetLoyaltyDict());
        StoresList.Add("ChatWordsStore", GetChatWordsDict());

        //Stores stores = new Stores(StoresList);
        //UserController.Instance.gtUser.StoresData = stores;
        //Debug.Log("stores " + stores);
    }

    #endregion Store Testing
}
