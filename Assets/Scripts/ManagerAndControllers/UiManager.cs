using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    public PlayerInputActions playerInputActions;

    /// <summary>
    /// Current Ui being displayed.
    /// </summary>
    [SerializeField]
    public GameObject CurrentUI
    {
        get;
        set;
    }
    /// <summary>
    /// List of Prefabs UI.
    /// </summary>
    public List<GameObject> listOfUis;
    public GameObject RoamingAndCombatUI;
    public GameObject InventoryUI;
    public GameObject TerminalUI;
    public GameObject LootUI;
    public GameObject SettingsUI;

    private UiController currentController;
    private static UiManager instance;

    void OnEnable()
    {
        playerInputActions.Player.Inventory.performed += ToggleInventory;
        playerInputActions.Player.Settings.performed += ToggleSettings;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        // assign Player Input class
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStartCombat += StartCombat;
        GameManager.Instance.OnEndCombat += EndCombat;
        GameManager.Instance.OnSceneChange += SceneChange;
        GameManager.Instance.OnGameModeChanged += UpdateUIForGameMode;

        UpdateUIForGameMode();

    }
    #region RoamingAndCombatUI
    public void UpdateCameraIndicator(CameraController.CameraState cameraState)
    {
        //Activate UI Obejct        
        GetCurrentController<RoamingAndCombatUiController>().CameraIndicator.SetActive(true);

        GetCurrentController<RoamingAndCombatUiController>().CameraModeIndicatorController.SwitchIndicatorTo(cameraState);
    }
    public void UpdateHealth(int currentHealth, int MaxHealth)
    {
        GetCurrentController<RoamingAndCombatUiController>().UpdateHealth(currentHealth, MaxHealth);
    }
    public void UpdateShield(int currentShield, int MaxShield)
    {
        GetCurrentController<RoamingAndCombatUiController>().UpdateShield(currentShield, MaxShield);
    }
    public void UpdateEnergy(int currentEnergy, int MaxEnergy)
    {
        GetCurrentController<RoamingAndCombatUiController>().UpdateEnergy(currentEnergy, MaxEnergy);
    }
    public void EndTurnButtonVisibility(bool Visiable)
    {
        GetCurrentController<RoamingAndCombatUiController>().ChangeEndButtonVisibility(Visiable);
    }
    public void EndTurnButtonInteractable(bool Interact)
    {
        GetCurrentController<RoamingAndCombatUiController>().EndTurnButton.interactable = Interact;
    }
    public void ReDrawPlayerhand()
    {
        StopCoroutine(GetCurrentController<RoamingAndCombatUiController>().RedrawPlayerHand());

        StartCoroutine(GetCurrentController<RoamingAndCombatUiController>().RedrawPlayerHand());
    }
    /// <summary>
    /// Change the state of the buttons on combat wheel
    /// </summary>
    /// <param name="State"></param>
    public void ChangeStateOfGear(bool State)
    {
        GetCurrentController<RoamingAndCombatUiController>().MakeGearInteractable(State);
    }
    #endregion
    #region InventoryUI
    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Roaming)
        {
            if (CurrentUI.name == InventoryUI.name )
            {
                SwitchScreen(RoamingAndCombatUI);
            }
            else
            {
                SwitchScreen(InventoryUI);
            }
        }
    }
    public void RefreshInventory()
    {
        GetCurrentController<InventoryController>().RefreshCurrentInventory();
    }
    #endregion
    #region TerminalUI
    public void UpdateScrapDisplay(int playerScrap)
    {
        GetCurrentController<UpgradeTerminalUIController>().UpdateScrapDisplay(playerScrap);
    }
    public string GetUserInput()
    {
        return GetCurrentController<UpgradeTerminalUIController>().UserInput.GetComponent<TMP_InputField>().text;
    }
    public void SetScrapDisplay(bool state)
    {
        GetCurrentController<UpgradeTerminalUIController>().ScrapPanel.SetActive( state );
    }
    public void FillData()
    {
        GetCurrentController<UpgradeTerminalUIController>().FillData();
    }
    #endregion
    #region LootUI
    /// <summary>
    /// Send loot info to the UI.
    /// </summary>
    /// <param name="Scrap"></param>
    /// <param name="Items"></param>
    /// <param name="Chips"></param>
    public void SendLoot(int Scrap,List<Item> Items, List<NewChip> Chips)
    {
        GetCurrentController<LootUiController>().LootScrap=Scrap;
        GetCurrentController<LootUiController>().LootItems.AddRange(Items);
        GetCurrentController<LootUiController>().LootChips.AddRange(Chips);
        GetCurrentController<LootUiController>().UpdateLootScreen();
    }
    public void SelectedChipToReplace(NewChip selectedChip)
    {
        GetCurrentController<LootUiController>().BringUpChipSelection(selectedChip);
    }
    public void SelectedChipToReplaceWith(NewChip replaceChip)
    {
        GetCurrentController<LootUiController>().ReplaceChip(replaceChip);
    }
    public void DropLoot(NewChip selection)
    {
        GetCurrentController<LootUiController>().LootChips.Remove(selection);
    }
    public void DropLoot(Item selection)
    {
        GetCurrentController<LootUiController>().LootItems.Remove(selection);
    }
    public void AddItemToInventory(Item item)
    {
        GearManager.Instance.Acquire(item);
        GetCurrentController<LootUiController>().LootItems.Remove(item);
        GetCurrentController<LootUiController>().UpdateLootScreen();
    }
    #endregion
    #region SettingUI
    /// <summary>
    /// Opens the settings UI
    /// </summary>
    /// <param name="context"></param>
    private void ToggleSettings(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Roaming)
        {
            if (CurrentUI.name == InventoryUI.name)
            {
                SwitchScreen(RoamingAndCombatUI);
            }
            else
            {
                SwitchScreen(SettingsUI);
            }
        }

    }

    /// <summary>
    /// Override for open settings ui
    ///If settings breaks it's 10000% this but I think it's working
    /// </summary>
    /// <param name="context"></param>
    public void ToggleSettings()
    {
        if (CurrentUI.name == InventoryUI.name)
        {
            SwitchScreen(RoamingAndCombatUI);
        }
        else
        {
            SwitchScreen(SettingsUI);
        }
    }
    /// <summary>
    /// Toggle settings at Title.
    /// </summary>
    private void ToggleSettingsAtTitle()
    {
        SettingsUI.transform.GetComponent<SettingsUIController>().miniSkip = true;
        SwitchScreen(SettingsUI);
    }

    /// <summary>
    /// Closes the settings UI on a button click
    /// </summary>
    public void CloseSettingsOnClick()
    {
        SwitchScreen(RoamingAndCombatUI);
    }
    public void CloseSettingsOnClickTitle()
    {
        Destroy(CurrentUI);
        SettingsUI.transform.GetComponent<SettingsUIController>().miniSkip = false;
    }
    #endregion
    /// <summary>
    /// Get Current controller for UI.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetCurrentController<T>() where T : UiController
    {
        T controller = currentController as T;
        if (controller == null)
        {
            Debug.LogWarning($"[UiManager] Current controller is not of type {typeof(T)}.");
        }
        return controller;
    }
    private void UpdateUIForGameMode()
    {       

        // Update UI elements based on the game mode
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameManager.GameMode.Title:
                //Delete current UI from scene
                if (CurrentUI != null)
                    Destroy(CurrentUI);

                GameObject.Find("Options Button").GetComponent<Button>().onClick.RemoveAllListeners();
                GameObject.Find("Options Button").GetComponent<Button>().onClick.AddListener(ToggleSettingsAtTitle);
                break;
            case GameManager.GameMode.Loading:
                //Delete current UI from scene
                if (CurrentUI != null)
                    Destroy(CurrentUI);
                break;
            case GameManager.GameMode.Settings:
            case GameManager.GameMode.Credits:
                break;
            case GameManager.GameMode.Combat:
            case GameManager.GameMode.Roaming:
                SwitchScreen(listOfUis.Find(ui => ui.name == RoamingAndCombatUI.name));
                break;
            case GameManager.GameMode.Pause:
                Debug.Log("[UiManager] Displaying Pause Menu.");
                break;
            case GameManager.GameMode.Interacting:
                SwitchScreen(listOfUis.Find(ui => ui.name == TerminalUI.name));
                break;
            case GameManager.GameMode.CombatLoot:
                SwitchScreen(listOfUis.Find(ui => ui.name == LootUI.name));
                break;            
            default:
                Debug.Log("[UiManager] Hiding all UI.");
                break;
        }
    }
    private void SwitchScreen(GameObject targetScreen)
    {
        if (listOfUis.Count == 0)
        {
            Debug.LogError("Ui list is empty!");
            return;
        }
        if (targetScreen == null)
        {
            Debug.LogError($"[UiManager] No UI prefab found for mode: {GameManager.Instance.CurrentGameMode}");
            return;
        }

        // Check if the target screen is already active
        if (CurrentUI != null && CurrentUI.name == targetScreen.name)
        {
            Debug.Log($"[UiManager] Target screen '{targetScreen.name}' is already active.");
            return;
        }

        // Destroy current UI if it exists
        if (CurrentUI != null)
        {
            Destroy(CurrentUI);
        }
        // Instantiate the target UI prefab under this object's transform (Canvas)
        CurrentUI = Instantiate(targetScreen, transform);

        CurrentUI.name=targetScreen.name;

        currentController = CurrentUI.GetComponent<UiController>();
        currentController?.Initialize();

        // Optionally reset the local position, rotation, and scale
        CurrentUI.transform.localPosition = Vector3.zero;
        CurrentUI.transform.localRotation = Quaternion.identity;
        CurrentUI.transform.localScale = Vector3.one;
    }
    private void StartCombat()
    {
        GetCurrentController<RoamingAndCombatUiController>().SwitchMode(true);
    }

    private void EndCombat()
    {
        GetCurrentController<RoamingAndCombatUiController>().SwitchMode(false);
    }

    private void SceneChange(Levels newLevel)
    {

    }    

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStartCombat -= StartCombat;
            GameManager.Instance.OnEndCombat -= EndCombat;
            GameManager.Instance.OnSceneChange -= SceneChange;
            GameManager.Instance.OnGameModeChanged -= UpdateUIForGameMode;
        }
    }
    void OnDisable()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Inventory.performed -= ToggleInventory;
            playerInputActions.Player.Settings.performed -= ToggleSettings;
        }
    }
}
