using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using static Item;

public class GearInfoController : MonoBehaviour
{
    [Range(0f, 5f)]
    public float AnimateTime = 1f;
    public Image GearImage;
    public TextMeshProUGUI GearName;
    public TextMeshProUGUI GearDescription;
    public TextMeshProUGUI GearType;
    public GameObject EffectsContainer;
    public PlayerInputActions playerInputActions;

    public Animator animator;

    private InputAction select;

    public Vector3 StartPosition { get; set; }
    public Vector3 TargetPosition { get; set; }

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        

        select = playerInputActions.Player.Select;
        select.Enable();
        select.performed += Shrink;
    }
    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }
    /// <summary>
    /// Enlarge
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    public void Enlarge(Vector3 startPosition, Vector3 targetPosition, float duration = 1f)
    {
        if (animator == null)
        {
            Debug.Log("No Animator");


            if (!TryGetComponent<Animator>(out animator))
                Debug.Log("still no animator.");
        }

        transform.position = targetPosition;
        animator.SetBool("IsEnlarging", true);
        animator.SetBool("IsShrinking", false);

        // Smoothly move to target position
        StopCoroutine(MoveToPosition(startPosition, targetPosition, duration));

        StartCoroutine(MoveToPosition(startPosition, targetPosition, duration));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    public void Shrink(InputAction.CallbackContext context)
    {
        if (animator == null)
        {
            Debug.LogError("Animator not found!");

            if (!TryGetComponent<Animator>(out animator))
                Debug.Log("still no animator.");
        }

        // Start shrinking animation
        animator.SetBool("IsShrinking", true);
        animator.SetBool("IsEnlarging", false);

        StartCoroutine(MoveToPosition(StartPosition, TargetPosition, AnimateTime));
    }

    public void SetUpGearInfo(Item item)
    {
        GearName.SetText(item.itemName);
        GearImage.sprite = item.itemImage;

        switch (item.itemType)
        {
            case ItemType.Weapon:
                GearType.color=Color.red;
            break;
            case ItemType.Armor:
                GearType.color=Color.blue;
            break;
            case ItemType.Equipment:
                GearType.color=Color.green;
            break;

        }
        GearType.SetText(item.itemType.ToString());        
        
        GearDescription.SetText(item.itemDescription);
    }

    private IEnumerator MoveToPosition(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        transform.position = targetPosition;

        Destroy(this.gameObject);
    }


    void OnDisable()
    {
        select.performed -= Shrink;
        select.Disable();
    }

}
