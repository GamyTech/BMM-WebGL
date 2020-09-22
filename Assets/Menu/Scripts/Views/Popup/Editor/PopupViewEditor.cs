using UnityEngine;
using UnityEditor;
using GT.Assets;

[CustomEditor(typeof(PopupView))]
public class PopupViewEditor : Editor
{
    PopupView myTarget;

    Enums.PopupId savePopup;
    Enums.PopupId loadPopup;

    void OnEnable()
    {
        myTarget = (PopupView)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Saving Current Configuration:");

        GUILayout.BeginHorizontal();
        savePopup = (Enums.PopupId)EditorGUILayout.EnumPopup("Save Type", savePopup);
        if (GUILayout.Button("Save"))
        {
            SaveCurrent(savePopup);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Loading Saved Configuration:");

        GUILayout.BeginHorizontal();
        loadPopup = (Enums.PopupId)EditorGUILayout.EnumPopup("Load Type", loadPopup);
        if (GUILayout.Button("Load"))
        {
            LoadSavedAsset(loadPopup);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Reset"))
        {
            myTarget.Reset();
        }
    }

    private void SaveCurrent(Enums.PopupId type)
    {
        PopupData popupData = null;

        PopupData[] popupList = Resources.LoadAll<PopupData>(PopupController.POPUP_RESOURCES_PATH);
        int x = 0;
        while(popupData == null && x < popupList.Length)
        {
            if (popupList[x].popupId == type.ToString())
                popupData = popupList[x];
            ++x;
        }

        if (popupData == null)
        {
            popupData = CreateInstance<PopupData>();
            AssetDatabase.CreateAsset(popupData, "/Menu/MenuAssets/Popup/Resources/Popups/" + type.ToString());
        }

        myTarget.SetToCurrentConfiguration(type, popupData);
        EditorUtility.SetDirty(popupData);
        AssetDatabase.SaveAssets();
    }

    private void LoadSavedAsset(Enums.PopupId type)
    {
        PopupData[] popupList = Resources.LoadAll<PopupData>(PopupController.POPUP_RESOURCES_PATH);
        for (int i = 0; i < popupList.Length; i++)
        {
            if (popupList[i].popupId.Equals(type.ToString()))
            {
                myTarget.Reset();
                myTarget.ShowPopupImmediate(popupList[i]);
                return;
            }
        }
        Debug.Log(type + " is missing from assets");
    }
}
