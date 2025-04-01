using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Generate Enums.
/// First time im doing this so cross fingers.
/// </summary>
public class SoundEnumGenerator
{
#if UNITY_EDITOR
    //[MenuItem("Tools/Generate Sound Enums")]
    public static void GenerateEnums()
    {

        // Paths to look for sound clips
        string bgFolder = "Assets/Resources/Sounds/BG";
        string fxFolder = "Assets/Resources/Sounds/FX";

        StringBuilder enumBuilder = new StringBuilder();

        // Generate BgSound enum
        enumBuilder.AppendLine("public enum BgSound");
        enumBuilder.AppendLine("{");
        enumBuilder.AppendLine("    None,");
        AddFilesToEnum(bgFolder, enumBuilder);
        enumBuilder.AppendLine("}");
        enumBuilder.AppendLine();

        // Generate SoundFX enum
        enumBuilder.AppendLine("public enum SoundFX");
        enumBuilder.AppendLine("{");
        enumBuilder.AppendLine("    None,");
        AddFilesToEnum(fxFolder, enumBuilder);
        enumBuilder.AppendLine("}");

        // Write the generated enums to a C# file in your project
        string enumFilePath = "Assets/Scripts/Generated Scripts/SoundEnums.cs";
        File.WriteAllText(enumFilePath, enumBuilder.ToString());

        AssetDatabase.Refresh(); // Refresh to update the new file in Unity Editor
        Debug.Log("Sound enums generated successfully.");
    }

    private static void AddFilesToEnum(string folderPath, StringBuilder enumBuilder)
    {
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath, "*.wav"); // Adjust file extension as needed

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string validEnumName = fileName.Replace(" ", "_"); // Ensure valid enum naming
                enumBuilder.AppendLine($"    {validEnumName},");
            }
        }
        else
        {
            Debug.LogWarning("Folder not found: " + folderPath);
        }
    }
#endif
}
