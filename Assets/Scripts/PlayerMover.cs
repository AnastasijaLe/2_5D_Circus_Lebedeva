using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private List<Transform> tiles = new List<Transform>();
    public int currentTileIndex = 0;
    private bool isMoving = false;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform nameField;
    public bool isFinished = false;
    private const int rowLength = 8;
    private HashSet<int> hurtTiles = new HashSet<int>() { 2, 7, 11, 17, 21, 28, 39, 44, 46 };
    public Vector3 offset;
    private int safeTileIndex;


    public void MoveSteps(int steps, System.Action onComplete = null)
    {
        int finalTileIndex = tiles.Count - 1;  // Final tile index
        safeTileIndex = currentTileIndex;

        if (!isMoving)
        {
            // Exactly finish on the final tile.
            if (currentTileIndex + steps == finalTileIndex)
            {
                Debug.Log($"[PlayerMover] {gameObject.name} landed exactly on the final tile.");
                StartCoroutine(MoveToTileWithFinish(finalTileIndex, () => {
                    StartCoroutine(CheckAndDoHurt(() => {
                        onComplete?.Invoke();
                    }));
                }));
            }
            // Overshoot: bounce back.
            else if (currentTileIndex + steps > finalTileIndex)
            {
                int forwardSteps = finalTileIndex - currentTileIndex;  // Steps to reach final tile.
                int backwardSteps = steps - forwardSteps;              // Extra steps to move backward.
                Debug.Log($"[PlayerMover] Bounce move: forward {forwardSteps} then backward {backwardSteps}");
                StartCoroutine(MoveForwardThenBackward(forwardSteps, backwardSteps, () => {
                    StartCoroutine(CheckAndDoHurt(() => {
                        onComplete?.Invoke();
                    }));
                }));
            }
            // Normal move.
            else
            {
                 Debug.Log($"[PlayerMover] Moving {steps} steps forward from tile {currentTileIndex}.");
                StartCoroutine(MoveToTile(currentTileIndex + steps, () => {
                    StartCoroutine(CheckAndDoHurt(() => {
                        onComplete?.Invoke();
                    }));
                }));
            }
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    // Moves one tile at a time until the target tile index is reached.
    // Note: We do NOT check for bridging after every tile, but only once after reaching the final tile.
    IEnumerator MoveToTile(int targetIndex, System.Action onComplete)
    {
        if (animator != null)
            animator.SetBool("isWalking", true);
        isMoving = true;

        while (currentTileIndex < targetIndex)
        {
            int nextTile = currentTileIndex + 1;
            Vector3 nextPos = tiles[nextTile].position + new Vector3(0, 0.8f, -0.7f) + offset;

            // Move toward the next tile.
            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed*Time.deltaTime);
                yield return null;
            }
            currentTileIndex++;
            // Apply flipping logic for every tile we land on.
            ApplyFlipLogic(currentTileIndex);
            yield return null;
        }

        // Final bridging check: only trigger if the final tile is a bridge start.
        yield return StartCoroutine(CheckAndDoBridging());
        ApplyFlipLogic(currentTileIndex);

        if (animator != null)
            animator.SetBool("isWalking", false);
        isMoving = false;
        onComplete?.Invoke();
    }

    // Checks if the current tile is a bridge start and, if so, performs the bridging move.
    IEnumerator CheckAndDoBridging()
    {
        if (boardManager.bridging.ContainsKey(currentTileIndex))
        {
            int destination = boardManager.bridging[currentTileIndex];
            Debug.Log($"[PlayerMover] Bridge from tile {currentTileIndex} to tile {destination} triggered!");
            Vector3 bridgeDestPos = tiles[destination].position + new Vector3(0, 0.8f, -0.7f) + offset;

            // Animate the bridging move.
            while (Vector3.Distance(transform.position, bridgeDestPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, bridgeDestPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            currentTileIndex = destination;
        }
    }

    // Flipping logic based on the 1-indexed level number.
     private void ApplyFlipLogic(int tileIndex)
    {
        int row = tileIndex / rowLength;
        Vector3 newScale = transform.localScale;
        // Assume row 0 is default (face right) and odd rows are flipped.
        newScale.x = (row % 2 == 0) ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        transform.localScale = newScale;
        
        if (nameField != null)
        {
            Vector3 nameScale = nameField.localScale;
            nameScale.x = (transform.localScale.x < 0) ? -Mathf.Abs(nameScale.x) : Mathf.Abs(nameScale.x);
            nameField.localScale = nameScale;
        }
    }

    IEnumerator MoveForwardThenBackward(int forwardSteps, int backwardSteps, System.Action onComplete)
    {
        int targetIndexForward = currentTileIndex + forwardSteps;
        yield return StartCoroutine(MoveToTile(targetIndexForward, null));

        int targetIndexBackward = currentTileIndex - backwardSteps;
        yield return StartCoroutine(MoveToTileBackward(targetIndexBackward, onComplete));
    }

    IEnumerator MoveToTileBackward(int targetIndex, System.Action onComplete)
    {
        if (animator != null)
            animator.SetBool("isWalking", true);
        isMoving = true;

        while (currentTileIndex > targetIndex)
        {
            int prevTile = currentTileIndex - 1;
            Vector3 nextPos = tiles[prevTile].position + new Vector3(0, 0.8f, -0.7f) + offset;
            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            currentTileIndex--;
            yield return new WaitForSeconds(0.1f);
        }
        if (animator != null)
            animator.SetBool("isWalking", false);
        isMoving = false;
        onComplete?.Invoke();
    }

    IEnumerator MoveToTileWithFinish(int targetIndex, System.Action onComplete)
    {
        if (animator != null)
            animator.SetBool("isWalking", true);
        isMoving = true;

        while (currentTileIndex < targetIndex)
        {
            int nextTile = currentTileIndex + 1;
            Vector3 nextPos = tiles[nextTile].position + new Vector3(0, 0.8f, -0.7f) + offset;
            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            currentTileIndex++;
            yield return new WaitForSeconds(0.1f);
        }

        // Final bridging check for finishing move.
        yield return StartCoroutine(CheckAndDoBridging());
        ApplyFlipLogic(currentTileIndex);

        // Slide a little to the left to indicate finishing.
        Vector3 finishPos = transform.position + new Vector3(-1f, 0, 0);
        while (Vector3.Distance(transform.position, finishPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finishPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isFinished = true;
        if (animator != null)
            animator.SetBool("isWalking", false);
        isMoving = false;
        onComplete?.Invoke();
    }

    IEnumerator CheckAndDoHurt(System.Action onHurtComplete)
{
    if (hurtTiles.Contains(currentTileIndex))
    {
        Debug.Log($"[PlayerMover] Hurt tile detected at tile {currentTileIndex}.");
        yield return StartCoroutine(HandleHurtTile());
    }
    onHurtComplete?.Invoke();
}

IEnumerator HandleHurtTile()
{
    // Play hurt animation.
    if (animator != null)
    {
        animator.SetBool("isHurt", true);
        yield return new WaitForSeconds(1.5f); // Duration of hurt animation.
        animator.SetBool("isHurt", false);
    }

    // Blink effect.
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    float duration = 2f;
    float blinkInterval = 0.3f;
    float timer = 0f;

     // "Respawn" the character to the previous tile (if available).
    currentTileIndex = safeTileIndex;
    transform.position = tiles[safeTileIndex].position + new Vector3(0, 0.8f, -0.7f) + offset;
    ApplyFlipLogic(currentTileIndex);
    yield return null;

    while (timer < duration)
    {
        foreach (Renderer r in renderers)
            r.enabled = false;
        yield return new WaitForSeconds(blinkInterval);
        foreach (Renderer r in renderers)
            r.enabled = true;
        yield return new WaitForSeconds(blinkInterval);
        timer += blinkInterval * 2;
    }

}

    public void InitializeBoard(BoardManager board)
    {
        boardManager = board;
        tiles = board.boardTiles;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

        public int GetCurrentTileIndex()
    {
        return currentTileIndex;
    }

}
