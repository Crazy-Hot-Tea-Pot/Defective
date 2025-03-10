using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    // NPC name
    public string speakerName;

    // NPC image
    public Sprite speakerImage;

    // Text content
    [TextArea(3, 5)]
    public string dialogueText;

    // If true, uses PlayerSpeak()
    public bool isPlayerSpeaking;

    // Letter-by-letter (default) or Word-by-word
    public bool revealByLetter = true;

    // Only for Player dialogue
    public float textSpeed = 0.05f;

    // Time before the next line appears automatically
    public float timeBetweenLines = 1.0f;
}