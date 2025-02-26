using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalController : MonoBehaviour
{
    private Coroutine activeScreenCoroutine;
    private Coroutine activeTextRevealCoroutine;

    private bool isInteracting;
    private NewChip selectedChip;

    // to avoid using getComponent repeatdly
    private TextMeshPro defaultScreenText;
    private TextMeshPro introScreenText;
    private TextMeshPro healthUpgradeScreenText;
    private TextMeshPro chipUpgradeScreenText;
    private TextMeshPro itemScreenText;
    private TextMeshPro dataScreenText;
    private TextMeshPro errorScreenText;

    /// <summary>
    /// If Player is interacting with terminal
    /// </summary>
    public bool IsInteractingWithMe
    {
        get
        {
            return isInteracting;
        }
        private set
        {
            isInteracting = value;           
        }
    }  

    /// <summary>
    /// Chip Player has selected
    /// </summary>
    public NewChip SelectedChip
    {
        get
        {
            return selectedChip;
        }
        private set
        {
            selectedChip = value;
        }
    }

    public enum DataMode
    {
        Title,
        View,
        Save
    }

    //Current Data mode the terminal is at.
    public DataMode currentDataMode;

    /// <summary>
    /// Created event for UI.
    /// </summary>
    public event Action<Screens> OnScreenChanged;

    /// <summary>
    /// When ever there is an error
    /// </summary>
    public event Action<string> OnErrorOccurred;

    [Header("Screens In Game World")]
    public List<GameObject> AllScreens;
    public GameObject DefaultScreen;
    public GameObject IntroScreen;
    public GameObject HealthUpgradeScreen;
    public GameObject ChipUpgradeScreen;
    public GameObject ItemScreen;
    public GameObject DataScreen;
    public GameObject ErrorScreen;

    public enum Screens
    {
        Default,
        Intro,
        HealthUpgrade,
        ChipUpgrade,
        Data,
        Items,
        Error,
        Exit
    }

    //Current Screen Active in game world
    private Screens currentScreen;

    private CameraController Camera;

    void Awake()
    {
        defaultScreenText = DefaultScreen.GetComponent<TextMeshPro>();
        introScreenText = IntroScreen.GetComponent<TextMeshPro>();
        healthUpgradeScreenText = HealthUpgradeScreen.GetComponent<TextMeshPro>();
        chipUpgradeScreenText = ChipUpgradeScreen.GetComponent<TextMeshPro>();
        dataScreenText = DataScreen.GetComponent<TextMeshPro>();
        errorScreenText = ErrorScreen.GetComponent<TextMeshPro>();
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SwitchToScreen(Screens.Default);        
    }

    /// <summary>
    /// Switches to the specified screen and deactivates the current screen.
    /// </summary>
    /// <param name="screen">The screen itemType to switch to.</param>
    public void SwitchToScreen(Screens screen)
    {
        // Stop specific coroutines before starting new ones
        StopAndClearCoroutine(ref activeScreenCoroutine);
        StopAndClearCoroutine(ref activeTextRevealCoroutine);

        OnScreenChanged?.Invoke(screen);
        

        switch (screen)
        {
            case Screens.Default:
                SetActiveScreen(DefaultScreen);

                activeTextRevealCoroutine = StartCoroutine(RevealText(DefaultScreen, true, 0.01f,true, 1, 3,true,5f));

                currentScreen = screen;
                break;
            case Screens.Intro:
                SetActiveScreen(IntroScreen);


                activeTextRevealCoroutine = StartCoroutine(RevealText(IntroScreen, false, 0.01f, false, 0, 0,false, 1000f));


                currentScreen = screen;
                currentDataMode = DataMode.Title;

                break;
            case Screens.HealthUpgrade:
                SetActiveScreen(HealthUpgradeScreen);
                

                PlayerController tempPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

                string tempText = string.Format("Getting <color=#A20000>*Error*</color> Health.\n" +
                "Current Health is <color=#A20000>{0}</color> of Max Health <color=#A20000>{1}</color>.\n" +
                "Will cost <b>150</b> Scrap to Upgrade to <color=#A20000>{2}</color>.\n" +
                "<color=#0000FF><b><u><link=\"UpgradeHealth\">Upgrade</link></u></b></color>\n" +
                "<color=#0000FF><b><u><link=\"Exit\">Exit</link></u></b></color>\n" +
                "<color=#0000FF><u><link=\"Back\">Back</link></u></color>",
                tempPlayer.Health, tempPlayer.MaxHealth, tempPlayer.MaxHealth + 10);


                healthUpgradeScreenText.SetText(tempText);

                activeTextRevealCoroutine = StartCoroutine(RevealText(HealthUpgradeScreen,false,0.01f,false,0,0,false, 10000f));

                currentScreen = screen;
                break;
            case Screens.Data:
                SetActiveScreen(DataScreen);
                string tempDataText = string.Format("Data");

                switch (currentDataMode)
                {
                    case TerminalController.DataMode.Title:
                        tempDataText = string.Format("<#80ff80>....Connecting To Data Servers.....</color>\n\n" +
                            "User <#A20000> *Error*</color> backup data have been found.\n\n" +
                            "<#80ff80>....Loading Options...</color>\n\n" +
                            "<color=#0000FF><link=\"View\"><u>View all backups</u></link></color>\n\n" +
                            "<color=#0000FF><link=\"Save\"><u>Create new backup</u></link></color>\n\n" +
                            "<color=#0000FF><link=\"Exit\"><u>Exit</u></link></color>");

                        dataScreenText.SetText(tempDataText);

                        StartCoroutine(RevealText(DataScreen, false, 0.01f, false, 0, 0, false, 0));
                        break;
                    case TerminalController.DataMode.View:
                        tempDataText = string.Format("<#80ff80>...Pinging Chip Tech Servers...</color>\n" +
                            "Sending Data Request\n" +
                            "...Retrieving Data...");
                        
                        dataScreenText.SetText(tempDataText);

                        StartCoroutine(RevealText(DataScreen, true, 0.01f, true, 0.1f, 5, false, 0));

                        break;
                    case TerminalController.DataMode.Save:
                        tempDataText = string.Format("<#80ff80>...Pinging Chip Tech Servers...</color>\n" +
                            "Preparing to send copy of memory Ciruits.");

                        dataScreenText.SetText(tempDataText);

                        StartCoroutine(RevealText(DataScreen, true, 0.01f, true, 0.1f, 5, false, 0));
                        break;                    
                }                

                currentScreen = screen;
                break;
            case Screens.ChipUpgrade:

                SetActiveScreen(ChipUpgradeScreen);

                if (SelectedChip == null)
                {
                    StartCoroutine(RevealText(ChipUpgradeScreen, true, 0.01f, true, 0.1f, 5, false, 100f));
                }
                else
                {

                    string tempText2 = string.Format("Chip inserted.\n" +
                        "...Loading Chip...\n" +
                        "Chip Info:\n" +
                        "Chip Rarity - {0}\n" +
                        "Chip Name - {1}\n" +
                        "Chip Description - {2}\n" +
                        "Cost to upgrade - <b>{3}</b> Scrap.\n-----\n" +
                        "<color=#0000FF><u><b><link=\"UpgradeSelectedChip\">Upgrade Chip</color></link></b></u>\n" +
                        "<color=#0000FF><u><b><link=\"Exit2\">Exit</color></link></b></u>\n" +
                        "<color=#0000FF><u><link=\"Back\">>Back</color></link></u>",
                    SelectedChip.chipRarity, SelectedChip.chipName, SelectedChip.description, SelectedChip.costToUpgrade);

                    chipUpgradeScreenText.SetText(tempText2);

                    StartCoroutine(RevealText(ChipUpgradeScreen, true, 0.01f, false, 0f, 0, false, 0));                    
                }

                currentScreen = screen;

            break;
            case Screens.Items:
                SetActiveScreen(ItemScreen);
                StartCoroutine(RevealText(ItemScreen, true, 0.01f, false, 0f, 0, false, 0));                
                break;
            case Screens.Error:

                SetActiveScreen(ErrorScreen);

                StartCoroutine(RevealText(ErrorScreen, false, 0.001f, false, 0f,0, false, 0f));
                break;
            case Screens.Exit:

                SetActiveScreen(DefaultScreen);
                

                StartCoroutine(RevealText(DefaultScreen, true, 0.01f, true, 1, 3,true, 5f));

                currentScreen = screen;

                StartCoroutine(ExitTerminal());

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Method for switching screens
    /// </summary>
    /// <param name="targetScreen"></param>
    private void SetActiveScreen(GameObject targetScreen)
    {
        if (AllScreens.Count == 0)
        {
            Debug.LogError("Screens are empty!");
        }
        foreach (var screen in AllScreens)
        {
            screen.SetActive(screen == targetScreen);
        }
    }

    /// <summary>
    /// umm chip selected?
    /// </summary>
    /// <param name="chip"></param>
    public void ChipSelectToUpgrade(NewChip chip)
    {
        selectedChip = chip;
        SwitchToScreen(Screens.ChipUpgrade);
    }

    public void LoadGame(string saveName)
    {
        DataManager.Instance.LoadData(saveName);
        Debug.Log($"Loaded save: {saveName}");

        GameManager.Instance.RequestScene(DataManager.Instance.CurrentGameData.Level);
    }
    /// <summary>
    /// Call DataManager to create save.
    /// </summary>
    public void AttemptToSave()
    {
        try
        {
            DataManager.Instance.Save(UiManager.Instance.GetUserInput());

            //For now lets exit the terminal all together
            SwitchToScreen(Screens.Exit);
        }
        catch
        {
            DisplayError("Failed to Upload Copy");
        }
    }

    public void AttemptToDeleteSave(string saveName)
    {
        if (DataManager.Instance.DeleteSave(saveName))
            UiManager.Instance.FillData();
        else
            DisplayError("Failed to Delete Copy.");
    }

    /// <summary>
    /// Try to upgrade Player HealthBar.
    /// </summary>
    public void AttemptToUpgradeHealth()
    {
        PlayerController tempPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (tempPlayer.Scrap >= 150)
        {
            // made this bank for later.
            var Bank = tempPlayer.TakeScrap(150);

            //Updade max HealthBar
            tempPlayer.UpgradeMaxHealth(10);

            //heal by same amount
            tempPlayer.Heal(10);

            //Display new info
            SwitchToScreen(TerminalController.Screens.HealthUpgrade);

            //Refresh Scrap Amount
            UiManager.Instance.UpdateScrapDisplay(
            GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>().Scrap
            );
        }
        else
        {
            DisplayError("<color=#FF0000><size=150%>Error</size></color>\n" +
                "Not enough <b><size=110%>scrap</size></b> to upgrade health.\n" +
                "<u><link=\"Exit\"><color=#808080>Exit</color></link></u>\n" +
                "<u><link=\"Back\"><color=#808080>Back</color></link></u>");
        }
    }

    /// <summary>
    /// Try to Upgrade chip
    /// </summary>
    public void AttemptToUpgradeChip()
    {

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Scrap >= SelectedChip.costToUpgrade)
        {           

            var Bank = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().TakeScrap(SelectedChip.costToUpgrade);

            ChipManager.Instance.PlayerDeck.Find(item => item == SelectedChip).IsUpgraded = true;

            // FOr now lets go back to main menu.
            SwitchToScreen(Screens.Intro);

            //Refresh Scrap Amount
            UiManager.Instance.UpdateScrapDisplay(
            GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>().Scrap
            );
        }
        else
        {
            DisplayError("<color=#FF0000><size=150%>Error</size></color>\n" +
                "Not enough <b><size=110%>scrap</size></b> to upgrade chip.\n" +
                "<u><link=\"Exit\"><color=#808080>Exit</color></link></u>\n" +
                "<u><link=\"Back\"><color=#808080>Back</color></link></u>");
        }
    }

    /// <summary>
    /// Set error message on both screens.
    /// </summary>
    /// <param name="message"></param>
    public void DisplayError(string message)
    {
        errorScreenText.SetText(message);

        // Notify listeners
        OnErrorOccurred?.Invoke(message);

        SwitchToScreen(Screens.Error);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !IsInteractingWithMe)
        {
            StartCoroutine(EnterTerminal());
        }
    }

    /// <summary>
    /// Plays animation and does other stuff before actually accessing terminal.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnterTerminal()
    {
        IsInteractingWithMe = true;

        //Rotate player
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().StartSmoothRotate(DefaultScreen.transform.position);

        //Change player Camera
        Camera.SwitchCamera(CameraController.CameraState.FirstPerson);
        Camera.FirstPersonCamera.LookAt = IntroScreen.transform;      

        yield return new WaitForSeconds(1f);

        //Set GameMode to interacting
        GameManager.Instance.UpdateGameMode(GameManager.GameMode.Interacting);

        SwitchToScreen(Screens.Intro);       

        UiManager.Instance.SetScrapDisplay(true);

        //Refresh Scrap Amount
        UiManager.Instance.UpdateScrapDisplay(
        GameObject.FindGameObjectWithTag("Player")
        .GetComponent<PlayerController>().Scrap
        );

    }

    /// <summary>
    /// Players animation and doe sother stuff so Player can exit terminal.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExitTerminal()
    {
        //deactive Scrap Panel
        //UiManager.Instance.SetScrapDisplay(false);

        GameManager.Instance.UpdateGameMode(GameManager.GameMode.Roaming);

        yield return new WaitForSeconds(1f);
        IsInteractingWithMe = false;        
        
        Camera.SwitchCamera(CameraController.CameraState.Default);
    }    

    /// <summary>
    /// Display Text onto screen based on parameters set.
    /// </summary>
    /// <param name="Screen"></param>
    /// <param name="byLetter"></param>
    /// <param name="revealSpeed"></param>
    /// <param name="blinkText"></param>
    /// <param name="blinkDuration"></param>
    /// <param name="blinkCount"></param>
    /// <param name="timeBeforeRestartAnimation"></param>
    /// <returns></returns>
    private IEnumerator RevealText(GameObject Screen, bool byLetter, float revealSpeed, bool blinkText, float blinkDuration, int blinkCount,bool loopAnimation, float timeBeforeRestartAnimation)
    {        

        // Cache the TextMeshPro component
        TextMeshPro tmpText = Screen.GetComponent<TextMeshPro>();
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
                        yield return new WaitForSeconds(timeBeforeRestartAnimation); // Pause before restarting
                    else
                        break;
                    visibleCount = 0;
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
                        break;
                }

                counter += 1;
                yield return new WaitForSeconds(revealSpeed * 10); // Adjust speed for word reveal
            }
        }
    }

    /// <summary>
    /// Blink text on terminal
    /// </summary>
    /// <param name="tmpText"></param>
    /// <param name="blinkDuration"></param>
    /// <param name="blinkCount"></param>
    /// <returns></returns>
    private IEnumerator BlinkText(TextMeshPro tmpText, float blinkDuration, int blinkCount)
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

    private void StopAndClearCoroutine(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }   
}
