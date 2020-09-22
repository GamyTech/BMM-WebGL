using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Widget), true)]
public class WidgetEditor : Editor
{
    private SerializedProperty m_Script;
    private SerializedProperty m_WidgetProperty;

    private string[] m_PropertyPathToExcludeForChildClasses;

    protected virtual void OnEnable()
    {
        this.m_Script = base.serializedObject.FindProperty("m_Script");
        this.m_WidgetProperty = base.serializedObject.FindProperty("widgetId");
        this.m_PropertyPathToExcludeForChildClasses = new string[]
            {
                this.m_Script.propertyPath,
                this.m_WidgetProperty.propertyPath,
            };
    }

    public override void OnInspectorGUI()
    {
        base.serializedObject.Update();
        if (!this.IsDerivedEditor())
            EditorGUILayout.PropertyField(this.m_Script, new GUILayoutOption[0]);
        Enums.WidgetId convertedWidgetID;

        if (!Utils.TryParseEnum(this.m_WidgetProperty.stringValue, out convertedWidgetID))
            Utils.TryParseEnum(target.GetType().ToString().Replace("Widget", ""), out convertedWidgetID);

        this.m_WidgetProperty.stringValue = (EditorGUILayout.EnumPopup("Widget Id", convertedWidgetID)).ToString();

        this.ChildClassPropertiesGUI();
        base.serializedObject.ApplyModifiedProperties();
    }

    private void ChildClassPropertiesGUI()
    {
        if (this.IsDerivedEditor())
            return;

        DrawPropertiesExcluding(base.serializedObject, this.m_PropertyPathToExcludeForChildClasses);
    }

    private bool IsDerivedEditor()
    {
        return base.GetType() != typeof(WidgetEditor);
    }
}
