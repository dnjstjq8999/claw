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
        { "BTN_HOW_TO_PLAY", ("게임 방법", "HOW TO PLAY") },
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

        // 도움말 (HOW TO PLAY) UI 매핑
        { "TITLE_HOW_TO_PLAY", ("게임 방법", "HOW TO PLAY") },

        { "HELP_CONTROLS_TITLE", ("■ 조작 방법", "■ CONTROLS") },
        { "HELP_CONTROLS_DESC", ("\n· 캐릭터 이동 : WASD\n· 시점 조작 : 마우스 이동\n· 상호작용 : E 키\n· 일시정지 / 설정 : ESC 키", "\n· Character Move : WASD\n· Camera Look : Mouse Move\n· Interaction : E Key\n· Pause / Settings : ESC Key") },

        { "HELP_MISSION_TITLE", ("■ 인형뽑기 & 미션 조작", "■ CLAW MACHINE & MISSION") },
        { "HELP_MISSION_DESC", ("\n· 크레인 이동 : 방향키 (상, 하, 좌, 우)\n· 집게 내리기 : 스페이스바 (Space)\n· 인형뽑기 흔들기(발차기) : Z 키\n· 사이드 화면 전환 : ALT 키 (누르는 동안)\n· 캡슐 오픈 : 더블 클릭 / 숫자 입력 : 마우스 드래그", "\n· Move Crane : Arrow Keys (Up, Down, Left, Right)\n· Drop Claw : Spacebar\n· Shake Claw (Kick) : Z Key\n· Switch Side View : ALT Key (Hold)\n· Open Capsule : Double Click / Input Numbers : Mouse Drag") },

        { "HELP_RULES_TITLE", ("■ 정답 제출 및 진행 규칙", "■ SUBMISSION & RULES") },
        { "HELP_RULES_DESC", ("\n· 정답 입력 후 [책상에 있는 버튼]을 눌러 제출하세요!\n· 정답 맞춤 : 다음 스테이지로 진입!\n· 오답 또는 시간 초과 : 초기화 (재도전!)", "\n· Enter the answer and press the [Desk Button] to submit!\n· Correct Answer : Proceed to the Next Stage!\n· Wrong Answer or Time Over : Reset (Retry!)") },

        { "HELP_GOAL_TITLE", ("■ 게임 목표", "■ GAME GOAL") },
        { "HELP_GOAL_DESC", ("\n· 1단계부터 10단계까지 모두 돌파하여 최종 엔딩을 맞이하세요!", "\n· Conquer all stages from Stage 1 to Stage 10 and reach the final ending!") },

        // 로딩 팁 텍스트 매핑
        { "TIP_1", ("팁: 스테이지를 클리어하여 더 많은 캡슐을 모아보세요! 뽑은 캡슐들로 업그레이드를 할 수 있습니다.", "Tip: Clear stages to collect more capsules! You can use them to upgrade your gear.") },
        { "TIP_2", ("팁: 공이 정확히 어디있는지 모르겠을땐 알트키(화면전환)을 활용해 보세요!", "Tip: Having trouble finding where the ball is? Press the Alt key to switch camera views!") },
        { "TIP_3", ("팁: 구석이나 끝에 있어서 안 뽑힌다고요? Z키(발차기)를 활용해 보세요!", "Tip: Struggling to reach items stuck in the corners? Try using the Z key (kick)!") },
        { "TIP_DEFAULT", ("팁: 즐거운 게임 되세요!", "Tip: Enjoy the game!") },

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