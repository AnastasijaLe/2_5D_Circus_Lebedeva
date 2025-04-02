using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    int[] otherPlayers;
    int index;
    private const string txtFileName = "playerNames";

    void Start()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.players.Clear();

        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex], 
            spawnPoint.transform.position, Quaternion.identity);
        mainCharacter.GetComponent<NameScript>().SetPlayerName(PlayerPrefs.GetString("PlayerName"));

        if (TurnManager.Instance != null)
        {
            PlayerMover mover = mainCharacter.GetComponent<PlayerMover>();
            if(mover != null)
                TurnManager.Instance.players.Add(mover);
        }

        otherPlayers = new int[PlayerPrefs.GetInt("PlayerCount")];
        string[] nameArray = ReadLineFromFile(txtFileName);

        for(int i = 0; i < otherPlayers.Length-1; i++) {
            spawnPoint.transform.position += new Vector3(0.49f, 0, 0.4f);
            index = Random.Range(0, playerPrefabs.Length);
            GameObject character = 
               Instantiate(playerPrefabs[index], spawnPoint.transform.position, Quaternion.identity);
            character.GetComponent<NameScript>().SetPlayerName(nameArray[Random.Range(0, nameArray.Length)]);

             if (TurnManager.Instance != null)
            {
                PlayerMover mover = character.GetComponent<PlayerMover>();
                if(mover != null){
                    mover.offset = new Vector3(-i * 0.3f, 0, i * 0.2f);
                    TurnManager.Instance.players.Add(mover);
                }
            }
        }
    }

    string[] ReadLineFromFile(string fileName) {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset != null)
            return textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        else
            Debug.LogError("File not found: " + fileName); return new string[0];
    }
}
