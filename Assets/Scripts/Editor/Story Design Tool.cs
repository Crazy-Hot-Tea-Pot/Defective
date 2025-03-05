using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static GameEnums;
using static EnemyManager;
using System;

public class StoryDesignTool : EditorWindow
{
    private string storyName = "New Story";
    private StoryPathType pathType = StoryPathType.Linear;
    private Difficulty difficulty = Difficulty.Normal;
    private List<LevelDefinition> levels = new List<LevelDefinition>();
    private Vector2 scrollPos;

    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private string[] enemyNames;

    [MenuItem("Tools/Story Designer")]
    public static void OpenWindow()
    {
        GetWindow<StoryDesignTool>("Story Design Tool");
    }

    private void OnEnable()
    {
        LoadEnemyPrefabs();
    }

    private void OnGUI()
    {
        GUILayout.Label("Story Designer", EditorStyles.boldLabel);

        // Story Details Section
        EditorGUILayout.Space();
        GUILayout.Label("Story Details", EditorStyles.boldLabel);
        storyName = EditorGUILayout.TextField("Story Name:", storyName);
        pathType = (StoryPathType)EditorGUILayout.EnumPopup("Path Type:", pathType);
        difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty:", difficulty);

        // Levels Section
        EditorGUILayout.Space();
        GUILayout.Label("Levels", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(500));
        try
        {
            for (int i = 0; i < levels.Count; i++)
            {
                GUILayout.BeginVertical("box");
                try
                {
                    DrawLevel(levels[i], i);
                }
                finally
                {
                    GUILayout.EndVertical();
                }
            }
        }
        finally
        {
            EditorGUILayout.EndScrollView();
        }

        if (levels.Count == 0)
        {
            if (GUILayout.Button("Add First Level"))
            {
                levels.Add(new LevelDefinition());
            }
        }

        // Save Story EquipButton
        EditorGUILayout.Space();
        if (GUILayout.Button("Save Story"))
        {
            SaveStory();
        }

        // Graph Visualization EquipButton
        if (GUILayout.Button("Visualize Path Graph"))
        {
            ShowPathGraph();
        }
    }
    private void DrawLevel(LevelDefinition level, int depth, string parentLabel = "First Level")
    {
        GUILayout.BeginVertical("box");

        // Show clear parent-child connection
        string levelLabel = depth == 0 ? $"{level.levelID}" : $"{parentLabel} > {level.levelID}";
        GUILayout.Label(levelLabel, EditorStyles.boldLabel);

        level.levelID = (Levels)EditorGUILayout.EnumPopup("Level ID:", level.levelID);
        level.terminalSpawnChance = EditorGUILayout.IntSlider("Terminal Spawn Chance", level.terminalSpawnChance, 0, 100);

        // Enemy Spawns
        GUILayout.Label("Enemy Spawns", EditorStyles.boldLabel);
        for (int j = 0; j < level.enemySpawns.Count; j++)
        {
            GUILayout.BeginHorizontal();
            try
            {
                level.enemySpawns[j].enemyType = (TypeOfEnemies)EditorGUILayout.EnumPopup("Enemy Type:", level.enemySpawns[j].enemyType);
                level.enemySpawns[j].enemyName = EditorGUILayout.TextField("Enemy Name:", level.enemySpawns[j].enemyName);

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    level.enemySpawns.RemoveAt(j);
                    break;
                }
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Add Enemy"))
        {
            level.enemySpawns.Add(new EnemySpawn());
        }

        // Next Levels Section
        GUILayout.Label("Next Levels", EditorStyles.boldLabel);

        for (int i = 0; i < level.NextLevels.Count; i++)
        {
            GUILayout.BeginVertical("box");

            GUILayout.Label($"Next Level {i + 1}: {level.NextLevels[i].levelID}", EditorStyles.boldLabel);

            GUIContent Tip = new GUIContent("Quest Condition (Optiona):", "The quest condition is a quest in the preceeding level that needs to be completed.");

            level.NextLevels[i].questCondition = (Quest)EditorGUILayout.ObjectField(Tip, level.NextLevels[i].questCondition, typeof(Quest), false);

            // Draw the next level
            DrawLevel(level.NextLevels[i], depth + 1, $"{level.levelID}");

            GUILayout.EndVertical();
        }


        // Add Next Level EquipButton (Respecting Path Rules)
        if ((pathType == StoryPathType.Linear && level.NextLevels.Count < 1) ||
            (pathType == StoryPathType.Branching && level.NextLevels.Count < 2))
        {
            if (GUILayout.Button("+ Add Next Level", GUILayout.Width(200)))
            {
                level.NextLevels.Add(new LevelDefinition());
            }
        }

        if (GUILayout.Button("Remove Level", GUILayout.Width(200)))
        {
            RemoveLevel(level);
        }

        GUILayout.EndVertical();
    }

    private void RemoveLevel(LevelDefinition level)
    {
        if (levels.Contains(level))
        {
            levels.Remove(level);
        }
        else
        {
            foreach(var parentLevel in levels)
            {
                if(parentLevel.nextLevels.Contains(level))
                {
                    parentLevel.NextLevels.Remove(level);
                    break;
                }
            }
        }
    }

    private void SaveStory()
    {
        if (string.IsNullOrWhiteSpace(storyName))
        {
            EditorUtility.DisplayDialog("Error", "Story Name cannot be empty!", "OK");
            return;
        }

        if (levels.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "A story must have at least one level!", "OK");
            return;
        }

        Story newStory = CreateInstance<Story>();
        newStory.storyName = storyName;
        newStory.pathType = pathType;
        newStory.difficulty = difficulty;
        newStory.levels = new List<LevelDefinition>(levels);

        string folderPath = "Assets/Resources/Scriptables/Stories";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources/Scriptables", "Stories");
        }

        string assetPath = $"{folderPath}/{storyName}.asset";
        AssetDatabase.CreateAsset(newStory, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Success", "Story saved successfully!", "OK");

        ResetFields();
    }

    private void ResetFields()
    {
        storyName = "New Story";
        pathType = StoryPathType.Linear;
        difficulty = Difficulty.Normal;
        levels.Clear();
    }

    private void ShowPathGraph()
    {
        PathGraphWindow.OpenWindow(levels, pathType);
    }

    private void LoadEnemyPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Enemies" });
        enemyPrefabs.Clear();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) enemyPrefabs.Add(prefab);
        }
        enemyNames = enemyPrefabs.ConvertAll(p => p.name).ToArray();
    }
}
