using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UpgradeController : MonoBehaviour
{
    public GameObject PlayerCanvas;

    private bool isInteracting;
    private NewChip selectedChip;
    private CameraController Camera;

    // to avoid using getComponent repeatdly
    private TextMeshPro defaultScreenText;
    private TextMeshPro introScreenText;
    private TextMeshPro healthUpgradeScreenText;
    private TextMeshPro chipUpgradeScreenText;
    private TextMeshPro errorScreenText;

    /// <summary>
    /// If player is interacting with terminal
    /// </summary>
    public bool IsInteracting
    {
        get
        {
            return isInteracting;
        }
        private set
        {
            isInteracting = value;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().IsInteracting = value;
        }
    }  

    /// <summary>
    /// Chip player has selected
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

    public UpgradeTerminalUIController UIController;

    /// <summary>
    /// Created event for UI.
    /// </summary>
    public event Action<Screens> OnScreenChanged;

    /// <summary>
    /// When ever there is an error
    /// </summary>
    public event Action<string> OnErrorOccurred;

    [Header("Screens In Game World")]
    public GameObject DefaultScreen;
    public GameObject IntroScreen;
    public GameObject HealthUpgradeScreen;
    public GameObject ChipUpgradeScreen;
    public GameObject ErrorScreen;

    public enum Screens
    {
        Default,
        Intro,
        HealthUpgrade,
        ChipUpgrade,
        Error,
        Exit
    }

    //Current Screen Active in game world
    private Screens currentScreen;

    void Awake()
    {
        defaultScreenText = DefaultScreen.GetComponent<TextMeshPro>();
        introScreenText = IntroScreen.GetComponent<TextMeshPro>();
        healthUpgradeScreenText = HealthUpgradeScreen.GetComponent<TextMeshPro>();
        chipUpgradeScreenText = ChipUpgradeScreen.GetComponent<TextMeshPro>();
        errorScreenText = ErrorScreen.GetComponent<TextMeshPro>();
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerCanvas = GameObject.FindGameObjectWithTag("PlayerCanvas");
        SwitchToScreen(Screens.Default);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Switches to the specified screen and deactivates the current screen.
    /// </summary>
    /// <param name="screen">The screen type to switch to.</param>
    public void SwitchToScreen(Screens screen)
    {
        OnScreenChanged?.Invoke(screen);

        StopAllCoroutines();

        switch (screen)
        {
            case Screens.Default:                              

                IntroScreen.SetActive(false);
                DefaultScreen.SetActive(true);
                HealthUpgradeScreen.SetActive(false);
                ChipUpgradeScreen.SetActive(false);
                ErrorScreen.SetActive(false);

                StartCoroutine(RevealText(DefaultScreen, true, 0.01f,true, 1, 3,true,5f));

                currentScreen = screen;
                break;
            case Screens.Intro:

                IntroScreen.SetActive(true);
                DefaultScreen.SetActive(false);
                HealthUpgradeScreen?.SetActive(false);
                ChipUpgradeScreen.SetActive(false);
                ErrorScreen.SetActive(false);

                StartCoroutine(RevealText(IntroScreen, false, 0.01f, false, 0, 0,false, 1000f));


                currentScreen = screen;
                break;
            case Screens.HealthUpgrade:

                IntroScreen.SetActive(false);
                DefaultScreen.SetActive(false);
                HealthUpgradeScreen.SetActive(true);
                ChipUpgradeScreen.SetActive(false);
                ErrorScreen.SetActive(false);

                PlayerController tempPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

                string tempText = string.Format("Getting <color=#A20000>*Error*</color> Health.\n" +
                "Current Health is <color=#A20000>{0}</color> of Max Health <color=#A20000>{1}</color>.\n" +
                "Will cost <b>150</b> Scrap to Upgrade to <color=#A20000>{2}</color>.\n" +
                "<b><u><link=\"UpgradeHealth\">Upgrade</link></u></b>\n" +
                "<b><u><link=\"Exit\">Exit</link></u></b>\n" +
                "<u><link=\"Back\"><color=#808080>Back</color></link></u>",
                tempPlayer.Health, tempPlayer.MaxHealth, tempPlayer.MaxHealth + 10);


                healthUpgradeScreenText.SetText(tempText);

                StartCoroutine(RevealText(HealthUpgradeScreen,false,0.01f,false,0,0,false, 10000f));

                currentScreen = screen;
                break;
            case Screens.ChipUpgrade:

                IntroScreen.SetActive(false);
                DefaultScreen.SetActive(false);
                HealthUpgradeScreen?.SetActive(false);
                ChipUpgradeScreen.SetActive(true);
                ErrorScreen.SetActive(false);

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
                        "<u><b><link=\"UpgradeSelectedChip\">Upgrade Chip</link></b></u>\n\n" +
                        "<u><b><link=\"Exit2\">Exit</link></b></u>\n" +
                        "<u><link=\"Back\"><color=#808080>Back</color></link></u>",
                    SelectedChip.chipRarity, SelectedChip.chipName, SelectedChip.description, SelectedChip.costToUpgrade);

                    chipUpgradeScreenText.SetText(tempText2);

                    StartCoroutine(RevealText(ChipUpgradeScreen, true, 0.01f, false, 0f, 0, false, 0));                    
                }

                currentScreen = screen;

            break;
            case Screens.Error:
                IntroScreen.SetActive(false);
                DefaultScreen.SetActive(false);
                HealthUpgradeScreen.SetActive(false);
                ChipUpgradeScreen.SetActive(false);
                ErrorScreen.SetActive(true);

                StartCoroutine(RevealText(ErrorScreen, false, 0.001f, false, 0f,0, false, 0f));
                break;
            case Screens.Exit:

                IntroScreen.SetActive(false);
                DefaultScreen.SetActive(true);
                HealthUpgradeScreen.SetActive(false);
                ChipUpgradeScreen.SetActive(false);
                ErrorScreen.SetActive(false);

                StartCoroutine(RevealText(DefaultScreen, true, 0.01f, true, 1, 3,true, 5f));

                currentScreen = screen;

                StartCoroutine(ExitTerminal());
                break;
            default:
                break;
        }
    }

    public void ChipSelectToUpgrade(NewChip chip)
    {
        selectedChip = chip;
        SwitchToScreen(Screens.ChipUpgrade);
    }
    /// <summary>
    /// Try to upgrade player health.
    /// </summary>
    public void AttemptToUpgradeHealth()
    {
        PlayerController tempPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (tempPlayer.Scrap >= 150)
        {
            // made this bank for later.
            var Bank = tempPlayer.TakeScrap(150);

            //Updade max health
            tempPlayer.UpgradeMaxHealth(10);

            //heal by same amount
            tempPlayer.Heal(10);

            //Display new info
            SwitchToScreen(UpgradeController.Screens.HealthUpgrade);
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
        //Added scrap for testing.
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GainScrap(500);

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Scrap >= SelectedChip.costToUpgrade)
        {           

            var Bank = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().TakeScrap(SelectedChip.costToUpgrade);

            GameManager.Instance.playerDeck.Find(item => item == SelectedChip).IsUpgraded = true;

            // FOr now lets go back to main menu.
            SwitchToScreen(Screens.Intro);
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
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !IsInteracting)
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
        IsInteracting = true;
        Camera.SwitchCamera(CameraController.CameraState.FirstPerson);
        Camera.FirstPersonCamera.LookAt = IntroScreen.transform;
        yield return new WaitForSeconds(1f);
        SwitchToScreen(Screens.Intro);

        //Turn off bad Ui
        PlayerCanvas.SetActive(false);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GainScrap(500);

    }
    /// <summary>
    /// Players animation and doe sother stuff so player can exit terminal.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExitTerminal()
    {
        Camera.SwitchCamera(CameraController.CameraState.Default);
        yield return new WaitForSeconds(1f);
        IsInteracting = false;

        // reactive bad Ui
        PlayerCanvas.SetActive(true);
        
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


}
