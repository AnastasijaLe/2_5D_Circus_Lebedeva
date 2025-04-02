using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private List<Transform> tiles = new List<Transform>();
    private int currentTileIndex = 0;
    private bool isMoving = false;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform nameField;
    public Vector3 offset;

    public void MoveSteps(int steps, System.Action onComplete = null)
    {
        if (!isMoving && currentTileIndex + steps < tiles.Count)
        {
            StartCoroutine(MoveToTile(currentTileIndex + steps, onComplete));
              Debug.Log($"MoveSteps called on {gameObject.name} with {steps} steps");
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    IEnumerator MoveToTile(int targetIndex, System.Action onComplete)
    {
        if (animator != null)
            animator.SetBool("isWalking", true);

        isMoving = true;

        while (currentTileIndex < targetIndex)
        {
            Vector3 nextPos = tiles[currentTileIndex + 1].position + new Vector3(0, 0.8f, -0.7f) + offset;

            while (Vector3.Distance(transform.position, nextPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentTileIndex++;

             // Determine level number (1-indexed)
            int levelNumber = currentTileIndex + 1;
            // If this tile is the first tile of a new row that requires flipping:
            if (levelNumber == 9 || levelNumber == 17 || levelNumber == 25 || levelNumber == 33 || levelNumber == 41)
            {
                // Calculate the row index (0-indexed)
                int row = (levelNumber - 1) / 8;
                Vector3 newScale = transform.localScale;
                if (row % 2 == 0)
                {
                    // Even rows (0, 2, …): face right (default orientation)
                    newScale.x = Mathf.Abs(newScale.x);
                }
                else
                {
                    // Odd rows (1, 3, …): face left (flip horizontally)
                    newScale.x = -Mathf.Abs(newScale.x);
                }
                transform.localScale = newScale;


                 if (nameField != null)
                {
                    Vector3 nameScale = nameField.localScale;
                    nameScale.x = (transform.localScale.x < 0) ? -Mathf.Abs(nameScale.x) : Mathf.Abs(nameScale.x);
                    nameField.localScale = nameScale;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

          if (animator != null)
            animator.SetBool("isWalking", false);

        isMoving = false;
        onComplete?.Invoke();
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


}
