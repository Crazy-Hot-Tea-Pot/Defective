using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Enemy), true)]
public class EnemyEditor : Editor
{
    private bool debugMode = false; // Flag to toggle debug mode

    public override void OnInspectorGUI()
    {
        // Get a reference to the Enemy script
        Enemy enemy = (Enemy)target;

        // Header
        EditorGUILayout.LabelField("Enemy Inspector", EditorStyles.boldLabel);

        // Show editable fields for the key properties
        EditorGUILayout.LabelField("Enemy Information", EditorStyles.boldLabel);
        string newName = EditorGUILayout.TextField("Enemy Name", enemy.EnemyName);
        if (newName != enemy.EnemyName)
        {
            enemy.SetEnemyName(newName);
        }

        enemy.maxHP = EditorGUILayout.IntField("Max HP", enemy.maxHP);
        EditorGUILayout.LabelField("Current HP Amount", enemy.CurrentHP.ToString());
        // Health Bar
        DrawHealthBar(enemy);

        EditorGUILayout.LabelField("Current Shield Amount",enemy.Shield.ToString());

        // Display fields for Drops
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Drops", EditorStyles.boldLabel);
        SerializedProperty chipsProperty = serializedObject.FindProperty("DroppedChips");
        EditorGUILayout.PropertyField(chipsProperty, new GUIContent("Dropped Chips"), true);
        SerializedProperty itemsProperty = serializedObject.FindProperty("DroppedItems");
        EditorGUILayout.PropertyField(itemsProperty, new GUIContent("Dropped Items"), true);
        enemy.DroppedScrap = EditorGUILayout.IntSlider("Dropped Scrap", enemy.DroppedScrap, 0, 100);

        // Debug Mode Button
        EditorGUILayout.Space();
        debugMode = EditorGUILayout.Toggle("Debug Mode", debugMode);

        // Show hidden properties if Debug Mode is enabled
        if (debugMode)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug Information", EditorStyles.boldLabel);
            DrawDefaultInspector(); // Draw all other fields normally
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();

        // Save changes to the object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemy);
        }
    }
    /// <summary>
    /// Draw health bar in inspector
    /// </summary>
    /// <param name="enemy"></param>
    private void DrawHealthBar(Enemy enemy)
    {
        // Calculate the fill percentage for the health bar
        float healthPercentage = (float)enemy.CurrentHP / enemy.maxHP;

        // Display health bar background
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.DrawRect(rect, Color.grey); // Background color (grey)

        // Create a rect for the filled portion
        Rect filledRect = new Rect(rect.x, rect.y, rect.width * healthPercentage, rect.height);
        EditorGUI.DrawRect(filledRect, Color.green); // Filled portion (green)

        // Overlay the health text
        EditorGUI.LabelField(rect, $"{enemy.CurrentHP} / {enemy.maxHP}", new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState { textColor = Color.white },
            fontStyle = FontStyle.Bold
        });

        // Add some spacing after the bar
        GUILayout.Space(5);
    }
}
