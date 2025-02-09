using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static GameEnums;

public class PathGraphWindow : EditorWindow
{
    private List<LevelDefinition> levels;
    private StoryPathType pathType;
    private Dictionary<LevelDefinition, Vector2> nodePositions = new Dictionary<LevelDefinition, Vector2>();

    public static void OpenWindow(List<LevelDefinition> storyLevels, StoryPathType type)
    {
        PathGraphWindow window = GetWindow<PathGraphWindow>("Story Graph");
        window.levels = storyLevels;
        window.pathType = type;
        window.InitializeGraph();
        window.Show();
    }

    private void InitializeGraph()
    {
        nodePositions.Clear();
        Vector2 startPos = new Vector2(500, 50);

        if (pathType == StoryPathType.Linear)
        {
            ArrangeLinearPath(levels, startPos, 0);
        }
        else
        {
            ArrangeBranchingPath(levels[0], startPos, 0, 400); // Start at the center
        }
    }

    private void ArrangeLinearPath(List<LevelDefinition> levels, Vector2 position, int depth)
    {
        if (levels == null) return;

        for (int i = 0; i < levels.Count; i++)
        {
            LevelDefinition level = levels[i];

            if (!nodePositions.ContainsKey(level))
            {
                nodePositions[level] = position;
            }

            // Move the next level straight down
            if (level.NextLevels != null && level.NextLevels.Count > 0)
            {
                Vector2 nextPos = position + new Vector2(0, 150); // Space below for next level
                ArrangeLinearPath(level.NextLevels, nextPos, depth + 1);
            }

            position += new Vector2(0, 150);
        }
    }


    private void ArrangeBranchingPath(LevelDefinition level, Vector2 position, int depth, float horizontalSpacing)
    {
        if (level == null) return;

        if (!nodePositions.ContainsKey(level))
        {
            nodePositions[level] = position;
        }

        // Ensure max 2 next levels in branching path
        if (level.NextLevels.Count > 2)
        {
            level.NextLevels.RemoveRange(2, level.NextLevels.Count - 2);
        }

        if (level.NextLevels.Count > 0)
        {
            float childSpacing = horizontalSpacing / 2;
            Vector2 leftPos = position + new Vector2(-childSpacing, 150); // Left Child
            Vector2 rightPos = position + new Vector2(childSpacing, 150); // Right Child

            if (level.NextLevels.Count > 0)
                ArrangeBranchingPath(level.NextLevels[0], leftPos, depth + 1, childSpacing);

            if (level.NextLevels.Count > 1)
                ArrangeBranchingPath(level.NextLevels[1], rightPos, depth + 1, childSpacing);
        }
    }


    private void OnGUI()
    {
        if (levels == null || levels.Count == 0)
        {
            EditorGUILayout.LabelField("No levels found. Create a story first!");
            return;
        }

        BeginWindows();

        foreach (var level in nodePositions.Keys)
        {
            Rect rect = new Rect(nodePositions[level], new Vector2(150, 60));
            GUI.Box(rect, level.levelID.ToString());
        }

        EndWindows();

        Handles.BeginGUI();
        Handles.color = Color.white;

        foreach (var level in nodePositions.Keys)
        {
            if (level.NextLevels == null || level.NextLevels.Count == 0) continue;

            foreach (var nextLevel in level.NextLevels)
            {
                if (nodePositions.ContainsKey(level) && nodePositions.ContainsKey(nextLevel))
                {
                    Vector2 start = nodePositions[level] + new Vector2(75, 60); // Bottom center
                    Vector2 end = nodePositions[nextLevel] + new Vector2(75, 0); // Top center of next node

                    Handles.DrawLine(start, end);
                }
            }
        }
        Handles.EndGUI();

    }
}
