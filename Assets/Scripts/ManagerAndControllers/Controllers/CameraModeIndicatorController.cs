using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraModeIndicatorController : MonoBehaviour
{
    public Sprite DefaultCameraImage;
    public Sprite RotationCameraImage;
    public Sprite FreeCameraImage;
    public Sprite BoarderCameraImage;
    public Sprite FirstPersonCameraImage;

    public Image ImageComponent;

    [Header("Indicator Settings")]

    [Tooltip("If enabled, the indicator will blink.")]
    public bool Blinking = false;

    [Tooltip("Number of times the indicator blinks after fading in.")]
    [Range(1, 10)]
    public int numberOfBlinks = 3;

    [Tooltip("Delay between each blink (in seconds).")]
    [Range(0f, 1f)]
    public float blinkDelay = 0.1f;

    [Tooltip("How long the indicator remains visible after blinking (in seconds).")]
    [Range(0f, 5f)]
    public float activeDuration = 1f;

    [Tooltip("Duration of the fade-out effect (in seconds).")]
    [Range(0f, 2f)]
    public float fadeOutSpeed = 0.5f;

    [Tooltip("Target opacity for the fade-out effect. 0 = fully transparent, 1 = fully opaque.")]
    [Range(0f, 1f)]
    public float fadeOutOpacity = 0f;

    [Tooltip("Duration of the fade-in effect (in seconds).")]
    [Range(0f, 2f)]
    public float fadeInSpeed = 0.5f;

    [Tooltip("Target opacity for the fade-in effect. 0 = fully transparent, 1 = fully opaque.")]
    [Range(0f, 1f)]
    public float fadeInOpacity = 1f;

    [Tooltip("Scale factor for the indicator when it appears. 1 = default scale.")]
    [Range(0.5f, 2f)]
    public float indicatorScale = 1f;


    [Tooltip("Initial delay before the indicator appears (in seconds).")]
    [Range(0f, 2f)]
    public float initialDelay = 0f;


    // Start is called before the first frame update
    void Start()
    {
        ImageComponent = this.GetComponent<Image>();
    }

    /// <summary>
    /// Tell the Player what mode the camera is in.
    /// </summary>
    /// <param name="state"></param>
    public void SwitchIndicatorTo(CameraController.CameraState state)
    {

        switch (state)
        {
            case CameraController.CameraState.Default:
            case CameraController.CameraState.Combat:
                StartCoroutine(SwitchImage(DefaultCameraImage));
                break;
            case CameraController.CameraState.Rotation:
                StartCoroutine(SwitchImage(RotationCameraImage));
                break;
            case CameraController.CameraState.Free:
                StartCoroutine(SwitchImage(FreeCameraImage));
                break;
            case CameraController.CameraState.BorderMovement:
                StartCoroutine(SwitchImage(BoarderCameraImage));
                break;
            case CameraController.CameraState.Call:
            case CameraController.CameraState.FirstPerson:                
                StartCoroutine(SwitchImage(FirstPersonCameraImage));
                break;
            default:
                Debug.LogWarning("Woah why we here!");
                break;
        }
    }
    private IEnumerator SwitchImage(Sprite cameraImage)
    {
        // Apply initial delay if needed
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        // Fade out the current image
        ImageComponent.CrossFadeAlpha(fadeOutOpacity, fadeOutSpeed, false);
        yield return new WaitForSeconds(fadeOutSpeed);

        // Switch to the new image and scale
        ImageComponent.sprite =cameraImage;
        ImageComponent.rectTransform.localScale = Vector3.one * indicatorScale;

        // Fade in the new image
        ImageComponent.CrossFadeAlpha(fadeInOpacity, fadeInSpeed, false);
        yield return new WaitForSeconds(fadeInSpeed);

        // Blink the indicator
        if(Blinking)
            for (int i = 0; i < numberOfBlinks; i++)
        {
            ImageComponent.CrossFadeAlpha(fadeOutOpacity, fadeOutSpeed, false);
            yield return new WaitForSeconds(fadeOutSpeed + blinkDelay);

            ImageComponent.CrossFadeAlpha(fadeInOpacity, fadeInSpeed, false);
            yield return new WaitForSeconds(fadeInSpeed + blinkDelay);
        }


        // Wait for the active UiDuration, then deactivate the object
        yield return new WaitForSeconds(activeDuration);

        // Deactivate the object
        this.gameObject.SetActive(false);
    }
}
