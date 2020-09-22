using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using GT.Assets;

[CustomEditor(typeof(PageData), true), CanEditMultipleObjects] 
public class PageDataEditor : Editor
{
    private ReorderableList floatingTopWidgets;
    private ReorderableList topWidgets;
    private ReorderableList middleWidgets;
    private ReorderableList bottomWidgets;
    private ReorderableList floatingBottomWidgets;

    private SerializedProperty m_Script;
    private SerializedProperty m_pageId;
    private SerializedProperty m_storeType;
    private SerializedProperty m_navigationState;
    private SerializedProperty m_navigationCategory;
    private SerializedProperty m_backPageId;
    private SerializedProperty m_floatingTopWidgets;
    private SerializedProperty m_topWidgets;
    private SerializedProperty m_middleWidgets;
    private SerializedProperty m_bottomWidgets;
    private SerializedProperty m_floatingBottomWidgets;

    private string[] m_PropertyPathToExcludeForChildClasses;

    protected virtual void OnEnable()
    {
        m_Script = serializedObject.FindProperty("m_Script");
        m_pageId = serializedObject.FindProperty("PageId");
        m_storeType = serializedObject.FindProperty("StoreType");
        m_navigationState = serializedObject.FindProperty("NavigationState");
        m_navigationCategory = serializedObject.FindProperty("NavigationCategory");
        m_backPageId = serializedObject.FindProperty("BackTransitionPageId");
        m_floatingTopWidgets = serializedObject.FindProperty("FloatingTopWidgets");
        m_topWidgets = serializedObject.FindProperty("TopWidgets");
        m_middleWidgets = serializedObject.FindProperty("MiddleWidgets");
        m_bottomWidgets = serializedObject.FindProperty("BottomWidgets");
        m_floatingBottomWidgets = serializedObject.FindProperty("FloatingBottomWidgets");
        m_PropertyPathToExcludeForChildClasses = new string[]
            {
                m_Script.propertyPath,
                m_pageId.propertyPath,
                m_storeType.propertyPath,
                m_floatingTopWidgets.propertyPath,
                m_topWidgets.propertyPath,
                m_middleWidgets.propertyPath,
                m_bottomWidgets.propertyPath,
                m_floatingBottomWidgets.propertyPath,
                m_navigationState.propertyPath,
                m_navigationCategory.propertyPath,
                m_backPageId.propertyPath,
            };

        floatingTopWidgets = new ReorderableList(serializedObject, m_floatingTopWidgets, true, true, true, true);
        InitReorderableList(floatingTopWidgets, m_floatingTopWidgets.name);
        floatingTopWidgets.onAddDropdownCallback = AssetUtils.AddWidgetsCallback(s => addItemClickHandler(floatingTopWidgets, s));

        topWidgets = new ReorderableList(serializedObject, m_topWidgets, true, true, true, true);
        InitReorderableList(topWidgets, m_topWidgets.name);
        topWidgets.onAddDropdownCallback = AssetUtils.AddWidgetsCallback(s => addItemClickHandler(topWidgets, s));

        middleWidgets = new ReorderableList(serializedObject, m_middleWidgets, true, true, true, true);
        InitReorderableList(middleWidgets, m_middleWidgets.name);
        middleWidgets.onAddDropdownCallback = AssetUtils.AddWidgetsCallback(s => addItemClickHandler(middleWidgets, s));

        bottomWidgets = new ReorderableList(serializedObject, m_bottomWidgets, true, true, true, true);
        InitReorderableList(bottomWidgets, m_bottomWidgets.name);
        bottomWidgets.onAddDropdownCallback = AssetUtils.AddWidgetsCallback(s => addItemClickHandler(bottomWidgets, s));

        floatingBottomWidgets = new ReorderableList(serializedObject, m_floatingBottomWidgets, true, true, true, true);
        InitReorderableList(floatingBottomWidgets, m_floatingBottomWidgets.name);
        floatingBottomWidgets.onAddDropdownCallback = AssetUtils.AddWidgetsCallback(s => addItemClickHandler(floatingBottomWidgets, s));
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

        Enums.StoreType storeType;
        if (Utils.TryParseEnum(m_storeType.stringValue, out storeType) == false)
            storeType = Enums.StoreType.None;
        storeType = (Enums.StoreType)EditorGUILayout.EnumPopup("Store Type", storeType);
        m_storeType.stringValue = storeType.ToString();

        Enums.NavigationState state;
        if (Utils.TryParseEnum(m_navigationState.stringValue, out state) == false)
            state = Enums.NavigationState.Cancel;
        state = (Enums.NavigationState)EditorGUILayout.EnumPopup("Navigation State", state);
        m_navigationState.stringValue = state.ToString();

        Enums.NavigationCategory Category;
        if (Utils.TryParseEnum(m_navigationCategory.stringValue, out Category) == false)
            Category = Enums.NavigationCategory.None;
        Category = (Enums.NavigationCategory)EditorGUILayout.EnumPopup("Navigation Category", Category);
        m_navigationCategory.stringValue = Category.ToString();

        Enums.PageId Backtransition;
        if (Utils.TryParseEnum(m_backPageId.stringValue, out Backtransition) == false)
            Backtransition = Enums.PageId.InitApp;
        Backtransition = (Enums.PageId)EditorGUILayout.EnumPopup("Back Transition", Backtransition);
        m_backPageId.stringValue = Backtransition.ToString();

        ChildClassPropertiesGUI();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        floatingTopWidgets.DoLayoutList();
        topWidgets.DoLayoutList();
        middleWidgets.DoLayoutList();
        bottomWidgets.DoLayoutList();
        floatingBottomWidgets.DoLayoutList();
    }

    private void ChildClassPropertiesGUI()
    {
        DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
    }

    #region Reorderable List Init
    private void InitReorderableList(ReorderableList list, string listName)
    {
        list.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, listName); };

        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField( new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 50, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
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
