using UnityEditor;
using GT.Assets;
using UnityEngine;

[CustomEditor(typeof(StoreItemData), true)]
public class StoreItemEditor : Editor
{
    private SerializedProperty m_Script;
    private SerializedProperty itemId;

    private string[] m_PropertyPathToExcludeForChildClasses;

    protected virtual void OnEnable()
    {
        m_Script = serializedObject.FindProperty("m_Script");
        itemId = serializedObject.FindProperty("itemId");
        m_PropertyPathToExcludeForChildClasses = new string[]
            {
                m_Script.propertyPath,
                itemId.propertyPath,
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //GUI.SetNextControlName("Item Id");
        itemId.stringValue = EditorGUILayout.TextField("Item Id", itemId.stringValue);
        //if(s.Equals(itemId.stringValue) == false)
        //{
        //    itemId.stringValue = s;
        //}
        //if (GUI.GetNameOfFocusedControl() != "Item Id")
        //{
        //    SaveToPathsDict();
        //}

        ChildClassPropertiesGUI();
        serializedObject.ApplyModifiedProperties();
    }

    private void ChildClassPropertiesGUI()
    {
        if (IsDerivedEditor())
        {
            return;
        }
        DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
    }

    //private void SaveToPathsDict()
    //{
    //    string path = AssetDatabase.GetAssetPath(target);
    //    int startIdx = path.IndexOf("Resources/") + 10;
    //    int endIdx = path.IndexOf(".asset");
    //    path = path.Substring(startIdx, endIdx - startIdx);
    //    StoreItemAssetManagement.UpdateStoreAssetPath(itemId.stringValue, path);
    //}

    private bool IsDerivedEditor()
    {
        return base.GetType() != typeof(StoreItemEditor);
    }
}
