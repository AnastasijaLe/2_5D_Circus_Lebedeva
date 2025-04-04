using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private DiceRollScript diceScript;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private FadeScript fadeScript;
    [SerializeField] private Image resetDiceImage;
    [SerializeField] private GameObject endGameObject; // End–game UI object

    private bool moveStarted = false;
    private bool extraRollUsed = false;
    private HashSet<int> chestTiles = new HashSet<int> { 3, 15, 27 }; 
    private bool gameEnded = false;

    void Start()
    {
        diceScript.ResetRollState();
        resetDiceImage.raycastTarget = true;
        if (TurnManager.Instance != null)
        {
            foreach (PlayerMover mover in TurnManager.Instance.players)
            {
                mover.InitializeBoard(boardManager);
            }
        }
        endGameObject.SetActive(false);
    }

    void Update()
    {
        // If the game has ended, do nothing.
        if (gameEnded)
            return;

        // Get the current player.
        PlayerMover currentPlayer = TurnManager.Instance.GetCurrentPlayer();
        resetDiceImage.raycastTarget = !moveStarted && !diceScript.firstThrow;

        // If the current player is finished, force a turn advance.
        if (currentPlayer != null && currentPlayer.isFinished)
        {
            TurnManager.Instance.NextTurn();
            CheckForGameEnd();
            return;
        }

        // When the dice has been thrown, landed, and not yet processed, trigger movement.
        if (diceScript.firstThrow && diceScript.rollCompleted && !moveStarted)
        {
            resetDiceImage.raycastTarget = false;
            diceScript.inputEnabled = false;
            moveStarted = true;
            StartCoroutine(MoveAfterLanding());
        }
    }


    IEnumerator MoveAfterLanding()
    {    
        if (int.TryParse(diceScript.diceFaceNum, out int number))
        {
            PlayerMover currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                currentPlayer.AddDiceRoll();
                currentPlayer.AddDiceValue(number);
                if (chestTiles.Contains(currentPlayer.GetCurrentTileIndex()))
                    currentPlayer.AddChest();
                currentPlayer.MoveSteps(number, () =>
                {
                    Debug.Log($"[GameController] Moved player {number} steps.");
                    if (chestTiles.Contains(currentPlayer.GetCurrentTileIndex())  &&
                        currentPlayer.lastTileType != PlayerMover.TileType.Hurt && !extraRollUsed)
                    {
                        Debug.Log("[GameController] Chest tile! Granting an extra roll.");
                        extraRollUsed = true;
                        diceScript.ResetRollState();
                        moveStarted = false;
                    }
                    else
                    {
                        TurnManager.Instance.NextTurn();
                        diceScript.ResetRollState();
                        moveStarted = false;
                        extraRollUsed = false;
                        CheckForGameEnd();
                    }
                });
            }
            else
            {
                Debug.LogWarning("No current player found in TurnManager.");
                moveStarted = false;
            }
        }
        else
        {
            moveStarted = false;
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
            foreach (PlayerMover p in TurnManager.Instance.players)
            {
                if (!p.isRecorded)
                {
                    p.isRecorded = true;

                    string name = p.GetComponent<NameScript>().GetPlayerName();
                    int points = CalculateScore(p);
                    string avatarName = "avatar_" + PlayerPrefs.GetInt("SelectedCharacter");

                    PlayerResult result = new PlayerResult(name, points, p.finishTime, avatarName);
                    FindObjectOfType<SaveLoadScript>().AddPlayerResult(result);
                }
            }

            Debug.Log("[GameController] All players finished! Ending game.");
            gameEnded = true;
            StartCoroutine(EndGameRoutine());
        }
    }

    private int CalculateScore(PlayerMover p)
    {
        float minutes = 0f;

        try
        {
            string[] parts = p.finishTime.Split(':');
            if (parts.Length == 2)
            {
                int min = int.Parse(parts[0]);
                int sec = int.Parse(parts[1]);
                minutes = min + sec / 60f;
            }
        }
        catch
        {
            minutes = 0f; // fallback
        }

        int score = Mathf.RoundToInt(
            100f - minutes - p.diceRollCount - p.hurtCount + p.chestCount * 10
        );

        return score;
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
