using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Assets/AssetsPathData")]
public class AssetsPathData : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField]
    private UnityEngine.Object bundlesFolder;
    [SerializeField]
    private UnityEngine.Object resourcesFolder;
    [SerializeField]
    private UnityEngine.Object ItemToMove;
    [SerializeField]
    private bool SpecificToPlatform = false;
    [SerializeField]
    private bool SpecificToGameID = false;
    [SerializeField]
    private bool KeepInResources = true;

    public string GetResourcePath()
    {
        return AssetDatabase.GetAssetPath(resourcesFolder);
    }

    public string GetBundlePath()
    {
        return AssetDatabase.GetAssetPath(bundlesFolder);
    }

    public bool IsTheItemASample()
    {
        return SpecificToGameID || SpecificToPlatform;
    }

    public bool IsSpecificToGameId()
    {
        return SpecificToGameID;
    }

    public bool IsSpecificToPlatform()
    {
        return SpecificToPlatform;
    }

    public bool AlwaysInResources()
    {
        return KeepInResources;
    }

    private string savedName;
    private string savedBaseName;
    public string getItemBaseName()
    {
        if(savedName != ItemToMove.name)
        {
            savedName = ItemToMove.name;
            string[] parts = ItemToMove.name.Split('_');

            savedBaseName = "";
            for (int x = 0; x < parts.Length; ++x)
                if (!AppInformation.GameIDListToLower.Contains(parts[x]) && !AppInformation.PlatformDesignList.Contains(parts[x]))
                    savedBaseName += (x != 0 ? "_" : "") + parts[x];
        }
        return savedBaseName;
    }

    public string getItemName()
    {
        return ItemToMove.name + System.IO.Path.GetExtension(GTDataManagementKit.ProjectPath + AssetDatabase.GetAssetPath(ItemToMove));
    }

    public bool IsDirectory()
    {
        return ItemToMove.GetType() == typeof(DefaultAsset);
    }

    public string GetSearchPattern()
    {
        return getItemBaseName() + '*' + System.IO.Path.GetExtension(GTDataManagementKit.ProjectPath + AssetDatabase.GetAssetPath(ItemToMove));
    }
#endif
}
