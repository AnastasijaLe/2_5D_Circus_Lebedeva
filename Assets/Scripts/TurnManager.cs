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
        currentTurnIndex = (currentTurnIndex + 1) % players.Count;
    }

}
