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
    private GridMovement[] farmers;
    private GridManager gridManager;

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
            if (!isMoving && stepsToTarget.Count > 0)
            {
                nextStepPos = stepsToTarget.Dequeue();

                if (gridManager.GetCellTypeInLocation(nextStepPos) == CellTile.CellType.water ||
                    gridManager.GetCellTypeInLocation(nextStepPos) == CellTile.CellType.none)
                {   // Avoid obstruction.
                    StartCoroutine(FindAPathWithoutObstruction(nextStepPos));
                    stepsToTarget.Clear();
                }
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
    /// Applies the Bresenham Algorithm to generate a queue of steps between two positions on a grid.
    /// The algorithm calculates each step using integer-based arithmetic to efficiently find
    /// the points along the line from startPos to endPos.
    /// 
    /// The method initializes with rounded start (x0, y0) and end (x1, y1) positions. It calculates
    /// differences (dx, dy) and step directions (sx, sy) between these positions. An error variable
    /// (err) adjusts based on the differences, and it uses a loop to determine each point along the
    /// line using Bresenham's line algorithm. Points are added to the steps queue and gizmoPoints list
    /// for visualization.
    /// </summary>
    /// <param name="startPos">Starting position for the Bresenham line.</param>
    /// <param name="endPos">Ending position for the Bresenham line.</param>
    /// <returns>A queue containing each step along the Bresenham line from start to end positions.</returns>
    private Queue<Vector3> Bresenham(Vector3 startPos, Vector3 endPos)
    {
        Queue<Vector3> steps = new Queue<Vector3>();

        // Prepare for line visualization
        shouldDrawLine = true;
        gizmoPoints.Clear();
        debugLinePos = startPos;
        debugLineTargetPos = endPos;

        // Round positions to nearest integers
        int x0 = Mathf.RoundToInt(startPos.x);
        int y0 = Mathf.RoundToInt(startPos.y);
        int x1 = Mathf.RoundToInt(endPos.x);
        int y1 = Mathf.RoundToInt(endPos.y);

        // Calculate differences and steps
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int lastX = x0, lastY = y0;

        // Generate points along the line using Bresenham's algorithm
        while (true)
        {
            steps.Enqueue(new Vector3(x0, y0, 0)); // Add current point to steps queue
            gizmoPoints.Add(new Vector3(x0, y0, 0)); // Add current point for visualization

            lastX = x0;
            lastY = y0;
            // Check if we've reached the end point
            if (x0 == x1 && y0 == y1)
                break;

            int e2 = err * 2;

            // Adjust error and move to next point
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

            if (lastX != x0 && lastY != y0)
            {
                steps.Enqueue(new Vector3(lastX, y0, 0));
            }
        }

        steps.Dequeue(); // Remove the starting position from the steps queue
        return steps;
    }

    // Coroutine for smoothly moving the object towards the next step in the movement path
    private IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        Vector3 origPos = transform.position;
        Vector3 targetPosition = direction;
        float distance = Vector3.Distance(targetPosition, origPos);
        float timeToMove = distance / movementSpeed;

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

    private IEnumerator FindAPathWithoutObstruction(Vector3 startPosition)
    {
        bool foundAPath = false;
        int triesX = 0;
        int triesY = 0;
        Vector3[] directions = new Vector3[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down
        };

        while (!foundAPath && triesX < 5 && triesY < 5)
        {
            foreach (Vector3 direction in directions)
            {
                Vector3 testPositionX = startPosition + (direction * (triesX + 1));
                Vector3 testPositionY = startPosition + (direction * (triesY + 1));

                if (gridManager.GetCellTypeInLocation(testPositionX) != CellTile.CellType.water &&
                    gridManager.GetCellTypeInLocation(testPositionX) != CellTile.CellType.none)
                {
                    Queue<Vector3> steps = Bresenham(testPositionX, absoluteargetPos);
                    bool obstructionFound = false;

                    foreach (Vector3 p in steps)
                    {
                        if (gridManager.GetCellTypeInLocation(p) == CellTile.CellType.water ||
                            gridManager.GetCellTypeInLocation(p) == CellTile.CellType.none)
                        {
                            obstructionFound = true;
                            break;
                        }
                    }

                    if (!obstructionFound)
                    {
                        foundAPath = true;
                        stepsToTarget = steps;
                        Debug.Log("Path found without obstructions!");
                        break;
                    }
                }

                if (gridManager.GetCellTypeInLocation(testPositionY) != CellTile.CellType.water &&
                    gridManager.GetCellTypeInLocation(testPositionY) != CellTile.CellType.none)
                {
                    Queue<Vector3> steps = Bresenham(testPositionY, absoluteargetPos);
                    bool obstructionFound = false;

                    foreach (Vector3 p in steps)
                    {
                        if (gridManager.GetCellTypeInLocation(p) == CellTile.CellType.water ||
                            gridManager.GetCellTypeInLocation(p) == CellTile.CellType.none)
                        {
                            obstructionFound = true;
                            break;
                        }
                    }

                    if (!obstructionFound)
                    {
                        foundAPath = true;
                        stepsToTarget = steps;
                        Debug.Log("Path found without obstructions!");
                        break;
                    }
                }
            }

            if (!foundAPath)
            {
                triesX++;
                triesY++;
                yield return null; // Wait a frame before retrying
            }
        }

        if (!foundAPath)
        {
            Debug.Log("Failed to find a path without obstructions after 5 tries.");
            isMovingToTarget = false;
            isMoving = false;
        }
    }

    // Draw the Bresenham line and its steps
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
