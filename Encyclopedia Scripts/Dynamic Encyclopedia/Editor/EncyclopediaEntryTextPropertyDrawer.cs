using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EncyclopediaEntryText))]
///<summary>
///Property drawer for the encyclopedia entry text class.
///</summary>
public class EncyclopediaEntryTextPropertyDrawer : PropertyDrawer {

    ///<summary>
    ///Height of a single editor line.
    ///</summary>
    float lineHeight = EditorGUIUtility.singleLineHeight;

    ///<summary>
    ///Space between editor lines.
    ///</summary>
    float lineHeightSpacing = 5f;

    /// <summary>
    /// Buffer space after the property.
    /// </summary>
    float propertyHeightBuffer = 10f;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Rect for the name value
        Rect nameRect = new Rect(position.x, position.y, position.width, lineHeight);

        // Rect for the unlock path property drawer
        Rect unlockPathRect = new Rect(position.x, nameRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Rect for the entry text value
        Rect entryTextRect = new Rect(position.x, unlockPathRect.y + lineHeight + lineHeightSpacing, position.width, lineHeight);

        // Add the fields for the encyclopedia entry text values to the property drawer
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative(EncyclopediaEntryTextMembers.Name), new GUIContent("Name"), true);
        EditorGUI.PropertyField(unlockPathRect, property.FindPropertyRelative(EncyclopediaEntryTextMembers.UnlockPath), new GUIContent("Unlock Path"), true);
        EditorGUI.PropertyField(entryTextRect, property.FindPropertyRelative(EncyclopediaEntryTextMembers.EntryText), new GUIContent("Entry Text Asset"), true);

        EditorGUI.EndProperty();

        property.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Override of the property drawer's height value that returns the height of all the property fields plus a buffer.
    /// </summary>
    /// <param name="property">The encyclopedia entry text property being drawn.</param>
    /// <param name="label">The label for the property.</param>
    /// <returns>The height of the encyclopedia entry text object.</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight * EncyclopediaEntryTextMembers.MemberNum) + propertyHeightBuffer;
    }
}
