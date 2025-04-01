using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
     [Header("UI")]
    public GameObject pausePanel;     
    public GameObject pauseButton;    
    public GameObject settingsPanel;  
    public GameObject ratingPanel;
    public GameObject numberBackground;
    public GameObject dice;

    private bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
        pauseButton.SetActive(!isPaused);

        if (numberBackground != null)
        numberBackground.SetActive(!isPaused);

        if (dice != null)
        dice.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

     public void ResumeGame()
    {
        isPaused = false;

        pausePanel.SetActive(false);
        pauseButton.SetActive(true);

        if (numberBackground != null)
        numberBackground.SetActive(true);

        if (dice!= null)
        dice.SetActive(true);

        Time.timeScale = 1f;

    }

      public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OpenRating()
    {
        if (ratingPanel != null)
            ratingPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenue");
    }
}
