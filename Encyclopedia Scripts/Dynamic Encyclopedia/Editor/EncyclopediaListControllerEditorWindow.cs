using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

public class EncyclopediaListControllerEditorWindow : EditorWindow {

    const int TOP_PADDING = 2;
    const string HELP_TEXT = "Can not find List or Sub List Controller component on selected GameObject.";

    static float lineHeightSpacing = 3f;
    static Vector2 minWindowSize = Vector2.one * 300f;
    static Rect helpRect = new Rect(0f, 0f, 300f, 100f);
    static Rect listRect = new Rect(Vector2.zero, minWindowSize);
    Vector2 mainTopicPos;
    Vector2 subTopicPos;

    SerializedObject listControllerObject;
    SerializedObject subEntryObject;
    SerializedProperty encyclopediaEntries;
    ReorderableList encyclopediaEntryList;
    ReorderableList relatedEntryList;
    SubListController SubListController;


    [MenuItem ("Window/Encyclopedia/Show Encyclopedia Window")]

    static void Initialize()
    {
        EncyclopediaListControllerEditorWindow window = EditorWindow.GetWindow<EncyclopediaListControllerEditorWindow>(false, "Encyclopedia");
        window.minSize = minWindowSize;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GameObject listObject = Selection.activeGameObject;

        if (listObject != null)
        {
            ListController listController = listObject.GetComponent<ListController>();

            if (listController != null)
            {
                listControllerObject = new SerializedObject(listController);
                encyclopediaEntries = listControllerObject.FindProperty("EncyclopediaEntries");

                if (encyclopediaEntryList == null || !SerializedProperty.EqualContents(encyclopediaEntryList.serializedProperty, encyclopediaEntries))
                {
                    encyclopediaEntryList = MakeEncyclopediaEntryList(encyclopediaEntries, "Encyclopedia Top Entries - " + listObject.name, listController);
                }
            }
            else
            {
                SubListController subListController = listObject.GetComponent<SubListController>();

                if (subListController != null)
                {
                    listControllerObject = new SerializedObject(subListController);
                    encyclopediaEntries = listControllerObject.FindProperty("entries");

                    if (encyclopediaEntryList == null || !SerializedProperty.EqualContents(encyclopediaEntryList.serializedProperty, encyclopediaEntries))
                    {
                        encyclopediaEntryList = MakeEncyclopediaSubEntryList(encyclopediaEntries, "Encyclopedia Sub Entries - " + listObject.name, subListController);
                    }
                }

                else
                {
                    encyclopediaEntryList = null;
                    relatedEntryList = null;
                    EditorGUI.HelpBox(helpRect, HELP_TEXT, MessageType.Warning);
                    return;
                }
            }
        }

        if (listControllerObject != null)
        {
            EditorGUILayout.BeginHorizontal();

            mainTopicPos = EditorGUILayout.BeginScrollView(mainTopicPos, false, false, GUILayout.MaxWidth(position.width / 2));

            encyclopediaEntryList.DoLayoutList();

            EditorGUILayout.EndScrollView();

            subTopicPos = EditorGUILayout.BeginScrollView(subTopicPos, false, false, GUILayout.MaxWidth(position.width / 2));

            if (relatedEntryList != null)
            {
                relatedEntryList.DoLayoutList();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();

            if (subEntryObject != null)
            {
                subEntryObject.ApplyModifiedProperties();
            }

            listControllerObject.ApplyModifiedProperties();
        }
        else
        {
            EditorGUI.HelpBox(helpRect, HELP_TEXT, MessageType.Warning);
        }
    }

    ReorderableList MakeEncyclopediaEntryList(SerializedProperty property, string label, ListController listController)
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
                    //GUI.enabled = index == property.arraySize;
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

            onSelectCallback = (ReorderableList l) =>
            {
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(l.index);

                GameObject subListObject = listController.SubTopicPanel;
                SubListController subListController = subListObject.GetComponent<SubListController>();

                if (subListController != null)
                {
                    bool hasEntries = subListController.entries.Any(i => i.TopicTag == element.FindPropertyRelative("MatchTag").stringValue);

                    if (!hasEntries)
                    {
                        relatedEntryList = null;
                        return;
                    }

                    subEntryObject = new SerializedObject(subListController);
                    relatedEntryList = null;
                    relatedEntryList = MakeMatchedSubEntryList(subEntryObject.FindProperty("entries"), element.FindPropertyRelative("Title").stringValue + " Sub Entries",
                        element.FindPropertyRelative("TopicTag").stringValue);
                }
            }
            //,

            //drawElementBackgroundCallback = (Rect rect, int index, bool active, bool focused) =>
            //{
            //    if (index % 2 == 0)
            //    {
            //        Texture2D tex = new Texture2D(1, 1);
            //        tex.SetPixel(0, 0, new Color(.95f, .89f, 79f, .33f));
            //        tex.Apply();
            //        GUI.DrawTexture(rect, tex as Texture);
            //    }
            //    else
            //    {
            //        Texture2D tex = new Texture2D(1, 1);
            //        tex.SetPixel(0, 0, new Color(.95f, .89f, 79f, 1f));
            //        tex.Apply();
            //        GUI.DrawTexture(rect, tex as Texture);
            //    }
            //}
        };

        return list;
    }

    ReorderableList MakeEncyclopediaSubEntryList(SerializedProperty property, string label, SubListController listController)
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
                    //GUI.enabled = index == property.arraySize;
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
                element.FindPropertyRelative("BackgroundImage").objectReferenceValue = null;
                element.FindPropertyRelative("UnlockPath").FindPropertyRelative("ChapterNum").intValue = 0;
                element.FindPropertyRelative("UnlockPath").FindPropertyRelative("SectionNum").intValue = 0;
                element.FindPropertyRelative("SubTopicText").arraySize = 0;
                element.FindPropertyRelative("SubEntries").arraySize = 0;
            },

            onRemoveCallback = (ReorderableList l) =>
            {
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(l.index);

                if (EditorUtility.DisplayDialog("Remove Item Warning", "Are you sure you want to delete the " +
                    element.FindPropertyRelative("Title").stringValue + " item?", "Yes", "No"))
                { 
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            },

            onSelectCallback = (ReorderableList l) =>
            {
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(l.index);

                GameObject subListObject = listController.SubTopicPanel;

                if (subListObject != null)
                {
                    SubListController subListController = subListObject.GetComponent<SubListController>();

                    if (subListController != null)
                    {
                        bool hasEntries = subListController.entries.Any(i => i.TopicTag == element.FindPropertyRelative("MatchTag").stringValue);

                        if (!hasEntries)
                        {
                            relatedEntryList = null;
                            return;
                        }

                        subEntryObject = new SerializedObject(subListController);
                        relatedEntryList = null;
                        relatedEntryList = MakeMatchedSubEntryList(subEntryObject.FindProperty("entries"), element.FindPropertyRelative("Title").stringValue + " Sub Entries",
                            element.FindPropertyRelative("TopicTag").stringValue);
                    }
                }
            }
            //,

            //drawElementBackgroundCallback = (Rect rect, int index, bool active, bool focused) =>
            //{
            //    if (index % 2 == 0)
            //    {
            //        Texture2D tex = new Texture2D(1, 1);
            //        tex.SetPixel(0, 0, new Color(.95f, .89f, 79f, .33f));
            //        tex.Apply();
            //        GUI.DrawTexture(rect, tex as Texture);
            //    }
            //    else
            //    {
            //        Texture2D tex = new Texture2D(1, 1);
            //        tex.SetPixel(0, 0, new Color(.95f, .89f, 79f, 1f));
            //        tex.Apply();
            //        GUI.DrawTexture(rect, tex as Texture);
            //    }
            //}
        };

        return list;
    }

    ReorderableList MakeMatchedSubEntryList(SerializedProperty property, string label, string topicTag)
    {
        ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, false)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, label);
            },

            elementHeightCallback = (int index) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                if (topicTag != element.FindPropertyRelative("MatchTag").stringValue)
                {
                    return 0;
                }

                float height = EditorGUI.GetPropertyHeight(element, true);

                return height + lineHeightSpacing;
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);

                if (topicTag != element.FindPropertyRelative("MatchTag").stringValue)
                {
                    return;
                }
                
                EditorGUI.PropertyField(rect, element, true);
            },

            onAddCallback = (ReorderableList l) =>
            {
                int index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;

                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("Title").stringValue = string.Empty;
                element.FindPropertyRelative("MatchTag").stringValue = topicTag;
                element.FindPropertyRelative("BackgroundImage").objectReferenceValue = null;
                element.FindPropertyRelative("SubTopicText").arraySize = 0;
                element.FindPropertyRelative("SubEntries").arraySize = 0;
            },

            onRemoveCallback = (ReorderableList l) =>
            {
                SerializedProperty element = l.serializedProperty.GetArrayElementAtIndex(l.index);

                if (EditorUtility.DisplayDialog("Remove Item Warning", "Are you sure you want to delete the " + 
                    element.FindPropertyRelative("Title").stringValue + " item?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }
            },

            drawElementBackgroundCallback = (Rect rect, int index, bool active, bool focused) =>
            {
                if (index % 2 == 0)
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(.9f, .79f, .95f, .33f));
                    tex.Apply();
                    GUI.DrawTexture(rect, tex as Texture);
                }
                else
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(.9f, .79f, .95f, 1f));
                    tex.Apply();
                    GUI.DrawTexture(rect, tex as Texture);
                }
          
            }
        };

        return list;
    }
}
