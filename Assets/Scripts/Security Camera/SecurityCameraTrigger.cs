using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraTrigger : MonoBehaviour
{
    public SecurityCameraController controller;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.StartRecording();
        }
    }
}
