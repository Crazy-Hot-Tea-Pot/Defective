﻿using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;

public class GenerateFXPLayerScript
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate FXPlayerToLinkAnimation Script")]
    public static void GenerateFXPlayerScript()
    {
        string enumFilePath = "Assets/Scripts/Generated Scripts/SoundEnums.cs";
        string outputScriptPath = "Assets/Scripts/Generated Scripts/FXPlayerToLinkAnimation.cs";

        // Read the SoundFX enum
        string[] enumLines = File.ReadAllLines(enumFilePath);
        var soundFXNames = ParseEnumValues(enumLines, "SoundFX");

        if (soundFXNames == null || soundFXNames.Count == 0)
        {
            Debug.LogError("SoundFX enum not found or empty.");
            return;
        }

        Debug.Log($"Parsed SoundFX names: {string.Join(", ", soundFXNames)}");

        // Read existing script (if available)
        StringBuilder outputBuilder = new StringBuilder();
        bool scriptExists = File.Exists(outputScriptPath);
        string[] existingScript = scriptExists ? File.ReadAllLines(outputScriptPath) : null;

        var preservedContent = ParseExistingContentWithCustomCode(existingScript);

        // Start writing the new script
        outputBuilder.AppendLine("using UnityEngine;");
        outputBuilder.AppendLine();
        outputBuilder.AppendLine("public class FXPlayerToLinkAnimation : MonoBehaviour");
        outputBuilder.AppendLine("{");

        // Add preserved custom methods or content
        foreach (var line in preservedContent["custom"])
        {
            outputBuilder.AppendLine(line);
        }

        // Add methods for each SoundFX
        foreach (string sound in soundFXNames)
        {
            if (preservedContent["methods"].Exists(m => m.Contains($"PlaySound_{sound}")))
            {
                // Preserve existing PlaySound_<SoundName> methods
                outputBuilder.AppendLine(preservedContent["methods"].Find(m => m.Contains($"PlaySound_{sound}")));
            }
            else
            {
                // Add a new method
                outputBuilder.AppendLine($"    public void PlaySound_{sound}()");
                outputBuilder.AppendLine("    {");
                outputBuilder.AppendLine($"        SoundManager.PlayFXSound(SoundFX.{sound});");
                outputBuilder.AppendLine("    }");
            }
        }

        outputBuilder.AppendLine("}");

        // Write the script
        File.WriteAllText(outputScriptPath, outputBuilder.ToString());
        AssetDatabase.Refresh();

        Debug.Log("FXPlayerToLinkAnimation generated successfully.");
    }

    private static HashSet<string> ParseEnumValues(string[] lines, string enumName)
    {
        var values = new HashSet<string>();
        bool inEnum = false;

        foreach (string line in lines)
        {
            if (line.Contains($"enum {enumName}"))
            {
                inEnum = true;
                continue;
            }

            if (inEnum)
            {
                if (line.Contains("}")) break; // End of enum
                string value = line.Trim().Split(',')[0].Trim(); // Get the value before the comma
                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^[a-zA-Z_]\w*$"))
                {
                    values.Add(value);
                }
            }
        }

        return values;
    }

    private static Dictionary<string, List<string>> ParseExistingContentWithCustomCode(string[] scriptLines)
    {
        var preservedContent = new Dictionary<string, List<string>> {
            { "custom", new List<string>() },
            { "methods", new List<string>() }
        };

        if (scriptLines == null) return preservedContent;

        StringBuilder currentContent = new StringBuilder();
        bool insideCustomCode = false;

        foreach (string line in scriptLines)
        {
            // Detect custom methods or sections
            if (line.Contains("// START CUSTOM"))
            {
                insideCustomCode = true;
                currentContent.AppendLine(line);
                continue;
            }

            if (line.Contains("// END CUSTOM"))
            {
                insideCustomCode = false;
                currentContent.AppendLine(line);
                preservedContent["custom"].Add(currentContent.ToString());
                currentContent.Clear();
                continue;
            }

            if (insideCustomCode)
            {
                currentContent.AppendLine(line);
                continue;
            }

            // Detect PlaySound methods
            if (line.TrimStart().StartsWith("public void PlaySound_"))
            {
                preservedContent["methods"].Add(line.Trim());
            }
        }

        return preservedContent;
    }
#endif
}