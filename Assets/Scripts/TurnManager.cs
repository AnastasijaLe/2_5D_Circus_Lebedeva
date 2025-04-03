using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
   public static TurnManager Instance { get; private set; }

    public List<PlayerMover> players = new List<PlayerMover>();
    public int currentTurnIndex = 0;

    void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }

    public PlayerMover GetCurrentPlayer()
    {
        if (players.Count == 0)
            return null;
        return players[currentTurnIndex];
    }

    public void NextTurn()
    {
        if (players.Count == 0) return;

        int startingIndex = currentTurnIndex;
        do
        {
            currentTurnIndex = (currentTurnIndex + 1) % players.Count;
            // If we've found a player who hasn't finished, break.
            if (!players[currentTurnIndex].isFinished)
                return;
        }
        // If we looped back to the start, all players are finished.
        while (currentTurnIndex != startingIndex);
        
        // Optionally, you could signal that the game is over here.
    }

}
