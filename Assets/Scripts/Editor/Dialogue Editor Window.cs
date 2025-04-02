using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class DialogueEditorWindow : EditorWindow
{
    private Dialogue dialogue;
    private string dialogueName = "NewDialogue";
    private List<DialogueLine> lines = new List<DialogueLine>();
    private Vector2 scrollPosition;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow()
    {
        GetWindow<DialogueEditorWindow>("Dialogue Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialogue Editor", EditorStyles.boldLabel);

        // Dialogue Name Input
        GUILayout.Label("Dialogue Name:");
        dialogueName = EditorGUILayout.TextField(dialogueName);

        // Load Existing Dialogue
        GUILayout.Space(5);
        dialogue = (Dialogue)EditorGUILayout.ObjectField("Load Existing Dialogue:", dialogue, typeof(Dialogue), false);

        if (dialogue != null)
        {
            dialogueName = dialogue.name;
            lines = dialogue.lines;
        }

        GUILayout.Space(10);
        GUILayout.Label("Dialogue Lines:", EditorStyles.boldLabel);

        // Scrollable List of Dialogue Lines
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
        for (int i = 0; i < lines.Count; i++)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            lines[i].isPlayerSpeaking = EditorGUILayout.Toggle("Is Player Speaking?", lines[i].isPlayerSpeaking);

            if (!lines[i].isPlayerSpeaking)
            {
                lines[i].speakerName = EditorGUILayout.TextField("Speaker Name:", lines[i].speakerName);
                lines[i].speakerImage = (Sprite)EditorGUILayout.ObjectField("Speaker Image:", lines[i].speakerImage, typeof(Sprite), false);
            }

            lines[i].dialogueText = EditorGUILayout.TextArea(lines[i].dialogueText, GUILayout.Height(50));
            lines[i].revealByLetter = EditorGUILayout.Toggle("Reveal by Letter?", lines[i].revealByLetter);

            if (lines[i].isPlayerSpeaking)
            {
                lines[i].textSpeed = EditorGUILayout.Slider("Text Speed:", lines[i].textSpeed, 0.01f, 0.1f);
            }

            lines[i].timeBetweenLines = EditorGUILayout.Slider("Time Between Lines:", lines[i].timeBetweenLines,1f,10f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move Up") && i > 0)
            {
                (lines[i], lines[i - 1]) = (lines[i - 1], lines[i]);
            }
            if (GUILayout.Button("Move Down") && i < lines.Count - 1)
            {
                (lines[i], lines[i + 1]) = (lines[i + 1], lines[i]);
            }
            if (GUILayout.Button("Delete"))
            {
                lines.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();

        // Buttons to Add Lines
        if (GUILayout.Button("+ Add Speaker Line"))
        {
            lines.Add(new DialogueLine { isPlayerSpeaking = false });
        }
        if (GUILayout.Button("+ Add Player Line"))
        {
            lines.Add(new DialogueLine { isPlayerSpeaking = true });
        }

        GUILayout.Space(10);

        // Clear All Button
        if (GUILayout.Button("Clear All"))
        {
            lines.Clear();
        }

        // Save Dialogue Button
        if (GUILayout.Button("Save Dialogue"))
        {
            SaveDialogue();
        }

        GUILayout.Space(20);
        GUILayout.Label("Dialogue Preview:", EditorStyles.boldLabel);
        string previewText = "";
        foreach (var line in lines)
        {
            string speaker = line.isPlayerSpeaking ? "Player" : line.speakerName;
            previewText += speaker + ": " + line.dialogueText + "\n";
        }
        EditorGUILayout.TextArea(previewText, GUILayout.Height(150));
    }

    private void SaveDialogue()
    {
        if (string.IsNullOrWhiteSpace(dialogueName))
        {
            EditorUtility.DisplayDialog("Error", "Dialogue Name cannot be empty!", "OK");
            return;
        }

        string path = $"Assets/Resources/Scriptables/Dialogues/Dialogue_{dialogueName}.asset";
        Dialogue newDialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(path);

        if (newDialogue == null)
        {
            newDialogue = ScriptableObject.CreateInstance<Dialogue>();
            AssetDatabase.CreateAsset(newDialogue, path);
        }

        newDialogue.lines = new List<DialogueLine>(lines);
        EditorUtility.SetDirty(newDialogue);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", "Dialogue saved successfully!", "OK");
    }
}
