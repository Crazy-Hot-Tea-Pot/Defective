using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void UpdateHealth(int currentHp, int maxHp)
    {
        float targetHealthPercentage = (float)currentHp / maxHp;

        // Stop any currently running health update coroutine
        StopCoroutine(UpdateHealthOverTime(targetHealthPercentage));

        // Start the coroutine to smoothly update the health bar
        StartCoroutine(UpdateHealthOverTime(targetHealthPercentage));
    }

    /// <summary>
    /// Update Enemy ShieldBar Bar
    /// </summary>
    public void UpdateShield(int currentShield, int maxShield)
    {       

        if (currentShield == 0 && maxShield == 0)
        {
            shieldContainer.SetActive(false);
        }
        else
        {
            shieldContainer.SetActive(true);               

            // Calculate the target shield percentage
            float shieldPercentage = (float)currentShield / maxShield;

            //ShieldBar.fillAmount = shieldPercentage;

            StopCoroutine(UpdateShieldOverTime(shieldPercentage, maxShield));

            // Start the coroutine to smoothly update the shield bar
            StartCoroutine(UpdateShieldOverTime(shieldPercentage, maxShield));
        }
    }

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
            string effectName = statusEffect.Effect.ToString();
            GameObject effectPrefab = effectPrefabs.Find(prefab => prefab.name == effectName);

            if (effectPrefab != null)
            {
                GameObject effectInstance = Instantiate(effectPrefab, EffectsPanel.transform);
                effectInstance.name = effectName;
                this.activeEffects.Add(effectInstance);
            }
            else
            {
                Debug.LogError($"Effect prefab not found for {effectName}");
            }
        }
    }

    /// <summary>
    /// Update intent box.
    /// </summary>
    /// <param name="intent"></param>
    public void UpdateIntent(Intent intent)
    {
        IntentContainer.SetActive(true);
        //IntentText.SetActive(true);
        var textComponent = IntentText.GetComponent<TextMeshProUGUI>();
        textComponent.SetText($"{intent.Name} - {intent.Damage} Damage\n{intent.AdditionalInfo}");
        textComponent.color = intent.IntentColor;
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

    private IEnumerator UpdateHealthOverTime(float targetFillAmount)
    {        

        float initialFillAmount = healthBar.fillAmount;
        float elapsedTime = 0f;       

        while (elapsedTime < UiDuration)
        {
            elapsedTime += Time.deltaTime;
            float newFillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsedTime / UiDuration);
            healthBar.fillAmount = newFillAmount;

            // Update health text as percentage
            int currentHealthPercentage = Mathf.RoundToInt(newFillAmount * 100);
            healthText.SetText($"{currentHealthPercentage}%");

            yield return null;
        }

        // Ensure it snaps to the final target amount
        healthBar.fillAmount = targetFillAmount;
        healthText.SetText($"{Mathf.RoundToInt(targetFillAmount * 100)}%");
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