using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [Header("Settings menu UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    private Resolution[] resolutions;

    [Header("Scene Loading")]
    [SerializeField] private GameObject loadingScreen;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Main Menu")]
    [SerializeField] private TMP_InputField saveGameInputField;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI fpsCounter;
    private float updateInterval;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].ToString() == Screen.currentResolution.ToString())
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        if (!ES3.KeyExists("ResolutionIndex"))
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        LoadSettings();
    }

    private void Update()
    {
        int current = (int)(1f / Time.unscaledDeltaTime);
        fpsCounter.text = current + " FPS";
        updateInterval = Time.deltaTime + 60.0f;
    }

    public void StartNewGame()
    {
        if(saveGameInputField.text.Length <= 3)
        {
            feedbackText.text = "Save name needs to be longer than 3 characters";
            return;
        }

        if (ES3.FileExists(saveGameInputField.text))
        {
            feedbackText.text = "A save with this name already exists";
            return;
        }

        ES3.Save("CurrentSaveName", saveGameInputField.text);

        SaveGame newSaveGame = new SaveGame();

        newSaveGame.AllBuildings = new List<BuildingSave>();
        newSaveGame.AllCitizens = new List<CitizenSave>();
        newSaveGame.SaveGameName = saveGameInputField.text;
        newSaveGame.AmountHappiness = 100;

        ES3.Save("SaveGame", newSaveGame, saveGameInputField.text + ".es3");

        StartCoroutine(LoadSceneAsync(1));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    //--------------Settings menu--------------\\
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        ES3.Save("MusicVolume", volume);
    }

    public void SetFxVolume(float volume)
    {
        audioMixer.SetFloat("FxVolume", volume);
        ES3.Save("FxVolume", volume);
    }

    private void LoadSettings()
    {
        if (ES3.KeyExists("MusicVolume"))
        {
            float volume = ES3.Load<float>("MusicVolume");
            audioMixer.SetFloat("MusicVolume", volume);
            musicSlider.SetValueWithoutNotify(volume);
        }
        if (ES3.KeyExists("FxVolume"))
        {
            float volume = ES3.Load<float>("FxVolume");
            audioMixer.SetFloat("FxVolume", volume);
            fxSlider.SetValueWithoutNotify(volume);
        }
        if (ES3.KeyExists("GraphicsIndex"))
        {
            int index = ES3.Load<int>("GraphicsIndex");
            QualitySettings.SetQualityLevel(index);
            graphicsDropdown.SetValueWithoutNotify(index);
            graphicsDropdown.RefreshShownValue();
        }
        if (ES3.KeyExists("IsFullscreen"))
        {
            bool isFullscreen = ES3.Load<bool>("IsFullscreen");
            Screen.fullScreen = isFullscreen;
            fullscreenToggle.SetIsOnWithoutNotify(isFullscreen);
        }
        if (ES3.KeyExists("ResolutionIndex"))
        {
            int index = ES3.Load<int>("ResolutionIndex");
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            resolutionDropdown.SetValueWithoutNotify(index);
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void SetGraphicsLevel(int graphicsIndex)
    {
        QualitySettings.SetQualityLevel(graphicsIndex);
        ES3.Save("GraphicsIndex", graphicsIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        ES3.Save("IsFullscreen", isFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        ES3.Save("ResolutionIndex", resolutionIndex);
    }
}
