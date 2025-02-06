using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Chip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
                ChipTitle = NewChip.chipName + " Chip";
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
    private GameObject chipinfoDisplay;
    private CombatController CombatController;
    private GameObject Player;
    private TerminalController UpgradeController;

    void Start()
    {        
        Player = GameObject.FindGameObjectWithTag("Player"); 
    }
    /// <summary>
    /// Runs Scriptable Chip
    /// </summary>
    public void ChipSelected()
    {        
        try
        {
                // Check if there is a target available
                if (CombatController.Target == null)
                {
                    throw new NullReferenceException("No target assigned.");
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
                    SoundManager.PlayFXSound(SoundFX.ChipPlayed);

                    newChip.IsActive = true;

                    if (Player.GetComponent<PlayerController>().IsMotivated)
                    {
                        if (newChip is DefenseChip defenseChip)
                        {
                            newChip.OnChipPlayed(Player.GetComponent<PlayerController>());
                            newChip.OnChipPlayed(Player.GetComponent<PlayerController>());
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
                                newChip.OnChipPlayed(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (chipinfoDisplay == null)
        {

            chipinfoDisplay = Instantiate(ChipinfoPrefab, UiManager.Instance.transform);


            ChipInfoController controller = chipinfoDisplay.GetComponent<ChipInfoController>();

            StartCoroutine(DelayForAnimator(controller));            
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(chipinfoDisplay != null)
        {
            ChipInfoController controller = chipinfoDisplay.GetComponent<ChipInfoController>();

            // Animate shrinking
            Vector3 targetPosition = this.transform.position;
            Vector3 startPosition = UiManager.Instance.transform.position;

            controller.Shrink(startPosition, targetPosition);

            // Optional: Delay destruction for animation
            Destroy(chipinfoDisplay, 1f);
        }
    }

    /// <summary>
    /// Tell the Upgrade Controller this is the chip the user selected to ugprade.
    /// </summary>
    private void UpgradeChipSelected()
    {
        UpgradeController.ChipSelectToUpgrade(newChip);
    }

    private IEnumerator DelayForAnimator(ChipInfoController controller)
    {
        yield return null;

        controller.ChipName.SetText(ChipTitle);
        controller.ChipImage.sprite = NewChip.chipImage;
        controller.ChipType.SetText(NewChip.ChipType.ToString());
        controller.ChipDescription.SetText(NewChip.description);

        //Animate

        Vector3 targetPosition = UiManager.Instance.transform.position;
        Vector3 startPosition = this.transform.position;
        controller.Enlarge(startPosition, targetPosition);
    }
    void OnDestroy()
    {
        if(chipinfoDisplay != null)
            Destroy(chipinfoDisplay);

        chipButton.onClick.RemoveAllListeners();
    }
    void OnDisable()
    {
        if (chipinfoDisplay != null)
            Destroy(chipinfoDisplay);
    }

}
