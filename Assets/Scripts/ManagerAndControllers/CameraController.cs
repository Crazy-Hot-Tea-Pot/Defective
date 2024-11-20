using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    private Transform player;

    private Transform target;
    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

    public GameObject freeLook;
    public GameObject CameraUIObject;
    public CameraModeIndicatorController CameraUI;
    private float distanceFromCamera = 10f;
    //5% from edge
    public float borderThreshold = 0.05f;
    public float screenWidth;
    public float screenHeight;
    public Vector2 mousePosition;
    public enum CameraState
    {
        Default,
        Rotation,
        Free,
        BorderMovement,
        FirstPerson
    }

    [Header("Cameras")]
    private CameraState currentCamera;
    private CameraState previousCamera;

    public CinemachineVirtualCamera DefaultCamera;
    public CinemachineFreeLook RotationCamera;
    public CinemachineVirtualCamera freeCamera;
    public CinemachineVirtualCamera BorderCamera;
    public CinemachineVirtualCamera FirstPersonCamera;
       
    public CameraState CurrentCamera
    {
        get
        {
            return currentCamera;  
        }
        set
        {
            previousCamera = currentCamera;           

            currentCamera = value;

            //Call Camera indicator if camera changes
            if (currentCamera != previousCamera)
            {
                //Activate UI Obejct
                CameraUIObject.SetActive(true);

                //Indicate to player what camera mode they changed to.
                CameraUI.SwitchIndicatorTo(value);
            }

            switch (value)
            {
                case CameraState.Default:
                    DefaultCamera.Follow = Target;
                    DefaultCamera.LookAt = Target;
                    break;
                case CameraState.Rotation:
                    RotationCamera.Follow = Target;

                    if(previousCamera==CameraState.Free)
                        RotationCamera.LookAt = freeLook.transform;
                    else
                        RotationCamera.LookAt = Target;
                    break;
                case CameraState.Free:
                    freeCamera.LookAt = freeLook.transform;
                    break;
                default:
                    break;
            }
        }
    }

    [Header("Camera Info")]
    [Header("Things to make change from camera settings later")]
    public bool AllowBoarderMovement;
    
    public float boarderCameraSpeed;
    public float freeCameraSpeed;
    // Sensitivity for moving the look target smoothly
    public float freeCameraLookSensitivity = 0.1f;
    public float rotationSensitivity = 0.1f;


    public bool IsMouseAtLeftBorder
    {
        get
        {
            return mousePosition.x <= screenWidth * borderThreshold;
        }
    }
    public bool IsMouseAtRightBorder
    {
        get
        {
            return mousePosition.x >= screenWidth * (1 - borderThreshold);
        }
    }
    public bool IsMouseAtTopBorder
    {
        get
        {
            return mousePosition.y >= screenHeight * (1 - borderThreshold);
        }
    }
    public bool IsMouseAtBottomBorder
    {
        get
        {
            return mousePosition.y <= screenHeight * borderThreshold;
        }
    }

    // Input actions for controlling the camera
    private PlayerInputActions playerInputActions;


    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        playerInputActions.CameraControls.Enable();
        playerInputActions.CameraControls.MoveCamera.Enable();

        playerInputActions.CameraControls.Click.performed += ctx =>
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // Create a LayerMask that excludes the "Player" layer
            int layerMask = ~LayerMask.GetMask("Player");
            int layerMask2 = ~LayerMask.GetMask("Ignore Raycast");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask2))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        SwitchCamera(CameraState.Default);
                    }
                }
            }            
        };

        playerInputActions.CameraControls.RotateCamera.performed += ctx => SwitchCamera(CameraState.Rotation);
        playerInputActions.CameraControls.FreeCam.performed += ctx => SwitchCamera(CameraState.Free);
        playerInputActions.CameraControls.ResetCamera.performed += OnResetCamera;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }    


    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        CameraUIObject = GameObject.FindGameObjectWithTag("CameraModeIndicator");
        CameraUI = CameraUIObject.GetComponent<CameraModeIndicatorController>();
        CameraUIObject.SetActive(false);

        Target = player;

        SwitchCamera(CameraState.Default);

        RotationCamera.m_XAxis.m_MaxSpeed = rotationSensitivity * 300;
        RotationCamera.m_YAxis.m_MaxSpeed = rotationSensitivity * 2;

    }

    void Update()
    {
        if (AllowBoarderMovement)
        {
            mousePosition = Mouse.current.position.ReadValue();
            if (IsMouseAtLeftBorder || IsMouseAtRightBorder || IsMouseAtTopBorder || IsMouseAtBottomBorder)
            {
                SwitchCamera(CameraState.BorderMovement);
                HandleBorderMovement();
            }
        }
        if (freeCamera.Priority == 10)
        {
            HandleFreeCameraMovement();

        }
    }

    /// <summary>
    /// ControlMovement for Free Camera
    /// </summary>
    private void HandleFreeCameraMovement()
    {
        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert the screen position to a world position at a specified distance from the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, distanceFromCamera));

        // Move the lookAtTarget to the calculated world position
       // freeLook.transform.position = worldPosition;

        // Smoothly move the lookAtTarget to the calculated world position based on sensitivity
        freeLook.transform.position = Vector3.Lerp(freeLook.transform.position, worldPosition, freeCameraLookSensitivity * Time.deltaTime);

        // Read the input value from the MoveCamera action in Camera Controls map
        Vector2 moveInput = playerInputActions.CameraControls.MoveCamera.ReadValue<Vector2>();

        // Only apply movement in Free mode
        if (currentCamera == CameraState.Free)
        {
            // Calculate movement direction using WASD input
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y) * freeCameraSpeed * Time.deltaTime;

            // Move freeCamera based on its orientation
            freeCamera.transform.position += freeCamera.transform.TransformDirection(moveDirection);
        }
    }
    /// <summary>
    /// Handles Boarder Camera Movement.
    /// </summary>
    private void HandleBorderMovement()
    {
        // Move the camera based on which border the mouse is at
        Vector3 movement = Vector3.zero;        


        if (IsMouseAtLeftBorder)
        {
            movement += -BorderCamera.transform.right; // Move left
        }
        if (IsMouseAtRightBorder)
        {
            movement += BorderCamera.transform.right; // Move right
        }
        if (IsMouseAtTopBorder)
        {
            movement += BorderCamera.transform.forward; // Move forward/up
        }
        if (IsMouseAtBottomBorder)
        {
            movement += -BorderCamera.transform.forward; // Move backward/down
        }

        // Apply the movement
        BorderCamera.transform.position += movement * boarderCameraSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Added this as we use R to reset camera and it could mess up some stuff.
    /// </summary>
    /// <param name="ctx"></param>
    private void OnResetCamera(InputAction.CallbackContext ctx)
    {
        // Prevent the Reset action if in FirstPerson mode
        if (CurrentCamera == CameraState.FirstPerson)
        {
            Debug.Log("Reset Camera action ignored in FirstPerson mode.");
            return;
        }

        // Switch to Default camera if not in FirstPerson
        SwitchCamera(CameraState.Default);
    }

    /// <summary>
    /// Switch the active camera based on the enum state
    /// Reset all cameras to lower priority
    /// Activate the desired camera based on the state
    /// </summary>
    /// <param name="state"></param>
    public void SwitchCamera(CameraState state)
    {       
        DefaultCamera.Priority = 0;
        RotationCamera.Priority = 0;
        freeCamera.Priority = 0;
        BorderCamera.Priority = 0;
        FirstPersonCamera.Priority = 0;
        
        switch (state)
        {
            case CameraState.Default:
                DefaultCamera.Priority = 10;
                break;
            case CameraState.Rotation:
                RotationCamera.Priority = 10;
                break;
            case CameraState.Free:
                freeLook.transform.position = player.transform.position;
                freeCamera.Priority = 10;
                break;
            case CameraState.BorderMovement:
                BorderCamera.Priority = 10;
                break;
            case CameraState.FirstPerson:
                FirstPersonCamera.Priority = 10;
                break;
            default:
                DefaultCamera.Priority = 10;
                break;
        }
        CurrentCamera = state;
    }

    void OnDisable()
    {

        playerInputActions.CameraControls.Disable();
        playerInputActions.CameraControls.MoveCamera.Disable();

        playerInputActions.CameraControls.ResetCamera.performed -= OnResetCamera;

    }
}
