using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEngine.Events;
using GT.Assets;

[CustomEditor(typeof(PagesGroup), true)]
public class PageGroupEditor : Editor
{
    private ReorderableList pageList;

    private SerializedProperty m_groupId;
    private SerializedProperty m_pageList;

    protected virtual void OnEnable()
    {
        m_groupId = serializedObject.FindProperty("groupId");
        m_pageList = serializedObject.FindProperty("pageList");

        pageList = new ReorderableList(serializedObject, m_pageList, true, true, true, true);
        InitReorderableList(pageList, m_pageList.name);
        pageList.onAddDropdownCallback = AssetUtils.AddPagesCallback(s => addItemClickHandler(pageList, s));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Current Group Id", m_groupId.stringValue);
        Enums.PageGroups id;
        if (Utils.TryParseEnum(m_groupId.stringValue, out id))
        {
            Enums.PageGroups tempId = id;
            id = (Enums.PageGroups)EditorGUILayout.EnumPopup("Change Group Id", id);
            if (tempId != id)
            {
                m_groupId.stringValue = id.ToString();
            }
        }
        else
        {
            id = (Enums.PageGroups)EditorGUILayout.EnumPopup("Change Group Id", Enums.PageGroups.None);
            if (id != Enums.PageGroups.None)
            {
                m_groupId.stringValue = id.ToString();
            }
            EditorGUILayout.HelpBox("Unrecognised Page Id. Please Select Page Id", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Space();

        pageList.DoLayoutList();
    }

    #region Reorderable List Init
    private void InitReorderableList(ReorderableList list, string listName)
    {
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, listName);
        };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 50, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };

        // Deleting callback
        list.onRemoveCallback = (ReorderableList l) =>
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
            serializedObject.ApplyModifiedProperties();
        };
    }

    /// <summary>
    /// Add click handler
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    private void addItemClickHandler(ReorderableList list, params string[] item)
    {
        for (int i = 0; i < item.Length; i++)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.stringValue = item[i];
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion Reorderable List Init
}
