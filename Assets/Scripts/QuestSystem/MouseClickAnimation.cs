using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickAnimation : MonoBehaviour
{
    private Camera playerCamera;

    void Awake()
    {
        playerCamera = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FaceCamera();
    }

    /// <summary>
    /// Make the canvas face the Player camera
    /// </summary>
    private void FaceCamera()
    {
        if (playerCamera != null)
        {
            Vector3 direction = (playerCamera.transform.position - this.transform.position).normalized;
            direction.y = 0;
            this.transform.rotation = Quaternion.LookRotation(-direction);
        }
    }
}
