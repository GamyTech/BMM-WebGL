using UnityEditor;
using GT.Assets;
using UnityEngine;
using GT.Collections;

[CustomEditor(typeof(CategoriesData), true)]
public class CategoriesDataEditor : Editor
{
    private SerializedProperty m_Script;

    private string[] m_PropertyPathToExcludeForChildClasses;

    protected virtual void OnEnable()
    {
        this.m_Script = base.serializedObject.FindProperty("m_Script");
        this.m_PropertyPathToExcludeForChildClasses = new string[]
            {
                this.m_Script.propertyPath,
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if(GUILayout.Button("Add Missing Items"))
        {
            if (target is CategoriesData)
            {
                (target as CategoriesData).CleanObsoleteKeys();
                (target as CategoriesData).AddMissingKeys();
            }
        }

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

    private bool IsDerivedEditor()
    {
        return base.GetType() != typeof(CategoriesDataEditor);
    }
}
