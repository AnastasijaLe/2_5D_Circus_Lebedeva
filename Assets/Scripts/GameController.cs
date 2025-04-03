using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private DiceRollScript diceScript;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private Image resetDiceImage;
    [SerializeField] private FadeScript fadeScript;
    [SerializeField] private GameObject endGameObject; // End–game UI object

    private bool moveStarted = false;
    private bool extraRollUsed = false;
    // Chest tiles (0-based indices): levels 4,16,28,46 → indices 3,15,27,45.
    private HashSet<int> chestTiles = new HashSet<int> { 3, 15, 27 }; // (adjust as needed)

    // Flag to indicate that the game has ended.
    private bool gameEnded = false;

    void Start()
    {
        if (TurnManager.Instance != null)
        {
            foreach (PlayerMover mover in TurnManager.Instance.players)
            {
                mover.InitializeBoard(boardManager);
            }
        }
        if (TurnManager.Instance.currentTurnIndex == 0)
        {
            diceScript.inputEnabled = true;
            resetDiceImage.raycastTarget = true;
        }
        else
        {
            diceScript.inputEnabled = false;
            resetDiceImage.raycastTarget = false;
        }
        endGameObject.SetActive(false);
    }

    void Update()
    {
        // If the game has ended, do nothing.
        if (gameEnded)
            return;

        if (TurnManager.Instance.currentTurnIndex == 0)
        {
            diceScript.inputEnabled = true;
            resetDiceImage.raycastTarget = true;
        }
        else
        {
            diceScript.inputEnabled = false;
            resetDiceImage.raycastTarget = false;
        }

        // Get the current player.
        PlayerMover currentPlayer = TurnManager.Instance.GetCurrentPlayer();

        // If the current player is finished, force a turn advance.
        if (currentPlayer != null && currentPlayer.isFinished)
        {
            TurnManager.Instance.NextTurn();
            CheckForGameEnd();
            return;
        }

        // If it's a bot turn (assume non-zero currentTurnIndex means bot), auto-roll.
        if (TurnManager.Instance.currentTurnIndex != 0)
        {
            if (!diceScript.firstThrow)
            {
                StartCoroutine(BotDiceRoll());
            }
        }

        // When the dice has been thrown, landed, and not yet processed, trigger movement.
        if (diceScript.firstThrow && diceScript.rollCompleted && !diceScript.rollConsumed && !moveStarted)
        {
            diceScript.inputEnabled = false;
            diceScript.rollConsumed = true;
            moveStarted = true;
            StartCoroutine(MoveAfterLanding());
        }
    }

    IEnumerator BotDiceRoll()
    {
        yield return new WaitForSeconds(1f);
        if (!diceScript.firstThrow)
        {
            diceScript.firstThrow = true;
            diceScript.rollCompleted = false;
            diceScript.rollConsumed = false;
            diceScript.diceFaceNum = "";
        
            Rigidbody rb = diceScript.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            diceScript.transform.rotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
            diceScript.RollDice();
        }
    }

    IEnumerator MoveAfterLanding()
    {
        float timeout = 10f;
        float timer = 0f;
        while ((string.IsNullOrEmpty(diceScript.diceFaceNum) ||
               !int.TryParse(diceScript.diceFaceNum, out _)) && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (timer >= timeout && TurnManager.Instance.currentTurnIndex != 0)
        {
            Debug.Log("Bot roll timeout reached. Reinitializing dice.");
            diceScript.Initialize(1);
            moveStarted = false;
            yield break;
        }
        if (int.TryParse(diceScript.diceFaceNum, out int number))
        {
            PlayerMover currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                currentPlayer.MoveSteps(number, () =>
                {
                    Debug.Log($"[GameController] Moved player {number} steps.");
                    // Use GetCurrentTileIndex() to check for chest tile.
                    if (chestTiles.Contains(currentPlayer.GetCurrentTileIndex()) && !extraRollUsed)
                    {
                        Debug.Log("[GameController] Chest tile! Granting an extra roll.");
                        extraRollUsed = true;
                        diceScript.ResetRollState();
                        moveStarted = false;
                        resetDiceImage.raycastTarget = (TurnManager.Instance.currentTurnIndex == 0);
                    }
                    else
                    {
                        TurnManager.Instance.NextTurn();
                        diceScript.ResetRollState();
                        moveStarted = false;
                        extraRollUsed = false;
                        resetDiceImage.raycastTarget = (TurnManager.Instance.currentTurnIndex == 0);
                        CheckForGameEnd();
                    }
                });
            }
            else
            {
                Debug.LogWarning("No current player found in TurnManager.");
                diceScript.ResetRollState();
                moveStarted = false;
                resetDiceImage.raycastTarget = (TurnManager.Instance.currentTurnIndex == 0);
            }
        }
        else
        {
            Debug.LogWarning("[GameController] Failed to parse diceFaceNum: " + diceScript.diceFaceNum);
            diceScript.ResetRollState();
            moveStarted = false;
            resetDiceImage.raycastTarget = (TurnManager.Instance.currentTurnIndex == 0);
        }
        yield return null;
    }

    private void CheckForGameEnd()
    {
        bool allFinished = true;
        foreach (PlayerMover mover in TurnManager.Instance.players)
        {
            if (!mover.isFinished)
            {
                allFinished = false;
                break;
            }
        }
        if (allFinished)
        {
            Debug.Log("[GameController] All players finished! Ending game.");
            gameEnded = true;
            StartCoroutine(EndGameRoutine());
        }
    }

    IEnumerator EndGameRoutine()
    {
        endGameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(fadeScript.FadeOut(0.05f));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenue");
    }
}
