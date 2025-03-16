using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enemy;

public class EnemyUI : MonoBehaviour
{
    /// <summary>
    /// Name of Enemy Goes here.
    /// </summary>
    public TextMeshProUGUI EnemyNameBox;

    /// <summary>
    /// Enemy HealthBar Bar.
    /// </summary>
    public Image healthBar;

    /// <summary>
    /// Enemy HealthBar Text.
    /// </summary>
    public TextMeshProUGUI healthText;

    /// <summary>
    /// Enemy ShieldBar Container.
    /// </summary>
    public GameObject shieldContainer;

    /// <summary>
    /// Enemy sheild bar.
    /// </summary>
    public Image shieldBar;

    /// <summary>
    /// Enemy shield text.
    /// </summary>
    public TextMeshProUGUI shieldText;

    // Adjust this for smoother or quicker transitions
    public float UiDuration = 0.5f;

    /// <summary>
    /// reference to effects Panel.
    /// </summary>
    public GameObject EffectsPanel;

    /// <summary>
    /// Prefabs of Effects enemy will use."Case sensitive"
    /// </summary>
    public List<GameObject> effectPrefabs;

    /// <summary>
    /// list of active effects.
    /// </summary>
    public List<GameObject> activeEffects;

    /// <summary>
    /// Container for intentTextBox
    /// </summary>
    public GameObject IntentContainer;

    /// <summary>
    /// Intent text box.
    /// </summary>
    public GameObject IntentText;


    private Camera playerCamera;

    void Awake()
    {
        playerCamera = Camera.main;    
    }

    void LateUpdate()
    {        
        FaceCamera();
    }

    /// <summary>
    /// Displays EnemyName
    /// </summary>
    /// <param name="enemyName"></param>
    public void SetEnemyName(string enemyName)
    {
        EnemyNameBox.SetText(enemyName);
    }

    /// <summary>
    /// Update Enemy HP Bar
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    public void UpdateHealth(float currentHp, float maxHp)
    {

        // Stop any currently running health update coroutine
        StopAllCoroutines();

        // Start the coroutine to smoothly update the health bar
        StartCoroutine(UpdateHealthOverTime(currentHp,maxHp));
    }

    /// <summary>
    /// Update Enemy ShieldBar Bar
    /// </summary>
    public void UpdateShield(float currentShield, float maxShield)
    {       

        if (currentShield == 0 && maxShield == 0)
        {
            shieldContainer.SetActive(false);
        }
        else
        {
            shieldContainer.SetActive(true);

            // Calculate the target shield percentage
            float shieldPercentage = currentShield / maxShield;

            //ShieldBar.fillAmount = shieldPercentage;

            StopAllCoroutines();

            // Start the coroutine to smoothly update the shield bar
            StartCoroutine(UpdateShieldOverTime(shieldPercentage, maxShield));
        }
    }

    /// <summary>
    /// Update the Effects for Enemy
    /// </summary>
    /// <param name="activeEffects"></param>
    /// <summary>
    /// Update the Effects for Enemy
    /// </summary>
    /// <param name="activeEffects"></param>
    public void UpdateEffectsPanel(List<Effects.StatusEffect> activeEffects)
    {
        // Clear existing UI elements
        foreach (var effect in this.activeEffects)
        {
            Destroy(effect);
        }
        this.activeEffects.Clear();

        // Populate the panel with new effects
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

            // Proceed if a valid effect name was found
            if (!string.IsNullOrEmpty(effectName))
            {
                GameObject effectPrefab = effectPrefabs.Find(prefab => prefab.name == effectName);

                if (effectPrefab != null)
                {
                    GameObject effectInstance = Instantiate(effectPrefab, EffectsPanel.transform);
                    effectInstance.name = effectName;
                    this.activeEffects.Add(effectInstance);
                }
                else
                {
                    Debug.LogError($"[EnemyUI] Effect prefab not found for {effectName}");
                }
            }
        }
    }


    /// <summary>
    /// Update intent box.
    /// </summary>
    /// <param name="intent"></param>
    public void DisplayIntent(string intentText,Enemy.IntentType intentType,int value = 0)
    {
        IntentContainer.SetActive(true);

        var textComponent = IntentText.GetComponent<TextMeshProUGUI>();

        // Define colors based on IntentType
        string colorTag = intentType switch
        {
            // Red for damage
            IntentType.Attack => "<color=#FF0000>",
            // Blue for shield
            IntentType.Shield => "<color=#0000FF>",
            // Green for buffs
            IntentType.Buff => "<color=#00FF00>",
            // Yellow for debuffs
            IntentType.Debuff => "<color=#FFFF00>",
            // Orange for unique actions
            IntentType.Unique => "<color=#FFA500>",
            // Default white
            _ => "<color=#FFFFFF>"
        };

        // Define corresponding icons based on IntentType
        string iconTag = intentType switch
        {
            IntentType.Attack => "<sprite name=Attacking>",
            IntentType.Shield => "<sprite name=Shielding>",
            IntentType.Buff => "<sprite name=Buff>",
            IntentType.Debuff => "<sprite name=Debuff>",
            IntentType.Unique => "<sprite name=Unique1> " + intentText + " <sprite name=Unique2>",
            _ => ""
        };

        // If there is a numeric value, format appropriately
        string formattedText = value > 0 ? $"{colorTag}{value}</color> {iconTag}" : $"{iconTag}";

        // Set the text
        textComponent.SetText(formattedText);
    }

    /// <summary>
    /// Make the canvas face the Player camera
    /// </summary>
    private void FaceCamera()
    {
        if (playerCamera != null)
        {
            Vector3 direction = (playerCamera.transform.position - this.transform.position).normalized;
            direction.y = 0;
            this.transform.rotation = Quaternion.LookRotation(-direction);
        }
    }

    private IEnumerator UpdateHealthOverTime(float currentHp, float maxHp)
    {
        float initialFillAmount = healthBar.fillAmount;
        float targetFillAmount = currentHp / maxHp;
        float elapsedTime = 0f;

        // Correct initial HP
        float initialHP = Mathf.Round(initialFillAmount * maxHp * 100f) / 100f;

        // Track displayed HP separately
        float displayedHP = initialHP;

        while (elapsedTime < UiDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / UiDuration;

            // Smooth transition for health bar
            float newFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, progress);
            healthBar.fillAmount = newFillAmount;

            // Smooth transition for health text
            displayedHP = Mathf.RoundToInt(Mathf.Lerp(initialHP, currentHp, progress));
            healthText.SetText($"{displayedHP}");

            yield return null;
        }

        // Snap to exact final value to prevent small inconsistencies
        healthBar.fillAmount = targetFillAmount;
        healthText.SetText($"{currentHp}");
    }




    private IEnumerator UpdateShieldOverTime(float targetFillAmount,float maxShield)
    {
        float initialFillAmount = shieldBar.fillAmount;
        float elapsedTime = 0f;

        // Initial shield amount
        int initialCurrentShield = Mathf.RoundToInt(initialFillAmount * maxShield);

        // Target shield amount
        int targetCurrentShield = Mathf.RoundToInt(targetFillAmount * maxShield);

        // Store the initial max shield value
        int initialMaxShield = (int)maxShield;

        // Target max shield value
        int finalMaxShield = Mathf.RoundToInt(targetFillAmount * maxShield);

        while (elapsedTime < UiDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / UiDuration;

            // Lerp shield bar fill amount
            float newFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, progress);
            shieldBar.fillAmount = newFillAmount;

            // Dynamically calculate current and max shield values
            int currentShield = Mathf.RoundToInt(Mathf.Lerp(initialCurrentShield, targetCurrentShield, progress));
            int updatedMaxShield = Mathf.RoundToInt(Mathf.Lerp(initialMaxShield, finalMaxShield, progress));

            // Update the shield text
            shieldText.SetText($"{currentShield}/{updatedMaxShield}");

            yield return null;
        }

        // Ensure it snaps to the final values
        shieldBar.fillAmount = targetFillAmount;
        shieldText.SetText($"{targetCurrentShield}/{finalMaxShield}");
    }
}