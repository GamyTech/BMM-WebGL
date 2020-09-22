using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Assets/AutoToolsData")]
public class AutoToolsData : ScriptableObject
{
#if UNITY_EDITOR
    public SceneAsset MainScenePath;
    public BuildTarget CurrentPlatfrom = BuildTarget.NoTarget;
    public bool MainSceneWasLoaded = true;
    public bool ActiveAutoTools = true;

    private static AutoToolsData instance = null;
    public static AutoToolsData Instance
    {
        get
        {
            if (instance == null)
                instance = AssetDatabase.LoadAssetAtPath("Assets/Menu/MenuAssets/AppInformation/AutoToolsData.asset", typeof(AutoToolsData)) as AutoToolsData;
            return instance;
        }
    }
#endif
}
