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

    [Header("To See Preview turn on Monitor\nAlso leave checked if you want display always on in Game World.")]
    public bool useSecurityMonitor = false;

    [Header("Components")]
    public Camera securityCamera;
    public GameObject SecurityMonitor;
    public GameObject TriggerZonePrefab;
    public GameObject TriggerZone;  
    
    [HideInInspector]
    [SerializeField] private Vector3 triggerZonePosition;
    [HideInInspector]
    [SerializeField] private Vector3 triggerZoneRotation;
    [HideInInspector]
    [SerializeField] private Vector3 triggerZoneSize = Vector3.one;

    private Transform Player;
    private Material screenMaterial;
    private RenderTexture renderTexture;

    void OnValidate()
    {
        ApplySettings();
    }

    // Start is called before the first frame update
    void Start()
    {        
        if (followPlayer)
        {
           Player = GameObject.FindGameObjectWithTag("Player").transform;

            if (TriggerZone != null) {
                StartCoroutine(FollowPlayer());
            }
        }
    }
    /// <summary>
    /// Turn on monitor and follow player if checked
    /// </summary>
    public void StartRecording()
    {
        SecurityMonitor.SetActive(true);

        if(followPlayer)
            StartCoroutine(FollowPlayer());
    }
    /// <summary>
    /// Turn off monitor and follow player.
    /// </summary>
    public void StopRecording()
    {
        StopCoroutine(FollowPlayer());

        SecurityMonitor.SetActive(false);        
    }
    private void ApplySettings()
    {
        if (securityCamera)
        {
            securityCamera.transform.localPosition = cameraPosition;
            securityCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
            securityCamera.fieldOfView = fieldOfView;
        }

        SecurityMonitor.SetActive(useSecurityMonitor);

        if (useSecurityMonitor && SecurityMonitor)
        {
            Renderer monitorRenderer = SecurityMonitor.GetComponent<Renderer>();

            if (!Application.isPlaying)
            {
                if (monitorRenderer.sharedMaterial == null)
                    return;

                screenMaterial=monitorRenderer.sharedMaterial;
            }
            else
            {
                // Ensure each Security Monitor has its own material without creating leaks
                if (monitorRenderer.sharedMaterial == null || monitorRenderer.sharedMaterial.name.EndsWith("(Instance)"))
                {
                    screenMaterial = new Material(monitorRenderer.sharedMaterial); // Duplicate only if necessary
                    screenMaterial.name = "MonitorMaterial_" + GetInstanceID(); // Unique name for clarity
                    monitorRenderer.material = screenMaterial;
                }
                else
                {
                    screenMaterial = monitorRenderer.material; // Use existing one if already unique
                }

                // Ensure each monitor has its own render texture
                if (screenMaterial.mainTexture == null || !(screenMaterial.mainTexture is RenderTexture))
                {
                    renderTexture = new RenderTexture(1920, 1080, 16);
                    securityCamera.targetTexture = renderTexture;
                    screenMaterial.mainTexture = renderTexture;
                }
            }
        }

        if (TriggerZone)
        {
            TriggerZone.transform.localPosition = triggerZonePosition;
            TriggerZone.transform.localEulerAngles = triggerZoneRotation;
            TriggerZone.transform.localScale = triggerZoneSize;
        }
    }

    private IEnumerator FollowPlayer()
    {
        while (followPlayer && Player != null)
        {
            Vector3 direction = Player.position - securityCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            securityCamera.transform.rotation = Quaternion.Slerp(securityCamera.transform.rotation, targetRotation, Time.deltaTime * 2f);
            yield return null;
        }
    }   
}
