using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Camera cam;

    [Header("Camera Configuration")]
    [Header("Camera Limit (min, max)")]
    [SerializeField] private Vector2 xLimit; // Vector2(Xmin, Xmax)
    [SerializeField] private Vector2 yLimit; // Vector2(Ymin, Ymax)
    [SerializeField] private Vector2 cameraSizeLimit;

    [Space]

    [Header("Bools")]
    [SerializeField] private bool mouseDown;

#if UNITY_EDITOR
    [Space]

    [Header("Debug")]
    [SerializeField] Color debugColour = Color.blue;
#endif

    private Vector3 dragOrigin;

    private void Awake()
    {
        cam = Camera.main;
        if (cam == null) Debug.LogError("Camera is null");
    }

    private void Update()
    {
        Move();
        ScaleCam();
    }

    private void Move()
    {
        if (Input.GetButtonDown("CameraMove"))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetButton("CameraMove"))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            transform.position = ClampCamera(transform.position + difference);
        }
    }

    private void ScaleCam()
    {
        if (Input.mouseScrollDelta.y > 0f)
        {
            if (cam.orthographicSize > cameraSizeLimit.x)
            {
                cam.orthographicSize -= .5f;
                transform.position = ClampCamera(transform.position);
            }
        }
        else if (Input.mouseScrollDelta.y < 0f)
        {
            if (cam.orthographicSize < cameraSizeLimit.y)
            {
                cam.orthographicSize += .5f;
                transform.position = ClampCamera(transform.position);
            }
        }
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = xLimit.x + camWidth;
        float maxX = xLimit.y - camWidth;

        float minY = yLimit.x + camHeight;
        float maxY = yLimit.y - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }

    private void OnDrawGizmos()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        Gizmos.color = debugColour;

        // Draw the camera limits as wireframe boxes
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        Vector3 center = new Vector3((xLimit.x + xLimit.y) / 2f, (yLimit.x + yLimit.y) / 2f, 0f);
        Vector3 size = new Vector3(xLimit.y - xLimit.x, yLimit.y - yLimit.x, 0f);
        
        Gizmos.DrawWireCube(center, size);
    }
}
