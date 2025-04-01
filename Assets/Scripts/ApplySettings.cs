using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySettings : MonoBehaviour
{
    void Awake()
    {
        ApplySavedSettings();
    }

    void ApplySavedSettings()
    {
        if(PlayerPrefs.HasKey("musicVolume"))
            AudioListener.volume = PlayerPrefs.GetFloat("musicVolume");

        if (PlayerPrefs.HasKey("brightness"))
        {
            float brightness = PlayerPrefs.GetFloat("brightness");
            GameObject overlay = GameObject.Find("Overlay");
            if (overlay != null)
            {
                var img = overlay.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    Color tempColor = img.color;
                    tempColor.a = Mathf.Lerp(0.9f, 0f, brightness);
                    img.color = tempColor;
                }
            } 
        }

         if (PlayerPrefs.HasKey("fullscreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("fullscreen") == 1;
        }     

        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            Resolution[] allResolutions = Screen.resolutions;
            int savedIndex = PlayerPrefs.GetInt("resolutionIndex");
            if (savedIndex >= 0 && savedIndex < allResolutions.Length)
            {
                Resolution res = allResolutions[savedIndex];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            }
        } 
    }

}