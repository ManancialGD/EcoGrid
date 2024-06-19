using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls grid-based movement of a GameObject towards a target position.
/// </summary>
public class GridMovement : MonoBehaviour
{
    [SerializeField] private bool isMoving; // Flag indicating if the object is currently moving
    private Vector3 absoluteargetPos, nextStepPos; // Target position and next step in the movement path
    [SerializeField] private float movementSpeed = 0.25f;
    [SerializeField] private float moveDelay = 0.2f; // Delay between consecutive movements
    [SerializeField] private bool isMovingToTarget; // Flag indicating if currently moving towards the target
    [SerializeField] private bool shouldDrawLine = false;

    private List<Vector3> gizmoPoints = new List<Vector3>();
    private Queue<Vector3> stepsToTarget = new Queue<Vector3>();
    GridMovement[] farmers;
    CellTile.CellType cellType;
    GridManager gridManager;

    Vector3 debugLinePos;
    Vector3 debugLineTargetPos;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        farmers = FindObjectsOfType<GridMovement>();
        farmers = farmers.Where(GM => GM != this).ToArray();
    }

    void Update()
    {
        ClickToMove();
    }
    private void ClickToMove()
    {
        // Check for input to initiate movement if not already moving
        if (Input.GetButtonDown("Click1") && !isMoving)
        {
            // Convert mouse position to world coordinates and round to nearest integers for grid movement
            absoluteargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            absoluteargetPos = new Vector3(Mathf.Round(absoluteargetPos.x), Mathf.Round(absoluteargetPos.y), transform.position.z);

            if (absoluteargetPos == transform.position) return;
            stepsToTarget = Bresenham(transform.position, absoluteargetPos);
            isMovingToTarget = true;
        }

        // Check if currently moving towards the target
        if (isMovingToTarget)
        {
            // Start movement towards the next step in the path if not already moving
            if (!isMoving)
            {
                nextStepPos = stepsToTarget.Dequeue();

                // This will check if the tile he is moving to is none, so it doesn't move.
                if (gridManager.GetCellTypeInLocation(nextStepPos) == CellTile.CellType.none)/* Find another alternative */ return;
                else
                {
                    StartCoroutine(Move(nextStepPos));
                }
            }
        }
        else if (!isMoving)
        {
            Vector3 roundedPosition = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            transform.position = roundedPosition;
        }
    }

    /// <summary>
    /// This apply the Bresenham Algorithm to calculate each moviment and add all to a queue to return, the algorithm works based on the simple line function f(x) =ax+b, where 
    /// </summary>
    /// <param name="currentPos">This is the start position for the Bresenham line</param>
    /// <param name="endPos">This is the end position</param>
    /// <returns>A Queue that has every steps between the start pos and end pos using Bresenham function</returns>
    private Queue<Vector3> Bresenham(Vector3 startPos, Vector3 endPos)
    {
        Queue<Vector3> steps = new Queue<Vector3>();

        shouldDrawLine = true;
        gizmoPoints.Clear();
        debugLinePos = startPos;
        debugLineTargetPos = endPos;

        int x0 = Mathf.RoundToInt(startPos.x);
        int y0 = Mathf.RoundToInt(startPos.y);
        int x1 = Mathf.RoundToInt(endPos.x);
        int y1 = Mathf.RoundToInt(endPos.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            steps.Enqueue(new Vector3(x0, y0, 0));
            gizmoPoints.Add(new Vector3(x0, y0, 0));

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = err * 2;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        steps.Dequeue(); // This is to remove the first step, that is the same pos as the current pos.
        return steps;
    }

    // Coroutine for smoothly moving the object towards the next step in the movement path
    private IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        Vector3 origPos = transform.position;
        Vector3 targetPosition = direction;
        Vector3 dPos = targetPosition - origPos;
        float distance = dPos.x + dPos.y;
        distance = Mathf.Abs(distance);
        float timeToMove;

        timeToMove = distance / movementSpeed;

        float elapsedTime = 0;

        // Move towards the target position over timeToMove seconds
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // Check if reached the absolute target (absoluteargetPos)
        if (transform.position == absoluteargetPos)
        {
            isMovingToTarget = false;
        }

        yield return new WaitForSeconds(moveDelay); // Introduce delay between movements

        isMoving = false;
    }

    // Draw the Bersenham line and it's steps
    void OnDrawGizmos()
    {
        if (shouldDrawLine)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(debugLinePos, debugLineTargetPos);

            foreach (var point in gizmoPoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
