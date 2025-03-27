using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject ShieldIcon;    
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
    public GameObject EffectsInfoObject;
    public TextMeshProUGUI EffectsInfo;
    public List<Sprite> EffectImages;

    private bool introCombatAnimationFinish = false;

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
        if (introCombatAnimationFinish)
        { 
            EndTurnButton.interactable = isInteractable;

            if (isInteractable)
            {
                if (ChipManager.Instance.UsedChips.Count != 8)
                {
                    PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Open);
                    PlayerHand.GetComponent<PlayerHandContainer>().SliderButton.interactable = true;
                }
            }
            else
            {
                PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Close);
                PlayerHand.GetComponent<PlayerHandContainer>().FillPlayerHand();
                PlayerHand.GetComponent<PlayerHandContainer>().SliderButton.interactable = false;
            }

            EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", isInteractable);

            CombatAnimation.SetActive(!isInteractable);
        }
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
            ShieldIcon.SetActive(false);            
        }
        else
        {            
            ShieldIcon.SetActive(true);

            //float shieldPercentage = (float)Shield / MaxShield;

            // Directly update the shield bar and text
            //ShieldBar.fillAmount = shieldPercentage;
            ShieldText.SetText($"{Shield}");
        }
    }

    /// <summary>
    /// Updates the UI for Player Energy
    /// </summary>
    /// <param name="currentEnergy"></param>
    /// <param name="maxEnergy"></param>
    public void UpdateEnergy(float currentEnergy, float maxEnergy)
    {            

        switch (currentEnergy)
        {
            case 10f:
                EnergyBar.fillAmount = 1f;
                break;
            case 9f:
                EnergyBar.fillAmount = 0.94f;
                break;
            case 8f:
                EnergyBar.fillAmount = 0.843f;
                break;
            case 7f:
                EnergyBar.fillAmount = 0.720f;
                break;
            case 6f:
                EnergyBar.fillAmount = 0.618f;
                break;
            case 5f:
                EnergyBar.fillAmount = 0.51f;
                break;
            case 4f:
                EnergyBar.fillAmount = 0.406f;
                break;
            case 3f:
                EnergyBar.fillAmount = 0.308f;
                break;
            case 2f:
                EnergyBar.fillAmount = 0.19f;
                break;
            case 1f:
                EnergyBar.fillAmount = 0.072f;
                break;
            case 0f:
                EnergyBar.fillAmount = 0.0f;
                break;
            default:
                break;
        }
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
        CombatAnimation.SetActive(false);
    }

    /// <summary>
    /// Remove Combat UI for puzzles
    /// </summary>
    public void RemoveCombatUIPuzzle()
    {
        // Directly disable Combat UI without delay
        PlayerHand.SetActive(false);
        EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", false);
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
            EndTurnButton.interactable = true;
        }
    }

    /// <summary>
    /// Start combat mode for puzzles
    /// </summary>
    public void StarPrepCombatStartPuzzle()
    {
        PlayerHand.SetActive(true);
        PlayerHand.GetComponent<PlayerHandContainer>().FillPlayerHand();
        EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", true);
    }
    /// <summary>
    /// After aniamtor gets half way this is called.
    /// </summary>
    public void ContPrepCombatStart()
    {
        if (!introCombatAnimationFinish)
        {
            EnergyAndGearContainer.GetComponent<Animator>().SetBool("Visible", true);
            PlayerHand.GetComponent<PlayerHandContainer>().FillPlayerHand();

            if (ChipManager.Instance.UsedChips.Count != 8)
                PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Open);

            introCombatAnimationFinish = true;

            Invoke("FinishCombatStart", 1f);
        }
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

        if(EquipmentButton.transform.parent.name != "Lucky Trinket")
            EquipmentButton.interactable = Interactable;
    }

    public void UpdateGearButtonStates(float currentEnergy)
    {
        // Get all equipped items
        Item armor = GearManager.Instance.GetEquippedItem(Item.ItemType.Armor);
        Item weapon = GearManager.Instance.GetEquippedItem(Item.ItemType.Weapon);
        Item equipment = GearManager.Instance.GetEquippedItem(Item.ItemType.Equipment);

        // Check if each gear button should be enabled or disabled based on energy cost
        ArmorButton.interactable = (armor != null && CanUseItem(armor, currentEnergy));
        WeaponButton.interactable = (weapon != null && CanUseItem(weapon, currentEnergy));

        if (equipment.itemEffects.Any(effect => effect is EquipmentEffect equipmentEffect && equipmentEffect.HasPassiveEffect))
        {
            EquipmentButton.interactable = false;
        }
        else
        {
            EquipmentButton.interactable = (equipment != null && CanUseItem(equipment, currentEnergy));
        }        
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
                    effectPrefab.GetComponent<EffectsInfo>().SetAmountOfEffect(statusEffect.StackCount);
                }
                catch
                {
                    Debug.LogWarning("Could not find Effect Image");
                }
            }
        }
    }
    /// <summary>
    /// Popup description of the effect
    /// </summary>
    /// <param name="effectName"></param>
    public void DisplayEffectInfo(string effectName)
    {
        EffectsInfoObject.SetActive(true);

        switch (effectName) {
            case "Drained":
                EffectsInfo.SetText("Drained: While <b>Drained</b>, your Attacks do 20% less damage.");
                break;
            case "Galvanize":
                EffectsInfo.SetText("Galvanize: Gain <color=#3EF4D3>Shield</color> at the end of your turn, equal to your amount of Galvanize.<sub>(This stacks)</sub>");
                break;
            case "Impervious":
                EffectsInfo.SetText("Impervious: Take no damage for a turn.");
                break;
            case "Jam":
                EffectsInfo.SetText("Jam: While you are <b>Jammed</b>, you may not use Chips.");
                break;
            case "Motivation":
                EffectsInfo.SetText("Motivation: When this effect appears, your next played chip will activate twice.");
                break;
            case "Power":
                EffectsInfo.SetText("Attacks deal additional damage equal to your amount of <b>power</b>.<sub>(until end of combat)</sub>");
                break;
            case "WornDown":
                EffectsInfo.SetText("While <b>Worn Down</b>, your <color=#3EF4D3>Shield</color> provides 30% less <color=#3EF4D3>Shield</color>");
                break;
            case "LuckyTrinket":
                EffectsInfo.SetText("At End of Combat gain more bonus <color=#FFFF00>scrap</color>.");
                break;
            default:
                EffectsInfo.SetText("Error!");
                EffectsInfo.color = Color.red;
                break;
        }
    }
    /// <summary>
    /// Disable the InfoBox
    /// </summary>
    public void HideEffectInfo()
    {
        EffectsInfoObject.SetActive(false);
    }
    /// <summary>
    /// Checks if item can be used.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="currentEnergy"></param>
    /// <returns></returns>
    private bool CanUseItem(Item item, float currentEnergy)
    {
        if (item == null) return false;

        float energyCost = 0f;
        foreach (ItemEffect effect in item.itemEffects)
        {
            energyCost += effect.energyCost;
        }

        return currentEnergy >= energyCost;
    }

    void OnDestroy()
    {
        EndTurnButton.onClick.RemoveAllListeners();
    }
}