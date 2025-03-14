using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeTerminalUIController : UiController, IPointerClickHandler
{          

    [Header("Scrap Display")]
    public GameObject ScrapPanel;
    private int currentScrapAmount;
    public TextMeshProUGUI ScrapTextAmount;

    [Header("Intro Screen")]
    public GameObject IntroPanel;
    public TMP_Text IntroConsole;    

    [Header("Heal Upgrade Screen")]
    public GameObject HealthPanel;
    public TMP_Text HealthConsole;

    [Header("Chip Upgrade Screen")]
    public GameObject ChipPrefab;
    public GameObject ChipPanel;    
    public GameObject ChipSelectionPanel;
    public GameObject ChipHolder;
    public GameObject CancelButton;

    public TMP_Text ChipConsole;

    [Header("Data Screen")]
    public GameObject DataPrefab;
    public GameObject DataPanel;
    public GameObject DataConsolePanel;
    public TMP_Text DataConsole;
    public GameObject ViewDataPanel;
    public GameObject DataHolder;
    public GameObject ViewDataExitButton;
    public GameObject SaveDataPanel;
    public TMP_Text SaveDataConsole;
    public GameObject UserInput;
    public GameObject UploadButton;
    public GameObject ExitButton;
    
    public Vector2 startPosition;    
    public Vector2 endPosition;

    public float TimeForPanelToGetToCenter = 2.0f;

    [Header("Item Screen")]
    public GameObject ItemPanel;
    public GameObject ItemSelectionContainer;
    public GameObject GearPrefab;
    public GameObject ItemScreenExitButton;

    [Header("Error Screen")]
    public GameObject ErrorPanel;
    public TMP_Text ErrorConsole;

    public List<GameObject> allUIPanels;
    public List<TMP_Text> Consoles = new List<TMP_Text>();
    public bool IsWaitingForInput
    {
        get
        {
            return isWaitingForInput;
        }
        set
        {
            isWaitingForInput = value;
        }
    }

    private bool isWaitingForInput;
    private TerminalController controller;
    private Coroutine activeRevealCoroutine;
    private Coroutine activePanelCoroutine;
    void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("UpgradeController").GetComponent<TerminalController>();
    }
    void OnEnable()
    {
        controller.OnScreenChanged += UpdateUIScreen;
        controller.OnErrorOccurred += UpdateErrorScreen;
    }

    void OnDisable()
    {
        controller.OnScreenChanged -= UpdateUIScreen;
        controller.OnErrorOccurred -= UpdateErrorScreen;
    }
    public override void Initialize()
    {
        Debug.Log("Upgrade Terminal Initalize");

        UpdateScrapDisplay(GameObject.Find("Player").GetComponent<PlayerController>().Scrap);
    }
    /// <summary>
    /// When Player clicks one of the links.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsWaitingForInput)
            foreach (var console in Consoles)
            {
                // Skip inactive components
                if (console == null || !console.gameObject.activeInHierarchy)
                    continue;

                int linkIndex = TMP_TextUtilities.FindIntersectingLink(console, eventData.position, null);

                if (linkIndex != -1) // If a link was clicked
                {
                    TMP_LinkInfo linkInfo = console.textInfo.linkInfo[linkIndex];
                    string linkID = linkInfo.GetLinkID();

                    // Handle each link based on its ID
                    HandleLinkClick(linkID);
                    return; // Exit after handling the first valid link click
                }
            }
    }

    /// <summary>
    /// Fill screen with all save data.
    /// </summary>
    public void FillData()
    {
        IsWaitingForInput = false;

        // Remove all existing child objects from DataHolder
        foreach (Transform child in DataHolder.transform)
        {
            if(child != null)
                Destroy(child.gameObject);
        }

        List<GameData> saves = DataManager.Instance.GetAllSaves();
        foreach (var save in saves)
        {
            GameObject UIData = Instantiate(DataPrefab, DataHolder.transform);

            UIData.GetComponent<DataPrefabController>().UpgradeController = controller;
            UIData.GetComponent<DataPrefabController>().Name = save.SaveName;
            UIData.GetComponent<DataPrefabController>().Level=save.Level.ToString();
            UIData.GetComponent<DataPrefabController>().Time = save.TimeStampString;
            
        }

    }

    /// <summary>
    /// Update the display amount of scrap
    /// </summary>
    public void UpdateScrapDisplay(int NewScrapAmount)
    {        
        StartCoroutine(UpdateScrapAmount(NewScrapAmount));
    }

    /// <summary>
    /// Loads the terminal with items
    /// </summary>
    public void LoadItemsIntoTerminal()
    {
        // Remove any previous items displayed
        foreach (Transform child in ItemSelectionContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Get all player-owned items
        foreach (Item item in GearManager.Instance.PlayerCurrentGear)
        {
            GameObject itemUI = Instantiate(GearPrefab, ItemSelectionContainer.transform);
            itemUI.GetComponent<GearInventory>().Item = item;
            itemUI.GetComponent<GearInventory>().PrefabMode = GearInventory.Mode.Terminal;
        }
    }

    /// <summary>
    /// When UI link is clicked this method is called.
    /// </summary>
    /// <param name="linkID"></param>
    private void HandleLinkClick(string linkID)
    {
        switch (linkID)
        {
            case"UpgradeHealthScreen":

                controller.SwitchToScreen(TerminalController.Screens.HealthUpgrade);

                break;
            case "UpgradeHealth":

                controller.AttemptToUpgradeHealth();

                break;
            case "UpgradeChipScreen":
                controller.SwitchToScreen(TerminalController.Screens.ChipUpgrade);
                break;
            case "UpgradeSelectedChip":               
                controller.AttemptToUpgradeChip();
                break;
            case "DataServer":
                controller.SwitchToScreen(TerminalController.Screens.Data);
                break;
            case "View":
                controller.currentDataMode=TerminalController.DataMode.View;

                controller.SwitchToScreen(TerminalController.Screens.Data);
               
                break;
            case "Save":
                controller.currentDataMode = TerminalController.DataMode.Save;

                controller.SwitchToScreen(TerminalController.Screens.Data);
                break;
            case "Items":
                controller.SwitchToScreen(TerminalController.Screens.Items); 
                break;
            case "Back":
                controller.SwitchToScreen(TerminalController.Screens.Intro);
                break;
            case "Exit":
            case "Exit1":
            case "Exit2":
                controller.SwitchToScreen(TerminalController.Screens.Exit);
                ExitButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ViewDataExitButton.GetComponent<Button>().onClick.RemoveAllListeners();
                UserInput.SetActive(false);
                ExitButton.SetActive(false);                
                break;
            default:

            break;
        }
    }

    /// <summary>
    /// Switches to the specified UI screen and deactivates all others.
    /// </summary>
    /// <param name="screen">The UI screen to activate.</param>
    private void UpdateUIScreen(TerminalController.Screens screen)
    {
        StopAndClearCoroutine(ref activePanelCoroutine);
        StopAndClearCoroutine(ref activeRevealCoroutine);

        switch (screen)
        {
            case TerminalController.Screens.Default:
            case TerminalController.Screens.Exit:
                IntroPanel.SetActive(false);
                HealthPanel.SetActive(false);
                ChipPanel.SetActive(false);
                DataPanel.SetActive(false);
                ErrorPanel.SetActive(false);
                break;
            case TerminalController.Screens.Intro:

                SetActiveUIElement(IntroPanel);

                activeRevealCoroutine = StartCoroutine(RevealText(IntroConsole, true, 0.01f, false,0f,0,false,0));
                break;
            case TerminalController.Screens.HealthUpgrade:

                SetActiveUIElement(HealthPanel);

                PlayerController tempPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                string tempText = string.Format("Getting <color=#A20000>*Error*</color> Health.\n" +
                "Current Health is <color=#A20000>{0}</color> of Max Health <color=#A20000>{1}</color>.\n" +
                "Will cost <b>150</b> Scrap to Upgrade to <color=#A20000>{2}</color>.\n" +
                "<color=#0000FF><b><u><link=\"UpgradeHealth\">Upgrade</link></u></b></color>\n" +
                "<color=#0000FF><b><u><link=\"Exit\">Exit</link></u></b></color>\n" +
                "<color=#0000FF><u><link=\"Back\">Back</link></u></color>",
                tempPlayer.Health, tempPlayer.MaxHealth, tempPlayer.MaxHealth + 10);

                HealthConsole.SetText(tempText);

                activeRevealCoroutine = StartCoroutine(RevealText(HealthConsole, false, 0.01f, false, 0f,0,false,0));
                break;
            case TerminalController.Screens.ChipUpgrade:

                SetActiveUIElement(ChipPanel);

                if (controller.SelectedChip == null)
                {

                    StartCoroutine(RevealText(ChipConsole, true, 0.01f, true,0.1f,5,false,0));
                    StartCoroutine(BringUpChipSelector());

                    CancelButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    CancelButton.GetComponent<Button>().onClick.AddListener(() => controller.SwitchToScreen(TerminalController.Screens.Intro));
                }
                else
                {
                    ChipConsole.ForceMeshUpdate();

                    ChipSelectionPanel.SetActive(false);

                    ChipConsole.textInfo.Clear();

                    string tempText2 = string.Format("Chip inserted.\n" +
                        "...Loading Chip...\n" +
                        "Chip Info:\n" +
                        "Chip Rarity - {0}\n" +
                        "Chip Name - {1}\n" +
                        "Chip Description - {2}\n" +
                        "Cost to upgrade - <b>{3}</b> Scrap.\n-----\n" +
                        "<color=#0000FF><b><u><link=\"UpgradeSelectedChip\">Upgrade Chip</color></link></u></b>\n" +
                        "<color=#0000FF><b><u><link=\"Exit2\">Exit</color></link></u></b>\n" +
                        "<color=#0000FF><u><link=\"Back\">Back</color></link></u>",
                    controller.SelectedChip.chipRarity, controller.SelectedChip.chipName, controller.SelectedChip.description, controller.SelectedChip.costToUpgrade);



                    ChipConsole.SetText(tempText2);

                    IntroConsole.ForceMeshUpdate();
                    HealthConsole.ForceMeshUpdate();
                    ChipConsole.ForceMeshUpdate();  
                    ErrorConsole.ForceMeshUpdate();



                    StartCoroutine(RevealText(ChipConsole, true, 0.01f, false, 0f,0,false,0));

                }

                break;
            case TerminalController.Screens.Items:
                SetActiveUIElement(ItemPanel);
                LoadItemsIntoTerminal();

                ItemScreenExitButton.SetActive(true);
                ItemScreenExitButton.GetComponent<Button>().onClick.AddListener(() => controller.SwitchToScreen(TerminalController.Screens.Exit));
                break;
            case TerminalController.Screens.Data:

                SetActiveUIElement(DataPanel);

                string tempDataText="";

                switch (controller.currentDataMode)
                {
                    case TerminalController.DataMode.Title:
                        tempDataText = string.Format("<#80ff80>....Connecting To Data Servers.....</color>\n\n" +
                            "User <#A20000> *Error*</color> backup data have been found.\n\n" +
                            "<#80ff80>....Loading Options...</color>\n\n" +
                            "<color=#0000FF><link=\"View\"><u>View all backups</u></link></color>\n\n" +
                            "<color=#0000FF><link=\"Save\"><u>Create new backup</u></link></color>\n\n" +
                            "<color=#0000FF><link=\"Exit\"><u>Exit</u></link></color>");

                        DataConsolePanel.SetActive(true);
                        SaveDataPanel.SetActive(false);
                        DataConsole.SetText(tempDataText);

                        StartCoroutine(RevealText(DataConsole, false, 0.01f, false, 0, 0, false, 0));
                        break;
                    case TerminalController.DataMode.View:
                        tempDataText = string.Format("<#80ff80>...Pinging Chip Tech Servers...</color>\n" +
                            "Sending Data Request\n" +
                            "...Retrieving Data...");

                        DataConsolePanel.SetActive(true);
                        SaveDataPanel.SetActive(false);

                        DataConsole.SetText(tempDataText);

                        StartCoroutine(RevealText(DataConsole, true, 0.01f, true, 0.1f, 5, false, 0));
                       

                        FillData();
                        StartCoroutine(BringUpDataScreen());

                        ViewDataExitButton.GetComponent<Button>().onClick.RemoveAllListeners();
                        ViewDataExitButton.GetComponent<Button>().onClick.AddListener(() => controller.SwitchToScreen(TerminalController.Screens.Exit));
                        break;
                    case TerminalController.DataMode.Save:
                        tempDataText = string.Format("<#80ff80>...Pinging Chip Tech Servers...</color>\r\n\r\n" +
                            "Preparing to send copy of memory Ciruits.\r\n" +
                            "                    _____________\r\n" +
                            "Enter Name of Copy: |_____________|");

                        DataConsolePanel.SetActive(false);
                        SaveDataPanel.SetActive(true);

                        SaveDataConsole.SetText(tempDataText);

                        StartCoroutine(RevealText(SaveDataConsole, false, 0.01f, false, 0, 0, false, 0f));

                        // Start RevealText coroutine and pass a callback for activation
                        StartCoroutine(RevealTextWithCallback(SaveDataConsole, false, 0.01f, false, 0, 0, false, 0f, () =>
                        {
                            // Activate UserInput and ExitButton after the text is fully revealed
                            UserInput.SetActive(true);
                            ExitButton.SetActive(true);

                            ExitButton.GetComponent<Button>().onClick.RemoveAllListeners();

                            ExitButton.GetComponent<Button>().onClick.AddListener(() => controller.SwitchToScreen(TerminalController.Screens.Exit));
                            UploadButton.GetComponent<Button>().onClick.AddListener(() => controller.AttemptToSave());
                        }));

                        break;                    
                }
                
                break;
            case TerminalController.Screens.Error:
                SetActiveUIElement(ErrorPanel);

                StartCoroutine(RevealText(ErrorConsole, false, 0.01f, false, 0f, 0, false, 0f));
                break;                            
            default:
                Debug.LogWarning("There should always be an option here.");
                break;
        }
    }

    /// <summary>
    /// Active Required Panel.
    /// Since dont have multiple canvases and use panels just one line of code to active one panel.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="isActive"></param>
    private void SetActiveUIElement(GameObject targetPanel)
    {
        if(allUIPanels.Count== 0)
        {
            Debug.LogError("Panels are empty!");
        }
        foreach (var panel in allUIPanels)
        {
            panel.SetActive(panel == targetPanel);
        }
    }

    /// <summary>
    /// Error Message.
    /// </summary>
    /// <param name="message"></param>
    private void UpdateErrorScreen(string message)
    {
        ErrorConsole.SetText(message);
    }    

    /// <summary>
    /// Populate the panel with Chips first and then,
    /// Bring up window to select Chip.
    /// </summary>
    private IEnumerator BringUpChipSelector()
    {
        IsWaitingForInput = false;

        ChipHolder.SetActive(false);

        foreach(NewChip newChip in ChipManager.Instance.PlayerDeck)
        {
            if (newChip.canBeUpgraded)
            {
                GameObject UIChip = Instantiate(ChipPrefab, ChipHolder.transform);

                UIChip.GetComponent<Chip>().NewChip = newChip;

                UIChip.GetComponent<Chip>().SetChipModeTo(Chip.ChipMode.WorkShop);                                
            }
            
        }

        ChipHolder.SetActive(true);

        float tempTime = 0;
        RectTransform tempRectTransform = ChipSelectionPanel.GetComponent<RectTransform>();

        while (tempTime < TimeForPanelToGetToCenter)
        {
            tempTime += Time.deltaTime;

            float temp = Mathf.Clamp01(tempTime / TimeForPanelToGetToCenter);

            tempRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, temp);

            yield return null;
        }

        tempRectTransform.anchoredPosition=endPosition;

        CancelButton.SetActive(true);
    }    

    /// <summary>
    /// Bring Up window to select Data
    /// </summary>
    /// <returns></returns>
    private IEnumerator BringUpDataScreen()
    {
        ViewDataPanel.SetActive(true);
        DataHolder.SetActive(true);        

        float tempTime = 0;
        RectTransform tempRectTransform = ViewDataPanel.GetComponent<RectTransform>();

        while (tempTime < TimeForPanelToGetToCenter)
        {
            tempTime += Time.deltaTime;

            float temp = Mathf.Clamp01(tempTime / TimeForPanelToGetToCenter);

            tempRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, temp);

            yield return null;
        }

        tempRectTransform.anchoredPosition = endPosition;
    }

    /// <summary>
    /// Reveals text to the UI base on request.
    /// </summary>
    /// <param name="tmpText"></param>
    /// <param name="byLetter"></param>
    /// <param name="revealSpeed">Lower the number faster it appears.</param>
    /// <param name="timeBeforeRestartAnimation"></param>
    /// <returns></returns>
    private IEnumerator RevealText(TMP_Text tmpText, bool byLetter, float revealSpeed, bool blinkText, float blinkDuration, int blinkCount,bool loopAnimation, float timeBeforeRestartAnimation)
    {
        IsWaitingForInput = false;

        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;


        int totalVisibleCharacters = textInfo.characterCount;
        int totalWordCount = textInfo.wordCount;
        int visibleCount = 0;

        bool hasTextChanged = false;
        int counter = 0;
        int currentWord = 0;

        while (true)
        {
            if (hasTextChanged)
            {
                totalVisibleCharacters = textInfo.characterCount;
                hasTextChanged = false;
            }

            if (byLetter)
            {
                if (visibleCount > totalVisibleCharacters)
                {                   
                    if (blinkText)
                        yield return StartCoroutine(BlinkText(tmpText, blinkDuration, blinkCount));

                    if (loopAnimation)
                    {
                        yield return new WaitForSeconds(timeBeforeRestartAnimation); // Pause before restarting
                        visibleCount = 0;
                    }
                    else
                    {
                        IsWaitingForInput = true;
                        break;
                    }
                }

                tmpText.maxVisibleCharacters = visibleCount; // Set visible characters
                visibleCount += 1;
                yield return new WaitForSeconds(revealSpeed);
            }
            else
            {
                currentWord = counter % (totalWordCount + 1);

                // Determine visible character count based on the current word
                if (currentWord == 0)
                    visibleCount = 0;
                else if (currentWord < totalWordCount)
                    visibleCount = textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
                else if (currentWord == totalWordCount)
                    visibleCount = totalVisibleCharacters;

                tmpText.maxVisibleCharacters = visibleCount;

                if (visibleCount >= totalVisibleCharacters)
                {
                    if (blinkText)
                        yield return StartCoroutine(BlinkText(tmpText, blinkDuration, blinkCount));

                    if (loopAnimation)
                        yield return new WaitForSeconds(timeBeforeRestartAnimation);
                    else
                    {
                        IsWaitingForInput = true;
                        break;
                    }
                }

                counter += 1;
                yield return new WaitForSeconds(revealSpeed * 10); // Adjust speed for word reveal
            }
        }        
    }

    /// <summary>
    /// This is for when you want something to be done after the Text is revealed
    /// </summary>
    /// <param name="tmpText"></param>
    /// <param name="byLetter"></param>
    /// <param name="revealSpeed"></param>
    /// <param name="blinkText"></param>
    /// <param name="blinkDuration"></param>
    /// <param name="blinkCount"></param>
    /// <param name="loopAnimation"></param>
    /// <param name="timeBeforeRestartAnimation"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator RevealTextWithCallback(TMP_Text tmpText, bool byLetter, float revealSpeed, bool blinkText, float blinkDuration, int blinkCount, bool loopAnimation, float timeBeforeRestartAnimation, Action onComplete)
    {
        IsWaitingForInput = false;

        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;
        int totalVisibleCharacters = textInfo.characterCount;
        int visibleCount = 0;

        while (visibleCount <= totalVisibleCharacters)
        {
            tmpText.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(revealSpeed);
        }

        // Perform any blinking if needed
        if (blinkText)
        {
            yield return StartCoroutine(BlinkText(tmpText, blinkDuration, blinkCount));
        }

        yield return new WaitForSeconds(1f);

        // Execute the callback after revealing the text
        onComplete?.Invoke();
    }

    private IEnumerator BlinkText(TMP_Text tmpText, float blinkDuration, int blinkCount)
    {
        int totalVisibleCharacters = tmpText.textInfo.characterCount;
        for (int i = 0; i < blinkCount; i++)
        {
            tmpText.maxVisibleCharacters = 0;
            yield return new WaitForSeconds(blinkDuration);

            tmpText.maxVisibleCharacters = totalVisibleCharacters;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private IEnumerator UpdateScrapAmount(int newScrapAmount)
    {
        int startingAmount = Int32.Parse(ScrapTextAmount.text);
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            currentScrapAmount = Mathf.RoundToInt(Mathf.Lerp(startingAmount, newScrapAmount, elapsed / 2f));
            ScrapTextAmount.SetText(currentScrapAmount.ToString());
            yield return null;
        }

        // Ensure the final value is set
        currentScrapAmount = newScrapAmount;
        ScrapTextAmount.SetText(currentScrapAmount.ToString());
    }

    private void StopAndClearCoroutine(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}