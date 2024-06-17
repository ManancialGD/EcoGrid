using System.Collections;
using UnityEngine;

/// <summary>
/// Controls grid-based movement of a GameObject towards a target position.
/// </summary>
public class GridMovement : MonoBehaviour
{
    [SerializeField] private bool isMoving; // Flag indicating if the object is currently moving
    private Vector3 targetPos, nextStepPos; // Target position and next step in the movement path
    [SerializeField] private float timeToMove = 0.2f; // Time taken to move from one cell to another
    [SerializeField] private float moveDelay = 0.2f; // Delay between consecutive movements
    [SerializeField] private bool isMovingToTarget; // Flag indicating if currently moving towards the target

    void Update()
    {
        // Check for input to initiate movement if not already moving
        if (Input.GetButtonDown("Click1") && !isMoving)
        {
            // Convert mouse position to world coordinates and round to nearest integers for grid movement
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector3(Mathf.Round(targetPos.x), Mathf.Round(targetPos.y), transform.position.z);
            
            if (targetPos == transform.position) return;
            isMovingToTarget = true;
        }

        // Check if currently moving towards the target
        if (isMovingToTarget)
        {
            // Start movement towards the next step in the path if not already moving
            if (!isMoving)
            {
                // Determine direction towards the next step in the movement path
                Vector3 direction = GetNextStepDirection(transform.position, targetPos);
                nextStepPos = transform.position + direction;
                StartCoroutine(Move(direction));
            }
        }
    }

    // Calculate the direction towards the next step in the movement path
    private Vector3 GetNextStepDirection(Vector3 currentPos, Vector3 targetPos)
    {
        Vector3 direction = Vector3.zero;
        float xDiff = Mathf.Abs(targetPos.x - currentPos.x);
        float yDiff = Mathf.Abs(targetPos.y - currentPos.y);

        if (xDiff > yDiff)
        {
            // Move along x-axis
            direction = (targetPos.x > currentPos.x) ? Vector3.right : Vector3.left;
        }
        else
        {
            // Move along y-axis
            direction = (targetPos.y > currentPos.y) ? Vector3.up : Vector3.down;
        }

        return direction;
    }

    // Coroutine for smoothly moving the object towards the next step in the movement path
    private IEnumerator Move(Vector3 direction)
    {
        isMoving = true;

        Vector3 origPos = transform.position;
        Vector3 targetPosition = origPos + direction;

        float elapsedTime = 0;

        // Move towards the target position over timeToMove seconds
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // Check if reached the absolute target (targetPos)
        if (transform.position == targetPos)
        {
            isMovingToTarget = false;
        }

        yield return new WaitForSeconds(moveDelay); // Introduce delay between movements
        
        isMoving = false;
    }
}
