using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WinController : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI StatDisplay;
    public GameObject ChipCollectedContainer;
    public GameObject ChipDefault;

    [Header("Title Effect Settings")]
    [Tooltip("Switch between Glitch and Scramble effects.")]
    public bool useScrambleEffect = false;

    [Range(0.0f, 1.0f)]
    [Tooltip("How often the effect happens (increase this to slow down the effect)")]
    public float effectInterval = 0.5f;

    [Range(0.00f, 1.00f)]
    [Tooltip("How long each effect lasts (increase this to make effects last longer)")]
    public float effectDuration = 0.15f;

    [Range(0f, 10f)]
    [Tooltip("How much the text shakes (reduce this for subtle movement)")]
    public float positionJitter = 1f;

    [Header("Win Data Text Reveal Settings")]
    [Range(0.000f, 0.100f)]
    public float revealSpeed = 0.05f;
    public int wordsPerCycle = 1;
    public bool Wordmode;

    [Header("Chip Reveal Settings")]
    public bool RevealInstantly = false;
    public float chipRevealSpeed = 0.5f;

    private bool hasTextChanged;
    private string originalText;
    private InputAction backToTitleAction;
    private PlayerInputActions playerInputActions;

    void OnEnable()
    {
        backToTitleAction = playerInputActions.Player.Escape;

        backToTitleAction.Enable();

        backToTitleAction.performed += ToCredits;
    }

    void Awake()
    {
        // assign Player Input class
        playerInputActions = new PlayerInputActions();
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateChips();
        SetUpStatsText();       

        originalText = TitleText.text;

        StartCoroutine(useScrambleEffect ? ScrambleEffect() : GlitchEffect());
    }
    private void PopulateChips()
    {
        foreach (Transform child in ChipCollectedContainer.transform)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(SpawnLootWithDelay());
    }
    private void SetUpStatsText()
    {
        float completionTime = Time.time - GameStatsTracker.Instance.SessionStartTime;


        string completionTimeFormatted = $"{(int)completionTime / 60:D2}:{(int)completionTime % 60:D2}";
        string fastestCombatTimeFormatted = GameStatsTracker.Instance.FastestCombatTime == Mathf.Infinity ? "N/A" : $"{(int)GameStatsTracker.Instance.FastestCombatTime / 60:D2}:{(int)GameStatsTracker.Instance.FastestCombatTime % 60:D2}";
        string highestDamageDealtFormatted = $"{GameStatsTracker.Instance.HighestDamageDealt:F0}";
        string totalScrapCollectedFormatted = $"<color=yellow>{GameStatsTracker.Instance.TotalScrapCollected}</color>";


        StatDisplay.SetText($"Completion Time: {completionTimeFormatted}\n" +
                            $"fastest Combat Time: {fastestCombatTimeFormatted}\n" +
                            $"Highest Damage Dealt: {highestDamageDealtFormatted}\n" +
                            $"Total <color=yellow>Scrap</color> Collected: {totalScrapCollectedFormatted}");
        

        if (Wordmode)
            StartCoroutine(RevealWords(StatDisplay));
        else
            StartCoroutine(RevealCharacters(StatDisplay));
    }
    private void ToCredits(InputAction.CallbackContext context)
    {
        GameManager.Instance.RequestScene(Levels.Credits);
    }    
    /// <summary>
    /// 20% chance to glitch each character.
    /// Replace with random ASCII character.
    /// </summary>
    /// <param name="input"></param>
    private string GlitchText(string input)
    {
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (Random.value > 0.8f)
            {
                chars[i] = (char)UnityEngine.Random.Range(33, 126);
            }
        }
        return new string(chars);
    }
    private IEnumerator RevealCharacters(TMP_Text textComponent)
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
                yield break;
                //yield return new WaitForSeconds(1.0f);
                //visibleCount = 0;
            }

            textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(revealSpeed); // Delay based on revealSpeed
        }
    }
    private IEnumerator RevealWords(TMP_Text textComponent)
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
        //yield return new WaitForSeconds(1.0f);
        yield break;
    }
    private IEnumerator ScrambleEffect()
    {
        yield return new WaitForSeconds(2f);
        char[] scrambleChars = "0123456789ABCDEF@#%&*<>?/\\|".ToCharArray();

        while (true)
        {
            yield return new WaitForSeconds(effectInterval);
            char[] chars = originalText.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (!char.IsWhiteSpace(chars[i]))
                {
                    char originalChar = chars[i];
                    for (int j = 0; j < 5; j++)
                    {
                        chars[i] = scrambleChars[Random.Range(0, scrambleChars.Length)];
                        TitleText.text = new string(chars);
                        yield return new WaitForSeconds(effectDuration / 5f);
                    }
                    chars[i] = originalChar;
                    TitleText.text = new string(chars);
                }
            }

            Vector3 randomOffset = new Vector3(Random.Range(-positionJitter, positionJitter), Random.Range(-positionJitter, positionJitter), 0);
            TitleText.rectTransform.anchoredPosition += new Vector2(randomOffset.x, randomOffset.y);
            yield return new WaitForSeconds(effectDuration);
            TitleText.rectTransform.anchoredPosition -= new Vector2(randomOffset.x, randomOffset.y);
        }
    }
    private IEnumerator GlitchEffect()
    {
        while (true) // Runs infinitely until title screen changes
        {
            yield return new WaitForSeconds(effectInterval);

            string glitchedText = GlitchText(originalText);
            Vector3 randomOffset = new Vector3(Random.Range(-positionJitter, positionJitter), Random.Range(-positionJitter, positionJitter), 0);

            TitleText.text = glitchedText;
            // Convert to Vector2
            Vector2 offset2D = new Vector2(randomOffset.x, randomOffset.y);
            TitleText.rectTransform.anchoredPosition += offset2D;

            yield return new WaitForSeconds(effectDuration);

            TitleText.text = originalText;
            TitleText.rectTransform.anchoredPosition -= offset2D;
        }
    }
    private IEnumerator SpawnLootWithDelay()
    {
        foreach (NewChip newChip in GameStatsTracker.Instance.TotalChipsCollected)
        {
            GameObject Chip = Instantiate(ChipDefault, ChipCollectedContainer.transform);

            Chip.GetComponent<Chip>().SetChipModeTo(global::Chip.ChipMode.Inventory);

            Chip.GetComponent<Chip>().NewChip = newChip;

            // Wait for second before spawning the next
            yield return new WaitForSeconds(chipRevealSpeed);
        }
    }
    void OnDisable()
    {
        backToTitleAction.performed -= ToCredits;
        backToTitleAction.Disable();
    }
}
