using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Chip : MonoBehaviour, IPointerClickHandler
{
    public enum ChipMode
    {
        None,
        Combat,
        WorkShop,
        Inventory,
        Delete
    }
    /// <summary>
    /// What mode is the chip in.
    /// </summary>
    public ChipMode Mode
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
        }
    }
        
    public GameObject ChipinfoPrefab;    

    /// <summary>
    /// Chip title is the title that the card will display in game and the ID of the card.
    /// </summary>
    public string ChipTitle
    {
        get
        {
            return chipTitle;
        }
        private set
        {
            chipTitle = value;
        }
    }

    /// <summary>
    /// Button Component so Player can click on card
    /// </summary>
    public Button chipButton;

    public NewChip NewChip
    {
        get
        {
            return newChip;
        }
        set
        {
            newChip = value;

            if (NewChip != null)
            {
                ChipTitle = NewChip.chipName;// + " Chip";
                this.gameObject.name = ChipTitle;
                NewChip.ThisChip = this.gameObject;
                // Set image to chip
                GetComponent<Image>().sprite = newChip.chipImage;

                SetChipModeTo(Mode);
            }
        }
    }

    private ChipMode mode = ChipMode.None;

    private string chipTitle;        
    private NewChip newChip;
    private GameObject chipInfoDisplay;
    private CombatController CombatController;
    private PuzzleController PuzzleController;
    private GameObject Player;
    private TerminalController UpgradeController;
    private int attemptCounter=0;

    void Start()
    {        
        Player = GameObject.FindGameObjectWithTag("Player");
        PuzzleController = GameObject.FindGameObjectWithTag("PuzzleController").GetComponent<PuzzleController>();
    }
    /// <summary>
    /// Runs Scriptable Chip
    /// </summary>
    public void ChipSelected()
    {
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
        {
            #region combatStuff
            try
            {
                // Check if there is a target available
                if (CombatController.Target == null)
                {
                    attemptCounter++;

                    if (attemptCounter > 3)
                    {
                        UiManager.Instance.PopUpMessage("You must select a target by clicking them with you mouse to attack first!");
                    }

                    return;
                }

                //Check if Player is jammed
                if (Player.GetComponent<PlayerController>().IsJammed)
                {
                    return;
                }

                //Check if newChip is assigned
                if (newChip != null)
                {
                    //Plays chip use sound
                    SoundManager.PlayFXSound(NewChip.ChipActivate);

                    newChip.IsActive = true;

                    //If motivated to play chip twice
                    if (Player.GetComponent<PlayerController>().IsMotivated)
                    {
                        if (newChip is DefenseChip defenseChip)
                        {
                            newChip.OnChipPlayed(Player.GetComponent<PlayerController>());
                            newChip.OnChipPlayed(Player.GetComponent<PlayerController>());

                            Player.GetComponent<PlayerController>().RemoveEffect(Effects.SpecialEffects.Motivation);
                        }
                        else
                        {
                            if (newChip.hitAllTargets)
                            {
                                // Looping to attack twice
                                for (int i = 0; i < 2; i++)
                                {
                                    foreach (GameObject target in EnemyManager.Instance.CombatEnemies)
                                    {
                                        newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), target.GetComponent<Enemy>());
                                    }
                                }
                            }
                            else
                            {
                                newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());
                                newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());

                            }
                        }
                        //Remove effect after it has been used.
                        Player.GetComponent<PlayerController>().RemoveEffect(Effects.SpecialEffects.Motivation);
                    }
                    else
                    {
                        if (newChip is DefenseChip defenseChip)
                            newChip.OnChipPlayed(Player.GetComponent<PlayerController>());
                        else
                        {
                            if (newChip.hitAllTargets)
                            {
                                foreach (GameObject target in EnemyManager.Instance.CombatEnemies)
                                {
                                    newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), target.GetComponent<Enemy>());
                                }
                            }
                            else
                            {
                                newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());
                            }

                        }

                    }

                    ChipManager.Instance.AddToUsedChips(this.gameObject);

                    UiManager.Instance.CanMakeAnyMoreMoves();
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.LogWarning($"Null reference error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Generic catch for any other exceptions that may occur
                Debug.LogError($"An unexpected error occurred: {ex.Message}");
            }
            #endregion
        }
        else { 
            #region puzzlestuff
            try
            {
                //Check if targeted
                if (PuzzleController.Target == null)
                {
                    throw new NullReferenceException("No target assigned for puzzles.");
                }
                //Plays chip use sound
                SoundManager.PlayFXSound(NewChip.ChipActivate);

                //Check if Player is jammed
                if (Player.GetComponent<PlayerController>().IsJammed)
                {
                    return;
                }

                //Check if newChip is assigned
                if (newChip != null)
                {
                    //Plays chip use sound
                    SoundManager.PlayFXSound(SoundFX.ChipsPlay);

                    newChip.IsActive = true;

                    if (newChip is DefenseChip defenseChip)
                        newChip.OnChipPlayed(Player.GetComponent<PlayerController>());
                    else
                    {
                        if (newChip.hitAllTargets)
                        {
                            foreach (GameObject target in EnemyManager.Instance.CombatEnemies)
                            {
                                newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), target.GetComponent<Enemy>());
                            }
                        }
                        else
                        {
                            if (PuzzleController.Target.tag == "Puzzle")
                            {
                                newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), PuzzleController.Target.GetComponent<PuzzleRange>());
                            }
                        }

                    }



                    ChipManager.Instance.AddToUsedChips(this.gameObject);
                }
                else
                {
                    throw new NullReferenceException("No chip script attached.");
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.LogWarning($"Null reference error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Generic catch for any other exceptions that may occur
                Debug.LogError($"An unexpected error occurred: {ex.Message}");
            }
            #endregion
        }
    }
    /// <summary>
    /// Set chip prefab to different mode so it can be used in multiple different enviroments.
    /// </summary>
    /// <param name="modeToBe">Mode the chip to be in.</param>
    public void SetChipModeTo(ChipMode modeToBe)
    {
        Mode = modeToBe;

        switch (modeToBe)
        {
            case ChipMode.Combat:
                CombatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
                chipButton.interactable = true;
                chipButton.onClick.AddListener(ChipSelected);                
                newChip.IsActive = true;                
                break;
            case ChipMode.WorkShop:
                UpgradeController = GameObject.FindGameObjectWithTag("UpgradeController").GetComponent<TerminalController>();
                chipButton.onClick.AddListener(UpgradeChipSelected);
                chipButton.interactable = true;                
                break;
            case ChipMode.Inventory:
                chipButton.interactable = false;
                chipButton.enabled = false;
                break;
            case ChipMode.Delete:
                chipButton.interactable = true;
                chipButton.onClick.AddListener(() => UiManager.Instance.SelectedChipToReplaceWith(NewChip));
                break;
            case ChipMode.None:
            default:
                break;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (chipInfoDisplay == null)
            {

                chipInfoDisplay = Instantiate(ChipinfoPrefab, UiManager.Instance.transform);


                ChipInfoController controller = chipInfoDisplay.GetComponent<ChipInfoController>();

                controller.SetUpChipInfo(NewChip);

                switch (NewChip.ChipType)
                {
                    case NewChip.TypeOfChips.Attack:
                        controller.ChipType.color = Color.red;
                        break;
                    case NewChip.TypeOfChips.Defense:
                        controller.ChipType.color = Color.blue;
                        break;
                    case NewChip.TypeOfChips.Skill:
                        controller.ChipType.color = Color.green;
                        break;
                    default:
                        controller.ChipType.color = Color.white;
                        break;
                }

                controller.animator.SetBool("IsEnlarging", true);
                controller.animator.SetBool("IsShrinking", false);

                controller.TargetPosition = this.transform.position;
                controller.StartPosition = UiManager.Instance.transform.position;
            }
        }
    }    

    /// <summary>
    /// Tell the Upgrade Controller this is the chip the user selected to ugprade.
    /// </summary>
    private void UpgradeChipSelected()
    {
        UpgradeController.ChipSelectToUpgrade(newChip);
    }  
    void OnDestroy()
    {
        if(chipInfoDisplay != null)
            Destroy(chipInfoDisplay);

        chipButton.onClick.RemoveAllListeners();
    }
    void OnDisable()
    {
        if (chipInfoDisplay != null)
            Destroy(chipInfoDisplay);
    }

}
