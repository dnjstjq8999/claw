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

        // 설정창 라벨 매핑
        { "LABEL_MUSIC", ("배경음", "BGM") },
        { "LABEL_VOLUME", ("효과음", "SFX") },
        { "LABEL_FPS", ("프레임", "FPS") },
        { "LABEL_RESOLUTION", ("해상도", "RES") },
        { "LABEL_FULLSCREEN", ("전체 화면", "FULL SCREEN") },
        { "LABEL_MOUSE", ("감도", "MOUSE") },
        { "LABEL_LANGUAGE", ("언어", "LANGUAGE") },
        { "BTN_BACK", ("돌아가기", "BACK") },

        // 업그레이드 패널 1: Move Speed
        { "UPG_MOVE_SPEED_TITLE", ("이동 속도", "MOVE SPEED") },
        { "UPG_MOVE_SPEED_DESC", ("맵 상에서 플레이어의 이동 속도를 증가시킵니다.", "Increases the player's movement speed across the map.") },

        // 업그레이드 패널 2: Claw Speed
        { "UPG_CLAW_SPEED_TITLE", ("집게 속도", "CLAW SPEED") },
        { "UPG_CLAW_SPEED_DESC", ("크레인 집게의 이동 및 복귀 속도를 증가시킵니다.", "Increases the moving and retracting speed of the crane claw.") },

        // 업그레이드 패널 3: Catch Skill
        { "UPG_CATCH_SKILL_TITLE", ("뽑기 기술", "CATCH SKILL") },
        { "UPG_CATCH_SKILL_DESC", ("중앙 포획 영역을 확장하고 잡기 정확도를 향상시킵니다.", "Expands the center catching area and improves grab accuracy.") },

        // 업그레이드 패널 4: Time Extension
        { "UPG_TIME_EXT_TITLE", ("시간 연장", "TIME EXTENSION") },
        { "UPG_TIME_EXT_DESC", ("더 많은 행동을 할 수 있도록 전체 스테이지 제한 시간을 연장합니다.", "Extends the overall stage time limit for more actions.") },

        // 공통 업그레이드 UI 레이블
        { "UPG_CURRENT_LEVEL", ("현재 레벨", "Current Level") },
        { "UPG_NOW", ("현재 :", "Now :") },
        { "UPG_NEXT", ("다음 :", "Next :") },
        { "UPG_REQUIRED_CAPSULES", ("필요 캡슐 :", "Required Capsules :") },
        { "BTN_UPG_UPGRADE", ("업그레이드", "UPGRADE") },

        // 일시정지 (PAUSE) UI 매핑
        { "TITLE_PAUSE", ("일시정지", "PAUSE") },
        { "BTN_RESUME", ("계속하기", "RESUME") },
        { "BTN_MAIN_MENU", ("메인 메뉴", "MAIN MENU") },

        // 상호작용 UI 매핑
        { "INTERACT_PLAY", ("[E] 뽑기", "[E] PLAY") },
        { "INTERACT_SUBMIT", ("[E] 채점", "[E] SUBMIT") },

        { "LABEL_CURRENT_LANG", ("한국어", "ENGLISH") }
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