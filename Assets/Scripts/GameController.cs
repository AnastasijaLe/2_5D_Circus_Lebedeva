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
        // Trigger a move only when the dice has been thrown, has landed, and the roll hasn't been processed yet.
        if (diceScript.firstThrow && diceScript.rollCompleted && !diceScript.rollConsumed && !moveStarted)
        {
            diceScript.inputEnabled = false;
            diceScript.rollConsumed = true;
            moveStarted = true;
            StartCoroutine(MoveAfterLanding());
        }
    }

    IEnumerator MoveAfterLanding()
    {
        float timeout = 10f;
        float timer = 0f;
        // Wait briefly for the dice face value to update.
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
                    // Advance turn once movement is complete.
                    TurnManager.Instance.NextTurn();
                    // Reset only the roll state so that a new roll is required.
                    diceScript.ResetRollState();
                    moveStarted = false;
                    resetDiceImage.raycastTarget = true;
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
    }
}


