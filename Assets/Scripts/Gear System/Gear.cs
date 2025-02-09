using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gear : MonoBehaviour, IPointerClickHandler, ICanvasRaycastFilter
{
    public PolygonCollider2D polygonCollider;
    public CombatController CombatController;
    public GameObject Player;
    public Button Button;
    public Image GearImage;

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
        CombatController=GameObject.FindGameObjectWithTag("CombatController").GetComponent<CombatController>();

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
                if (CombatController.Target == null)
                    Item.ItemActivate(Player.GetComponent<PlayerController>());
                else
                {
                        Item.ItemActivate(Player.GetComponent<PlayerController>(), CombatController.Target.GetComponent<Enemy>());
                }
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
