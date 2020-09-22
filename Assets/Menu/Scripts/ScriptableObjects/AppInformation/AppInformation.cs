using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

///// <summary>
///// Persistant data that is constant for every app 
///// </summary>
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class AppInformation : ScriptableObject
{
    private const string AppInformationAssetName = "AppInformation";
    private const string AppInformationPath = "Menu/MenuAssets/AppInformation/Resources";
    private const string AppInformationDataPath = "Assets/Menu/MenuAssets/AppInformation/AppInformationData";
    private const string AppInformationAssetExtension = ".asset";

    private static AppInformation m_instance;
    public static AppInformation Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Resources.Load(AppInformationAssetName) as AppInformation;
#if UNITY_EDITOR
                if (m_instance == null)
                {
                    m_instance = CreateInstance<AppInformation>();
                    string properPath = Path.Combine(Application.dataPath, AppInformationPath);
                    if (!Directory.Exists(properPath))
                        Directory.CreateDirectory(properPath);

                    string fullPath = Path.Combine(
                        Path.Combine("Assets", AppInformationPath),
                        AppInformationAssetName + AppInformationAssetExtension);
                    AssetDatabase.CreateAsset(m_instance, fullPath);
                }
#endif
            }
            return m_instance;
        }
    }

    [SerializeField]
    private string gameName;
    [SerializeField]
    private Enums.GameID gameId;
    [SerializeField]
    private string bundleId;
    [SerializeField]
    private string companyName;
    [SerializeField]
    private string assetBundleVersion;
    [SerializeField]
    private int releasedVersion;
    [SerializeField]
    private string bundleVersion;
    [SerializeField]
    private Enums.MatchKind matchKind;
    [SerializeField]
    private Sprite logo;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private string facebookAppID;
    [SerializeField]
    private string signingAppleTeam;
    [SerializeField]
    private string keyStoreName;
    [SerializeField]
    private string keyStorePassword;
    [SerializeField]
    private string keyStoreAlias;

    private static List<string> m_gameIDListToLower;
    public static List<string> GameIDListToLower
    {
        get
        {
#if UNITY_EDITOR
            return System.Enum.GetNames(typeof(Enums.GameID)).ToListOfStrings().ToLower();
#else
            if(m_gameIDListToLower == null)
                m_gameIDListToLower = System.Enum.GetNames(typeof(Enums.GameID)).ToListOfStrings().ToLower();
            return m_gameIDListToLower;
#endif
        }
    }

    public static List<string> PlatformDesignList = new List<string>() { "Mobile", "PC" };

    public static string CurrentPlatformDesign
    {
        get
        {
#if UNITY_EDITOR
            if (AutoToolsData.Instance.CurrentPlatfrom == BuildTarget.Android ||
                AutoToolsData.Instance.CurrentPlatfrom == BuildTarget.iOS)
                return "Mobile";
            else
                return "PC";
#elif UNITY_IOS || UNITY_ANDROID
            return "Mobile";
#else
            return "PC";
# endif
        }
    }

#if UNITY_EDITOR
    private static AppInformationData m_dataSource;
    public static AppInformationData dataSource
    {
        get
        {
            if (m_dataSource == null)
                m_dataSource = FindSource(GAME_ID);
            return m_dataSource;
        }
        set { m_dataSource = value; }
    }
#endif


    public static string GAME_NAME
    {
        get { return Instance.gameName; }
#if UNITY_EDITOR
        set
        {
            Instance.gameName = value;
            AssetDatabase.RenameAsset(AppInformationDataPath + "/" + dataSource.name + AppInformationAssetExtension, value);
            DirtyEditor();
        }
#endif
    }

    public static Enums.GameID GAME_ID
    {
        get { return Instance.gameId; }
#if UNITY_EDITOR
        set
        {
            Instance.gameId = value;
            DirtyEditor();
        }
#endif
    }


    private static string m_gameIDToLower;
    public static string GameIDToLower
    {
        get
        {
            return GAME_ID.ToString().ToLower();
        }
    }

    public static string COMPANY_NAME
    {
        get { return Instance.companyName; }
#if UNITY_EDITOR
        set
        {
            Instance.companyName = value;
            DirtyEditor();
        }
#endif
    }

    public static Enums.MatchKind MATCH_KIND
    {
        get { return Instance.matchKind; }
#if UNITY_EDITOR
        set
        {
            Instance.matchKind = value;
            DirtyEditor();
        }
#endif
    }

    public static string ASSET_BUNDLE_VERSION
    {
        get { return Instance.assetBundleVersion; }
#if UNITY_EDITOR
        set
        {
            Instance.assetBundleVersion = value;
            DirtyEditor();
        }
#endif
    }

    public static Sprite LOGO
    {
        get { return Instance.logo; }
#if UNITY_EDITOR
        set
        {
            Instance.logo = value;
            DirtyEditor();
        }
#endif
    }
    public static Sprite ICON
    {
        get { return Instance.icon; }
#if UNITY_EDITOR
        set
        {
            Instance.icon = value;
            DirtyEditor();
        }
#endif
    }

    public static string BUNDLE_VERSION
    {
        get { return Instance.bundleVersion; }
#if UNITY_EDITOR
        set
        {
            Instance.bundleVersion = value;
            DirtyEditor();
        }
#endif
    }

    public static int RELEASED_VERSION
    {
        get { return Instance.releasedVersion; }
#if UNITY_EDITOR
        set
        {
            Instance.releasedVersion = value;
            DirtyEditor();
        }
#endif
    }

    public static string SIGNING_APPLE_TEAM
    {
        get { return Instance.signingAppleTeam; }
#if UNITY_EDITOR
        set
        {
            Instance.signingAppleTeam = value;
            DirtyEditor();
        }
#endif
    }

    public static string BUNDLE_ID
    {
        get { return Instance.bundleId; }
#if UNITY_EDITOR
        set
        {
            Instance.bundleId = value;
            DirtyEditor();
        }
#endif
    }

    public static string KEY_STORE_NAME
    {
        get { return Instance.keyStoreName; }
#if UNITY_EDITOR
        set
        {
            Instance.keyStoreName = value;
            DirtyEditor();
        }
#endif
    }
    public static string KEY_STORE_PASSWORD
    {
        get { return Instance.keyStorePassword; }
#if UNITY_EDITOR
        set
        {
            Instance.keyStorePassword = value;
            DirtyEditor();
        }
#endif
    }
    public static string KEY_STORE_ALIAS
    {
        get { return Instance.keyStoreAlias; }
#if UNITY_EDITOR
        set
        {
            Instance.keyStoreAlias = value;
            DirtyEditor();
        }
#endif
    }
    public static string FACEBOOK_APP_ID
    {
        get { return Instance.facebookAppID; }
#if UNITY_EDITOR
        set
        {
            Instance.facebookAppID = value;
            DirtyEditor();
        }
#endif
    }


    public static AppInformationData FindSource(Enums.GameID gameID)
    {
        return FindSource(gameID.ToString());
    }

    public static AppInformationData FindSource(string gameID)
    {
        AppInformationData source = null;
#if UNITY_EDITOR
        string[] GUIDs = AssetDatabase.FindAssets("t:" + typeof(AppInformationData).ToString(), new string[] { AppInformationDataPath });
        int x = 0;
        while (x < GUIDs.Length && source == null)
        {
            AppInformationData data = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(GUIDs[x]), typeof(AppInformationData)) as AppInformationData;
            if (data.gameId.ToString() == gameID)
                source = data;
            ++x;
        }

        if (source == null)
            Debug.LogError("No AppInformationData corresponding to this gameID");

#endif
        return source;
    }

    public static void ChangeSource(Enums.GameID targetGame)
    {
#if UNITY_EDITOR
        GAME_ID = targetGame;

        dataSource = FindSource(GAME_ID);
        if (dataSource == null)
            return;


        GAME_NAME = dataSource.name;
        COMPANY_NAME = dataSource.companyName;
        BUNDLE_ID = dataSource.bundleId;
        MATCH_KIND = dataSource.matchKind;
        ASSET_BUNDLE_VERSION = dataSource.version;
        LOGO = dataSource.logo;
        ICON = dataSource.icon;
        BUNDLE_VERSION = dataSource.BundleVersion;
        SIGNING_APPLE_TEAM = dataSource.SigningAppleTeam;
        RELEASED_VERSION = dataSource.ReleasedVersion;
        KEY_STORE_NAME = dataSource.keyStoreName;
        KEY_STORE_PASSWORD = dataSource.keyStorePassword;
        KEY_STORE_ALIAS = dataSource.keyStoreAlias;
        FACEBOOK_APP_ID = dataSource.facebookAppID;

        DirtyEditor();
#endif
    }

    public void SaveDataSource()
    {
        DirtyEditor();
    }

    private static void DirtyEditor()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(Instance);
#endif
    }

}
