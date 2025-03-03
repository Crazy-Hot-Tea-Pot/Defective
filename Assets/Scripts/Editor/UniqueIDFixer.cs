using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class UniqueIDFixer : EditorWindow
{
    [MenuItem("Tools/Fix Unique Level IDs")]
    public static void FixUniqueIDs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Story");
        List<Story> stories = new List<Story>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Story story = AssetDatabase.LoadAssetAtPath<Story>(path);
            if (story != null)
            {
                stories.Add(story);
            }
        }

        int updatedCount = 0;
        HashSet<string> usedIDs = new HashSet<string>();

        foreach (Story story in stories)
        {
            foreach (LevelDefinition level in story.levels)
            {
                AssignUniqueIDsRecursively(level, usedIDs, ref updatedCount);
            }
            EditorUtility.SetDirty(story);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Fixed {updatedCount} levels with missing Unique IDs.");
    }

    private static void AssignUniqueIDsRecursively(LevelDefinition level, HashSet<string> usedIDs, ref int updatedCount)
    {
        if (level == null) return;

        if (string.IsNullOrEmpty(level.uniqueLevelID) || usedIDs.Contains(level.uniqueLevelID))
        {
            level.uniqueLevelID = Guid.NewGuid().ToString();
            usedIDs.Add(level.uniqueLevelID);
            updatedCount++;
        }

        foreach (LevelDefinition nextLevel in level.NextLevels)
        {
            AssignUniqueIDsRecursively(nextLevel, usedIDs, ref updatedCount);
        }
    }
}
