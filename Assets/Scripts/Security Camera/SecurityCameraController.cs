using Cinemachine;
using System.IO;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
    public float fieldOfView = 60f;
    public bool followPlayer = false;
    public bool useSecurityMonitor = true;
    public bool saveVideo = false;
    public float recordingTimeLimit = 5f;

    [Header("Components")]
    public Camera securityCamera;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject SecurityMonitor;
    public GameObject TriggerZonePrefab;
    public GameObject TriggerZone;

    private RenderTexture renderTexture;
    private Material screenMaterial;
    private bool isRecording = false;
    private float recordingTimer = 0f;
    private string savePath;
    void OnValidate()
    {
        ApplySettings();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isRecording)
        {
            recordingTimer += Time.deltaTime;
            if (recordingTimer >= recordingTimeLimit)
            {
                StopRecording();
            }
        }
    }
    public void StartRecording()
    {
        if (!isRecording && saveVideo)
        {
            isRecording = true;
            recordingTimer = 0f;
            savePath = Path.Combine(Application.persistentDataPath, $"SecurityCam_{System.DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            Debug.Log($"Recording started: {savePath}");
        }
    }
    private void ApplySettings()
    {
        if (securityCamera)
        {
            securityCamera.transform.localPosition = cameraPosition;
            securityCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
            securityCamera.fieldOfView = fieldOfView;
        }

        if (virtualCamera)
        {
            virtualCamera.gameObject.SetActive(followPlayer);
        }

        if (SecurityMonitor)
        {
            SecurityMonitor.SetActive(useSecurityMonitor);
        }

        if (useSecurityMonitor)
        {
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(256, 256, 16);
                securityCamera.targetTexture = renderTexture;
            }
            if (screenMaterial == null && SecurityMonitor)
            {
                screenMaterial = new Material(SecurityMonitor.GetComponent<Renderer>().sharedMaterial);
                SecurityMonitor.GetComponent<Renderer>().material = screenMaterial;
            }
            screenMaterial.mainTexture = renderTexture;
        }

        if (TriggerZone)
        {
            TriggerZone.transform.localPosition = Vector3.zero;
        }
    }    

    private void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            Debug.Log($"Recording saved: {savePath}");
        }
    }
}
