using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    public GameObject GameOverUI;

    public GameObject Popup;

    private UiController currentController;
    private static UiManager instance;

    private GameManager.GameMode GameMode;

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
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.GameOver || GameManager.Instance.CurrentGameMode == GameManager.GameMode.BrowseringInventory)
            return;

        //Activate UI Obejct        
        GetCurrentController<RoamingAndCombatUiController>().CameraIndicator.SetActive(true);

        GetCurrentController<RoamingAndCombatUiController>().CameraModeIndicatorController.SwitchIndicatorTo(cameraState);
    }
    public void UpdateHealth(float currentHealth, float MaxHealth)
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameManager.GameMode.Interacting:
            case GameManager.GameMode.GameOver:
            case GameManager.GameMode.Credits:
                break;
            default:
                GetCurrentController<RoamingAndCombatUiController>().UpdateHealth(currentHealth, MaxHealth);
                break;
        }        
    }
    public void UpdateShield(float currentShield, float MaxShield)
    {
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.GameOver)
            return;
        GetCurrentController<RoamingAndCombatUiController>().UpdateShield(currentShield, MaxShield);
    }
    public void UpdateEnergy(float currentEnergy, float MaxEnergy)
    {
        GetCurrentController<RoamingAndCombatUiController>().UpdateEnergy(currentEnergy, MaxEnergy);
    }
    public void ChangeCombatScreenTemp(bool Interact)
    {
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.GameOver)
            return;

        GetCurrentController<RoamingAndCombatUiController>().ChangeCombatScreenTemp(Interact);
    }
    /// <summary>
    /// Change the state of the buttons on combat wheel
    /// </summary>
    /// <param name="State"></param>
    public void ChangeStateOfGear(bool State)
    {
        GetCurrentController<RoamingAndCombatUiController>().MakeGearInteractable(State);
    }
    /// <summary>
    /// Update gear buttons
    /// </summary>
    /// <param name="energy"></param>
    public void UpdateGearButtonsStates(float energy)
    {
        var controller = GetCurrentController<RoamingAndCombatUiController>();
        if (controller != null)
        {
            controller.UpdateGearButtonStates(energy);
        }
    }
    /// <summary>
    /// Update the Player Effects Panel
    /// </summary>
    /// <param name="activeEffects"></param>
    public void UpdateEffects(List<Effects.StatusEffect> activeEffects)
    {
        var controller = GetCurrentController<RoamingAndCombatUiController>();
        if (controller != null)
        {
            controller.UpdateEffects(activeEffects);
        }

    }
    /// <summary>
    /// Check if player can make anymore moves.
    /// </summary>
    public void CanMakeAnyMoreMoves()
    {
        if (
            GameObject.Find("Player").GetComponent<PlayerController>().Energy == 0
            &&
            ChipManager.Instance.IsHandEmpty
            )
        {
            GetCurrentController<RoamingAndCombatUiController>().PlayerHand.GetComponent<PlayerHandContainer>().TogglePanel(PlayerHandContainer.PlayerHandState.Hide);
            GetCurrentController<RoamingAndCombatUiController>().EndTurnButtonAnimator.SetTrigger("Click Me");
        }
    }
    /// <summary>
    /// Setup screen for CombatMode
    /// </summary>
    public void StartCombat()
    {
        GetCurrentController<RoamingAndCombatUiController>().StartPrepCombatStart();
    }

    /// <summary>
    /// Setup screen for puzzle combat mode
    /// </summary>
    public void StartPuzzleCombat()
    {
        GetCurrentController<RoamingAndCombatUiController>().StarPrepCombatStartPuzzle();
    }

    /// <summary>
    /// Ends the screen for puzzle combat
    /// </summary>
    public void EndPuzzleCombat()
    {
        GetCurrentController<RoamingAndCombatUiController>().RemoveCombatUIPuzzle();
    }
    /// <summary>
    /// Remove combat screen for roaming
    /// </summary>
    public void EndCombat()
    {
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.GameOver)
            GetCurrentController<RoamingAndCombatUiController>().RemoveCombatUI();
    }
    #endregion
    #region InventoryUI
    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (context.performed 
            && 
            (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Roaming|| GameManager.Instance.CurrentGameMode == GameManager.GameMode.BrowseringInventory))
        {
            if (CurrentUI.name == InventoryUI.name )
            {
                SwitchScreen(RoamingAndCombatUI);
                GameManager.Instance.UpdateGameMode(GameManager.GameMode.Roaming);
            }
            else
            {
                SwitchScreen(InventoryUI);
                GameManager.Instance.UpdateGameMode(GameManager.GameMode.BrowseringInventory);
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
    public void UpdateItemsInTermianl()
    {
        GetCurrentController<UpgradeTerminalUIController>().LoadItemsIntoTerminal();
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
        GetCurrentController<LootUiController>().UpdateLootScreen();
    }
    public void DropLoot(Item selection)
    {
        GetCurrentController<LootUiController>().LootItems.Remove(selection);
        GetCurrentController<LootUiController>().UpdateLootScreen();
    }
    public void AddItemToInventory(Item item)
    {
        Item gearInstance = Instantiate(item);
        GearManager.Instance.Acquire(gearInstance);
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
        if (context.performed && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Roaming || context.performed && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Dialogue || context.performed && GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
        {
            if (CurrentUI.name == InventoryUI.name)
            {
                AdditiveSceneLoadandUnload("Settings", true);
            }
            else
            {
                AdditiveSceneLoadandUnload("Settings", false);
            }
        }

    }

    /// <summary>
    /// Override for open settings ui
    /// </summary>
    /// <param name="context"></param>
    public void ToggleSettings()
    {
        if (CurrentUI.name == InventoryUI.name)
        {
            AdditiveSceneLoadandUnload("Settings", true);
        }
        else
        {
            AdditiveSceneLoadandUnload("Settings", false);
        }
    }
    /// <summary>
    /// Toggle settings at Title.
    /// </summary>
    private void ToggleSettingsAtTitle()
    {
        AdditiveSceneLoadandUnload("Settings", false);
    }

    /// <summary>
    /// A check for if we are in the title screen for settings UI mainly
    /// </summary>
    /// <returns></returns>
    public bool TitleCheck()
    {
        //Are we in the title sceen
        if (GameObject.Find("Player") == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// We need a way to find title controller in the correct scene for settings UI
    /// </summary>
    /// <param name="name"></param>
    public GameObject FindTitleController(string name)
    {
        return GameObject.Find(name);
    }

    /// <summary>
    /// Closes the settings UI on a button click
    /// </summary>
    public void CloseSettingsOnClick()
    {
        AdditiveSceneLoadandUnload("Settings", true);
    }

    public void AdditiveSceneLoadandUnload(string scene, bool unload)
    {
        if(unload)
        {
            SceneManager.UnloadSceneAsync(scene);
            //Allows the character to move
            GameManager.Instance.UpdateGameMode(GameMode);
        }
        else
        {
            GameMode = GameManager.Instance.CurrentGameMode;
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }
    public void CloseSettingsOnClickTitle()
    {
        Destroy(CurrentUI);
    }
    #endregion
    #region GameOver
    public void ShowGameOverScreen()
    {
        // Switch UI to Game Over Screen
        SwitchScreen(GameOverUI);
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
        EventSystem.current.SetSelectedGameObject(null);

        // Update UI elements based on the game mode
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameManager.GameMode.Title:
                //Delete current UI from scene
                if (CurrentUI != null)
                    Destroy(CurrentUI);

                GameObject.Find("Options Button").GetComponent<Button>().onClick.RemoveAllListeners();
                GameObject.Find("Options Button").GetComponent<Button>().onClick.AddListener(ToggleSettingsAtTitle);

                EventSystem.current.SetSelectedGameObject(GameObject.Find("Play Button"));
                break;
            case GameManager.GameMode.Loading:
            case GameManager.GameMode.Credits:
            case GameManager.GameMode.Won:
            case GameManager.GameMode.Trailer:
                //Delete current UI from scene
                if (CurrentUI != null)
                    Destroy(CurrentUI);
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

    public void SwichScreenPuzzle(GameObject targetScreen)
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
        }

        // Destroy current UI if it exists
        if (CurrentUI != null)
        {
            Destroy(CurrentUI);
        }
        // Instantiate the target UI prefab under this object's transform (Canvas)
        CurrentUI = Instantiate(targetScreen, transform);

        CurrentUI.name = targetScreen.name;

        currentController = CurrentUI.GetComponent<UiController>();
        currentController?.Initialize();

        // Optionally reset the local position, rotation, and scale
        CurrentUI.transform.localPosition = Vector3.zero;
        CurrentUI.transform.localRotation = Quaternion.identity;
        CurrentUI.transform.localScale = Vector3.one;
    }
    /// <summary>
    /// Bring up pop up window to player.
    /// </summary>
    /// <param name="message">Message to tell player.</param>
    /// <param name="CancelButtonVisible">If you want the cancel Button Visible. Default true</param>
    /// <param name="MethodToCallOnConfirm">Method to call when player presses confirm. Default null.</param>
    public void PopUpMessage(string message,Action MethodToCallOnConfirm=null, bool CancelButtonVisible = true)
    {
        GameObject PopUpWindow = GameObject.FindGameObjectWithTag("Confirm");
        if (PopUpWindow == null)
        {
            GameObject temp = Instantiate(Popup, Instance.transform);
            temp.GetComponent<ConfirmationWindow>().SetUpComfirmationWindow(message, CancelButtonVisible, MethodToCallOnConfirm);            
        }
        else
        {
            PopUpWindow.GetComponent<Animator>().SetTrigger("Alert");
        }
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
