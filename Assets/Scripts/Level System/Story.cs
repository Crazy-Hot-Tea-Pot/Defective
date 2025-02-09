using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class Story : ScriptableObject
{
    public string storyName;
    public StoryPathType pathType;
    public Difficulty difficulty;

    // Levels are now stored directly inside the Story
    public List<LevelDefinition> levels = new List<LevelDefinition>();
}
