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
            if (overlay != null && overlay.TryGetComponent(out UnityEngine.UI.Image img))
            {
                    Color tempColor = img.color;
                    tempColor.a = Mathf.Lerp(0.9f, 0f, brightness);
                    img.color = tempColor;
                
            } 
        }

         if (PlayerPrefs.HasKey("fullscreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("fullscreen") == 1;
        }     

        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            Resolution[] allResolutions = Screen.resolutions;
            List<Resolution> uniqueResolutions = new List<Resolution>();
            HashSet<string> labels = new HashSet<string>();
            foreach (var res in allResolutions)
            {
                string label = $"{res.width}x{res.height}";
                if (!labels.Contains(label))
                {
                    uniqueResolutions.Add(res);
                    labels.Add(label);
                }
            }

            int savedIndex = PlayerPrefs.GetInt("resolutionIndex");
            if (savedIndex >= 0 && savedIndex < uniqueResolutions.Count)
            {
                Resolution res = uniqueResolutions[savedIndex];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            }
        } 
    }

}