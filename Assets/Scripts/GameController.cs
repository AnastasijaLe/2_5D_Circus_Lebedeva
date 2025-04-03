using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private DiceRollScript diceScript;
    [SerializeField] private BoardManager boardManager;
    // This flag prevents multiple simultaneous move coroutines.
    private bool moveStarted = false;
    public Image resetDiceImage;
    private HashSet<int> chestTiles = new HashSet<int> {3, 15, 27};
    private bool extraRollUsed = false;

    void Start()
    {
        if (TurnManager.Instance != null)
        {
            foreach (PlayerMover mover in TurnManager.Instance.players)
            {
                mover.InitializeBoard(boardManager);
            }
        }

        resetDiceImage.raycastTarget = false;
    }

    void Update()
    {
         if (TurnManager.Instance.currentTurnIndex != 0)
        {
            // Bot turn: If the dice hasn't been rolled, automatically simulate a dice roll.
            if (!diceScript.firstThrow)
            {
                StartCoroutine(BotDiceRoll());
            }
        }
        // Trigger a move only when the dice has been thrown, has landed, and the roll hasn't been processed yet.
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
        // Delay for a bot "thinking" time.
        yield return new WaitForSeconds(1f);
        // If still not rolled (to avoid double-triggering), start the dice roll.
        if (!diceScript.firstThrow)
        {
            diceScript.firstThrow = true;
            diceScript.rollCompleted = false;
            diceScript.rollConsumed = false;
            diceScript.diceFaceNum = "";

            diceScript.RollDice();
        }
    }

    IEnumerator MoveAfterLanding()
    {
        float timeout = 10f;
        float timer = 0f;
        // Wait for a valid dice face value.
        while ((string.IsNullOrEmpty(diceScript.diceFaceNum) || 
               !int.TryParse(diceScript.diceFaceNum, out _)) && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (int.TryParse(diceScript.diceFaceNum, out int number))
        {
            PlayerMover currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                currentPlayer.MoveSteps(number, () =>
                {
                    Debug.Log($"[GameController] Moved player {number} steps.");
                    // Check if the final landing tile is a chest tile.
                    // (Assumes PlayerMover has a public method GetCurrentTileIndex().)
                    if (chestTiles.Contains(currentPlayer.currentTileIndex) && !extraRollUsed)
                    {
                        Debug.Log("[GameController] Chest tile! Granting an extra roll.");
                        // Mark extra roll as used so that the player only gets two rolls per turn.
                        extraRollUsed = true;
                        // Reset the dice so the same player gets another roll.
                        diceScript.ResetRollState();
                        moveStarted = false;
                        resetDiceImage.raycastTarget = true;
                        // Do NOT call NextTurn() so that the current player gets another roll.
                    }
                    else
                    {
                        // Normal turn end.
                        TurnManager.Instance.NextTurn();
                        diceScript.ResetRollState();
                        moveStarted = false;
                        extraRollUsed = false; // Reset for the next turn.
                        resetDiceImage.raycastTarget = true;
                    }
                });
            }
            else
            {
                Debug.LogWarning("No current player found in TurnManager.");
                diceScript.ResetRollState();
                moveStarted = false;
                resetDiceImage.raycastTarget = true;
            }
        }
        else
        {
            Debug.LogWarning("[GameController] Failed to parse diceFaceNum: " + diceScript.diceFaceNum);
            diceScript.ResetRollState();
            moveStarted = false;
            resetDiceImage.raycastTarget = true;
        }
        yield return null;
    }
}


