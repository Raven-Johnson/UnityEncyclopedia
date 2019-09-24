using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;

[CustomPropertyDrawer(typeof(EncyclopediaEntry))]
///<summary>
///Property drawer for the encyclopedia entry class.
///Shows all encyclopedia entry fields, and places all encyclopedia entry text values in a reorderable list.
///</summary>
public class EncyclopediaEntryPropertyDrawer : PropertyDrawer {

    ///<summary>
    ///Height of a single editor line.
    ///</summary>
    float lineHeight = EditorGUIUtility.singleLineHeight;

    ///<summary>
    ///Space between editor lines.
    ///</summary>
    float lineHeightSpacing = 5f;

    /// <summary>
    /// Default height for empty reorderable lists.
    /// </summary>
    float emptyReorderableListHeight = 15f;

    ///<summary>
    ///Dictionary to hold references to individual instances of encyclopedia entry text reorderable lists.
    ///Entries use the list's property name for the dictionary key.
    ///</summary>
    Dictionary<string, ReorderableList> entryTextLists = new Dictionary<string, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Rect for the list title
        Rect titleRect = new Rect(position.x, position.y, position.width, lineHeight);

        // Rect for the topic tag value
        Rect topicTagRect = new Rect(position.x, titleRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Rect for the background image value
        Rect backgroundImageRect = new Rect(position.x, topicTagRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Rect for the unlock path property drawer
        Rect unlockPathRect = new Rect(position.x, backgroundImageRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Add the fields for the encyclopedia entry values to the property drawer
        EditorGUI.PropertyField(titleRect, property.FindPropertyRelative(EncyclopediaEntryMembers.Title), new GUIContent("Title"), true);
        EditorGUI.PropertyField(topicTagRect, property.FindPropertyRelative(EncyclopediaEntryMembers.TopicTag), new GUIContent("Topic Tag"), true);
        EditorGUI.PropertyField(backgroundImageRect, property.FindPropertyRelative(EncyclopediaEntryMembers.BackgroundImage), new GUIContent("Background Image"), true);
        EditorGUI.PropertyField(unlockPathRect, property.FindPropertyRelative(EncyclopediaEntryMembers.UnlockPath), new GUIContent("Unlock Path"), true);

        // Obtain the property for the encyclopedia entry text list
        SerializedProperty entryText = property.FindPropertyRelative(EncyclopediaEntryMembers.EntryText);

        // Create the reorderable list to hold the encyclopedia entry text values
        ReorderableList entryTextList;

        // If the dictionary of existing reorderable lists doesn't contain an entry for the current object's list,
        // create one and add it to the dictionary, else continue with the existing returned reorderable list.
        if (!entryTextLists.TryGetValue(property.propertyPath, out entryTextList))
        {
            entryTextList = this.CreateEntryTextList(entryText, "Text Entries");
            entryTextLists[property.propertyPath] = entryTextList;
        }

        // Rect for the encyclopedia entry text reorderable list
        Rect entryTextListRect;

        // If the reorderable list is expanded, make the rect the height of the entire list, else just show the collapsed space
        if (property.isExpanded)
        {
            entryTextListRect = new Rect(position.x, unlockPathRect.y + lineHeight + lineHeightSpacing, position.width, entryTextList.GetHeight());
        }
        else
        {
            entryTextListRect = new Rect(position.x, unlockPathRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);
        }

        // Display eneyclopedia entry text reorderable list
        entryTextList.DoList(entryTextListRect);

        EditorGUI.EndProperty();

        // Apply any changes to the property
        property.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Override of the property drawer's height value that accounts for the current state of the encyclopedia
    /// entry text reorderable list's expanded state.
    /// </summary>
    /// <param name="property">The encyclopedia entry property being drawn.</param>
    /// <param name="label">The label for the property.</param>
    /// <returns>The height of the encyclopedia entry object.</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get current default height of the property members (minus the reorderable list)
        float height = EditorGUI.GetPropertyHeight(property, true);

        // Reorderable list for the encyclopedia entry text list
        ReorderableList entryTextList;

        // If a reorderable list has been created for the property's encyclopedia entry text values, add its height to the property height
        if (entryTextLists.TryGetValue(property.propertyPath, out entryTextList))
        {
            // If the list is empty, add the empty list height. If not, add a height of the number of list entries * the number of elements
            // in its property drawer
            if (entryTextList.count == 0)
            {
                height += emptyReorderableListHeight;
            }
            else
            {
                height += entryTextList.count * EncyclopediaEntryTextMembers.MemberNum;
            }
        }
  
        return height;
    }

    /// <summary>
    /// Creates a reorderable list for the encyclopedia entry text list.
    /// </summary>
    /// <param name="property">Encyclopedia entry text list from the encyclopedia entry.</param>
    /// <param name="label">Label for the reorderable list.</param>
    /// <returns>Reorderable list of encyclopedia entry text objects.</returns>
    private ReorderableList CreateEntryTextList(SerializedProperty property, string label)
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

            // Set all values to default when adding a new element, instead of copying the previous value
            onAddCallback = (ReorderableList l) =>
            {
                // Increase the list's size and get the index for the new element
                int index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;

                // Set all of the new element's values to default
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative(EncyclopediaEntryTextMembers.Name).stringValue = string.Empty;
                element.FindPropertyRelative(EncyclopediaEntryTextMembers.EntryText).objectReferenceValue = null;
                element.FindPropertyRelative(EncyclopediaEntryTextMembers.UnlockPath).FindPropertyRelative(ProgressPathMembers.ChapterNum).intValue = 0;
                element.FindPropertyRelative(EncyclopediaEntryTextMembers.UnlockPath).FindPropertyRelative(ProgressPathMembers.SectionNum).intValue = 0;
            },

            // Display a warning message before removing values from the list
            onRemoveCallback = (ReorderableList l) =>
            {
                if (EditorUtility.DisplayDialog("Remove Item Warning", "Are you sure you want to delete this item?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            },

            // Draw the element backgrounds in alternating colors for better readability
            drawElementBackgroundCallback = (Rect rect, int index, bool active, bool focused) =>
            {
                if (index % 2 == 0)
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(.1f, .33f, 1f, .10f));
                    tex.Apply();
                    GUI.DrawTexture(rect, tex as Texture);
                }
                else
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(.1f, .33f, 1f, .33f));
                    tex.Apply();
                    GUI.DrawTexture(rect, tex as Texture);
                }
            }
        };

        return list;
    }
}
