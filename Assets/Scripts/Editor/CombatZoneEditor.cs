using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static CombatZone;

[CustomEditor(typeof(CombatZone))]
public class CombatZoneEditor : Editor
{
    private bool showColorSettings = true;
    private bool showPositionSettings = true;
    private bool showEnemySettings = true;

    public override void OnInspectorGUI()
    {
        CombatZone combatZone = (CombatZone)target;
        SerializedObject serializedObject = new SerializedObject(target);

        // Display CombatZoneSet at the top
        EditorGUILayout.LabelField("Combat Zone Status", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CombatZoneSet"), new GUIContent("Combat Zone Set"));

        serializedObject.Update();

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

        showPositionSettings = EditorGUILayout.Foldout(showPositionSettings, "Position Settings");
        if (showPositionSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaSize"), new GUIContent("Combat Area Size"));
            SerializedProperty playerPositionOffset = serializedObject.FindProperty("playerPositionXZOffset");

            Vector2 offset = playerPositionOffset.vector2Value;
            EditorGUILayout.LabelField("Player Position Offset (X and Z)", EditorStyles.boldLabel);
            offset.x = EditorGUILayout.FloatField("X Offset", offset.x);
            offset.y = EditorGUILayout.FloatField("Z Offset", offset.y);
            playerPositionOffset.vector2Value = offset;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraPositionOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraTargetPositionOffset"));
        }

        GUILayout.Space(10);

        // Enemy Settings
        showEnemySettings = EditorGUILayout.Foldout(showEnemySettings, "Enemy Settings");
        if (showEnemySettings)
        {
            EditorGUILayout.LabelField("Enemies in Combat Zone", EditorStyles.boldLabel);

            // Display existing enemies
            for (int i = 0; i < combatZone.EnemiesInZone.Count; i++)
            {
                var enemyData = combatZone.EnemiesInZone[i];

                if (enemyData.enemyObject == null)
                {
                    combatZone.EnemiesInZone.RemoveAt(i);
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                enemyData.enemyObject = (GameObject)EditorGUILayout.ObjectField($"Enemy {i + 1}", enemyData.enemyObject, typeof(GameObject), true);
                enemyData.enemyType = (EnemyManager.EnemyType)EditorGUILayout.EnumPopup(enemyData.enemyType);

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    DestroyImmediate(enemyData.enemyObject);
                    combatZone.EnemiesInZone.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                // Convert world position to local offset
                Vector3 enemyOffset = combatZone.transform.InverseTransformPoint(enemyData.enemyObject.transform.position);

                // Display as offset, same as Player Position
                enemyOffset = EditorGUILayout.Vector3Field("Enemy Offset", enemyOffset);

                // Apply the offset back to the enemy's position
                enemyData.enemyObject.transform.position = combatZone.transform.TransformPoint(enemyOffset);

                enemyData.enemyObject.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", enemyData.enemyObject.transform.rotation.eulerAngles));

                GUILayout.Space(10);
            }


            if (GUILayout.Button("Add Enemy"))
            {
                ShowAddEnemyMenu(combatZone);
            }
        }

        GUILayout.Space(10);

        if (combatZone.CombatZoneSet)
        {
            if (GUILayout.Button("Edit Combat Zone"))
            {
                combatZone.EditCombatZone();
            }
        }
        else
        {
            if (combatZone.EnemiesInZone.Count > 0)
            {
                if (GUILayout.Button("Save Combat Zone"))
                {
                    combatZone.SaveCombatZone();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("At least one enemy must be added before saving.", MessageType.Error);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowAddEnemyMenu(CombatZone combatZone)
    {
        GenericMenu menu = new GenericMenu();
        foreach (EnemyManager.EnemyType type in System.Enum.GetValues(typeof(EnemyManager.EnemyType)))
        {
            menu.AddItem(new GUIContent(type.ToString()), false, () => AddEnemyToCombatZone(combatZone, type));
        }
        menu.ShowAsContext();
    }

    private void AddEnemyToCombatZone(CombatZone combatZone, EnemyManager.EnemyType type)
    {
        GameObject enemyPlaceholderPrefab = combatZone.EnemyPlaceholder;

        if (enemyPlaceholderPrefab == null)
        {
            Debug.LogError("Enemy placeholder prefab not found!");
            return;
        }

        GameObject newEnemy = PrefabUtility.InstantiatePrefab(enemyPlaceholderPrefab) as GameObject;
        newEnemy.transform.position = combatZone.transform.position;
        newEnemy.transform.SetParent(combatZone.transform);

        combatZone.EnemiesInZone.Add(new EnemyPlacementData(newEnemy, type));

        Debug.Log($"Added {type} placeholder to the combat zone.");
    }
}
