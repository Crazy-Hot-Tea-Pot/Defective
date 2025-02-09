using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipInfoController : MonoBehaviour
{
    public Image ChipImage;
    public TextMeshProUGUI ChipName;
    public TextMeshProUGUI ChipDescription;
    public TextMeshProUGUI ChipType;

    public Animator animator;
    void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Enlarge
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="duration"></param>
    public void Enlarge(Vector3 startPosition, Vector3 targetPosition, float duration = 2f)
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
    public void Shrink(Vector3 startPosition, Vector3 targetPosition, float duration = 1f)
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


        StopCoroutine(MoveToPosition(startPosition, targetPosition, duration));
        StartCoroutine(MoveToPosition(startPosition, targetPosition, duration));
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
    }
}
