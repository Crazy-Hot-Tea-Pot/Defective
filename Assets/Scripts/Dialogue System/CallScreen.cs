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

        // Set the full text but make it hidden initially
        dialogueText.SetText(fullText);

        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate(); // Ensure TextMesh updates

        int totalVisibleCharacters = dialogueText.textInfo.characterCount;
        int visibleCount = 0;

        // Reveal text letter-by-letter
        if (revealByLetter)
        {
            for (visibleCount = 0; visibleCount <= totalVisibleCharacters; visibleCount++)
            {
                dialogueText.maxVisibleCharacters = visibleCount;
                yield return new WaitForSeconds(revealSpeed);
            }
        }
        else
        {
            // Reveal text word-by-word
            TMP_TextInfo textInfo = dialogueText.textInfo;
            int totalWordCount = textInfo.wordCount;
            int currentWord = 0;

            while (currentWord <= totalWordCount)
            {
                if (currentWord < totalWordCount)
                {
                    visibleCount = textInfo.wordInfo[currentWord].lastCharacterIndex + 1;
                }
                else
                {
                    visibleCount = totalVisibleCharacters;
                }

                dialogueText.maxVisibleCharacters = visibleCount;
                currentWord++;
                yield return new WaitForSeconds(revealSpeed * 2);
            }
        }

        isRevealingText = false;
        yield return new WaitForSeconds(timeBetweenLines);

        // Move to the next line
        DialogueManager.Instance.ShowNextLine();
    }
}
