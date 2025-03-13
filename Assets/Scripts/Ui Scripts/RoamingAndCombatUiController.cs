using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoamingAndCombatUiController : UiController
{
    public float SpeedOfFill = 1f;

    [Header("Camera UI")]
    public GameObject CameraIndicator;
    public CameraModeIndicatorController CameraModeIndicatorController;

    [Header("Health")]
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    // Green (395E44 in RGB normalized)
    public Color fullHealthColor = new Color(0.23f, 0.37f, 0.27f);
    public Color lowHealthColor = Color.red;

    [Header("Shield")]
    public GameObject ShieldContainer;
    public Image ShieldBar;
    public TextMeshProUGUI ShieldText;

    [Header("Energy")]
    public Image EnergyBar;

    [Header("Gear")]
    public Gear Armor;
    public Button ArmorButton;
    public Gear Weapon;
    public Button WeaponButton;
    public Gear Equipment;
    public Button EquipmentButton;

    [Header("Combat Mode Stuff")]
    public GameObject PlayerHand;
    public GameObject EnergyAndGearContainer;
    public GameObject EndTurn;
    public Button EndTurnButton;
    public Animator EndTurnButtonAnimator;
    public GameObject CombatAnimation;
    public GameObject DeathAnimation;

    [Header("Effects")]
    public GameObject EffectPrefab;
    public GameObject EffectsContainer;
    public List<Sprite> EffectImages;

    // Start is called before the first frame update
    void Start()
    {
        EndTurnButton.onClick.AddListener(() => GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().EndTurn());
        EndTurnButton.onClick.AddListener(() => ChangeCombatScreenTemp(false));
        EndTurnButton.onClick.AddListener(() => GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>().EndTurn(GameObject.FindGameObjectWithTag("Player")));        
        
        EndTurn.SetActive(false);
        PlayerHand.SetActive(false);
        CameraIndicator.SetActive(false);

        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (player != null)
        {
            UpdateHealth(player.Health, player.MaxHealth);
            UpdateShield(player.Shield, player.MaxShield);
            UpdateEnergy(player.Energy, player.MaxEnergy);
        }

        Armor.EquipItem(GearManager.Instance.GetEquippedItem(Item.ItemType.Armor));
        Weapon.EquipItem(GearManager.Instance.GetEquippedItem(Item.ItemType.Weapon));
        Equipment.EquipItem(GearManager.Instance.GetEquippedItem(Item.ItemType.Equipment));
    }

    public override void Initialize()
    {
        Debug.Log("RoamingAndCombatUiController initialized");        
    }

    /// <summary>
    /// Temp remove combat screen
    /// </summary>
    /// <param name="isInteractable"></param>
    public void ChangeCombatScreenTemp(bool isInteractable)
    {
        EndTurnButton.interactable = isInteractable;

        if (isInteractable)
        {
            if(ChipManager.Instance.UsedChips.Count!=8)
                PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Open);
        }
        else
        {
            PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Close);
            PlayerHand.GetComponent<PlayerHandContainer>().FillPlayerHand();
        }

        EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", isInteractable);

        CombatAnimation.SetActive(!isInteractable);      
    }

    /// <summary>
    /// Updates the UI for Player HealthBar
    /// </summary>
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        // Directly update the health bar
        float targetHealthPercentage = (float)currentHealth / maxHealth;
        HealthBar.fillAmount = targetHealthPercentage;
        HealthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, targetHealthPercentage);

        // Update the health text
        //int percentage = Mathf.RoundToInt(targetHealthPercentage * 100);
        HealthText.SetText(currentHealth.ToString());//$"{percentage}%");

        //float targetHealthPercentage = (float)currentHealth / maxHealth;

        //StopCoroutine(UpdateHealthOverTime(targetHealthPercentage));

        //// Start the coroutine to smoothly update the health bar
        //StartCoroutine(UpdateHealthOverTime(targetHealthPercentage));
    }   
    
    /// <summary>
    /// Updates the UI for Player ShieldAmount
    /// </summary>
    public void UpdateShield(int Shield, int MaxShield)
    {
        if (Shield == 0 && MaxShield == 100)
        {
            ShieldContainer.SetActive(false);
        }
        else
        {
            ShieldContainer.SetActive(true);

            float shieldPercentage = (float)Shield / MaxShield;

            // Directly update the shield bar and text
            ShieldBar.fillAmount = shieldPercentage;
            ShieldText.SetText($"{Shield}/{MaxShield}");
        }
        //if (Shield == 0 && MaxShield == 100)
        //{
        //    ShieldContainer.SetActive(false);
        //}
        //else
        //{
        //    ShieldContainer.SetActive(true);

        //    // Calculate the target ShieldAmount percentage
        //    float shieldPercentage = (float)Shield / (float)MaxShield;

        //    StopCoroutine(UpdateShieldOverTime(shieldPercentage,Shield,MaxShield));

        //    // Start the coroutine to smoothly update the ShieldAmount bar
        //    StartCoroutine(UpdateShieldOverTime(shieldPercentage, Shield, MaxShield));
        //}
    }

    /// <summary>
    /// Updates the UI for Player Energy
    /// </summary>
    /// <param name="currentEnergy"></param>
    /// <param name="maxEnergy"></param>
    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        // Directly update the energy bar
        float energyPercentage = (float)currentEnergy / maxEnergy;
        EnergyBar.fillAmount = energyPercentage;
        //// Normalize the energy value to a 0-1 range
        //float tempTargetFillAmount = (float)currentEnergy / maxEnergy;


        //StopCoroutine(FillEnergyOverTime(tempTargetFillAmount));

        //StartCoroutine(FillEnergyOverTime(tempTargetFillAmount));
    }

    /// <summary>
    /// Remove Combat UI
    /// </summary>
    public void RemoveCombatUI()
    {
        // Directly disable Combat UI without delay
        PlayerHand.SetActive(false);
        EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", false);
        EndTurn.SetActive(false);
    }
    /// <summary>
    /// Prepare screen for CombatStart
    /// </summary>
    public void StartPrepCombatStart()
    {
        CombatAnimation.SetActive(true);
        PlayerHand.SetActive(true);

        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
        {
            PlayerHand.GetComponent<PlayerHandContainer>().FillPlayerHand();
            EndTurn.SetActive(true);
        }
    }
    /// <summary>
    /// After aniamtor gets half way this is called.
    /// </summary>
    public void ContPrepCombatStart()
    {
        EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", true);
        PlayerHand.GetComponent<PlayerHandContainer>().FillPlayerHand();
        
        if (ChipManager.Instance.UsedChips.Count != 8)
            PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Open);

        Invoke("FinishCombatStart", 1f);
    }
    private void FinishCombatStart()
    {
        // Hide CombatAnimation if it's temporary
        CombatAnimation.SetActive(false);
    }

    public void MakeGearInteractable(bool Interactable)
    {
        ArmorButton.interactable = Interactable;
        WeaponButton.interactable = Interactable;
        EquipmentButton.interactable = Interactable;
    }

    public void UpdateGearButtonStates(int currentEnergy)
    {
        // Get all equipped items
        Item armor = GearManager.Instance.GetEquippedItem(Item.ItemType.Armor);
        Item weapon = GearManager.Instance.GetEquippedItem(Item.ItemType.Weapon);
        Item equipment = GearManager.Instance.GetEquippedItem(Item.ItemType.Equipment);

        // Check if each gear button should be enabled or disabled based on energy cost
        ArmorButton.interactable = (armor != null && CanUseItem(armor, currentEnergy));
        WeaponButton.interactable = (weapon != null && CanUseItem(weapon, currentEnergy));
        EquipmentButton.interactable = (equipment != null && CanUseItem(equipment, currentEnergy));
    }
    /// <summary>
    /// Update the Player Effects Panel
    /// </summary>
    /// <param name="activeEffects"></param>
    public void UpdateEffects(List<Effects.StatusEffect> activeEffects)
    {
        // Clear the panel
        foreach (Transform effect in EffectsContainer.transform)
        {
            Destroy(effect.gameObject);
        }
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
                GameObject effectPrefab = Instantiate(EffectPrefab,EffectsContainer.transform);
                effectPrefab.name = effectName;
                try
                {
                    effectPrefab.GetComponent<Image>().sprite = EffectImages.Find(sprite => sprite.name == effectName);
                }
                catch
                {
                    Debug.LogWarning("Could not find Effect Image");
                }
            }
        }
    }
    /// <summary>
    /// Checks if item can be used.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="currentEnergy"></param>
    /// <returns></returns>
    private bool CanUseItem(Item item, int currentEnergy)
    {
        if (item == null) return false;

        int energyCost = 0;
        foreach (ItemEffect effect in item.itemEffects)
        {
            energyCost += effect.energyCost - item.GetEnergyCostDecreaseBy();
        }

        return currentEnergy >= energyCost;
    }

    private IEnumerator UpdateHealthOverTime(float targetFillAmount)
    {
        // While the bar is not at the target fill amount, update it
        while (Mathf.Abs(HealthBar.fillAmount - targetFillAmount) > 0.001f)
        {
            // Lerp between current fill and target fill by the fill speed
            HealthBar.fillAmount = Mathf.Lerp(HealthBar.fillAmount, targetFillAmount, SpeedOfFill * Time.deltaTime);

            // Lerp the color based on the health percentage
            HealthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, HealthBar.fillAmount);

            // Display percentage as an integer (0 to 100)
            int percentage = Mathf.RoundToInt(HealthBar.fillAmount * 100);
            HealthText.SetText(percentage + "%");

            // Ensure the fill value gradually updates each frame
            yield return null;
        }

        // Ensure it snaps to the exact target amount at the end
        HealthBar.fillAmount = targetFillAmount;
        HealthBar.color = Color.Lerp(lowHealthColor, fullHealthColor, HealthBar.fillAmount);

        // Display percentage as an integer (0 to 100)
        int finalPercentage = Mathf.RoundToInt(targetFillAmount * 100);
        HealthText.SetText(finalPercentage + "%");
    }

    private IEnumerator UpdateShieldOverTime(float targetFillAmount, int Shield, int MaxShield)
    {      

        int initialCurrentShield = Mathf.RoundToInt(ShieldBar.fillAmount * MaxShield);
        int targetCurrentShield = Shield;

        int initialMaxShield = MaxShield;

        float elapsedTime = 0f;

        while (elapsedTime < SpeedOfFill)
        {
            elapsedTime += Time.deltaTime;

            // Lerp the ShieldAmount bar fill amount
            float newFillAmount = Mathf.Lerp(ShieldBar.fillAmount, targetFillAmount, elapsedTime / SpeedOfFill);
            ShieldBar.fillAmount = newFillAmount;

            // Dynamically calculate ShieldAmount values
            int currentShield = Mathf.RoundToInt(Mathf.Lerp(initialCurrentShield, targetCurrentShield, elapsedTime / SpeedOfFill));
            int maxShield = Mathf.RoundToInt(Mathf.Lerp(initialMaxShield, MaxShield, elapsedTime / SpeedOfFill));

            // Update the ShieldAmount text
            ShieldText.SetText($"{currentShield}/{maxShield}");

            yield return null;
        }

        // Snap to final values
        ShieldBar.fillAmount = targetFillAmount;
        ShieldText.SetText($"{targetCurrentShield}/{MaxShield}");
    }

    /// <summary>
    /// Fill EnergyBar by amount over time.
    /// </summary>
    /// <param name="targetFillAmount"></param>
    /// <returns></returns>
    private IEnumerator FillEnergyOverTime(float targetFillAmount)
    {

        // While the bar is not at the target fill amount, update it
        while (Mathf.Abs(EnergyBar.fillAmount - targetFillAmount) > 0.001f)
        {
            // Lerp between current fill and target fill by the fill speed
            EnergyBar.fillAmount = Mathf.Lerp(EnergyBar.fillAmount, targetFillAmount, SpeedOfFill * Time.deltaTime);            
           

            // Ensure the fill value gradually updates each frame
            yield return null;
        }

        // Ensure it snaps to the exact target amount at the end
        EnergyBar.fillAmount = targetFillAmount;
    }

    void OnDestroy()
    {
        EndTurnButton.onClick.RemoveAllListeners();
    }
}