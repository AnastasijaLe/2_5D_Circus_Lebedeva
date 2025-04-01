using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsScript : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] Image Overlay;

    private List<Resolution> validResolutions = new List<Resolution>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Load();
    }

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

         int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", -1);
        if (savedResolutionIndex >= 0 && savedResolutionIndex < validResolutions.Count)
        {
            resolutionDropdown.value = savedResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            OnResolutionChanged(savedResolutionIndex); // Apply it
        }
        else
        {
            // Match current screen resolution
            for (int i = 0; i < validResolutions.Count; i++)
            {
                if (validResolutions[i].width == Screen.currentResolution.width &&
                    validResolutions[i].height == Screen.currentResolution.height)
                {
                    resolutionDropdown.value = i;
                    resolutionDropdown.RefreshShownValue();
                    break;
                }
            }
        }
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
    }

    private void OnResolutionChanged(int index)
    {
    if (index >= 0 && index < validResolutions.Count)
        {
            Resolution chosen = validResolutions[index];
            bool isFullscreen = fullscreenToggle.isOn;
            Screen.SetResolution(chosen.width, chosen.height, isFullscreen);
            PlayerPrefs.SetInt("resolutionIndex", index); // Save resolution
            Debug.Log($"Changed resolution to: {chosen.width} x {chosen.height}");
        }
    }

  public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);

        Debug.Log("Fullscreen toggled: " + isFullscreen);
    }

    public void ChangeBrightness()
    {
        float minAlpha = 0.9f;
        float maxAlpha = 0f;

        var tempColor = Overlay.color;
        tempColor.a = Mathf.Lerp(minAlpha, maxAlpha, brightnessSlider.value);
        Overlay.color = tempColor;

        PlayerPrefs.SetFloat("brightness", brightnessSlider.value);
    }

    public void changeVolume()
    {
        AudioListener.volume=volumeSlider.value;
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
            AudioListener.volume = volumeSlider.value;
        }

        if (PlayerPrefs.HasKey("brightness"))
        {
            brightnessSlider.value = PlayerPrefs.GetFloat("brightness");
            ChangeBrightness(); // Apply it
        }

         if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
            Screen.fullScreen = isFullscreen;
            fullscreenToggle.isOn = isFullscreen;
        }
    }

   
    public void ResetToDefaults()
    {
        // Reset values
        volumeSlider.value = 1f;
        brightnessSlider.value = 1f;
        changeVolume();
        ChangeBrightness();
        fullscreenToggle.isOn = false; // force a refresh
        fullscreenToggle.isOn = true;
        ToggleFullscreen(true);

        // Reset resolution to current screen resolution
        for (int i = 0; i < validResolutions.Count; i++)
        {
            if (validResolutions[i].width == Screen.currentResolution.width &&
                validResolutions[i].height == Screen.currentResolution.height)
            {
                resolutionDropdown.value = i;
                resolutionDropdown.RefreshShownValue();
                OnResolutionChanged(i);
                break;
            }
        }

        Debug.Log("Settings reset to defaults.");
    }
}
