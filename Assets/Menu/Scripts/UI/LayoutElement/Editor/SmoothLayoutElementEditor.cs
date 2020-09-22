using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(SmoothLayoutElement), true)]
    public class SmoothLayoutElementEditor : LayoutElementEditor
    {
        private SerializedProperty m_smoothTime;
        private SerializedProperty m_targetSize;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_smoothTime = base.serializedObject.FindProperty("smoothTime");
            this.m_targetSize = base.serializedObject.FindProperty("m_targetSize");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            base.serializedObject.Update();
            EditorGUILayout.Space();
            m_smoothTime.floatValue = EditorGUILayout.FloatField("Smooth Time", m_smoothTime.floatValue);
            m_targetSize.vector2Value = EditorGUILayout.Vector2Field("Target Size", m_targetSize.vector2Value);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
