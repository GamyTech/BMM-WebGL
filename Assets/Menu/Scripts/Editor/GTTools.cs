using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GTTools : MonoBehaviour
{
    [MenuItem("GTTools/Reset PlayerPrefs")]
    static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("GTTools/CustomTool")]
    public static void CustomTool()
    {
    }

    [MenuItem("GTTools/Print PlayerPrefs")]
    static void PrintPlayerPrefs()
    {
        string toPrint = "Player Prefs -->    ";
        foreach (var pref in Enum.GetNames(typeof(Enums.PlayerPrefsVariable)))
        {
            string res = "Missing";
            int i = PlayerPrefs.GetInt(pref, -1);
            if (i != -1)
                res = i.ToString();

            float f = PlayerPrefs.GetFloat(pref, -1);
            if (f != -1)
                res = f.ToString();

            string str = PlayerPrefs.GetString(pref, null);
            if (string.IsNullOrEmpty(str) == false)
                res = str;

            toPrint += pref + " : " + res + "  |  ";
        }
        GTDataManagementKit.SaveStringToFile(GTDataManagementKit.LocalFolder.Default, "prefs", toPrint);
        print(toPrint);
    }

    [MenuItem("GTTools/Save Selected Assets")]
    static void SaveSelectedAssets()
    {
        Debug.Log("Saving " + Selection.objects.Length + " objects");
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            Debug.Log("[" + i + "] " + Selection.objects[i].name);
            EditorUtility.SetDirty(Selection.objects[i]);
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("GTTools/Save All Assets")]
    static void SaveAllAssets()
    {
        string[] paths = AssetDatabase.GetAllAssetPaths();
        Debug.Log("Saving " + paths.Length + " objects");
        for (int i = 0; i < paths.Length; i++)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(paths[i], typeof(UnityEngine.Object));
            EditorUtility.SetDirty(asset);
        }
        AssetDatabase.SaveAssets();
    }
}
