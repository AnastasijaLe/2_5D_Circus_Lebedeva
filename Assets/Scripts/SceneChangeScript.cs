using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeScript : MonoBehaviour
{
    public FadeScript fadeScript;
    public SaveLoadScript saveLoadScript;

   public void CloseGame() {
        StartCoroutine(Delay("quit", -1, ""));
    }

   public IEnumerator Delay(string command, int character, string name) {
        if (string.Equals(command, "quit", System.StringComparison.OrdinalIgnoreCase)) {
            yield return fadeScript.FadeOut(0.1f);
            PlayerPrefs.DeleteAll();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

        } else if (string.Equals(command, "play", System.StringComparison.OrdinalIgnoreCase)) {
            yield return fadeScript.FadeOut(0.1f);
            saveLoadScript.SaveGame(character, name);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
