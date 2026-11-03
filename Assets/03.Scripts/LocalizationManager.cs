using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public enum Language
    {
        KO,
        EN
    }

    [Header("--- 현재 언어 설정 ---")]
    public Language currentLanguage = Language.EN;

    public event Action OnLanguageChanged;

    // 텍스트 사전 (Key, [KO, EN])
    private Dictionary<string, (string ko, string en)> localizedDictionary = new Dictionary<string, (string, string)>()
    {
        // 메인 메뉴 텍스트 매핑
        { "TITLE_GAME", ("암산 뽑기", "MATH CLAW") },
        { "BTN_START", ("게임 시작", "START GAME") },
        { "BTN_SETTINGS", ("설정", "SETTINGS") },
        { "BTN_REINFORCE", ("강화", "REINFORCE") },
        { "BTN_QUIT", ("게임 종료", "QUIT GAME") },

        // 설정창 및 기타 UI
        { "LABEL_BGM", ("배경음악", "BGM") },
        { "LABEL_SFX", ("효과음", "SFX") },
        { "BTN_BACK", ("돌아가기", "BACK") },

        { "LABEL_CURRENT_LANG", ("한국어", "English") }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            string savedLang = PlayerPrefs.GetString("SelectedLanguage", "EN");
            if (Enum.TryParse(savedLang, out Language loadedLang))
            {
                currentLanguage = loadedLang;
            }
            else
            {
                currentLanguage = Language.EN;
            }

            OnLanguageChanged?.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeLanguage(Language newLanguage)
    {
        currentLanguage = newLanguage;
        PlayerPrefs.SetString("SelectedLanguage", currentLanguage.ToString());
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke();
    }

    // 언어 토글 (KO <-> EN)
    public void ToggleLanguage()
    {
        Language nextLang = (currentLanguage == Language.KO) ? Language.EN : Language.KO;
        ChangeLanguage(nextLang);
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedDictionary.TryGetValue(key, out var value))
        {
            return currentLanguage == Language.KO ? value.ko : value.en;
        }

        return key;
    }
}