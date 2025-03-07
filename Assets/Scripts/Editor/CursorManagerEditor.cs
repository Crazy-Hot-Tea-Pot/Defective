using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CursorManager))]
public class CursorManagerEditor : Editor
{
    private SerializedProperty cursorMappings;

    private void OnEnable()
    {
        cursorMappings = serializedObject.FindProperty("cursorMappings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Cursor Manager", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Cursor"))
        {
            cursorMappings.arraySize++;
        }

        for (int i = 0; i < cursorMappings.arraySize; i++)
        {
            SerializedProperty cursorData = cursorMappings.GetArrayElementAtIndex(i);
            SerializedProperty layerIndex = cursorData.FindPropertyRelative("layerIndex");
            SerializedProperty cursorTexture = cursorData.FindPropertyRelative("cursorTexture");

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Cursor " + (i + 1), EditorStyles.boldLabel);

            // Layer dropdown
            layerIndex.intValue = EditorGUILayout.LayerField("Layer", layerIndex.intValue);

            // Cursor texture field
            EditorGUILayout.PropertyField(cursorTexture, new GUIContent("Cursor Texture"));

            if (GUILayout.Button("Remove Cursor"))
            {
                cursorMappings.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
