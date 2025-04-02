using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDefinition
{
    //Unique Level ID
    public string uniqueLevelID;
    public Levels levelID;
    [Range(0,100)]
    public int terminalSpawnChance;

    // Enemies now exist directly inside the Story object
    public List<EnemySpawn> enemySpawns = new List<EnemySpawn>();

    // Next levels (for branching and random paths)
    [SerializeReference]
    public List<LevelDefinition> nextLevels = new List<LevelDefinition>();

    public bool isCompleted = false;

    public List<LevelDefinition> NextLevels
    {
        get => nextLevels;
        set
        {
            if (value.Count > 2) // Prevent more than 2 branches
            {
                Debug.LogWarning($"Level {levelID} cannot have more than 2 next levels.");
                return;
            }
            nextLevels = value;
        }
    }

    public Quest questCondition;
    /// <summary>
    /// Enqure a unique ID is generated for each level
    /// </summary>
    public LevelDefinition()
    {
        uniqueLevelID = Guid.NewGuid().ToString();
    }
}
