using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("--- 플레이어 세이브 데이터 (JSON 연동) ---")]
    public GameData currentData = new GameData();

    [Header("--- 게임 설정 (세팅창용 - PlayerPrefs) ---")]
    public float mouseSensitivity = 2.0f;

    [Header("--- 스테이지 진행 상태 ---")]
    public int currentStage = 1; // 기본 1스테이지 시작

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 게임 시작 시 JSON 파일에서 데이터 불러오기
            currentData = SaveManager.LoadData();

            // 플레이어프랩스에서 마우스 감도 불러오기
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);

            // 저장된 FPS 불러와서 적용하기 (기본값 60)
            int savedFPS = PlayerPrefs.GetInt("UserSelectedFPS", 60);
            Application.targetFrameRate = savedFPS;
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 재화 추가 및 자동 저장
    public void AddCapsules(int amount)
    {
        currentData.totalCapsules += amount;
        SaveManager.SaveData(currentData);
    }

    // 재화 소모 및 자동 저장
    public bool ConsumeCapsules(int amount)
    {
        if (currentData.totalCapsules >= amount)
        {
            currentData.totalCapsules -= amount;
            SaveManager.SaveData(currentData);
            return true;
        }
        return false;
    }

    // 1. 플레이어 이동 속도 (기본 5.0f ~ 10레벨 시 10.0f)
    public float GetCurrentMoveSpeed()
    {
        float baseSpeed = 5.0f;
        float ratio = (float)(currentData.moveSpeedLevel - 1) / 9f;
        return Mathf.Lerp(baseSpeed, baseSpeed * 2.0f, ratio);
    }

    // 2. 집게 이동 속도 (기본 3.0f ~ 10레벨 시 6.0f)
    public float GetCurrentClawSpeed()
    {
        float baseSpeed = 3.0f;
        float ratio = (float)(currentData.clawSpeedLevel - 1) / 9f;
        return Mathf.Lerp(baseSpeed, baseSpeed * 2.0f, ratio);
    }

    // 3. 집게 파워 - 판정 구 Y 좌표 계산 (-0.05 ~ -0.68)
    public float GetCurrentClawCenterY()
    {
        float minY = -0.05f;
        float maxY = -0.68f;
        float ratio = (float)(currentData.clawPowerLevel - 1) / 9f;
        return Mathf.Lerp(minY, maxY, ratio);
    }

    // 4. 총 제한 시간 (기본 60초 + 1레벨당 10초씩 증가, 15레벨 시 200초)
    public float GetExtraTime()
    {
        float baseTime = 60f; // 기본 스테이지 시간
        return baseTime + (currentData.timeLimitLevel - 1) * 10f;
    }
}