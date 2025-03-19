using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class SettingsUIController : UiController
{
    //First Layer Settings Buttons
    private Button Optionsbtn;
    private Button Continuebtn;
    private Button Exitbtn;
    private Button MainMenubtn;

    //Buttons for tabs
    private Button VideoTabbtn;
    private Button AudioTabbtn;

    //UI Containers
    private GameObject largeSettingMenu;
    private GameObject smallSettingMenu;
    private GameObject videoSettingTab;
    private GameObject audioSettingTab;

    ////Global Volume Component profile list
    //public List<VolumeProfile> VolumeSettings;

    ////Global Volume Component Defaults list
    //public List<VolumeProfile> VolumeDefaults;

    //Buttons for video settings
    private Slider GammaSlider;
    private Slider GainSlider;
    private Toggle bloomOn;
    private TMP_Dropdown resolutionDropDown;
    private TMP_Dropdown aspectDropDown;
    private Toggle windowedOn;

    //Buttons for audio settings
    private Slider SoundEffectsSlider;
    private Slider MusicSlider;
    private Toggle MusicToggle;
    private Toggle SFXToggle;

    //Apply&Discard buttons
    private Button Applybtn;
    private Button Discardbtn;
    private Button RestoreDefaultsbtn;
    private Button BackBtn;
    private GameObject titleScreenUI;

    private bool hasOpenedVideo;
    private bool hasOpenedAudio;

    // Start is called before the first frame update
    void Start()
    {
        //At start we need to find a series of containers for the ui and some buttons that are now visible

        //Menu Containers
        largeSettingMenu = this.gameObject.transform.Find("OptionsScreen").gameObject;
        smallSettingMenu = this.gameObject.transform.Find("ShortMenu").gameObject;

        //When adding buttons remove all listeners for when we go to main menu and back to in game otherwise we could have alot of listeners
        //Buttons for tabs
        VideoTabbtn = this.gameObject.transform.Find("OptionsScreen").Find("VideoSettingsbtn").GetComponent<Button>();
        VideoTabbtn.onClick.RemoveAllListeners();
        //Add a button for opening video settings
        VideoTabbtn.onClick.AddListener(OpenVideoSettingsTab);
        AudioTabbtn = this.gameObject.transform.Find("OptionsScreen").Find("Audiobtn").GetComponent<Button>();
        AudioTabbtn.onClick.RemoveAllListeners();
        //Add a button for opening audio settings
        AudioTabbtn.onClick.AddListener(OpenAudioSettingsTab);

        //Button for applying
        Applybtn = this.gameObject.transform.Find("OptionsScreen").Find("Applybtn").GetComponent<Button>();
        Applybtn.onClick.RemoveAllListeners();
        //Add a button to apply settings
        Applybtn.onClick.AddListener(ApplySettings);
        //Start inactive
        Applybtn.gameObject.SetActive(false);

        //Button for discarding
        Discardbtn = this.gameObject.transform.Find("OptionsScreen").Find("Discardbtn").GetComponent<Button>();
        Discardbtn.onClick.RemoveAllListeners();
        //Add a button to discard settings
        Discardbtn.onClick.AddListener(DiscardSettings);
        //Start inactive
        Discardbtn.gameObject.SetActive(false);

        //Button for restoring defaults
        RestoreDefaultsbtn = this.gameObject.transform.Find("OptionsScreen").Find("Defaultsbtn").GetComponent<Button>();
        RestoreDefaultsbtn.onClick.RemoveAllListeners();
        //Add a button to restore defaults
        RestoreDefaultsbtn.onClick.AddListener(RestoreDefaults);
        //start inactive
        RestoreDefaultsbtn.gameObject.SetActive(false);

        //Back button
        BackBtn = this.gameObject.transform.Find("OptionsScreen").Find("Backbtn").GetComponent<Button>();
        BackBtn.onClick.RemoveAllListeners();
        //Add a button to return to mini menu
        BackBtn.onClick.AddListener(ReturnToMiniMenu);

        //If title screen skip mini menu
        if (UiManager.Instance.TitleCheck())
        {
            SkipMiniMenu();
            titleScreenUI = UiManager.Instance.FindTitleController("TitleController");

            //Make UI Manager above the title canvas.
            UiManager.Instance.GetComponent<Canvas>().sortingOrder = 1;

            //Pause time
            UnityEngine.Time.timeScale = 0;
        }
        else
        {
            //Pause time
            UnityEngine.Time.timeScale = 0;

            //Find the options button
            Optionsbtn = this.gameObject.transform.Find("ShortMenu").Find("Optionsbtn").GetComponent<Button>();
            Optionsbtn.onClick.RemoveAllListeners();
            //Add a button for options
            Optionsbtn.onClick.AddListener(Options);
            //Find the button for continue
            Continuebtn = this.gameObject.transform.Find("ShortMenu").Find("Continuebtn").GetComponent<Button>();
            Continuebtn.onClick.RemoveAllListeners();
            //Make the button for continue functional
            Continuebtn.onClick.AddListener(Continue);
            //Find the exit button
            Exitbtn = this.gameObject.transform.Find("ShortMenu").Find("Exitbtn").GetComponent<Button>();
            Exitbtn.onClick.RemoveAllListeners();
            //Make the exit button functional
            Exitbtn.onClick.AddListener(Exit);
            //Find the main menu button
            MainMenubtn = this.gameObject.transform.Find("ShortMenu").Find("MainMenubtn").GetComponent<Button>();
            //Add the Main menu button
            MainMenubtn.onClick.RemoveAllListeners();
            MainMenubtn.onClick.AddListener(MainMenu);
        }
    }

    /// <summary>
    /// Open and close the options menu
    /// </summary>
    public void Options()
    {
        //If the larger UI is open then close options
        if (largeSettingMenu.activeSelf)
        {
            largeSettingMenu.SetActive(false);
            //If we aren't skipping main menu open it again
            if (UiManager.Instance.TitleCheck() == false)
            {
                smallSettingMenu.SetActive(true);
            }
        }
        //If the smaller UI then open larger options
        else if (smallSettingMenu.activeSelf)
        {
            //Open the large menu
            largeSettingMenu.SetActive(true);
            //Close the small menu
            smallSettingMenu.SetActive(false);

            //Buttons for video tab
            VideoTabbtn = this.gameObject.transform.Find("OptionsScreen").Find("VideoSettingsbtn").GetComponent<Button>();
            VideoTabbtn.onClick.RemoveAllListeners();
            VideoTabbtn.onClick.AddListener(OpenVideoSettingsTab);
            //Buttons for audio tab
            AudioTabbtn = this.gameObject.transform.Find("OptionsScreen").Find("Audiobtn").GetComponent<Button>();
            AudioTabbtn.onClick.RemoveAllListeners();
            AudioTabbtn.onClick.AddListener(OpenAudioSettingsTab);

            //Buttons for applying,discard and restore defaults
            Applybtn = this.gameObject.transform.Find("OptionsScreen").Find("Applybtn").GetComponent<Button>();
            Applybtn.onClick.RemoveAllListeners();
            Applybtn.onClick.AddListener(ApplySettings);
            //make active
            Applybtn.gameObject.SetActive(false);
            Discardbtn = this.gameObject.transform.Find("OptionsScreen").Find("Discardbtn").GetComponent<Button>();
            Discardbtn.onClick.RemoveAllListeners();
            Discardbtn.onClick.AddListener(DiscardSettings);
            //make active
            Discardbtn.gameObject.SetActive(false);
            RestoreDefaultsbtn = this.gameObject.transform.Find("OptionsScreen").Find("Defaultsbtn").GetComponent<Button>();
            RestoreDefaultsbtn.onClick.RemoveAllListeners();
            RestoreDefaultsbtn.onClick.AddListener(RestoreDefaults);
            //make active
            RestoreDefaultsbtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Close this UI
    /// </summary>
    public void Continue()
    {
        //Reset UI Manager Canvas to default
        UiManager.Instance.GetComponent<Canvas>().sortingOrder = 0;

        UiManager.Instance.CloseSettingsOnClick();
        //Pause time
        UnityEngine.Time.timeScale = 1;
    }

    /// <summary>
    /// Settings for opening and closing video settings tab
    /// </summary>
    public void OpenVideoSettingsTab()
    {
        hasOpenedVideo = true;
        //Find the settings tabs
        videoSettingTab = VideoTabbtn.transform.parent.Find("VideoSettingsTab").gameObject;
        AudioTabbtn.transform.parent.Find("AudioSettingsTab").gameObject.SetActive(false);
        //Open/Close tab
        if (videoSettingTab.activeSelf)
        {
            videoSettingTab.SetActive(false);
        }
        else
        {
            videoSettingTab.SetActive(true);
            GammaSlider = videoSettingTab.transform.Find("GammaSlider").GetComponent<Slider>();
            bloomOn = videoSettingTab.transform.Find("ToggleBloom").GetComponent<Toggle>();
            GainSlider = videoSettingTab.transform.Find("GainSlider").GetComponent<Slider>();
        }

        //Set the values
        setVolumeValues();

        //Enable buttons
        RestoreDefaultsbtn.gameObject.SetActive(true);
        Applybtn.gameObject.SetActive(true);
        Discardbtn.gameObject.SetActive(true);

        //Disable back button
        BackBtn.gameObject.SetActive(false);
    }

    ///<summary>
    ///Settings for opening and closing audio settings tab
    ///</summary>
    public void OpenAudioSettingsTab()
    {
        hasOpenedAudio = true;
        //Find the settings tabs
        audioSettingTab = AudioTabbtn.transform.parent.Find("AudioSettingsTab").gameObject;
        VideoTabbtn.transform.parent.Find("VideoSettingsTab").gameObject.SetActive(false);

        //Find buttons for audio effect and assign them
        SoundEffectsSlider = audioSettingTab.transform.Find("SoundEffectsSlider").GetComponent<Slider>();
        MusicSlider = audioSettingTab.transform.Find("MusicSlider").GetComponent<Slider>();
        MusicToggle = audioSettingTab.transform.Find("MuteMusic").GetComponent<Toggle>();
        SFXToggle = audioSettingTab.transform.Find("MuteEffects").GetComponent<Toggle>();

        if (audioSettingTab.activeSelf)
        {
            audioSettingTab.SetActive(false);
        }
        else
        {
            audioSettingTab.SetActive(true);
        }

        //Enable buttons
        RestoreDefaultsbtn.gameObject.SetActive(true);
        Applybtn.gameObject.SetActive(true);
        Discardbtn.gameObject.SetActive(true);

        //Disable back button
        BackBtn.gameObject.SetActive(false);

        setAudioValues();
    }

    /// <summary>
    /// Set the default values for audio
    /// </summary>
    public void setAudioValues()
    {
        
        //Audio settings
        MusicSlider.value = SettingsManager.Instance.SoundSettings.BGMVolume;
        SoundEffectsSlider.value = SettingsManager.Instance.SoundSettings.SFXVolume;
        MusicToggle.isOn = SettingsManager.Instance.SoundSettings.BGMMute;
        SFXToggle.isOn = SettingsManager.Instance.SoundSettings.SFXMute;
    }
    /// <summary>
    ///Set all buttons values inside of video settings
    /// </summary>
    public void setVolumeValues()
    {
        #region VolumProfileAdjustments
        GammaSlider.enabled = true;
        bloomOn.enabled = true;
        GainSlider.enabled = true;

        //Set values
        //Try to get the variable for gain
        if (SettingsManager.Instance.VolumeSettings[0].TryGet(out LiftGammaGain gainSettings))
        {
            //Set gamma to meet this value. W represents the value of the intensity and we add +0.5f so it's usable as this value uses negative values but sliders don't.
            GammaSlider.value = gainSettings.gamma.value.w + 0.5f;
            //Repeat the same process for gain
            GainSlider.value = gainSettings.gain.value.w + 0.5f;
        }
        //If this value doesn't exist
        else
        {
            Debug.Log("There is no gain");
            GammaSlider.enabled = false;
        }

        //Enable and disable bloom check
        if (SettingsManager.Instance.VolumeSettings[0].TryGet(out Bloom bloomSettings))
        {
            bloomOn.isOn = bloomSettings.active;
        }
        //If there is no bloom
        else
        {
            Debug.Log("There is no bloom");
            bloomOn.enabled = false;
        }
        #endregion
        #region UnitySettingsChange
        aspectDropDown = videoSettingTab.transform.Find("AspectDropDown").GetComponent<TMP_Dropdown>();
        ////aspectDropDown.enabled = true;
        resolutionDropDown = videoSettingTab.transform.Find("ResolutionDropDown").GetComponent<TMP_Dropdown>();
        //resolutionDropDown.enabled = true;
        windowedOn = videoSettingTab.transform.Find("ToggleWindowed").GetComponent<Toggle>();
        windowedOn.enabled = true;

        //Screen resolution
        if (UnityEngine.Screen.currentResolution.width == 1920 && UnityEngine.Screen.currentResolution.height == 1080)
        {
            resolutionDropDown.value = 0;
            Debug.Log("1920x1080");
        }
        else if (UnityEngine.Screen.currentResolution.width == 1366 && UnityEngine.Screen.currentResolution.height == 763)
        {
            resolutionDropDown.value = 1;
            Debug.Log("1366x763");
        }
        else if (UnityEngine.Screen.currentResolution.width == 2560 && UnityEngine.Screen.currentResolution.height == 1440)
        {
            resolutionDropDown.value = 2;
            Debug.Log("2560x1440");
        }
        else if (UnityEngine.Screen.currentResolution.width == 3840 && UnityEngine.Screen.currentResolution.height == 2160)
        {
            resolutionDropDown.value = 3;
            Debug.Log("3840x2160");
        }
        else
        {
            resolutionDropDown.value = 0;
            UnityEngine.Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            Debug.Log("We don't know");
        }

        //Check if windowed and assign the toggle to match
        if (UnityEngine.Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow))
        {
            windowedOn.isOn = false;
            Debug.Log("Full screen");
        }
        else
        {
            windowedOn.isOn = true;
            Debug.Log("Windowed");
        }

        #endregion
    }

    //If we are applying settings
    public void ApplySettings()
    {
        if (videoSettingTab != null && videoSettingTab.activeSelf == true)
        {
            foreach (VolumeProfile levelProfile in SettingsManager.Instance.VolumeSettings)
            {
                //Try to get the variable for gain
                if (levelProfile.TryGet(out LiftGammaGain gainSettings))
                {
                    //Save the gain and gamma
                    SettingsManager.Instance.VideoSettings.SetandSaveGainandGamma(gainSettings, GammaSlider.value, GainSlider.value);
                    ////Set brightness to meet the new value. W represents the value of the intensity and we add +0.5f so it's usable as this value uses negative values but sliders don't.
                    //gainSettings.gamma.value += new Vector4(0, 0, 0, GammaSlider.value - 0.5f);
                    ////Repeat the same process for gain
                    //gainSettings.gain.value += new Vector4(0, 0, 0, GainSlider.value - 0.5f);
                }
                //If this value doesn't exist
                else
                {
                    Debug.Log("There is no gain");
                }

                //Enable and disable bloom check
                if (levelProfile.TryGet(out Bloom bloomSettings))
                {
                    if (bloomOn.isOn)
                    {
                        SettingsManager.Instance.VideoSettings.DisableBloom(bloomSettings);
                    }
                    else
                    {
                        SettingsManager.Instance.VideoSettings.EnabledBloom(bloomSettings);
                    }
                    //bloomSettings.active = bloomOn.isOn;
                }
                //If there is no bloom
                else
                {
                    Debug.Log("There is no bloom");
                }
            }

            //Apply Unity Settings
            //Check if windowed and assign the toggle to match
            if (windowedOn.isOn == false)
            {
                //Sets full screen
                SettingsManager.Instance.VideoSettings.IsFullScreen(true);
                //UnityEngine.Screen.fullScreen = true;
                Debug.Log("Full screen");
            }
            else
            {
                //Sets windowed
                SettingsManager.Instance.VideoSettings.IsFullScreen(false);
                //UnityEngine.Screen.fullScreen = false;
                Debug.Log("Windowed");
            }


            //Screen resolution
            SettingsManager.Instance.VideoSettings.SetandSaveResolution(resolutionDropDown.value);
            //if (resolutionDropDown.value == 0)
            //{
            //    UnityEngine.Screen.SetResolution(1920, 1080, !windowedOn.isOn);
            //    Debug.Log("1920x1080");
            //}
            //else if (resolutionDropDown.value == 1)
            //{
            //    UnityEngine.Screen.SetResolution(1366, 763, !windowedOn.isOn);
            //    Debug.Log("1366x763");
            //}
            //else if (resolutionDropDown.value == 2)
            //{

            //    UnityEngine.Screen.SetResolution(2560, 1440, !windowedOn.isOn);
            //    Debug.Log("2560x1440");
            //}
            //else if (resolutionDropDown.value == 3)
            //{

            //    UnityEngine.Screen.SetResolution(3840, 2160, !windowedOn.isOn);
            //    Debug.Log("3840x2160");
            //}

            Debug.Log("Graphics settings applied");
        }
        else
        {
            //Audio settings
            SettingsManager.Instance.SoundSettings.BGMVolume = MusicSlider.value;
            SettingsManager.Instance.SoundSettings.SFXVolume = SoundEffectsSlider.value;
            SettingsManager.Instance.SoundSettings.BGMMute = MusicToggle.isOn;
            SettingsManager.Instance.SoundSettings.SFXMute = SFXToggle.isOn;
        }

        Options();
        ReturnToMiniMenu();
    }

    /// <summary>
    /// If we don't apply any of the settings
    /// </summary>
    public void DiscardSettings()
    {
        if (hasOpenedVideo)
        {
            setVolumeValues();
        }
        else if (hasOpenedAudio)
        {
            setAudioValues();
        }
        Options();
        ReturnToMiniMenu();
    }

    //Restore default values
    public void RestoreDefaults()
    {
        ApplySettings();
        if (SettingsManager.Instance.VolumeSettings.Count == SettingsManager.Instance.VolumeDefaults.Count)
        {
            //Run through the list of settings
            for (int i = 0; i < SettingsManager.Instance.VolumeSettings.Count; i++)
            {
                //Try to get the variable for gain
                if (SettingsManager.Instance.VolumeSettings[i].TryGet(out LiftGammaGain gainSettings))
                {
                    //Get the defaults
                    if (SettingsManager.Instance.VolumeDefaults[i].TryGet(out LiftGammaGain gainDefaults))
                    {
                        //Set gain equal to the defult profile in the same column of the list
                        gainSettings.gamma.value = gainDefaults.gamma.value;
                        //Repeat for gain
                        gainSettings.gain.value = gainDefaults.gain.value;
                    }
                }
                //If this value doesn't exist
                else
                {
                    Debug.Log("There is no gain");
                }

                //Enable and disable bloom check
                if (SettingsManager.Instance.VolumeSettings[i].TryGet(out Bloom bloomSettings))
                {
                    if (SettingsManager.Instance.VolumeDefaults[i].TryGet(out Bloom bloomDefaults))
                    {
                        bloomSettings.active = bloomDefaults.IsActive();
                    }
                }
                //If there is no bloom
                else
                {
                    Debug.Log("There is no bloom");
                }
            }
        }
        else
        {
            Debug.Log("Developer error there must be as many volume profiles as defaults");
        }

        //Audio Defaults
        MusicSlider.value = 100f;
        SoundEffectsSlider.value = 100f;
        MusicToggle.isOn = false;
        SFXToggle.isOn = false;

        DiscardSettings();
    }

    /// <summary>
    /// Return to the first menu
    /// </summary>
    public void ReturnToMiniMenu()
    {
        //Skip this menu if applicable
        if (UiManager.Instance.TitleCheck())
        {
            titleScreenUI.SetActive(true);
            largeSettingMenu.SetActive(false);
            smallSettingMenu.SetActive(false);
            UiManager.Instance.CloseSettingsOnClickTitle();
        }
        else
        {
            largeSettingMenu.SetActive(false);
            smallSettingMenu.SetActive(true);
        }
        UnityEngine.Time.timeScale = 1;
    }

    public void SkipMiniMenu()
    {
        largeSettingMenu.SetActive(true);
        smallSettingMenu.SetActive(false);
    }

    /// <summary>
    /// Opens the main menu
    /// </summary>
    public void MainMenu()
    {
        smallSettingMenu.SetActive(false);
        UnityEngine.Time.timeScale = 1;
        GameManager.Instance.RequestScene(Levels.Title);
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
