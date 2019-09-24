using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SubListController))]
public class EncyclopediaSubListEditor : Editor
{
    SerializedProperty EncyclopediaItemPrefab;
    SerializedProperty ContentPanel;
    SerializedProperty ImagePanel;
    SerializedProperty SubTopicPanel;
    SerializedProperty EntryTextDisplay;
    ReorderableList subControllerList;
    float lineHeight = EditorGUIUtility.singleLineHeight;
    float lineHeightSpacing = 3f;

    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }

        EncyclopediaItemPrefab = serializedObject.FindProperty("EncyclopediaItemPrefab");
        ContentPanel = serializedObject.FindProperty("ContentPanel");
        ImagePanel = serializedObject.FindProperty("ImagePanel");
        SubTopicPanel = serializedObject.FindProperty("SubTopicPanel");
        EntryTextDisplay = serializedObject.FindProperty("EntryTextDisplay");

        if (subControllerList == null)
        {
            subControllerList = MakeSubEntryList(serializedObject.FindProperty("entries"), "Sub Entries");
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(EncyclopediaItemPrefab, new GUIContent("Encyclopedia Panel Prefab"));
        EditorGUILayout.PropertyField(ContentPanel, new GUIContent("Content Panel"));
        EditorGUILayout.PropertyField(ImagePanel, new GUIContent("Image Panel"));
        EditorGUILayout.PropertyField(SubTopicPanel, new GUIContent("Sub Topic Panel"));
        EditorGUILayout.PropertyField(EntryTextDisplay, new GUIContent("Entry Text Display"));

        subControllerList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    ReorderableList MakeSubEntryList(SerializedProperty property, string label)
    {
        ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                Rect newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
                property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, label);
            },

            elementHeightCallback = (int index) =>
            {
                if (!property.isExpanded)
                {
                    return 0;
                }

                SerializedProperty element = property.GetArrayElementAtIndex(index);

                float height = EditorGUI.GetPropertyHeight(element, true);

                return height + lineHeightSpacing;
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (!property.isExpanded)
                {
                    GUI.enabled = index == property.arraySize;
                    return;
                }

                SerializedProperty element = property.GetArrayElementAtIndex(index);

                EditorGUI.PropertyField(rect, element, true);
            },

            onAddCallback = (ReorderableList l) =>
            {
                int index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;

                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("Title").stringValue = string.Empty;
                element.FindPropertyRelative("TopicTag").stringValue = string.Empty;
                element.FindPropertyRelative("MatchTag").stringValue = string.Empty;
                element.FindPropertyRelative("BackgroundImage").objectReferenceValue = null;
                element.FindPropertyRelative("SubTopicText").arraySize = 0;
                element.FindPropertyRelative("SubEntries").arraySize = 0;
            },

            onRemoveCallback = (ReorderableList l) =>
            {
                if (EditorUtility.DisplayDialog("Remove Item Warning", "Are you sure you want to delete this item?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            }
        };

        return list;
    }
}
