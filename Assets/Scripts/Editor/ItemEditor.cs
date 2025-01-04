using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    private SerializedProperty valueIncreaseBy;
    private SerializedProperty energyCostDecreaseBy;
    private SerializedProperty scrapValue;

    private void OnEnable()
    {
        // Cache the serialized properties
        valueIncreaseBy = serializedObject.FindProperty("valueIncreaseBy");
        energyCostDecreaseBy = serializedObject.FindProperty("energyCostDecreaseBy");
        scrapValue = serializedObject.FindProperty("scrapValue");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Draw default inspector for other fields
        DrawPropertiesExcluding(serializedObject, "valueIncreaseBy", "energyCostDecreaseBy", "scrapValue");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tier values to increase by", EditorStyles.boldLabel);

        // Display the Tier Values as the enum names
        DrawTierList("Damage/Shield Increase", valueIncreaseBy);

        EditorGUILayout.LabelField("Tier values to Decrease by", EditorStyles.boldLabel);
        DrawTierList("Energy Cost Decrease", energyCostDecreaseBy);

        EditorGUILayout.LabelField("Scale Value for each Teir", EditorStyles.boldLabel);
        DrawTierList("Scrap Value", scrapValue);

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTierList(string label, SerializedProperty list)
    {
        EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

        string[] tierNames = System.Enum.GetNames(typeof(Item.Teir));

        for (int i = 0; i < tierNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tierNames[i], GUILayout.Width(100));
            list.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(list.GetArrayElementAtIndex(i).intValue);
            EditorGUILayout.EndHorizontal();
        }
    }
}
