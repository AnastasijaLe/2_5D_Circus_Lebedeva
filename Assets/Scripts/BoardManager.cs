using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
   public List<Transform> boardTiles = new List<Transform>();

    void Awake()
    {
        LoadBoard();
    }

    void LoadBoard()
    {
        boardTiles.Clear();

        for (int i = 1; i <= 48; i++)
        {
            string tileName = "Level" + i;
            GameObject tile = GameObject.Find(tileName);
            if (tile != null)
            {
                boardTiles.Add(tile.transform);
            }
            else
            {
                Debug.LogWarning("Tile not found: " + tileName);
            }
        }
    }

    public Transform GetTileAt(int index)
    {
        if (index >= 0 && index < boardTiles.Count)
            return boardTiles[index];

        return null;
    }
}
