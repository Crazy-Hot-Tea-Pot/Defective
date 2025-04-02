using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChipInfoController : MonoBehaviour
{
    [Range(0f,5f)]
    public float AnimateTime = 1f;
    public Image ChipImage;
    public TextMeshProUGUI ChipName;
    public TextMeshProUGUI ChipDescription;
    public TextMeshProUGUI ChipType;
    public PlayerInputActions playerInputActions;

    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set
        {
            targetPosition = value;
        }
    }

    public Vector3 StartPosition
    {
        get
        {
            return startPosition;
        }
        set
        {
            startPosition = value;
        }
    }

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private InputAction select;

    public Animator animator;
    void OnEnable()
    {
        animator = GetComponent<Animator>();
        select = playerInputActions.Player.Select;
        select.Enable();
        select.performed += Shrink;
    }
    void Awake()
    {
        // assign Player Input class
        playerInputActions = new PlayerInputActions();
    }
    /// <summary>
    /// Setup the info for chip
    /// </summary>
    /// <param name="chip"></param>
    public void SetUpChipInfo(NewChip chip)
    {
        ChipName.SetText(chip.chipName);
        ChipImage.sprite = chip.chipImage;

        switch(chip.ChipType)
        {
            case NewChip.TypeOfChips.Attack:
                ChipType.color = Color.red;
                break;
            case NewChip.TypeOfChips.Defense:
                ChipType.color = Color.blue;
                break;
            case NewChip.TypeOfChips.Skill:
                ChipType.color = Color.green;
                break;
        }
        ChipType.SetText(chip.ChipType.ToString());
        ChipDescription.SetText(chip.ChipDescription);
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

            if (!TryGetComponent<Animator>(out animator))
            {
                Debug.LogWarning("still no animator.");
                return;
            }
        }

        // Start shrinking animation
        animator.SetBool("IsShrinking", true);
        animator.SetBool("IsEnlarging", false);
        
        StartCoroutine(MoveToPosition(StartPosition, TargetPosition, AnimateTime));
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
        select.Disable();
        select.performed -= Shrink;
    }
    void OnDestroy()
    {
        select.Disable();
        select.performed -= Shrink;
    }
}
