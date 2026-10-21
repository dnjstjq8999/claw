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

    // 해상도 후보군 리스트 (실행할 때 모니터 지원 여부에 따라 필터링 됨)
    private List<Res> resolutions = new List<Res>();
    private List<string> resolutionOptions = new List<string>();

    [Header("--- FPS 제한 설정 (드롭다운) ---")]
    public TMP_Dropdown fpsDropdown;

    private void Start()
    {
        // 1. 볼륨 초기화
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // 2. 언어 초기화
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
    }

    // 모니터가 지원하는 해상도만 골라 담는 함수
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
        if (languageText != null) languageText.text = languages[currentLangIndex];
        PlayerPrefs.SetInt("LanguageIndex", currentLangIndex);
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
}