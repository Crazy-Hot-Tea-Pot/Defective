using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class CreditsController : MonoBehaviour
{
    public TMP_Text m_TextComponent;

    [Range(0.000f,0.100f)]
    public float revealSpeed = 0.05f; 
    public int wordsPerCycle = 1;
    public bool Wordmode;
    public PlayerInputActions playerInputActions;
    public VideoPlayer VideoPlayer;

    private bool hasTextChanged;
    private InputAction backToTitleAction;    

    void OnEnable()
    {
        backToTitleAction = playerInputActions.Player.Escape;

        backToTitleAction.Enable();

        backToTitleAction.performed += BackToTitle;
    }

    void Awake()
    {
        // assign Player Input class
        playerInputActions = new PlayerInputActions();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Wordmode)
            StartCoroutine(RevealWords(m_TextComponent));
        else
            StartCoroutine(RevealCharacters(m_TextComponent));

    }     
    private void BackToTitle(InputAction.CallbackContext context)
    {
        GameManager.Instance.RequestScene(Levels.Title);
    }
    IEnumerator RevealCharacters(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;
        int totalVisibleCharacters = textInfo.characterCount; // Get total characters
        int visibleCount = 0;

        while (true)
        {
            if (hasTextChanged)
            {
                totalVisibleCharacters = textInfo.characterCount;
                hasTextChanged = false;
            }

            if (visibleCount > totalVisibleCharacters)
            {
                yield return new WaitForSeconds(1.0f);
                visibleCount = 0;
            }

            textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(revealSpeed); // Delay based on revealSpeed
        }
    }
    IEnumerator RevealWords(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        int totalWordCount = textComponent.textInfo.wordCount;
        int totalVisibleCharacters = textComponent.textInfo.characterCount;
        int visibleCount = 0;

        for (int currentWord = 0; currentWord < totalWordCount; currentWord += wordsPerCycle)
        {
            if (hasTextChanged)
            {
                totalVisibleCharacters = textComponent.textInfo.characterCount;
                totalWordCount = textComponent.textInfo.wordCount;
                hasTextChanged = false;
            }

            int lastVisibleCharIndex = textComponent.textInfo.wordInfo[
                Mathf.Min(currentWord + wordsPerCycle - 1, totalWordCount - 1)
            ].lastCharacterIndex;

            visibleCount = lastVisibleCharIndex + 1;

            textComponent.maxVisibleCharacters = visibleCount;

            yield return new WaitForSeconds(revealSpeed);
        }

        // Optionally pause at the end of all words
        yield return new WaitForSeconds(1.0f);
    }

    void OnDisable()
    {
        backToTitleAction.performed -= BackToTitle;
        backToTitleAction.Disable();
    }

}
