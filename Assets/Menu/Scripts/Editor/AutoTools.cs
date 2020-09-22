using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static class AutoTools {
    private const string AutoToolsTogglePath = "GTTools/Enable AutoTools";
    [MenuItem(AutoToolsTogglePath)]
    public static void ToggleAutoTools()
    {
        AutoToolsData.Instance.ActiveAutoTools = !AutoToolsData.Instance.ActiveAutoTools;
        ShouldSave = true;
    }
    [MenuItem(AutoToolsTogglePath, priority = 0, validate = true)]
    public static bool ToggleAutoToolsValidate()
    {
        Menu.SetChecked(AutoToolsTogglePath, AutoToolsData.Instance.ActiveAutoTools);
        return true;
    }

    static AutoTools() {
        EditorApplication.update += Update;
        Debug.Log("AutoTools loaded");
    }

    static bool ShouldSave = false;

    static void Update ()
    {
        if (ShouldSave)
        {
            EditorUtility.SetDirty(AutoToolsData.Instance);
            AssetDatabase.SaveAssets();
            ShouldSave = false;
        }

        if (!AutoToolsData.Instance.ActiveAutoTools || Application.isPlaying)
            return;

        bool assetControllerInstance = Object.FindObjectOfType<AssetController>() != null;

        if (!AutoToolsData.Instance.MainSceneWasLoaded)
        {
            if (assetControllerInstance)
            {
                ShouldSave = true;
                Debug.Log("Main Scene Loading Detected : Auto QuickUpdate");
                AppInformationEditor.QuickUpdate();
            }
            else
            {
                EditorBuildSettingsScene mainScene = GTDataManagementKit.GetInBuildScene(SceneController.SceneName.Main);
                if (mainScene != null)
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(mainScene.path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }
        }
        else if (AutoToolsData.Instance.CurrentPlatfrom != EditorUserBuildSettings.activeBuildTarget)
        {
            Debug.Log("Platform changing Detected : " + AutoToolsData.Instance.CurrentPlatfrom + " to " + EditorUserBuildSettings.activeBuildTarget);
            AutoToolsData.Instance.CurrentPlatfrom = EditorUserBuildSettings.activeBuildTarget;
            ShouldSave = true;
            GTDataManagementKit.SwitchAssetPath(AssetController.UseAssetsBundle);
            GTDataManagementKit.SwitchExcludedPluginsToPlatform();
        }

        if(AutoToolsData.Instance.MainSceneWasLoaded != assetControllerInstance)
        {
            AutoToolsData.Instance.MainSceneWasLoaded = assetControllerInstance;
            ShouldSave = true;
        }
    }
}

