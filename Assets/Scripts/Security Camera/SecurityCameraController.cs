using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
    public float fieldOfView = 40f;
    public bool followPlayer = false;

    [Header("To See Preview turn on Monitor")]
    public bool useSecurityMonitor = false;

    [Header("Take save footage for Win Condition")]
    public bool saveVideo = false;

    [Range(0f,60f)]
    [Tooltip("This is in seconds!")]
    public float recordingTimeLimit = 5f;

    [Header("Components")]
    public Camera securityCamera;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject SecurityMonitor;
    public GameObject TriggerZonePrefab;
    public GameObject TriggerZone;    
    [HideInInspector]
    [SerializeField] private Vector3 triggerZonePosition;
    [HideInInspector]
    [SerializeField] private Vector3 triggerZoneRotation;
    [HideInInspector]
    [SerializeField] private Vector3 triggerZoneSize = Vector3.one;


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
        if (followPlayer)
        {
            virtualCamera.LookAt = GameObject.FindGameObjectWithTag("Player").transform;
        }
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
            savePath = Path.Combine(DataManager.Instance.GetFootageDirectory(), $"SecurityCam_{System.DateTime.Now:yyyyMMdd_HHmmss}.vid");
            Debug.Log($"Recording started: {savePath}");


            //Start capturing frames
            StartCoroutine(CaptureVideo());

            // Add the file to the GameData record list
            DataManager.Instance.CurrentGameData.SecurityCamRecordings.Add(savePath);
        }
    }
    private void ApplySettings()
    {
        if (securityCamera)
        {            
            if (followPlayer)
            {
                virtualCamera.transform.localPosition = cameraPosition;
                virtualCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
                virtualCamera.m_Lens.FieldOfView = fieldOfView;
            }
            else
            {
                securityCamera.transform.localPosition = cameraPosition;
                securityCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
                securityCamera.fieldOfView = fieldOfView;
            }
        }

        if (virtualCamera)
        {
            virtualCamera.gameObject.SetActive(followPlayer);

            if (followPlayer)
            {
                // Prevent movement
                virtualCamera.Follow = null;
                // Only rotate to look at player
                virtualCamera.LookAt = GameObject.FindGameObjectWithTag("Player").transform;
                securityCamera.GetComponent<CinemachineBrain>().enabled = true;
            }
            else
            {
                virtualCamera.LookAt = null;
                securityCamera.GetComponent<CinemachineBrain>().enabled = false;
            }
        }

        if (SecurityMonitor)
        {
            SecurityMonitor.SetActive(useSecurityMonitor);
        }

        if (useSecurityMonitor)
        {
            if (renderTexture == null)
            {
                renderTexture = new RenderTexture(1920, 1080, 16);
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
            TriggerZone.transform.localPosition = triggerZonePosition;
            TriggerZone.transform.localEulerAngles = triggerZoneRotation;
            TriggerZone.transform.localScale = triggerZoneSize;
        }
    }
    private IEnumerator CaptureVideo()
    {
        List<byte[]> frameDataList = new List<byte[]>(); // Store raw frame data
        float startTime = Time.time;

        while (isRecording && (Time.time - startTime) < recordingTimeLimit)
        {
            yield return new WaitForEndOfFrame(); // Wait until frame is fully rendered

            // Capture the current frame
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = securityCamera.targetTexture;

            Texture2D frameTexture = new Texture2D(securityCamera.targetTexture.width, securityCamera.targetTexture.height, TextureFormat.RGB24, false);
            frameTexture.ReadPixels(new Rect(0, 0, frameTexture.width, frameTexture.height), 0, 0);
            frameTexture.Apply();

            RenderTexture.active = currentRT; // Restore previous render texture

            // Convert frame to raw byte data
            byte[] frameData = frameTexture.EncodeToPNG(); // PNG format for lossless quality
            frameDataList.Add(frameData);

            Destroy(frameTexture); // Clean up texture to avoid memory leaks

            yield return null; // Wait for the next frame
        }

        // Save all frames into a .vid file
        SaveVideoFile(frameDataList);

        isRecording = false;
        Debug.Log($"Recording saved: {savePath}");
    }
    private void SaveVideoFile(List<byte[]> frameDataList)
    {
        try
        {
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                // Write a simple file header (resolution, frame count)
                writer.Write("VIDF"); // Custom file identifier
                writer.Write(securityCamera.targetTexture.width);
                writer.Write(securityCamera.targetTexture.height);
                writer.Write(frameDataList.Count);

                // Write each frame as raw PNG byte data
                foreach (byte[] frameData in frameDataList)
                {
                    writer.Write(frameData.Length);
                    writer.Write(frameData);
                }
            }

            Debug.Log($"Security Camera footage saved: {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving video file: {e.Message}");
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
