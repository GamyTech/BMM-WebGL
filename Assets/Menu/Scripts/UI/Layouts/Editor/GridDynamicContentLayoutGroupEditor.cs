using UnityEngine;
using System.Collections;
using UnityEditor;


namespace UnityEngine.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(GridDynamicContentLayoutGroup), true)]
    public class GridDynamicContentLayoutGroupEditor : Editor
    {
        private SerializedProperty m_Padding;

        private SerializedProperty m_CellSize;

        private SerializedProperty m_Spacing;

        private SerializedProperty m_StartCorner;

        private SerializedProperty m_StartAxis;

        private SerializedProperty m_ChildAlignment;

        private SerializedProperty m_Constraint;

        private SerializedProperty m_ConstraintCount;

        private SerializedProperty m_PoolPriority;

        protected virtual void OnEnable()
        {
            this.m_Padding = base.serializedObject.FindProperty("m_Padding");
            this.m_CellSize = base.serializedObject.FindProperty("m_CellSize");
            this.m_Spacing = base.serializedObject.FindProperty("m_Spacing");
            this.m_StartCorner = base.serializedObject.FindProperty("m_StartCorner");
            this.m_StartAxis = base.serializedObject.FindProperty("m_StartAxis");
            this.m_ChildAlignment = base.serializedObject.FindProperty("m_ChildAlignment");
            this.m_Constraint = base.serializedObject.FindProperty("m_Constraint");
            this.m_ConstraintCount = base.serializedObject.FindProperty("m_ConstraintCount");
            this.m_PoolPriority = base.serializedObject.FindProperty("m_poolPriority");
        }

        /// <summary>
        ///   <para>See Editor.OnInspectorGUI.</para>
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Padding, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_CellSize, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Spacing, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_StartCorner, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_StartAxis, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_ChildAlignment, true, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Constraint, true, new GUILayoutOption[0]);
            if (this.m_Constraint.enumValueIndex > 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this.m_ConstraintCount, true, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
            }

            m_PoolPriority.boolValue = EditorGUILayout.Toggle("Pool Priority", m_PoolPriority.boolValue);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
