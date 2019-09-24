using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProgressPath))]
///<summary>
///Property drawer for the Progress Path object.
///Places both path values on a single line.
///</summary>
public class ProgressPathPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Set the position and tab property of the label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Place the field at the same indent level as other default editor fields and save the previous value
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Rect for the chapter number value
        Rect chapterNum = new Rect(position.x, position.y, position.width * 0.5f - 5, position.height);

        // Rect for the section number value
        Rect sectionNum = new Rect(position.x + position.width * 0.5f + 5, position.y, position.width * 0.5f - 5, position.height);

        // Add the fields for the progress object values to the drawer
        EditorGUI.PropertyField(chapterNum, property.FindPropertyRelative(ProgressPathMembers.ChapterNum), GUIContent.none);
        EditorGUI.PropertyField(sectionNum, property.FindPropertyRelative(ProgressPathMembers.SectionNum), GUIContent.none);

        // Restore the editor indent level to its previous value
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
