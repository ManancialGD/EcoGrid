using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("CameraConfiguration")]
    [Header("Camera Limit (min, max)")]
    [SerializeField] private Vector2 xLimit; // Vector2(Xmin, Xmax)
    [SerializeField] private Vector2 yLimit; // Vector2(Ymin, Ymax)

    [Header("Sensibility")]

    [SerializeField] private float sensibility = 0.01f;
    [Space]

    [Header("Bools")]
    [SerializeField] private bool mouseDown;

    // This is for debugging, so it will run this only if you are in the editor
    #if UNITY_EDITOR
    [Space]

    [Header("Debug")]
    [SerializeField] Color debugColour = Color.blue;
    #endif

    private Vector3 initialMousePosition;
    private Vector3 initialCameraPosition;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (Input.GetButtonDown("CameraMove"))
        {
            mouseDown = true;
            initialMousePosition = Input.mousePosition;
            initialCameraPosition = transform.position;
        }
        else if (Input.GetButtonUp("CameraMove"))
        {
            mouseDown = false;
        }

        if (mouseDown)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 difference = currentMousePosition - initialMousePosition;

            Vector3 targetPosition = initialCameraPosition - new Vector3(difference.x, difference.y, 0) * sensibility;

            // Clamp the target position within the specified limits
            float clampedX = Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y);
            float clampedY = Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y);
            float clampedZ = transform.position.z; // Maintain current z-position

            Vector3 clampedPosition = new Vector3(clampedX, clampedY, clampedZ);
            transform.position = clampedPosition;
        }
    }

    /// <summary>
    /// This will draw a box with the limits on the scene.
    /// </summary>
    private void OnDrawGizmos()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        Gizmos.color = debugColour;

        // Calculate the center and size of the rectangle, adjusted for camera size
        float camHeight = cam.orthographicSize * 2;
        float camWidth = camHeight * cam.aspect;

        Vector3 center = new Vector3((xLimit.x + xLimit.y) / 2, (yLimit.x + yLimit.y) / 2, transform.position.z);
        Vector3 size = new Vector3(xLimit.y - xLimit.x + camWidth, yLimit.y - yLimit.x + camHeight, 1);

        // Draw the wireframe cube
        Gizmos.DrawWireCube(center, size);
    }
}
