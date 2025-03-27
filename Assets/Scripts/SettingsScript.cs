using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsScript : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private List<Resolution> validResolutions = new List<Resolution>();

    private void Start()
    {
        // Gather all system resolutions
        Resolution[] allResolutions = Screen.resolutions;

        // We'll store just "width x height" labels here
        List<string> resolutionLabels = new List<string>();

        // Filter out duplicates (same width/height, different refresh rates)
        foreach (Resolution res in allResolutions)
        {
            string label = $"{res.width} x {res.height}";
            if (!resolutionLabels.Contains(label))
            {
                resolutionLabels.Add(label);
                validResolutions.Add(res);
            }
        }

        // Populate the dropdown with these filtered resolutions
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionLabels);

        // Optionally set the dropdown's default value to the current screen resolution
        for (int i = 0; i < validResolutions.Count; i++)
        {
            if (validResolutions[i].width == Screen.currentResolution.width &&
                validResolutions[i].height == Screen.currentResolution.height)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        // Listen for changes on the dropdown
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void OnResolutionChanged(int index)
    {
        // Make sure the index is valid
        if (index >= 0 && index < validResolutions.Count)
        {
            Resolution chosen = validResolutions[index];
            // If you want to keep whatever fullscreen mode the user has:
            bool isFullscreen = Screen.fullScreen;
            Screen.SetResolution(chosen.width, chosen.height, isFullscreen);
            Debug.Log($"Changed resolution to: {chosen.width} x {chosen.height}, Fullscreen: {isFullscreen}");
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
