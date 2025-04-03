using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMessage : MonoBehaviour
{
     [SerializeField] private Text messageText;
    [SerializeField] private float clearDelay = 3f; // Time after which the message will clear

    // Sets a message and clears it after a delay.
    public void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            StopAllCoroutines();
            StartCoroutine(ClearMessageAfterDelay());
        }
    }

    IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(clearDelay);
        messageText.text = "";
    }

    // Called when it's a player's turn.
    // isUserTurn true means it's your turn; false means it's a bot's turn.
    public void SetTurnMessage(string playerName, bool isUserTurn)
    {
        if (isUserTurn)
            SetMessage("Your turn. Roll dice.");
        else
            SetMessage($"It is {playerName}'s turn. Rolling dice...");
    }

    // Called when a trap (hurt tile) is encountered.
    public void SetTrapMessage(string playerName, bool isUser)
    {
        if (isUser)
            SetMessage("You went into a trap and go back to a previous field.");
        else
            SetMessage($"{playerName} went into a trap and goes back to a previous field.");
    }

    // Called when a chest is encountered.
    public void SetChestMessage(string playerName, bool isUser)
    {
        if (isUser)
            SetMessage("You found a chest. Roll dice one more time.");
        else
            SetMessage($"{playerName} found a chest. Rolling dice one more time...");
    }

    // Called after a move is completed.
    public void SetMoveDoneMessage(string playerName, bool isUser, int level)
    {
        if (isUser)
            SetMessage($"Move is done. You are on level {level}.");
        else
            SetMessage($"Move is done. {playerName} is on level {level}.");
    }
}
