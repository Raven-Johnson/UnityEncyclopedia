using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// Code not in use

//[CustomEditor(typeof(EncyclopediaSubEntry))]
public class EncyclopediaSubEntryEditor : Editor {

    ReorderableList entryTextList;
    ReorderableList subEntryList;
    SerializedProperty title;
    SerializedProperty majorTopicNum;
    SerializedProperty imagePanel;
    float lineHeight = EditorGUIUtility.singleLineHeight;
    float lineHeightSpacing = 3f;

    private void OnEnable()
    {
        if (target == null)
        {
            Debug.Log("Sub Entry Custom Editor Empty");
            return;
        }

        title = serializedObject.FindProperty("Title");
        majorTopicNum = serializedObject.FindProperty("MajorTopicNum");
        imagePanel = serializedObject.FindProperty("ImagePanel");
        //entryText = serializedObject.FindProperty("SubTopicText");

        //entryTextList = 

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(title, new GUIContent("Title"));
        EditorGUILayout.PropertyField(majorTopicNum, new GUIContent("Major Topic Number"));
        EditorGUILayout.PropertyField(imagePanel, new GUIContent("Image Panel"));

        entryTextList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
