using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatZone))]
public class CombatZoneEditor : Editor
{
    private bool showColorSettings = true;
    private bool showPositionSettings = true;

    public override void OnInspectorGUI()
    {
        CombatZone combatZone = (CombatZone)target;
        SerializedObject serializedObject = new SerializedObject(target);

        // Display CombatZoneSet at the top
        EditorGUILayout.LabelField("Combat Zone Status", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CombatZoneSet"), new GUIContent("Combat Zone Set"));

        // Validate CombatZone
        //ValidateCombatZone(combatZone);

        serializedObject.Update();

        // Group: Color Settings
        showColorSettings = EditorGUILayout.Foldout(showColorSettings, "Color Settings");
        if (showColorSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SavedColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("savedTransparicy"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NotSavedColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NotSaveTransparicy"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InCombatColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CombatTransparicy"));
        }
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("areaSize"));

        GUILayout.Space(10);

        // Group: Position Settings
        showPositionSettings = EditorGUILayout.Foldout(showPositionSettings, "Position Settings");
        if (showPositionSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerPositionXZOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraPositionOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraTargetPositionOffset"));
        }

        GUILayout.Space(10);

        // Buttons
        if (combatZone.CombatZoneSet)
        {
            if (GUILayout.Button("Edit Combat Zone"))
            {
                combatZone.EditCombatZone();
            }
        }
        else
        {
            if (GUILayout.Button("Save Combat Zone"))
            {
                combatZone.SaveCombatZone();
            }
        }


        serializedObject.ApplyModifiedProperties();
    }

    //private void ValidateCombatZone(CombatZone combatZone)
    //{
    //    Bounds combatAreaBounds = combatZone.zoneCollider.bounds;

    //    // Check if PlayerPosition is outside CombatArea
    //    if (!combatAreaBounds.Contains(combatZone.PlayerPosition.transform.position))
    //    {
    //        EditorGUILayout.HelpBox("PlayerPosition is outside the CombatArea!", MessageType.Warning);
    //    }

    //    // Check if CameraPosition is outside CombatArea
    //    if (!combatAreaBounds.Contains(combatZone.CombatCameraPosition.transform.position))
    //    {
    //        EditorGUILayout.HelpBox("CameraPosition is outside the CombatArea!", MessageType.Warning);
    //    }
    //}

}
