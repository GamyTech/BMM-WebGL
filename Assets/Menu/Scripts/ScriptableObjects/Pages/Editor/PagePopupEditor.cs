using UnityEditor;

namespace GT.Assets
{
    [CustomEditor(typeof(PagePopup), true)]
    public class PagePopupEditor : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_pageId;

        private string[] m_PropertyPathToExcludeForChildClasses;

        protected virtual void OnEnable()
        {
            m_Script = serializedObject.FindProperty("m_Script");
            m_pageId = serializedObject.FindProperty("pageId");
            m_PropertyPathToExcludeForChildClasses = new string[]
                {
                    m_Script.propertyPath,
                    m_pageId.propertyPath,
                };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Current Page Id", m_pageId.stringValue);
            Enums.PageId id;
            if (Utils.TryParseEnum(m_pageId.stringValue, out id))
            {
                Enums.PageId tempId = id;
                id = (Enums.PageId)EditorGUILayout.EnumPopup("Change Page Id", id);
                if (tempId != id)
                    m_pageId.stringValue = id.ToString();
            }
            else
            {
                id = (Enums.PageId)EditorGUILayout.EnumPopup("Change Page Id", Enums.PageId.InitApp);
                if (id != Enums.PageId.InitApp)
                    m_pageId.stringValue = id.ToString();
                EditorGUILayout.HelpBox("Unrecognised Page Id. Please Select Page Id", MessageType.Warning);
            }

            ChildClassPropertiesGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void ChildClassPropertiesGUI()
        {
            DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
        }
    }
}
