using Cinemachine;
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
        Combat,
        FirstPerson,
        Call
    }

    [Header("Cameras")]
    private CameraState currentCamera;
    private CameraState previousCamera;

    public CinemachineVirtualCamera DefaultCamera;
    public CinemachineFreeLook RotationCamera;
    public CinemachineVirtualCamera freeCamera;
    public CinemachineVirtualCamera BorderCamera;
    public CinemachineVirtualCamera FirstPersonCamera;
    public CinemachineVirtualCamera CallCamera;
    public CinemachineVirtualCamera CombatCamera
    {
        get
        {
            return combatCamera;
        }
        private set
        {
            combatCamera = value;
        }
    }
    private CinemachineVirtualCamera combatCamera;

    public CameraState CurrentCamera
    {
        get
        {
            return currentCamera;  
        }
        private set
        {
            previousCamera = currentCamera;           

            currentCamera = value;

            //Call Camera indicator if camera changes
            if (currentCamera != previousCamera)
            {

                //Indicate to Player what camera mode they changed to.
                UiManager.Instance.UpdateCameraIndicator(value);                
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

    private float baseCameraSpeed;

    public float CurrentCameraSpeed
    {
        get;
        private set;
    }
    
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
                    if (hit.collider.CompareTag("Ground") && (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Roaming || GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat))
                    {
                        OnResetCamera();
                    }
                }
            }            
        };

        // Handle rotation start and stop
        playerInputActions.CameraControls.RotateCamera.started += ctx => SwitchCamera(CameraState.Rotation);
        playerInputActions.CameraControls.RotateCamera.canceled += ctx => OnResetCamera();


        playerInputActions.CameraControls.FreeCam.performed += ctx => SwitchCamera(CameraState.Free);
        playerInputActions.CameraControls.ResetCamera.performed += ctx => OnResetCamera();
        playerInputActions.CameraControls.IncreaseCameraSpeed.performed += OnIncreaseCameraSpeed;
        playerInputActions.CameraControls.IncreaseCameraSpeed.canceled += OnResetCameraSpeed;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }    


    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        CallCamera = GameObject.Find("CallCamera").GetComponent<CinemachineVirtualCamera>();

        Target = player;

        SwitchCamera(CameraState.Default);

        RotationCamera.m_XAxis.m_MaxSpeed = rotationSensitivity * 300;
        RotationCamera.m_YAxis.m_MaxSpeed = rotationSensitivity * 2;

        //Set Base cameraSpeed;
        baseCameraSpeed = SettingsManager.Instance.CameraSettings.CameraSpeed;

        CurrentCameraSpeed = baseCameraSpeed;

        GameManager.Instance.OnStartCombat += StartCombat;
        GameManager.Instance.OnEndCombat += EndCombat;

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
    /// Set Combat camera
    /// </summary>
    /// <param name="CurrentCombatCamera"></param>
    public void UpdateCombatCamera(GameObject CurrentCombatCamera)
    {
        CombatCamera = CurrentCombatCamera.GetComponent<CinemachineVirtualCamera>();      
    }

    /// <summary>
    /// ControlMovement for Free Camera
    /// </summary>
    private void HandleFreeCameraMovement()
    {
        if (IsPlayerInteracting())
        {
            Debug.Log("Camera reset ignored because the player is interacting.");
            return;
        }
        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert the screen position to a world position at a specified distance from the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, distanceFromCamera));

        // Smoothly move the lookAtTarget to the calculated world position based on sensitivity
        freeLook.transform.position = Vector3.Lerp(freeLook.transform.position, worldPosition, freeCameraLookSensitivity * Time.deltaTime);

        // Read the input value from the MoveCamera action in Camera Controls map
        Vector2 moveInput = playerInputActions.CameraControls.MoveCamera.ReadValue<Vector2>();

        // Only apply movement in Free mode
        if (currentCamera == CameraState.Free)
        {
            // Handle forward/backward movement (W/S) based on the camera's forward vector
            Vector3 forwardMovement = freeCamera.transform.forward * moveInput.y;

            // Handle lateral movement (A/D) based on the world space right vector
            Vector3 lateralMovement = freeCamera.transform.right * moveInput.x;

            // Combine movements and ignore vertical movement (Y-axis)
            Vector3 moveDirection = forwardMovement + lateralMovement;
            moveDirection.y = 0;

            // Move the camera's position
            freeCamera.transform.position += moveDirection * CurrentCameraSpeed * Time.deltaTime;

            // Move the LookAtTarget (freeLook) to maintain its relative position to the camera
            freeLook.transform.position += lateralMovement * CurrentCameraSpeed * Time.deltaTime;
        }
    }
    /// <summary>
    /// Handles Boarder Camera Movement.
    /// </summary>
    private void HandleBorderMovement()
    {
        if (IsPlayerInteracting())
        {
            Debug.Log("Camera reset ignored because the player is interacting.");
            return;
        }

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
        BorderCamera.transform.position += movement * CurrentCameraSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Added this as we use R to reset camera and it could mess up some stuff.
    /// </summary>
    private void OnResetCamera()
    {
        // Prevent the Reset action if Player is interacting
        if (IsPlayerInteracting())
        {
            Debug.Log("Camera reset ignored because the player is interacting.");
            return;
        }
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameManager.GameMode.Roaming:
                if (CurrentCamera != CameraState.Default)
                    // Switch to Default camera if not in FirstPerson
                    SwitchCamera(CameraState.Default);
                break;
            case GameManager.GameMode.Combat:
                SwitchCamera(CameraState.Combat);
                break;
        }        
    }

    /// <summary>
    /// Switch the active camera based on the enum state
    /// Reset all cameras to lower priority
    /// Activate the desired camera based on the state
    /// </summary>
    /// <param name="state"></param>
    public void SwitchCamera(CameraState state)
    {
        if (IsPlayerInteracting())
        {
            Debug.Log("Camera reset ignored because the player is interacting.");
            return;
        }
        DefaultCamera.Priority = 0;
        RotationCamera.Priority = 0;
        freeCamera.Priority = 0;
        BorderCamera.Priority = 0;
        FirstPersonCamera.Priority = 0;
        CallCamera.Priority = 0;

        if(CombatCamera!=null)
            CombatCamera.Priority = 0;
        
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
            case CameraState.Combat:
                CombatCamera.Priority = 10;
                break;
            case CameraState.Call:
                CallCamera.Priority = 10;
                break;
            default:
                DefaultCamera.Priority = 10;
                break;
        }
        CurrentCamera = state;
    }

    /// <summary>
    /// Check if Player is interacting
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerInteracting()
    {
        if(GameManager.Instance.CurrentGameMode==GameManager.GameMode.Interacting || GameManager.Instance.CurrentGameMode == GameManager.GameMode.CombatLoot)
            return true;
        else
            return false;        
    }

    private void OnIncreaseCameraSpeed(InputAction.CallbackContext ctx)
    {
        CurrentCameraSpeed = baseCameraSpeed * 2;
    }

    private void OnResetCameraSpeed(InputAction.CallbackContext ctx)
    {
        CurrentCameraSpeed = baseCameraSpeed;
    }

    private void StartCombat()
    {
        if (CombatCamera == null)
        {
            Debug.Log("Why are you null!!");
            Debug.Log(CombatCamera.transform.position);
            Debug.Log(this.gameObject.name);
            Debug.Log(this.gameObject.transform.position);
            Debug.Log("why are you null");
        }

        SwitchCamera(CameraState.Combat);
    }
    private void EndCombat()
    {
        SwitchCamera(CameraState.Default);

        CombatCamera = null;
    }
    void OnDisable()
    {

        playerInputActions.CameraControls.Disable();
        playerInputActions.CameraControls.MoveCamera.Disable();

        playerInputActions.CameraControls.IncreaseCameraSpeed.performed -= OnIncreaseCameraSpeed;
        playerInputActions.CameraControls.IncreaseCameraSpeed.canceled -= OnResetCameraSpeed;

        GameManager.Instance.OnStartCombat -= StartCombat;
        GameManager.Instance.OnEndCombat -= EndCombat;

    }
}
