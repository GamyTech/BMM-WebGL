using UnityEngine;
using System.Collections.Generic;
using GT.Assets;
using AssetBundles;
using System.Collections;
using GT.Collections;
using System.IO;

public class AssetController : MonoBehaviour
{
    private static AssetController m_instance;
    public static AssetController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<AssetController>()); }
        private set { m_instance = value; }
    }

    static string ASSET_BUNDLE_SERVER_URL;

    [System.Serializable()]
    public class CardTypeSpriteDictionary : SerializableDictionaryBase<Enums.CreditCardType, Sprite> { }

    [SerializeField]
    public CardTypeSpriteDictionary CreditCard;

    public Sprite EmptySprite;
    public Sprite DefaultImage;
    public Sprite DefaultAvatar;
    public AudioClip BonusCashBoughtSound;

    private LoadingData m_loadingData;
    public LoadingData loadingData
    {
        get
        {
            if (m_loadingData == null)
                m_loadingData = Resources.Load<LoadingData>("Loading_" + AppInformation.GAME_ID.ToString().ToLower());
            return m_loadingData;
        }
    }
    [HideInInspector]
    public bool temp;
    static int m_UseAssetsBundle = -1;
    public bool Initialized { get; private set; }
    public static bool UseAssetsBundle
    {
        get
        {
            if (m_UseAssetsBundle == -1)
                return Instance.temp;
            return m_UseAssetsBundle != 0;
        }

#if UNITY_EDITOR
        set
        {
            Instance.temp = value;
            int newValue = value ? 1 : 0;
            if (newValue != m_UseAssetsBundle)
            {
                m_UseAssetsBundle = newValue;
                UnityEditor.EditorPrefs.SetBool("UseAssetsBundle", value);
                GTDataManagementKit.SwitchAssetPath(value);
            }
        }
#endif
    }

    private Dictionary<Enums.WidgetId, Widget> widgetsPrefabsDictionary = new Dictionary<Enums.WidgetId, Widget>();
    private Dictionary<string, BoardItemData> boardAssetDictionary = new Dictionary<string, BoardItemData>();
    private Dictionary<string, StoreItemData> storeAssetDictionary = new Dictionary<string, StoreItemData>();
    private Dictionary<string, SoundData> soundAssetDictionary = new Dictionary<string, SoundData>();
    [HideInInspector]
    public List<MedalData> MedalList = new List<MedalData>();
    [HideInInspector]
    public FlagProvider.LocalDictionary CountryFlags = new FlagProvider.LocalDictionary();
    [HideInInspector]
    public List<TutorialStep> WelcomeTutorialSteps = new List<TutorialStep>();
    private GameSpecificData gameSpecificData;


    #region Unity Functions
    void OnDestroy()
    {
        m_instance = null;
    }

    void Update()
    {
        AssetBundleManager.Update();
    }
#endregion Unity Functions

    public void InitializeAssetBundles()
    {
        if (UseAssetsBundle)
            StartCoroutine(Utils.WaitForCoroutine(InitAssetsFromWeb, () => Debug.Log("-------------------- Assets loaded --------------------")));
        else
            StartCoroutine(Utils.WaitForCoroutine(InitAssetsFromRessources, () => Debug.Log("-------------------- Assets loaded --------------------")));
    }

    private IEnumerator InitAssetsFromWeb()
    {
        yield return StartCoroutine(Initialize());
        LoadingController.Instance.StartAssetLoading();

        List<string> notInstantAssetBundle = new List<string>() { "board_" };

        yield return StartCoroutine(AssetBundleManager.DownloadAllBundlesOneByOne(notInstantAssetBundle));
        yield return new WaitUntil(() => !AssetBundleManager.IsDownloading);

        yield return StartCoroutine(DownloadNewBoard("52", null));
        yield return new WaitUntil(() => !AssetBundleManager.IsDownloading);

        yield return StartCoroutine(InitStoreItemsFromBundle());
        yield return StartCoroutine(InitSoundsFromBundle());
        yield return StartCoroutine(InitRankDataFromBundle());
        yield return StartCoroutine(InitGameSpecificFromBundle());
        yield return StartCoroutine(InitTutorialStepsFromBundle());
        yield return StartCoroutine(InitWidgetsFromBundle());
        LoadingController.Instance.StopAssetLoading();

        Initialized = true;
    }

    private IEnumerator InitAssetsFromRessources()
    {
        InitStoreItemsAssetDictionary();
        yield return InitGameSpecificData();
        InitSoundAssetDictionary();
        InitRankList();
        yield return InitTutorialStepList();
        InitBoards();
        yield return InitFlags();
        InitWidgets();

        Initialized = true;
    }

    private void InitStoreItemsAssetDictionary()
    {
        StoreItemData[] items = Resources.LoadAll<StoreItemData>("StoreItemsData");
        for (int i = 0; i < items.Length; i++)
            storeAssetDictionary.AddOrOverrideValue(items[i].itemId, items[i]);
    }

    private void InitSoundAssetDictionary()
    {
        SoundData[] items = Resources.LoadAll<SoundData>("Sounds");
        for (int i = 0; i < items.Length; i++)
            soundAssetDictionary.AddOrOverrideValue(items[i].soundId, items[i]);
    }

    private void InitRankList()
    {
        MedalList = new List<MedalData>(Resources.LoadAll<MedalData>("MedalsData"));
    }

    private IEnumerator InitFlags()
    {
        ResourceRequest request = Resources.LoadAsync<FlagProvider>("FlagProvider");
        yield return request;
        CountryFlags = (request.asset as FlagProvider).CountryFlag;
    }

    private IEnumerator InitGameSpecificData()
    {
        ResourceRequest request = Resources.LoadAsync<GameSpecificData>("GameSpecific_" + AppInformation.GameIDToLower);
        yield return request;
        gameSpecificData = request.asset as GameSpecificData;
    }

    private IEnumerator InitTutorialStepList()
    {
        ResourceRequest request = Resources.LoadAsync<TutorialStepList>("Steps_" + AppInformation.CurrentPlatformDesign + "_" + AppInformation.GameIDToLower);
        yield return request;
        TutorialStepList tutos = request.asset as TutorialStepList;
        if (tutos != null)
        {
            WelcomeTutorialSteps = tutos.WelcomeSteps;
        }
    }

    private void InitBoards()
    {
        List<BoardItemData> boardList = new List<BoardItemData>(Resources.LoadAll<BoardItemData>("Boards"));
        for (int x = 0; x < boardList.Count; ++x)
            boardAssetDictionary.AddOrOverrideValue(boardList[x].itemId, boardList[x]);
    }

    private void InitWidgets()
    {
        Widget[] prefabs = Resources.LoadAll<Widget>("Widgets_Prefabs/");
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i] != null)
            {
                Enums.WidgetId id;
                if (Utils.TryParseEnum(prefabs[i].widgetId, out id))
                    widgetsPrefabsDictionary.AddOrOverrideValue(id, prefabs[i]);
            }
        }
    }

    private IEnumerator InitStoreItemsFromBundle()
    {
        List<StoreItemData> items = new List<StoreItemData>();
        yield return StartCoroutine(LoadAssetsFromBundle("storeitems", items));
        for (int i = 0; i < items.Count; i++)
            storeAssetDictionary.AddOrOverrideValue(items[i].itemId, items[i]);
    }

    /// <summary>
    /// Init Prefabs from prefab folder from assets bundle
    /// </summary>
    private IEnumerator InitWidgetsFromBundle()
    {
        List<Widget> prefabs = new List<Widget>();
        yield return StartCoroutine(LoadAssetsFromBundle("menu_widgets_prefabs", prefabs));
        for (int i = 0; i < prefabs.Count; i++)
        {
            if (prefabs[i] != null)
            {
                Enums.WidgetId id;
                if (Utils.TryParseEnum(prefabs[i].widgetId, out id))
                    widgetsPrefabsDictionary.AddOrOverrideValue(id, prefabs[i]);
            }
        }
    }

    private IEnumerator InitSoundsFromBundle()
    {
        List<SoundData> items = new List<SoundData>();
        yield return StartCoroutine(LoadAssetsFromBundle("sounds", items));
        for (int i = 0; i < items.Count; i++)
            soundAssetDictionary.AddOrOverrideValue(items[i].soundId, items[i]);
    }


    private IEnumerator InitRankDataFromBundle()
    {
        yield return StartCoroutine(LoadAssetsFromBundle("rank", MedalList));
    }

    private IEnumerator InitGameSpecificFromBundle()
    {
        List<GameSpecificData> gameSpecificDatas = new List<GameSpecificData>();
        yield return StartCoroutine(LoadAssetsFromBundle("game_specific_" + AppInformation.GameIDToLower, gameSpecificDatas));
        if (gameSpecificDatas.Count > 0)
            gameSpecificData = gameSpecificDatas[0];
    }

    private IEnumerator InitTutorialStepsFromBundle()
    {
        List<TutorialStepList> tutos = new List<TutorialStepList>();
        yield return StartCoroutine(LoadAssetsFromBundle(AppInformation.CurrentPlatformDesign.ToLower() + "_tutorialstep_" + AppInformation.GameIDToLower, tutos));
        if (tutos.Count > 0)
            WelcomeTutorialSteps = tutos[0].WelcomeSteps;
    }

    private IEnumerator InitCountryFlag()
    {
        List<FlagProvider> flagProvider = new List<FlagProvider>();
        yield return StartCoroutine(LoadAssetsFromBundle("flag", flagProvider));
        if (flagProvider.Count > 0)
            CountryFlags = flagProvider[0].CountryFlag;
    }

    // Initialize the downloading URL.
    // eg. Development server / iOS ODR / web URL
    private void InitializeSourceURL()
    {
#if UNITY_WEBGL
        ASSET_BUNDLE_SERVER_URL = "https://securegameserver.gamytechapis.com:446/BackgammonForFriends.com/Backgammon3/AssetBundles/WebGL";
#else
        string platform = "android";
#if UNITY_EDITOR
        platform = UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();
#elif UNITY_ANDROID
            platform = "android";
#elif UNITY_IOS
            platform = "ios";
#endif
        ASSET_BUNDLE_SERVER_URL = GTDataManagementKit.GetFromPrefs(Enums.PlayerPrefsVariable.AssetBundleURL) + platform + AppInformation.ASSET_BUNDLE_VERSION + "/";
#endif

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project.
        //      Another approach would be to make this configurable in the standalone player.)
        AssetBundleManager.SetDevelopmentAssetBundleServer(ASSET_BUNDLE_SERVER_URL);
        return;
#else
        // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
        //AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
        // Or customize the URL based on your deployment or configuration
        AssetBundleManager.SetSourceAssetBundleURL(ASSET_BUNDLE_SERVER_URL);
        return;
#endif
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        InitializeSourceURL();

        //Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();

        if (request != null)
            yield return StartCoroutine(request);
    }

    protected IEnumerator InitializeLevelAsync(SceneController.SceneName levelName, bool isAdditive)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        string bundleName = "scene_" + levelName.ToString().ToLower();
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(bundleName, levelName.ToString(), isAdditive);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }

    protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();

        if (prefab != null)
            GameObject.Instantiate(prefab);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }

    protected IEnumerator FillAssetsFromBundleAsync<T>(string assetBundleName, List<T> toList) where T : Object
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetsOperation request = AssetBundleManager.LoadAllAssetsAsync(assetBundleName, typeof(T));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the assets.
        toList.AddRange(request.GetAssets<T>());

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetBundleName + " loaded, returning " + toList.Count + " objects in " + elapsedTime + " seconds");
    }

    private IEnumerator DownloadNewBoard(string assetId, System.Action<BoardItemData> callback)
    {
        string bundleName = "board_" + assetId;
        yield return StartCoroutine(AssetBundleManager.DownloadBundle(bundleName));
        yield return new WaitUntil(() => !AssetBundleManager.IsDownloading);


        string assetName = "BoardData_" + assetId;
        List<BoardItemData> items = new List<BoardItemData>();
        yield return StartCoroutine(LoadAssetsFromBundle(bundleName, items));

        if (items.Count == 0 || items[0] == null)
        {
            Debug.LogError(assetName + " is not in the " + bundleName + " AssetBundle");
            if (callback != null)
                callback(null);
            yield break;
        }

        boardAssetDictionary.AddOrOverrideValue(assetId, items[0]);
        if (callback != null)
            callback(items[0]);
    }

    private IEnumerator LoadAssetsFromBundle<T>(string bundleName, List<T> targetList) where T : Object
    {
        yield return StartCoroutine(FillAssetsFromBundleAsync(bundleName, targetList));
    }

#region Public Methods
    public StoreItemData GetStoreAsset(string assetId)
    {
        if (string.IsNullOrEmpty(assetId))
            return null;

        StoreItemData asset;
        storeAssetDictionary.TryGetValue(assetId, out asset);
        if (asset == null)
            Debug.LogError("StoreItemData " + assetId + " is not found");
        
        return asset;
    }

    public Widget GetWidget(Enums.WidgetPosition widgetPosition, Enums.WidgetId widget)
    {
        Widget prefab;

        if (widgetsPrefabsDictionary == null || !widgetsPrefabsDictionary.TryGetValue(widget, out prefab) || prefab == null)
        {
            Debug.LogWarning("Requested widget [" + widget + "] prefab not found. Building default widget");
            prefab = Widget.CreateDefaultWidget(widget);
        }
        else
            prefab = Instantiate(prefab).GetComponent<Widget>();

        return prefab;
    }

    public void GetBoardAsset(string assetId, System.Action<BoardItemData> callback)
    {
        if (string.IsNullOrEmpty(assetId) && callback != null)
            callback(null);

        BoardItemData board;
        if(boardAssetDictionary.TryGetValue(assetId, out board))
        {
            if (callback != null)
                callback(board);
        }
        else
        {
            if (UseAssetsBundle)
                StartCoroutine(DownloadNewBoard(assetId, callback));
            else
            {
                Debug.Log("Board " + assetId + " is not in ressources");
                if (callback != null)
                    callback(null);
            }

        }
    }

    public SoundData GetSoundData(string soundId)
    {
        if (string.IsNullOrEmpty(soundId))
            return null;

        SoundData asset;
        soundAssetDictionary.TryGetValue(soundId, out asset);
        return asset;
    }

    public GameSpecificData GetGameSpecific()
    {
        return gameSpecificData;
    }

    public IEnumerator GetStoreImage(string url, System.Action<Sprite> callback)
    {
        string fileName = Path.GetFileName(url);
        string localName = GTDataManagementKit.GetLocalPath(GTDataManagementKit.LocalFolder.StorePicture, fileName);

        if (File.Exists(localName))
            yield return Utils.DownloadPic(localName, callback, DefaultImage);
        else
        {
            System.Action<Texture2D> SaveAndCallback = (s) =>
            {
                GTDataManagementKit.SaveBinaryToFile(GTDataManagementKit.LocalFolder.StorePicture, fileName, s.EncodeToPNG());
                callback(s.ToSprite());
            };

            yield return Utils.DownloadPicOrError(url, SaveAndCallback, s => callback(DefaultImage));
        }
    }

    public IEnumerator LoadBundleScene(SceneController.SceneName sceneName, bool isAdditive)
    {
        yield return StartCoroutine(InitializeLevelAsync(sceneName, isAdditive));
    }
    #endregion Public Methods
}
