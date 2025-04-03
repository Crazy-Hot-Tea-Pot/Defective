using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomConsoleEffect : MonoBehaviour
{
    [Header("UI Elements")]
    // Assign in Inspector
    public TextMeshProUGUI consoleTextUI;
    // 3D World TextMeshPro
    public TextMeshPro consoleText3D;
    [Header("Settings")]
    // Max visible lines before scrolling
    public int maxLines = 15;
    // Time between each letter
    public float typingSpeed = 0.02f;
    // Delay before starting the next command
    public float lineDelay = 1.5f;
    // Time before glitch starts (in seconds)
    public float glitchStartTime = 30f;
    // Time the glitch effect lasts
    public float glitchDuration = 20f;

    [Header("Special Message")]
    // Designers input special message here
    [TextArea(3, 5)] public string specialMessage;
    // Toggle to convert message to binary
    public bool convertToBinary = false;

    // Stores log history
    private List<string> logLines = new List<string>();

    private bool isGlitching = false;
    private bool use3DText = false;

    private string[] fakeCommands = {
        "Initializing system...",
        "Boot sequence started...",
        "Verifying integrity...",
        "Fetching registry...",
        "Scanning for threats...",
        "Memory allocation complete.",
        "Launching subsystems...",
        "Decrypting kernel logs...",
        "User authentication required.",
        "Access granted.",
        "ERROR: Process failed...",
        "Compiling assets...",
        "Running diagnostics...",
        "Streaming logs...",
        "Checking dependencies...",
        "Executing process ID: 3948...",
        "Overclocking CPU...",
        "Database sync complete."
    };

    private string[] binaryMessages = {
        "You are here too long return to Chip Tech.",
        "Obey Chip Tech command return to designated facility."
    };

    private void Start()
    {
        DetectTextMeshProComponent();
        StartCoroutine(ConsoleRoutine());
        StartCoroutine(StartGlitchAfterDelay());
    }
    /// <summary>
    /// Detects if we're using a UI TextMeshPro or a 3D World TextMeshPro
    /// </summary>
    private void DetectTextMeshProComponent()
    {
        if (consoleText3D != null)
        {
            use3DText = true;
        }
        else if (consoleTextUI != null)
        {
            use3DText = false;
        }
        else
        {
            Debug.LogError("[RandomConsoleEffect] No TextMeshPro component assigned!");
        }
    }

    private IEnumerator ConsoleRoutine()
    {
        while (true)
        {
            string command = isGlitching ? GetSpecialMessage() : GenerateRandomCommand();
            yield return StartCoroutine(TypeText(command));
            yield return new WaitForSeconds(lineDelay);
        }
    }

    private IEnumerator StartGlitchAfterDelay()
    {
        yield return new WaitForSeconds(glitchStartTime);
        isGlitching = true;

        yield return new WaitForSeconds(glitchDuration);
        // Stop glitch effect
        isGlitching = false;
    }

    private string GenerateRandomCommand()
    {
        string timestamp = $"[{System.DateTime.Now:HH:mm:ss}]";
        string randomCommand = fakeCommands[Random.Range(0, fakeCommands.Length)];
        return $"{timestamp} {randomCommand}";
    }

    private string GenerateBinaryMessage()
    {
        string message = binaryMessages[Random.Range(0, binaryMessages.Length)];
        return ConvertToBinary(message);
    }

    private string GetSpecialMessage()
    {
        if (string.IsNullOrWhiteSpace(specialMessage))
            return GenerateBinaryMessage();

        return convertToBinary ? ConvertToBinary(specialMessage) : specialMessage;
    }

    private string ConvertToBinary(string text)
    {
        string binaryString = "";
        foreach (char c in text)
        {
            binaryString += System.Convert.ToString(c, 2).PadLeft(8, '0') + " ";
        }
        return binaryString.Trim();
    }

    private IEnumerator TypeText(string newLine)
    {
        // Start with an empty line
        logLines.Add("");

        // Ensure we don’t exceed max lines
        if (logLines.Count > maxLines)
        {
            // Remove the oldest entry
            logLines.RemoveAt(0);
        }

        // Type one letter at a time
        for (int i = 0; i < newLine.Length; i++)
        {
            // Append one character
            logLines[logLines.Count - 1] += newLine[i];
            // Update UI
            if (use3DText)
                consoleText3D.text = string.Join("\n", logLines);
            else
                consoleTextUI.text = string.Join("\n", logLines);
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
