using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GTDataManagementKit
{

    public enum ScaleMode { FillCrop, Fit, ScaleOnly }

#region Player Prefs
    public static void RemoveFromPrefs(Enums.PlayerPrefsVariable key)
    {
        PlayerPrefs.DeleteKey(key.ToString());
        PlayerPrefs.Save();
    }

    public static void SaveToPlayerPrefs(Enums.PlayerPrefsVariable dictKey, string value)
    {
        PlayerPrefs.SetString(dictKey.ToString(), value.ToString());
        PlayerPrefs.Save();
    }

    public static T GetFromPrefs<T>(Enums.PlayerPrefsVariable key, T defaultValue, System.Func<string, T> convertFunc)
    {
        string currentPref = GetFromPrefs(key);
        T returnValue;
        if (string.IsNullOrEmpty(currentPref))
        {
            returnValue = defaultValue;
            SaveToPlayerPrefs(key, defaultValue.ToString());
        }
        else
            returnValue = convertFunc(currentPref);

        return returnValue;
    }

    public static void SaveToPlayerPrefs(Enums.PlayerPrefsVariable dictKey, int value)
    {
        PlayerPrefs.SetInt(dictKey.ToString(), value);
        PlayerPrefs.Save();
    }

    public static void SaveToPlayerPrefs(Enums.PlayerPrefsVariable dictKey, float value)
    {
        PlayerPrefs.SetFloat(dictKey.ToString(), value);
        PlayerPrefs.Save();
    }

    public static string GetFromPrefs(Enums.PlayerPrefsVariable dictKey, string defaultValue = "")
    {
        if (PlayerPrefs.HasKey(dictKey.ToString()))
            return PlayerPrefs.GetString(dictKey.ToString());
        return defaultValue;
    }

    public static int GetIntFromPrefs(Enums.PlayerPrefsVariable dictKey, int defaultValue = 0)
    {
        if (PlayerPrefs.HasKey(dictKey.ToString()))
            return PlayerPrefs.GetInt(dictKey.ToString());
        return defaultValue;
    }

    public static float GetFloatFromPrefs(Enums.PlayerPrefsVariable dictKey, float defaultValue = 0)
    {
        if (PlayerPrefs.HasKey(dictKey.ToString()))
            return PlayerPrefs.GetFloat(dictKey.ToString());
        return defaultValue;
    }

    public static bool GetBoolFromPrefs(Enums.PlayerPrefsVariable dictKey, bool defaultValue = false)
    {
        if (PlayerPrefs.HasKey(dictKey.ToString()))
            return PlayerPrefs.GetString(dictKey.ToString()).ParseBool();
        return defaultValue;
    }
#endregion Player Prefs

#region File Handling

    public enum LocalFolder
    {
        StorePicture,
        Temp,
        Default,
    }

    private static string m_projectPath;
    public static string ProjectPath
    {
        get
        {
            if (m_projectPath == null)
                m_projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            return m_projectPath;
        }
    }

    public static string GetLocalPath(LocalFolder folder, string fileName)
    {
        return GetLocalFolderPath(folder) + fileName;
    }

    public static string GetLocalFolderPath(LocalFolder folder)
    {
#if UNITY_EDITOR
        string projectPath = ProjectPath;
#else
        string projectPath = Application.persistentDataPath;
#endif
        switch (folder)
        {
            case LocalFolder.StorePicture:
                return projectPath + "/storeItems/";
            case LocalFolder.Temp:
                return projectPath + "/temp/";
            default:
                return projectPath + "/";
        }
    }

    private static void CreateDirectoryIfNotExists(LocalFolder folder)
    {
        String path = GetLocalFolderPath(folder);
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
                Debug.Log("Create directory " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("Falied To create a directory " + path + " : " + e.Message);
                return;
            }
        }
    }

    public static void DeleteFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);
    }

    public static void DeleteDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return;

        Directory.Delete(directoryPath, true);
    }

    public static void SaveStringToFile(LocalFolder folder, string fileName, string message, bool toAppend = false)
    {
        string file = GetLocalPath(folder, fileName + ".txt");
        try
        {
            if (File.Exists(file) && toAppend)
                File.AppendAllText(file, "\n" + message);
            else
                File.WriteAllText(file, message);
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError(e.Message);
        }
    }

#region Save/Load Serializable Class File
    public static void SaveSerializableClassToFile(LocalFolder folder, string fileName, object obj)
    {
        Stream stream = null;
        try
        {
            IFormatter formatter = new BinaryFormatter();
            stream = new FileStream(GetLocalPath(folder, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Falied To save file: " + e.Message);
            return;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }
    }

    public static object LoadSerializableClassFromBytes(byte[] bytes)
    {
        object obj = null;
        Stream stream = null;
        try
        {
            IFormatter formatter = new BinaryFormatter();
            stream = new MemoryStream(bytes);
            obj = formatter.Deserialize(stream);
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadFromBytes: " + e.Message);
            return null;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }
        return obj;
    }

    public static object LoadSerializableClassFromFile(LocalFolder folder, string fileName)
    {
        string localPath = GetLocalPath(folder, fileName);
        if (!File.Exists(localPath))
            return null;

        object obj = null;
        Stream stream = null;
        try
        {
            IFormatter formatter = new BinaryFormatter();
            stream = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            obj = formatter.Deserialize(stream);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Falied To load file: " + e.Message);
            return null;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }
        return obj;
    }
#endregion Save/Load Serializable Class File

#region Save/Load Binary File
    public static void SaveBinaryToFile(LocalFolder folder, string fileName, byte[] data)
    {
        CreateDirectoryIfNotExists(folder);
        try
        {
            File.WriteAllBytes(GetLocalPath(folder, fileName), data);
            Debug.Log("save file in " + GetLocalPath(folder, fileName));
        }
        catch (Exception e)
        {
            Debug.LogError("Failed To save file: " + e.Message);
            return;
        }
    }

    public static object LoadBinaryFromFile(LocalFolder folder, string fileName)
    {
        byte[] fileData;

        try
        {
            fileData = File.ReadAllBytes(GetLocalPath(folder, fileName));
        }
        catch (Exception e)
        {
            Debug.LogError("Falied To save file: " + e.Message);
            return null;
        }
        return fileData;
    }
#endregion
#endregion General File Handling

#region Textures

    /// <summary>
    /// Resize picture file to have a specified max size
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="filePath"></param>
    /// <param name="maxSize"></param>
    /// <returns></returns>
    public static void ResizeTexture(ScaleMode mode, int maxSize, Texture2D texture)
    {
        Debug.Log("Resizing Picture with " + mode);
        switch (mode)
        {
            case ScaleMode.FillCrop:
                FillCrop( texture, maxSize);
                break;
            case ScaleMode.Fit:
                Fit( texture, maxSize);
                break;
            case ScaleMode.ScaleOnly:
                ScaleOnly( texture, maxSize);
                break;
        }
    }

    private static void FillCrop( Texture2D tex, int maxSize)
    {
        int SmallestBorder = tex.width < tex.height ? tex.width : tex.height;
        int startX = (tex.width / 2) - (SmallestBorder / 2);
        int startY = (tex.height / 2) - (SmallestBorder / 2);
        TextureScale.Bilinear(tex, startX, startY, SmallestBorder, SmallestBorder, maxSize, maxSize);
    }

    private static void Fit( Texture2D tex, int maxSize)
    {
        TextureScale.Bilinear( tex, 0, 0, tex.width, tex.height, maxSize, maxSize);
    }

    private static void ScaleOnly( Texture2D tex, int maxSize)
    {
        bool widthMax = tex.width > tex.height;
        float scaleFactor = widthMax ? tex.width / maxSize : tex.height / maxSize;
        int newWidth = (int)(tex.width / scaleFactor);
        int newHeight = (int)(tex.height / scaleFactor);
        TextureScale.Bilinear(tex, 0, 0, tex.width, tex.height, newWidth, newHeight);
    }
#endregion Textures

#region Rotate Textures

    public static void RotateTextureToFile(Texture2D texture, string filePath, float angle)
    {
        Texture2D newScreenshot = RotateTexture(texture, angle);
        byte[] bytes = newScreenshot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }

    public static Texture2D RotateTexture(Texture2D tex, float angle)
    {
        Texture2D rotImage = new Texture2D(tex.width, tex.height);
        int x, y;
        float x1, y1, x2, y2;

        int w = tex.width;
        int h = tex.height;
        float x0 = rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
        float y0 = rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

        float dx_x = rot_x(angle, 1.0f, 0.0f);
        float dx_y = rot_y(angle, 1.0f, 0.0f);
        float dy_x = rot_x(angle, 0.0f, 1.0f);
        float dy_y = rot_y(angle, 0.0f, 1.0f);

        x1 = x0;
        y1 = y0;

        for (x = 0; x < tex.width; x++)
        {
            x2 = x1;
            y2 = y1;

            for (y = 0; y < tex.height; y++)
            {
                x2 += dx_x;//rot_x(angle, x1, y1);
                y2 += dx_y;//rot_y(angle, x1, y1);
                rotImage.SetPixel((int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex, x2, y2));
            }
            x1 += dy_x;
            y1 += dy_y;
        }
        rotImage.Apply();
        return rotImage;
    }

    private static Color getPixel(Texture2D tex, float x, float y)
    {
        Color pix;
        int x1 = (int)Mathf.Floor(x);
        int y1 = (int)Mathf.Floor(y);

        if (x1 > tex.width || x1 < 0 || y1 > tex.height || y1 < 0)
        {
            pix = Color.clear;
        }
        else
        {
            pix = tex.GetPixel(x1, y1);
        }
        return pix;
    }

    private static float rot_x(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return (x * cos + y*(-sin));
    }

    private static float rot_y(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return (x * sin + y * cos);
    }

    #endregion Rotate Textures

    #region Assets
#if UNITY_EDITOR
    private static string RefugeAssetPath { get { return ProjectPath + "RefugeAsset"; } }
    private static string PluginsAssetPath { get { return ProjectPath + "Assets/Menu/ExternalPlugins/TransitPlatformPlugins"; } }
    private static List<AssetsPathData> m_assetsPathDatas = new List<AssetsPathData>();
    private static EditorBuildSettingsScene[] m_ScenesBuildList = new EditorBuildSettingsScene[] { };
    public static Dictionary<SceneController.SceneName, EditorBuildSettingsScene> m_ScenesBuildDict = new Dictionary<SceneController.SceneName, EditorBuildSettingsScene>();

    public static void SwitchExcludedPluginsToPlatform()
    {
        string exludedMark = EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();
        List<string> refugedPlugins = Directory.GetDirectories(RefugeAssetPath).ToListOfStrings();
        for (int x = 0; x < refugedPlugins.Count; ++x)
        {
            refugedPlugins[x] = refugedPlugins[x].Replace('\\', '/');
            string[] PathChain = refugedPlugins[x].Split('/');
            string ItemName = PathChain[PathChain.Length - 1];
            if (!ItemName.Contains(exludedMark))
                Directory.Move(refugedPlugins[x], PluginsAssetPath + "/" + ItemName);
        }

        List<string> transitPlugins = Directory.GetDirectories(PluginsAssetPath).ToListOfStrings();
        for (int x = 0; x < transitPlugins.Count; ++x)
        {
            transitPlugins[x] = transitPlugins[x].Replace('\\', '/');
            string[] PathChain = transitPlugins[x].Split('/');
            string ItemName = PathChain[PathChain.Length - 1];
            if (ItemName.Contains(exludedMark))
                Directory.Move(transitPlugins[x], RefugeAssetPath + "/" + ItemName);
        }
    }

    private static void RefreshAssetsPathsData()
    {
        m_assetsPathDatas.Clear();
        string[] GUIDs = AssetDatabase.FindAssets("t:" + typeof(AssetsPathData).ToString(), new string[] { "Assets/Menu/MenuAssets/AssetsPaths" });

        if (GUIDs.Length == 0)
            Debug.LogError("Can't find AssetsPathData files.");

        for (int x = 0; x < GUIDs.Length; x++)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUIDs[x]);
            m_assetsPathDatas.Add(AssetDatabase.LoadAssetAtPath(path, typeof(AssetsPathData)) as AssetsPathData);
        }
    }

    public static void SwitchAssetPath(bool UseAssetsBundle)
    {
        RefreshAssetsPathsData();
        Debug.Log(UseAssetsBundle ? "move Assets to assets bundle path" : "move Assets to resources path");

        if (UseAssetsBundle)
        {
            for (int x = 0; x < m_assetsPathDatas.Count; x++)
            {
                AssetsPathData data = m_assetsPathDatas[x];
                string oldPath = data.GetResourcePath();
                string newPath = data.GetBundlePath();
                if (data.IsTheItemASample())
                {
                    if (data.AlwaysInResources())
                        SwapAssetsWithSpecificOne(oldPath, newPath, data.IsDirectory(), data.GetSearchPattern(), data.getItemBaseName(), data.IsSpecificToPlatform(), data.IsSpecificToGameId());
                    else
                        MoveAssets(oldPath, newPath, data.getItemBaseName());
                }
                else
                    MoveAsset(oldPath + '/' + m_assetsPathDatas[x].getItemName(), newPath + '/' + m_assetsPathDatas[x].getItemName());
            }
        }
        else
        {
            for (int x = 0; x < m_assetsPathDatas.Count; x++)
            {
                AssetsPathData data = m_assetsPathDatas[x];
                string oldPath = data.GetBundlePath();
                string newPath = data.GetResourcePath();

                if (data.IsTheItemASample())
                    SwapAssetsWithSpecificOne(newPath, oldPath, data.IsDirectory(), data.GetSearchPattern(), data.getItemBaseName(), data.IsSpecificToPlatform(), data.IsSpecificToGameId());
                else
                    MoveAsset(oldPath + '/' + data.getItemName(), newPath + '/' + data.getItemName());
            }

        }

        EditorBuildSettings.scenes = GetAllInBuildScenes();
    }

    public static void SwapAssetsWithSpecificOne(string mainPath, string storringPath, bool isDirectory, string searchPanel, string BaseName, bool specifcToPlatform, bool specificToGameId)
    {
        string defaultGameIDItemPath = null;
        string defaultGameIDItemName = null;

        bool FoundSpecificIdItem = false;
        DirectoryInfo info = new DirectoryInfo(ProjectPath + mainPath);
        FileSystemInfo[] fileInfo;

        if (isDirectory)
            fileInfo = info.GetDirectories(searchPanel, SearchOption.TopDirectoryOnly);
        else
            fileInfo = info.GetFiles(searchPanel, SearchOption.TopDirectoryOnly);

        for (int x = 0; x < fileInfo.Length; x++)
            MoveAsset(mainPath + '/' + fileInfo[x].Name, storringPath + '/' + fileInfo[x].Name);

        info = new DirectoryInfo(ProjectPath + storringPath);
        if (isDirectory)
            fileInfo = info.GetDirectories(searchPanel, SearchOption.TopDirectoryOnly);
        else
            fileInfo = info.GetFiles(searchPanel, SearchOption.TopDirectoryOnly);

        int y = 0;
        while (!FoundSpecificIdItem && y < fileInfo.Length)
        {
            bool matchGameId = specificToGameId ? fileInfo[y].Name.Contains(AppInformation.GameIDToLower) : true;
            bool matchPlatform = specifcToPlatform ? fileInfo[y].Name.Contains(AppInformation.CurrentPlatformDesign) : true;
            if (matchPlatform)
            {
                if (matchGameId)
                {
                    FoundSpecificIdItem = true;
                    MoveAsset(storringPath + '/' + fileInfo[y].Name, mainPath + '/' + fileInfo[y].Name);
                }
                else if (fileInfo[y].Name.Contains(default(Enums.GameID).ToString().ToLower()))
                {
                    defaultGameIDItemName = fileInfo[y].FullName;
                    defaultGameIDItemPath = storringPath + '/' + fileInfo[y].Name;
                }
            }
            ++y;
        }

        if (!FoundSpecificIdItem)
        {
            string assetName = (specifcToPlatform ? AppInformation.CurrentPlatformDesign + " " : "") + (specificToGameId ? AppInformation.GameIDToLower + " " : "") + "Specific " + BaseName;
            if (string.IsNullOrEmpty(defaultGameIDItemPath))
                Debug.LogWarning(assetName + " is not found and neither the default one in " + storringPath);
            else
            {
                Debug.LogWarning(assetName + " is not found, moving the default one : " + defaultGameIDItemName);
                MoveAsset(defaultGameIDItemPath, mainPath + '/' + defaultGameIDItemName);
            }
        }
    }

    private static void MoveAssets(string oldPath, string newPath, string BaseName)
    {
        string[] GUIDs = AssetDatabase.FindAssets(BaseName, new string[] { oldPath });

        for (int y = 0; y < GUIDs.Length; y++)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUIDs[y]);
            string[] folder = path.Split('/');
            string ItemName = folder[folder.Length - 1];

            MoveAsset(path, newPath + '/' + ItemName);
        }
    }

    public static void MoveAsset(string sourcePath, string targetPath)
    {
        string errorMessage;
        errorMessage = AssetDatabase.MoveAsset(sourcePath, targetPath);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            string pathText = sourcePath + "  to  " + targetPath + "  ";
            string path = ProjectPath + targetPath;
            if (Directory.Exists(path) || File.Exists(path))
                Debug.Log(pathText + "File not found but already in target folder");
            else
                Debug.LogWarning(pathText + errorMessage);
        }
    }

    public static EditorBuildSettingsScene[] GetAllInBuildScenes()
    {
        RefreshScenesPath();
        return m_ScenesBuildList;
    }

    public static EditorBuildSettingsScene GetInBuildScene(SceneController.SceneName keyName)
    {
        RefreshScenesPath();
        EditorBuildSettingsScene TargetedBuildScene = null;

        if (!m_ScenesBuildDict.TryGetValue(keyName, out TargetedBuildScene))
            Debug.LogError("No scene called " + keyName + " was found in Assets/Scenes/InBuildScene");

        return TargetedBuildScene;
    }

    private static void RefreshScenesPath()
    {
        string[] GUIDs = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Scenes/InBuildScene" });
        m_ScenesBuildDict.Clear();

        for (int x = 0; x < GUIDs.Length; ++x)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUIDs[x]);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));
            SceneController.SceneName name;
            if (Utils.TryParseEnum(asset.name, out name))
                m_ScenesBuildDict.AddOrOverrideValue(name, new EditorBuildSettingsScene(path, true));
            else
                Debug.LogError(asset.name + " is not an SceneController.SceneName enum, this scene is not added in build settings");
        }

        SceneController.SceneName[] possiblesSceneNames = (SceneController.SceneName[])System.Enum.GetValues(typeof(SceneController.SceneName));
        m_ScenesBuildList = new EditorBuildSettingsScene[GUIDs.Length];
        EditorBuildSettingsScene scene;
        int y = 0;
        for (int x = 0; x < possiblesSceneNames.Length; ++x)
            if (m_ScenesBuildDict.TryGetValue(possiblesSceneNames[x], out scene))
            {
                m_ScenesBuildList[y] = scene;
                ++y;
            }
    }

#endif
    #endregion Assets
}
