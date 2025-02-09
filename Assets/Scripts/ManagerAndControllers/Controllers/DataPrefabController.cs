using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class DataPrefabController : MonoBehaviour
{
    public string Name;
    public string Level;
    public string Time;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI TimeText;

    public Button LoadButton;
    public Button DeleteButton;

    public TerminalController UpgradeController;

    void Start()
    {
        NameText.SetText("Name: "+Name);
        LevelText.SetText("Levels: "+Level);
        TimeText.SetText("Time Saved: "+Time);


        // Assign button actions
        LoadButton.onClick.AddListener(() => UpgradeController.LoadGame(Name));
        DeleteButton.onClick.AddListener(() => UpgradeController.AttemptToDeleteSave(Name));
    }
}
