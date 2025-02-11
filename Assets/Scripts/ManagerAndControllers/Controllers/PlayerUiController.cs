using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This controller does not control shield and health only the effects and chatbox
/// </summary>
public class PlayerUiController : MonoBehaviour
{
    [Header("Effects UI")]
    //Panel to hold the effect icons
    public GameObject effectsPanel;
    //List of effect prefabs
    public List<GameObject> effectPrefabs;
    private List<GameObject> activeEffects = new List<GameObject>();

    [Header("ChatBox UI")]
    public GameObject ChatBox;
    //Text for the chatbox
    public TextMeshPro chatboxText;
    // Speed for revealing letters
    public float letterRevealSpeed = 0.05f;
    // Speed for revealing words
    public float wordRevealSpeed = 0.2f;

    private Coroutine chatboxCoroutine;
    /// <summary>
    /// Update the Player Effects Panel
    /// </summary>
    /// <param name="activeEffects"></param>
    /// <summary>
    /// Update the Player Effects Panel
    /// </summary>
    /// <param name="activeEffects"></param>
    public void UpdateEffectsPanel(List<Effects.StatusEffect> activeEffects)
    {
        // Clear the panel
        foreach (var effect in this.activeEffects)
        {
            Destroy(effect);
        }
        this.activeEffects.Clear();

        // Repopulate the panel with new effects
        foreach (var statusEffect in activeEffects)
        {
            string effectName = null;

            // Determine which effect type is active
            if (statusEffect.BuffEffect != Effects.Buff.None)
            {
                effectName = statusEffect.BuffEffect.ToString();
            }
            else if (statusEffect.DebuffEffect != Effects.Debuff.None)
            {
                effectName = statusEffect.DebuffEffect.ToString();
            }
            else if (statusEffect.SpecialEffect != Effects.SpecialEffects.None)
            {
                effectName = statusEffect.SpecialEffect.ToString();
            }

            // Only proceed if a valid effect name was found
            if (!string.IsNullOrEmpty(effectName))
            {
                GameObject effectPrefab = effectPrefabs.Find(prefab => prefab.name == effectName);

                if (effectPrefab != null)
                {
                    GameObject effectInstance = Instantiate(effectPrefab, effectsPanel.transform);
                    effectInstance.name = effectName;
                    this.activeEffects.Add(effectInstance);
                }
                else
                {
                    Debug.LogError($"[PlayerUiController] Effect prefab not found for {effectName}");
                }
            }
        }
    }


    /// <summary>
    /// Make it look like the Player character in the game is talking to Player.
    /// </summary>
    /// <param name="message">What you want the character to say.</param>
    /// <param name="byLetter">Reveal by letter or by word.</param>
    /// <param name="howFastToTalk">How fast you want the speed to be.</param>
    /// <param name="displayDuration">How long the message to be displayed. Default is 3 seconds.</param>
    public void PlayerTalk(string message, bool byLetter, float howFastToTalk, float displayDuration = 3f)
    {
        // Ensure ChatBox starts hidden
        ChatBox.SetActive(false);

        // Stop any currently running chatbox coroutine
        if (chatboxCoroutine != null)
        {
            StopCoroutine(chatboxCoroutine);
        }

        // Start the reveal coroutine
        chatboxCoroutine = StartCoroutine(RevealText(ChatBox, message, byLetter, howFastToTalk, displayDuration));
    }

    private IEnumerator RevealText(GameObject chatBox, string message, bool byLetter, float revealSpeed, float displayDuration)
    {
        // Get TextMeshPro component from the chatBox
        TextMeshPro tmpText = chatBox.GetComponent<TextMeshPro>();

        // Set and prepare the text
        // Clear any existing text
        tmpText.SetText(""); 
        // Ensure all characters are hidden
        tmpText.maxVisibleCharacters = 0;
        // Activate the chatbox
        chatBox.SetActive(true);
        // Set the full message
        tmpText.SetText(message);
        // Ensure mesh updates
        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;
        // Total characters in the text
        int totalVisibleCharacters = textInfo.characterCount;
        // Total words in the text
        int totalWordCount = textInfo.wordCount;

        // For letter-by-letter
        int visibleCount = 0;
        // Word index
        int currentWord = 0;

        if (byLetter)
        {
            // Reveal text letter-by-letter
            for (visibleCount = 0; visibleCount <= totalVisibleCharacters; visibleCount++)
            {
                tmpText.maxVisibleCharacters = visibleCount;
                yield return new WaitForSeconds(revealSpeed);
            }
        }
        else
        {
            // Reveal text word-by-word
            while (currentWord <= totalWordCount)
            {
                if (currentWord == 0)
                {
                    // No characters visible initially
                    visibleCount = 0;
                }
                else if (currentWord < totalWordCount)
                {
                    // Include all characters up to the last character of the current word
                    visibleCount = textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
                }
                else if (currentWord == totalWordCount)
                {
                    // Include all characters (for the last word and trailing punctuation)
                    visibleCount = totalVisibleCharacters;
                }

                tmpText.maxVisibleCharacters = visibleCount;

                currentWord++;
                yield return new WaitForSeconds(revealSpeed);
            }
        }

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the chatbox after the display duration
        chatBox.SetActive(false);
    }
}
