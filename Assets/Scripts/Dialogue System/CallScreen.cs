using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CallScreen : MonoBehaviour
{
    [Header("UI References")]
    // TextMeshPro for dialogue display
    public TextMeshPro dialogueText;
    // TextMeshPro for speaker name
    public TextMeshPro speakerNameText;
    // Quad for displaying the speaker image
    public Renderer speakerImageQuad;

    [Header("Settings")]
    // Maximum number of lines before old ones are removed
    public int maxLinesDisplayed = 5;
    // Stores active lines
    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isRevealingText = false;
    private Coroutine revealCoroutine;

    /// <summary>
    /// Sets Speaker name
    /// </summary>
    public string SpeakerName
    {
        get
        {
            return speakerNameText.text;
        }
        set
        {
            speakerNameText.SetText(value);
        }
    }

    public Texture SpeakerImage
    {
        set
        {
            if (value != null)
            {
                Material newMaterial = new Material(speakerImageQuad.material);
                newMaterial.mainTexture = value;
                speakerImageQuad.material = newMaterial;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Starts revealing text dynamically.
    /// </summary>
    public void NextText(string text, bool revealByLetter, float textSpeed, float timeBetweenLines)
    {
        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);
        revealCoroutine = StartCoroutine(RevealText(text, revealByLetter, textSpeed, timeBetweenLines));
    }
    /// <summary>
    /// Clears all dialogue.
    /// </summary>
    public void ClearDialogue()
    {
        dialogueQueue.Clear();
        dialogueText.text = "";
        speakerNameText.text = "";
    }

    /// <summary>
    /// Reveals text dynamically, then auto-moves to the next line.
    /// </summary>
    private IEnumerator RevealText(string fullText, bool revealByLetter, float revealSpeed, float timeBetweenLines)
    {
        isRevealingText = true;
        string displayedText = "";

        if (revealByLetter)
        {
            foreach (char letter in fullText)
            {
                displayedText += letter;
                UpdateText(displayedText);
                yield return new WaitForSeconds(revealSpeed);
            }
        }
        else
        {
            string[] words = fullText.Split(' ');
            foreach (string word in words)
            {
                displayedText += word + " ";
                UpdateText(displayedText);
                yield return new WaitForSeconds(revealSpeed * 2);
            }
        }

        isRevealingText = false;
        yield return new WaitForSeconds(timeBetweenLines);

        // Inform DialogueManager that the line is done
        DialogueManager.Instance.ShowNextLine();
    }
    /// <summary>
    /// Adds a new line of dialogue and removes the oldest if limit is reached.
    /// </summary>
    private void UpdateText(string newLine)
    {
        if (dialogueQueue.Count >= maxLinesDisplayed)
        {
            dialogueQueue.Dequeue(); // Remove the oldest dialogue line
        }

        dialogueQueue.Enqueue(newLine);
        dialogueText.text = string.Join("\n", dialogueQueue);
    }
}
