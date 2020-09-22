using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TutorialStepList))]
public class TutoStepListEditor : Editor
{
    private ReorderableList welcomeStepList;
    private SerializedProperty targetWelcomeList;

    private TutorialStepList listExample { get { return target as TutorialStepList; } }
    private string[] m_PropertyPathToExcludeForChildClasses;

    private void OnEnable()
    {
        targetWelcomeList = serializedObject.FindProperty("WelcomeSteps");
        SerializedProperty m_Script = base.serializedObject.FindProperty("m_Script");
        m_PropertyPathToExcludeForChildClasses = new string[] { targetWelcomeList.propertyPath, m_Script.propertyPath };

        welcomeStepList = new ReorderableList(listExample.WelcomeSteps, typeof(TutorialStep), true, true, true, true);
        welcomeStepList.drawHeaderCallback += r => DrawHeader(r, "Welcome Steps");
        welcomeStepList.drawElementCallback += (r,i,a,f) => DrawElement(r,i,a,f, listExample.WelcomeSteps, targetWelcomeList);
        welcomeStepList.onAddCallback += (r) => AddItem(r, listExample.WelcomeSteps);
        welcomeStepList.onRemoveCallback += (r) => RemoveItem(r, listExample.WelcomeSteps);
    }

    private void OnDisable()
    {
        welcomeStepList.drawHeaderCallback -= r => DrawHeader(r, "Welcome Steps");
        welcomeStepList.drawElementCallback -= (r, i, a, f) => DrawElement(r, i, a, f, listExample.WelcomeSteps, targetWelcomeList);
        welcomeStepList.onAddCallback -= (r) => AddItem(r, listExample.WelcomeSteps);
        welcomeStepList.onRemoveCallback -= (r) => RemoveItem(r, listExample.WelcomeSteps);
    }

    /// <summary>
    /// Draws the header of the list
    /// </summary>
    /// <param name="rect"></param>
    private void DrawHeader(Rect rect, string name)
    {
        GUI.Label(rect, name);
    }

    /// <summary>
    /// Draws one element of the list (ListItemExample)
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="index"></param>
    /// <param name="active"></param>
    /// <param name="focused"></param>
    private void DrawElement(Rect rect, int index, bool active, bool focused, List<TutorialStep> items, SerializedProperty property)
    {
        TutorialStep item = items[index];

        EditorGUI.BeginChangeCheck();
        items[index] = (TutorialStep)EditorGUI.ObjectField(rect, property.GetArrayElementAtIndex(index).objectReferenceValue, typeof(TutorialStep), false);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }

        // If you are using a custom PropertyDrawer, this is probably better
        // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
        // Although it is probably smart to cach the list as a private variable ;)
    }

    private void AddItem(ReorderableList list, List<TutorialStep> items)
    {
        items.Add(null);
        EditorUtility.SetDirty(target);
    }

    private void RemoveItem(ReorderableList list, List<TutorialStep> items)
    {
        items.RemoveAt(list.index);
        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        welcomeStepList.DoLayoutList();
        DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
        serializedObject.ApplyModifiedProperties();
    }
}