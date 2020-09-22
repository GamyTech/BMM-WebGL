using UnityEngine;
using System.Collections;

namespace UnityEditor
{

    [CustomEditor (typeof(TextMesh)),CanEditMultipleObjects]
    public class TextMesh2DInspector : Editor
    {

        private SerializedProperty m_Font;

        private void OnEnable()
        {
            this.m_Font = base.serializedObject.FindProperty("m_Font");
        }

        public override void OnInspectorGUI()
        {
            Font y = (!this.m_Font.hasMultipleDifferentValues) ? (this.m_Font.objectReferenceValue as Font) : null;
            base.DrawDefaultInspector();
            Font font = (!this.m_Font.hasMultipleDifferentValues) ? (this.m_Font.objectReferenceValue as Font) : null;


            if (font != null && font != y)
            {
                UnityEngine.Object[] targets = base.targets;
                for (int i = 0; i < targets.Length; i++)
                {
                    TextMesh textMesh = (TextMesh)targets[i];
                    MeshRenderer component = textMesh.GetComponent<MeshRenderer>();
                    if (component)
                    {
                        component.sharedMaterial = font.material;
                    }
                }
            }

            
            MeshRenderer mesh = ((TextMesh)target).gameObject.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                int sortingOrder = mesh.sortingOrder;
                sortingOrder = EditorGUILayout.IntField("Sorting order", sortingOrder);

                for (int i = 0; i < targets.Length; i++)
                {
                     mesh = ((TextMesh)targets[i]).gameObject.GetComponent<MeshRenderer>();
                    if (mesh != null)
                    {
                        mesh.sortingOrder = sortingOrder;
                    }
                }
            }

        }

    }

}