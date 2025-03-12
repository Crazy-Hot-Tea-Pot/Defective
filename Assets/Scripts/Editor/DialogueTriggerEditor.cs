using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{
    private bool showPositionSettings = true;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Show the Dialogue Data field
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueData"), new GUIContent("Dialogue Data"));

        // Show the Player Position field
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerPosition"), new GUIContent("Player Position"));

        // Show the position offset as foldout
        SerializedProperty offsetProp = serializedObject.FindProperty("playerPositionXZOffset");
        Vector2 offset = offsetProp.vector2Value;

        EditorGUILayout.LabelField("Player Position Offset (X and Z)", EditorStyles.boldLabel);
        offset.x = EditorGUILayout.FloatField("X Offset", offset.x);
        offset.y = EditorGUILayout.FloatField("Z Offset", offset.y);
        offsetProp.vector2Value = offset;


        serializedObject.ApplyModifiedProperties();
    }
}