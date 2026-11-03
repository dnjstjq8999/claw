using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [Header("--- 볼륨 슬라이더 ---")]
    public Slider musicSlider;
    public Slider volumeSlider;

    [Header("--- 언어 설정 ---")]
    public TMP_Text languageText;
    private string[] languages = { "English", "한국어" };
    private int currentLangIndex = 0;

    [Header("--- 해상도 설정 (드롭다운) ---")]
    public TMP_Dropdown resolutionDropdown;
    private struct Res { public int w; public int h; }

    // 해상도 리스트 (실행할 때 모니터 지원 여부에 따라 필터링)
    private List<Res> resolutions = new List<Res>();
    private List<string> resolutionOptions = new List<string>();

    [Header("--- FPS 제한 설정 (드롭다운) ---")]
    public TMP_Dropdown fpsDropdown;

    [Header("--- 마우스 감도 설정 ---")]
    public Slider sensitivitySlider;

    [Header("--- 창모드 설정 (토글) ---")]
    public Toggle screenModeToggle;

    private void Start()
    {
        // 1. 볼륨 초기화
        if (musicSlider != null)
        {
            float savedBgmVol = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
            musicSlider.value = savedBgmVol;
            if (SoundManager.Instance != null) SoundManager.Instance.SetBGMVolume(savedBgmVol);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (volumeSlider != null)
        {
            float savedSfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            volumeSlider.value = savedSfxVol;
            if (SoundManager.Instance != null) SoundManager.Instance.SetSFXVolume(savedSfxVol);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // 2. 언어 초기화 (저장된 값 불러오기)
        string savedLang = PlayerPrefs.GetString("SelectedLanguage", "EN");
        currentLangIndex = (savedLang == "KO") ? 1 : 0;
        UpdateLanguageDisplay();

        // 3. 해상도 드롭다운 초기화 (모니터 지원 여부 체크 포함)
        InitResolutions();

        // 4. FPS 제한 드롭다운 초기화
        if (fpsDropdown != null)
        {
            fpsDropdown.ClearOptions();
            List<string> fpsOptions = new List<string> { "30 FPS", "60 FPS", "120 FPS" };
            fpsDropdown.AddOptions(fpsOptions);

            int savedFpsIndex = PlayerPrefs.GetInt("FPSIndex", 1); // 기본값 60 FPS
            fpsDropdown.value = savedFpsIndex;
            OnFpsChanged(savedFpsIndex);

            fpsDropdown.onValueChanged.AddListener(OnFpsChanged);
        }

        // 5. 마우스 감도 초기화 (GameManager 연동)
        if (sensitivitySlider != null && GameManager.Instance != null)
        {
            sensitivitySlider.value = GameManager.Instance.mouseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        // 6. 창모드 토글 초기화
        if (screenModeToggle != null)
        {
            bool isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 1) == 1;
            screenModeToggle.isOn = isFullScreen;
            ApplyScreenMode(isFullScreen);
            screenModeToggle.onValueChanged.AddListener(OnScreenModeChanged);
        }
    }

    private void InitResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions.Clear();
        resolutionOptions.Clear();

        if (Screen.currentResolution.width >= 2560 && Screen.currentResolution.height >= 1440)
        {
            resolutions.Add(new Res { w = 2560, h = 1440 });
            resolutionOptions.Add("2560 x 1440 (2K)");
        }

        resolutions.Add(new Res { w = 1920, h = 1080 });
        resolutionOptions.Add("1920 x 1080 (FHD)");

        resolutions.Add(new Res { w = 1280, h = 720 });
        resolutionOptions.Add("1280 x 720 (HD)");

        resolutions.Add(new Res { w = 960, h = 540 });
        resolutionOptions.Add("960 x 540");

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);

        int defaultIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].w == 1920) { defaultIndex = i; break; }
        }

        int savedResIndex = PlayerPrefs.GetInt("ResIndex", defaultIndex);
        if (savedResIndex >= resolutions.Count) savedResIndex = defaultIndex;

        resolutionDropdown.value = savedResIndex;
        OnResolutionChanged(savedResIndex);

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void OnVolumeChanged(float value)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void OnClickNextLanguage()
    {
        currentLangIndex = (currentLangIndex + 1) % languages.Length;
        UpdateLanguageDisplay();
    }

    public void OnClickPrevLanguage()
    {
        currentLangIndex--;
        if (currentLangIndex < 0) currentLangIndex = languages.Length - 1;
        UpdateLanguageDisplay();
    }

    private void UpdateLanguageDisplay()
    {
        if (languageText != null)
        {
            languageText.text = languages[currentLangIndex];
        }

        // 선택된 언어 저장 및 LocalizationManager 동기화
        string langCode = (currentLangIndex == 1) ? "KO" : "EN";
        PlayerPrefs.SetString("SelectedLanguage", langCode);
        PlayerPrefs.Save();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.currentLanguage = (currentLangIndex == 1) ? LocalizationManager.Language.KO : LocalizationManager.Language.EN;
        }
    }

    public void OnResolutionChanged(int index)
    {
        if (index >= 0 && index < resolutions.Count)
        {
            Screen.SetResolution(resolutions[index].w, resolutions[index].h, Screen.fullScreen);
            PlayerPrefs.SetInt("ResIndex", index);
        }
    }

    public void OnFpsChanged(int index)
    {
        QualitySettings.vSyncCount = 0;

        switch (index)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 120;
                break;
        }

        PlayerPrefs.SetInt("FPSIndex", index);
    }

    public void OnSensitivityChanged(float value)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.mouseSensitivity = value;
            PlayerPrefs.SetFloat("MouseSensitivity", value);
            PlayerPrefs.Save();
        }
    }

    public void OnScreenModeChanged(bool isFullScreen)
    {
        ApplyScreenMode(isFullScreen);
        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyScreenMode(bool isFullScreen)
    {
        if (isFullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.fullScreen = false;
        }
    }
}