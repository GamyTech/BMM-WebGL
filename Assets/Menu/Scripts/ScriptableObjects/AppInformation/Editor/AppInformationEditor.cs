using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(AppInformation), true)]
public class AppInformationEditor : Editor
{
    [MenuItem("GTTools/App Settings", priority = 0)]
    public static void AppInformationMenu()
    {
        EditorGUIUtility.PingObject(AppInformation.Instance);
        Selection.activeObject = AppInformation.Instance;
    }

    [MenuItem("GTTools/Update App Settings", priority = 0)]
    public static void QuickUpdate()
    {
        GTDataManagementKit.SwitchExcludedPluginsToPlatform();
        UpDateGameID();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("GTTools/Go to SVN state", priority = 0)]
    public static void GoToSVNState()
    {
        AppInformation.GAME_ID = Enums.GameID.Backgammon4Money;
        AssetController.UseAssetsBundle = false;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
    }

    private int m_SavedCurrentAppInfo;
    private string m_SavedGameName;
    private Enums.GameID m_SavedGameID;
    private RuntimePlatform m_SavedPlatform;

    private SerializedProperty m_gameId;

    private string[] m_PropertyPathToExcludeForChildClasses;

    protected virtual void OnEnable()
    {
        m_SavedPlatform = Application.platform;
        m_gameId = base.serializedObject.FindProperty("gameId");
        SerializedProperty m_Script = base.serializedObject.FindProperty("m_Script");

        m_PropertyPathToExcludeForChildClasses = new string[] { m_Script.propertyPath, };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.HelpBox(
            "Game name: " + AppInformation.GAME_NAME + "\n\n"
            + "Game id: " + AppInformation.GAME_ID + "\n"
            + "Company name: " + AppInformation.COMPANY_NAME + "\n"
            + "MatchK kind : " + AppInformation.MATCH_KIND.ToString() + "\n"
            + "Revison: " + AppInformation.ASSET_BUNDLE_VERSION + "\n"
            + "Bundle identifier : " + AppInformation.BUNDLE_ID + "\n"
            + "Released version : " + AppInformation.BUNDLE_VERSION + "\n"
            + "Released version : " + AppInformation.RELEASED_VERSION + "\n"
            + "Apple signing team : " + AppInformation.SIGNING_APPLE_TEAM + "\n"
            + "key store path : " + GTDataManagementKit.ProjectPath + AppInformation.KEY_STORE_NAME + "\n"
            + "key store password : " + AppInformation.KEY_STORE_PASSWORD + "\n"
            + "key store alias : " + AppInformation.KEY_STORE_ALIAS + "\n"
            + "Facebook App ID : " + AppInformation.FACEBOOK_APP_ID + "\n"
            , MessageType.Info);

        if (Utils.SetProperty(ref m_SavedCurrentAppInfo, m_gameId.enumValueIndex))
        {
            AppInformation.ChangeSource((Enums.GameID)m_gameId.enumValueIndex);
            PlayerPrefs.DeleteAll();
        }

        ApplyChanges();

        DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
        serializedObject.ApplyModifiedProperties();
    }

    private void ApplyChanges()
    {
        if (Application.isPlaying)
            return;

        bool changed = false;

        Applychange(ref m_SavedGameID, AppInformation.GAME_ID, ref changed, UpDateGameID);

        Applychange(ref m_SavedPlatform, Application.platform, ref changed, GTDataManagementKit.SwitchExcludedPluginsToPlatform);

        Applychange(ref m_SavedGameName, AppInformation.GAME_NAME, ref changed, UpdateGameName);

        Applychange(ref AppInformation.dataSource.companyName, AppInformation.COMPANY_NAME, ref changed, UpdateCompanyName);

        Applychange(ref AppInformation.dataSource.bundleId, AppInformation.BUNDLE_ID, ref changed, UpdateBundleId);

        Applychange(ref AppInformation.dataSource.matchKind, AppInformation.MATCH_KIND, ref changed);

        Applychange(ref AppInformation.dataSource.version, AppInformation.ASSET_BUNDLE_VERSION, ref changed);

        Applychange(ref AppInformation.dataSource.logo, AppInformation.LOGO, ref changed, UpdateLogo);

        Applychange(ref AppInformation.dataSource.icon, AppInformation.ICON, ref changed, UpdateIcon);

        Applychange(ref AppInformation.dataSource.BundleVersion, AppInformation.BUNDLE_VERSION, ref changed, UpdateBundleVersion);

        Applychange(ref AppInformation.dataSource.ReleasedVersion, AppInformation.RELEASED_VERSION, ref changed);

        Applychange(ref AppInformation.dataSource.SigningAppleTeam, AppInformation.SIGNING_APPLE_TEAM, ref changed, UpdateBundleVersion); 

        Applychange(ref AppInformation.dataSource.keyStoreName, AppInformation.KEY_STORE_NAME, ref changed, UpadteKeyStoreData);

        Applychange(ref AppInformation.dataSource.keyStoreAlias, AppInformation.KEY_STORE_ALIAS, ref changed, UpadteKeyStoreData);

        Applychange(ref AppInformation.dataSource.keyStorePassword, AppInformation.KEY_STORE_PASSWORD, ref changed, UpadteKeyStoreData);

        Applychange(ref AppInformation.dataSource.facebookAppID, AppInformation.FACEBOOK_APP_ID, ref changed, UpadteFacebookID);

        if (changed)
        {
            EditorUtility.SetDirty(AppInformation.dataSource);
            AssetDatabase.SaveAssets();
        }
    }

    private void Applychange<T>(ref T currentValue, T newValue, ref bool changed, UnityAction UpdateAction = null)
    {
        if(Utils.SetProperty(ref currentValue, newValue))
        {
            changed = true;
            if (UpdateAction != null)
                UpdateAction();
        }
    }

    private static void UpDateGameID()
    {
        GTDataManagementKit.SwitchAssetPath(AssetController.UseAssetsBundle);

        UpdateGameName();
        UpdateCompanyName();
        UpdateBundleId();
        UpdateLogo();
        UpdateIcon();
        UpdateBundleVersion();
        UpadteKeyStoreData();
        UpadteFacebookID();
        UpdateAndroidManifest();
    }

    private static void UpdateAndroidManifest()
    {
        string path = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        if (!System.IO.File.Exists(path))
            return;

        byte[] buffer = System.IO.File.ReadAllBytes(path);
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding(false);
        string content = encoder.GetString(buffer);
        Debug.Log(content);
        string[] GameIDList = System.Enum.GetNames(typeof(Enums.GameID));
        for(int x = 0; x < GameIDList.Length; ++x)
        {
            if(GameIDList[x] != AppInformation.GAME_ID.ToString())
            {
                AppInformationData appData = AppInformation.FindSource(GameIDList[x]);
                content = content.Replace(appData.bundleId, AppInformation.BUNDLE_ID);
                content = content.Replace(appData.facebookAppID, AppInformation.FACEBOOK_APP_ID);
            }
        }
        Debug.Log("Android manifest Updated");
        System.IO.File.WriteAllText(path, content, encoder);
    }
    private static void UpdateGameName()
    {
        if(PlayerSettings.productName != AppInformation.GAME_NAME)
        {
            AppInformation.GAME_NAME = AppInformation.GAME_NAME;
            PlayerSettings.productName = AppInformation.GAME_NAME;
        }
    }

    private static void UpdateCompanyName()
    {
        if (PlayerSettings.companyName != AppInformation.COMPANY_NAME)
            PlayerSettings.companyName = AppInformation.COMPANY_NAME;
    }

    private static void UpdateBundleId()
    {
        if (PlayerSettings.GetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup) != AppInformation.BUNDLE_ID)
            PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, AppInformation.BUNDLE_ID);
    }

    private static void UpdateLogo()
    {
        PlayerSettings.SplashScreen.show = AppInformation.LOGO != null && EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL;
        PlayerSettings.SplashScreenLogo[] logos = new PlayerSettings.SplashScreenLogo[] { new PlayerSettings.SplashScreenLogo() };
        logos[0].logo = AppInformation.LOGO;
        PlayerSettings.SplashScreen.logos = logos;
    }

    private static void UpdateIcon()
    {
        PlayerSettings.SetIconsForTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup, new Texture2D[] { AppInformation.ICON.texture });
    }

    private static void UpdateSigningAppleTeam()
    {
#if UNITY_ANDROID
        if (PlayerSettings.iOS.appleDeveloperTeamID != AppInformation.SIGNING_APPLE_TEAM)
            PlayerSettings.iOS.appleDeveloperTeamID = AppInformation.SIGNING_APPLE_TEAM;
#endif
    }

    private static void UpdateBundleVersion()
    {
        if (PlayerSettings.bundleVersion != AppInformation.BUNDLE_VERSION)
            PlayerSettings.bundleVersion = AppInformation.BUNDLE_VERSION;
    }

    private static void UpadteKeyStoreData()
    {
#if UNITY_ANDROID
        string KeyStorePath = GTDataManagementKit.ProjectPath + "KeyStores/" + AppInformation.KEY_STORE_NAME;
        if (PlayerSettings.Android.keystoreName != KeyStorePath)
            PlayerSettings.Android.keystoreName = KeyStorePath;
        if (PlayerSettings.Android.keystorePass != AppInformation.KEY_STORE_PASSWORD)
            PlayerSettings.Android.keystorePass = AppInformation.KEY_STORE_PASSWORD;
        if (PlayerSettings.Android.keyaliasName != AppInformation.KEY_STORE_ALIAS)
            PlayerSettings.Android.keyaliasName = AppInformation.KEY_STORE_ALIAS;
        if (PlayerSettings.Android.keyaliasPass != AppInformation.KEY_STORE_PASSWORD)
            PlayerSettings.Android.keyaliasPass = AppInformation.KEY_STORE_PASSWORD;
#endif
    }

    private static void UpadteFacebookID()
    {
        Facebook.Unity.Settings.FacebookSettings.AppIds.Clear();
        Facebook.Unity.Settings.FacebookSettings.AppIds.Add(AppInformation.FACEBOOK_APP_ID);
        Object FBScriptableObject = Resources.Load("FacebookSettings");
        if(FBScriptableObject != null)
            EditorUtility.SetDirty(FBScriptableObject);
    }
}
