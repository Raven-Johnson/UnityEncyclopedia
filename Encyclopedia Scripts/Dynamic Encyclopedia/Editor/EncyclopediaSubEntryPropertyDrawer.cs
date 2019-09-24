using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(EncyclopediaSubEntry))]
///<summary>
///Property drawer for the encyclopedia sub entry class.
///Shows all encyclopedia sub entry fields, and creates reorderable lists for its encyclopedia entry text list
///and its encyclopedia entry list
///</summary>
public class EncyclopediaSubEntryPropertyDrawer : PropertyDrawer {

    ///<summary>
    ///Height of a single editor line.
    ///</summary>
    float lineHeight = EditorGUIUtility.singleLineHeight;

    ///<summary>
    ///Space between editor lines.
    ///</summary>
    float lineHeightSpacing = 5f;

    ///<summary>
    ///Dictionary to hold references to individual instances of encyclopedia entry text reorderable lists.
    ///Entries use the list's property name for the dictionary key.
    ///</summary>
    Dictionary<string, ReorderableList> entryTextLists = new Dictionary<string, ReorderableList>();

    ///<summary>
    ///Dictionary to hold references to individual instances of encyclopedia entry reorderable lists.
    ///Entries use the list's property name for the dictionary key.
    ///</summary>
    Dictionary<string, ReorderableList> subEntryLists = new Dictionary<string, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        label.text = "Encyclopedia Sub Entry";

        // Rect for the list title
        Rect titleRect = new Rect(position.x, position.y, position.width, lineHeight);

        // Rect for the topic tag value
        Rect topicTagRect = new Rect(position.x, titleRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Rect for the match tag value
        Rect matchTagRect = new Rect(position.x, topicTagRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Rect for the background image value
        Rect backgroundImageRect = new Rect(position.x, matchTagRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Add the fields for the encyclopedia sub entry values to the property drawer
        EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("Title"), new GUIContent("Title"), true);
        EditorGUI.PropertyField(topicTagRect, property.FindPropertyRelative("TopicTag"), new GUIContent("Topic Tag"), true);
        EditorGUI.PropertyField(matchTagRect, property.FindPropertyRelative("MatchTag"), new GUIContent("Match Tag"), true);
        EditorGUI.PropertyField(backgroundImageRect, property.FindPropertyRelative("BackgroundImage"), new GUIContent("Background Image"), true);

        // Create the reorderable list to hold the encyclopedia entry text values
        ReorderableList entryTextList;

        // If the dictionary of existing reorderable lists doesn't contain an entry for the current object's list,
        // create one and add it to the dictionary, else continue with the existing returned reorderable list.
        if (!entryTextLists.TryGetValue(property.propertyPath, out entryTextList))
        {
            entryTextList = this.CreateEncyclopediaEntryTextList(property.FindPropertyRelative("SubTopicText"), "Text Entries");
            entryTextLists[property.propertyPath] = entryTextList;
        }

        // Create the reorderable list to hold the encyclopedia entry values
        ReorderableList subEntryList;

        // If the dictionary of existing reorderable lists doesn't contain an entry for the current object's list,
        // create one and add it to the dictionary, else continue with the existing returned reorderable list.
        if (!subEntryLists.TryGetValue(property.propertyPath, out subEntryList))
        {
            subEntryList = this.CreateSubEntryList(property.FindPropertyRelative("SubEntries"), "Sub Entries");
            subEntryLists[property.propertyPath] = subEntryList;
        }

        // Rect for the encyclopedia entry text reorderable list
        Rect entryTextListRect;

        // If the reorderable list is expanded, make the rect the height of the entire list, else just show a gap between it and the next list
        if (property.isExpanded)
        {
            entryTextListRect = new Rect(position.x, backgroundImageRect.y + lineHeight + lineHeightSpacing, position.width, entryTextList.GetHeight());
        }
        else
        {
            entryTextListRect = new Rect(position.x, backgroundImageRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);
        }

        // Rect for the encyclopedia entry reorderable list
        Rect subEntryListRect;

        // If the reorderable list is expanded, make the rect the height of the entire list, else just show the collapsed space
        if (property.isExpanded)
        {
            subEntryListRect = new Rect(position.x, entryTextListRect.y + lineHeightSpacing + entryTextList.GetHeight(), position.width, subEntryList.GetHeight());
        }
        else
        {
            subEntryListRect = new Rect(position.x, entryTextListRect.y + lineHeightSpacing + entryTextList.GetHeight(), position.width, lineHeight);
        }

        // Display the reorderable lists
        entryTextList.DoList(entryTextListRect);
        subEntryList.DoList(subEntryListRect);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 5f;

        ReorderableList entryTextList;

        if (entryTextLists.TryGetValue(property.propertyPath, out entryTextList))
        {
            height += entryTextList.GetHeight() + (entryTextList.count * 3f);
        }

        ReorderableList subEntryList;

        if (subEntryLists.TryGetValue(property.propertyPath, out subEntryList))
        {
            height += subEntryList.GetHeight() + (subEntryList.count * 3f);
        }

        return height + 20f;
    }

    private ReorderableList CreateEncyclopediaEntryTextList(SerializedProperty property, string label)
    {
        ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                Rect newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
                property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, label);
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

            onAddCallback = (ReorderableList l) =>
            {
                int index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
            },

            onRemoveCallback = (ReorderableList l) =>
            {
                if (EditorUtility.DisplayDialog("Remove Item Warning", "Are you sure you want to delete this item?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            },

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

    private ReorderableList CreateSubEntryList(SerializedProperty property, string label)
    {
        ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                Rect newRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);
                property.isExpanded = EditorGUI.Foldout(newRect, property.isExpanded, label);
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

            elementHeightCallback = (int index) =>
            {
                if (!property.isExpanded)
                {
                    return 0;
                }

                SerializedProperty element = property.GetArrayElementAtIndex(index);

                float height = lineHeight * 4f;

                SerializedProperty textEntries = element.FindPropertyRelative("EntryText");
                float textEntriesHeight = EditorGUI.GetPropertyHeight(textEntries, true);
                int textEntriesCount = textEntries.arraySize;

                if (textEntriesCount == 0)
                {
                    height += 25f;

                    return height + 60f;
                }
                else
                {
                    height += EditorGUI.GetPropertyHeight(textEntries, true) + 10f;
                    height += textEntriesCount * 3f;

                    return height + 30f;
                }
            },

            onAddCallback = (ReorderableList l) =>
            {
                int index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;

                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("Title").stringValue = string.Empty;
                element.FindPropertyRelative("TopicTag").stringValue = string.Empty;
                element.FindPropertyRelative("BackgroundImage").objectReferenceValue = null;
                element.FindPropertyRelative("UnlockPath").FindPropertyRelative("ChapterNum").intValue = 0;
                element.FindPropertyRelative("UnlockPath").FindPropertyRelative("SectionNum").intValue = 0;
                element.FindPropertyRelative("EntryText").arraySize = 0;
            },

            onRemoveCallback = (ReorderableList l) =>
            {
                if (EditorUtility.DisplayDialog("Remove Item Warning", "Are you sure you want to delete this item?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            },

            drawElementBackgroundCallback = (Rect rect, int index, bool active, bool focused) =>
            {
                if (index % 2 == 0)
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(.72f, .98f, .58f, .10f));
                    tex.Apply();
                    GUI.DrawTexture(rect, tex as Texture);
                }
                else
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(.72f, .98f, .58f, .33f));
                    tex.Apply();
                    GUI.DrawTexture(rect, tex as Texture);
                }
            }
        };

        return list;
    }
}
