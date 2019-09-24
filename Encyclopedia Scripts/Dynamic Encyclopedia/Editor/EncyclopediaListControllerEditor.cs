using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ListController))]
///<summary>
///Custom editor for the list controller class.
///Displays its encyclopedia entries in a reorderable list.
///</summary>
public class EncyclopediaListControllerEditor : Editor {

    ///<summary>
    ///Height of a single editor line.
    ///</summary>
    float lineHeight = EditorGUIUtility.singleLineHeight;

    ///<summary>
    ///Space between editor lines.
    ///</summary>
    float lineHeightSpacing = 3f;

    // Reorderable list for the encyclopedia entry list
    ReorderableList encyclopediaEntryList;

    private void OnEnable()
    {
        // Don't show if there's no object
        if (target == null)
        {
            return;
        }

        // If the list of encyclopedia entries doesn't exist yet, create it
        if (encyclopediaEntryList == null)
        {
            encyclopediaEntryList = MakeEncyclopediaEntryList(serializedObject.FindProperty(ListControllerMembers.EncyclopediaEntries), "Encyclopedia Entries");
        }
    }

    public override void OnInspectorGUI()
    {
        // Add the fields for the list controller values to the editor
        EditorGUILayout.PropertyField(serializedObject.FindProperty(ListControllerMembers.EncyclopediaItemPrefab), new GUIContent("Encyclopedia Panel Prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(ListControllerMembers.ContentPanel), new GUIContent("Content Panel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(ListControllerMembers.ImagePanel), new GUIContent("Image Panel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(ListControllerMembers.SubTopicPanel), new GUIContent("Sub Topic Panel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(ListControllerMembers.EntryTextDisplay), new GUIContent("Entry Text Display"));

        // Display encyclopedia entry reorderable list
        encyclopediaEntryList.DoLayoutList();

        // Apply any changes to the object
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Creates a reorderable list for the encyclopedia entry list.
    /// </summary>
    /// <param name="property">Encyclopedia entry list property from the list controller.</param>
    /// <param name="label">Label for the reorderable list.</param>
    /// <returns>Reorderable list of encyclopedia entry text objects.</returns>
    ReorderableList MakeEncyclopediaEntryList(SerializedProperty property, string label)
    {
        // Create a new reorderable list for the property, and display all options
        ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true)
        {
            // Allow the reorderable list to be collapsed
            drawHeaderCallback = (Rect rect) =>
            {
                Rect newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
                property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, label);
            },

            // Return 0 for all element heights if the list is collapsed, else return the element height plus line spacing
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

            // If the list is collapsed, disable it. If not, display each element's property drawer
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

            // Set all values to default when adding a new element, instead of copying the previous value
            onAddCallback = (ReorderableList l) =>
            {
                // Increase the list's size and get the index for the new element
                int index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;

                // Set all of the new element's values to default
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative(EncyclopediaEntryMembers.Title).stringValue = string.Empty;
                element.FindPropertyRelative(EncyclopediaEntryMembers.TopicTag).stringValue = string.Empty;
                element.FindPropertyRelative(EncyclopediaEntryMembers.BackgroundImage).objectReferenceValue = null;
                element.FindPropertyRelative(EncyclopediaEntryMembers.UnlockPath).FindPropertyRelative(ProgressPathMembers.ChapterNum).intValue = 0;
                element.FindPropertyRelative(EncyclopediaEntryMembers.UnlockPath).FindPropertyRelative(ProgressPathMembers.SectionNum).intValue = 0;
                element.FindPropertyRelative(EncyclopediaEntryMembers.EntryText).arraySize = 0;
            },

            // Display a warning message before removing values from the list
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
