using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gear : MonoBehaviour, IPointerClickHandler, ICanvasRaycastFilter, IPointerEnterHandler, IPointerExitHandler
{
    public PolygonCollider2D polygonCollider;
    public CombatController CombatController;
    public PuzzleController PuzzleController;
    public GameObject Player;
    public Button Button;
    public Image GearImage;
    public GameObject GearTip;
    public TextMeshProUGUI GearTipText;

    public Item Item
    {
        get
        {
            return item;
        }
        private set
        {
            item = value;
        }
    }

    public string GearName
    {
        get
        {
            return gearName;
        }
        private set
        {
            gearName = value;
        }
    }

    private Item item;
    private string gearName;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

        Player = GameObject.FindGameObjectWithTag("Player");

        // Set image to chip
        //GetComponent<Image>().sprite = Item.itemImage
        //
        try
        {
            CombatController = GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();
            PuzzleController = GameObject.FindGameObjectWithTag("PuzzleController").GetComponent<PuzzleController>();
        }
        catch
        {
            Debug.LogWarning("Failed to find CombatController");
        }

        Button.onClick.AddListener(() =>  UseItem() );
    }
    // This method ensures raycasts only hit within the collider boundaries
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        // Convert screen point to world coordinates
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            transform as RectTransform, screenPoint, eventCamera, out worldPoint
        );

        // Check if the world point is inside the Polygon Collider
        return polygonCollider != null && polygonCollider.OverlapPoint(new Vector2(worldPoint.x, worldPoint.y));
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Item != null)
        {
            GearTipText.SetText(item.itemDescription);
            GearTip.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (GearTip.activeInHierarchy)
        {
            GearTip.SetActive(false);
        }
    }
    // Handle clicks within the collider area
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);

        if (polygonCollider.OverlapPoint(worldPoint))
        {
            //Debug.Log("Button clicked within Polygon Collider of "+this.gameObject.name);
            PerformButtonAction();
        }
    }
    public void UseItem()
    {
        try
        {
            // Check if there is a target available
            if (
                (CombatController.Target == null) 
                && 
                (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat))
                throw new NullReferenceException("No target assigned.");

            if (Item == null)
                throw new NullReferenceException("No Item equipped.");
            else
            {
                //  THIS IS A TEMP FIX AND MUST BE DONE FOR CASES SO THE FIRST IF STATEMENT ALWAYS RUN NO MATTER THE MODE
                if(GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
                {
                    if (CombatController.Target == null)
                    {
                        //Item.ItemActivate(Player.GetComponent<PlayerController>());
                    }
                    else
                    {
                        Item.ItemActivate(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());
                    }
                }
                else
                {
                    if (PuzzleController.Target == null)
                    {
                        //Don't want nulls in our itemactivate but don't have a use for them and I don't think it's valuable to warn the devs about
                    }
                    else if (PuzzleController.Target.tag == "Puzzle")
                    {
                        Item.ItemActivate(Player.GetComponent<PlayerController>(), PuzzleController.Target.GetComponent<PuzzleRange>());
                    }
                }
                //if (CombatController.Target == null)
                //{
                //    Item.ItemActivate(Player.GetComponent<PlayerController>());
                //}
                //else if (PuzzleController.Target == null && GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
                //{
                //    //Don't want nulls in our itemactivate but don't have a use for them and I don't think it's valuable to warn the devs about
                //}
                //else if (PuzzleController.Target.tag == "Puzzle" && GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
                //{
                //    Item.ItemActivate(Player.GetComponent<PlayerController>(), PuzzleController.Target.GetComponent<PuzzleRange>());
                //}
                //else
                //{
                //    Item.ItemActivate(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());
                //}
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
    public void EquipItem(Item newItem)
    {
        //Unequip Item
        if (Item != null)
        {
            Item.IsEquipped = false;
            Item = null;
        }

        //Equip new newItem
        Item = newItem;
        Item.IsEquipped = true;
        GearName = Item.itemName;
        this.gameObject.name = GearName;
        Color color = GearImage.color;
        color.a = 1f;
        GearImage.color=color;
        GearImage.sprite=Item.itemImage;
    }

    private void PerformButtonAction()
    {
        // Add your button logic here
        Debug.Log("Performing button action...");
    }
}
