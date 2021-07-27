using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;

public class SceneController : MonoBehaviour
{
    #region Enums
    public enum SceneName
    {
        Main = 0,
        Menu = 1,
        Game = 2,
        Replay = 3,
    }
    #endregion Enums

    private static SceneController m_instance;
    public static SceneController Instance
    {
        get { return m_instance ?? (m_instance = FindObjectOfType<SceneController>()); }
        private set { m_instance = value; }
    }

    public Camera MainSceneCamera;
    const float startPercent = .6f;

    AsyncOperation loadingSceneOperation;
    private Scene m_loadedScene;

    void OnEnable()
    {
        Instance = this;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        AssetBundleManager.OnLoadSceneProgressChanged += AssetBundleManager_OnLoadSceneProgressChanged;
    }

    void OnDisable()
    {
        Instance = null;

        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        AssetBundleManager.OnLoadSceneProgressChanged -= AssetBundleManager_OnLoadSceneProgressChanged;
    }

    void Update()
    {
        if (loadingSceneOperation != null)
        {
            float percent = startPercent + loadingSceneOperation.progress * (1 - startPercent);
            LoadingController.Instance.SetCurrentSceneLoadingProgress(percent, "Loading Scene: " + m_loadedScene.name);
            if (loadingSceneOperation.isDone)
                loadingSceneOperation = null;
        }
    }

    public void ChangeScene(SceneName scene)
    {
        StartCoroutine(Utils.WaitForCoroutine(UnloadAllSecondaryScenes, () => LoadingController.Instance.ShowSceneLoading(() => LoadScene(scene), true)));
    }

    public void RestartApp()
    {
        Debug.Log("RestartApp");

        StartCoroutine(Utils.WaitForCoroutine(UnloadAllSecondaryScenes, () => LoadScene(SceneName.Main, true, true)));
    }

    private IEnumerator UnloadAllSecondaryScenes()
    {
        List<AsyncOperation> unloadOps = new List<AsyncOperation>();
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            if (SceneManager.GetSceneByBuildIndex(i).isLoaded)
                unloadOps.Add(SceneManager.UnloadSceneAsync(i));

        int x = 0;
        while (x < unloadOps.Count)
        {
            yield return unloadOps[x];
            ++x;
        }
        GC.Collect();
    }

    private void LoadScene(SceneName scene, bool reload = false, bool fromBuild = false)
    {
        if(fromBuild || !AssetController.UseAssetsBundle)
            loadingSceneOperation = SceneManager.LoadSceneAsync((int)scene, reload ? LoadSceneMode.Single : LoadSceneMode.Additive);
        else
            StartCoroutine(AssetController.Instance.LoadBundleScene(scene, !reload));
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        Debug.Log("SceneManager_sceneUnloaded " + scene.name);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_loadedScene = scene;
        Resources.UnloadUnusedAssets();

        SceneName SceneEnum;
        Utils.TryParseEnum(m_loadedScene.name, out SceneEnum);
        switch (SceneEnum)
        {
            case SceneName.Menu:
                MenuSceneController.InitializeScene();
                Utils.ChangeOrientation(true);
#if UNITY_ANDROID
                Utils.ChangeOrientation(false);
#endif
                break;
            case SceneName.Game:
                GameSceneController.InitializeScene();
                Utils.ChangeOrientation(false);
                break;
            case SceneName.Replay:
                Utils.ChangeOrientation(false);
                break;
            default:
                break;
        }
    }

    private void AssetBundleManager_OnLoadSceneProgressChanged(LoadSceneEventArgs args)
    {
        if (args.LoadingOperation != null)
            loadingSceneOperation = args.LoadingOperation;
    }

    public SceneName GetLoadedSceneName()
    {
        SceneName sceneEnum;
        Utils.TryParseEnum(m_loadedScene.name, out sceneEnum);
        return sceneEnum;
    }
}
