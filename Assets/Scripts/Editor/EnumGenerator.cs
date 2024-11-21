using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EnumGenerator : Editor
{
    [MenuItem("Tools/Generate Levels Enum")]
    public static void GenerateSceneEnum()
    {
        // Define the output path for the generated script
        string path = "Assets/Scripts/Generated Scripts/Levels.cs";

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // Start building the enum script
        StringBuilder enumBuilder = new StringBuilder();
        enumBuilder.AppendLine("// Auto-generated Levels Enum");
        enumBuilder.AppendLine("// Generated by levelsEnumGenerator");
        enumBuilder.AppendLine("[System.Serializable]");
        enumBuilder.AppendLine("public enum Levels");
        enumBuilder.AppendLine("{");

        // Iterate through all scenes in the build settings
        var scenes = EditorBuildSettings.scenes;
        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                enumBuilder.AppendLine($"    {sceneName},");
            }
        }

        enumBuilder.AppendLine("}");

        // Write the generated script to the file
        File.WriteAllText(path, enumBuilder.ToString());

        // Refresh the asset database to make the script appear in Unity
        AssetDatabase.Refresh();

        Debug.Log("Levels enum script generated at: " + path);
    }
}
