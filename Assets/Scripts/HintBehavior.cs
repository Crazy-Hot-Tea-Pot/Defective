using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintBehavior : MonoBehaviour
{
    // The maximum height of the up-and-down motion
    public float movementAmplitude = 1f;

    // Speed of the up-and-down motion
    public float movementSpeed = 1f;

    // Speed of the rotation
    public float rotationSpeed = 10f;

    private Vector3 initialPosition;

    private Camera playerCamera;

    void Awake()
    {
        playerCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Up and down movement
        float yOffset = Mathf.Sin(Time.time * movementSpeed) * movementAmplitude;
        transform.position = initialPosition + new Vector3(0, yOffset, 0);

        // Rotation
        //transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        if (playerCamera != null)
        {
            Vector3 direction = (playerCamera.transform.position - this.transform.position).normalized;
            direction.y = 0;
            this.transform.rotation = Quaternion.LookRotation(-direction);
        }
    }
}
