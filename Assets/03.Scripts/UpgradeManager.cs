using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public enum UpgradeType { MoveSpeed, ClawSpeed, ClawPower, TimeLimit }

    [Header("--- 어떤 업그레이드 패널인지 선택 ---")]
    public UpgradeType upgradeType;

    [Header("--- UI 연결 (선택사항) ---")]
    public TextMeshProUGUI levelText;      // 현재 레벨 표시 텍스트
    public TextMeshProUGUI costText;       // 업그레이드 비용 표시 텍스트
    public Button upgradeButton;           // 업그레이드 버튼

    [Header("--- 비용 설정 ---")]
    public int baseCost = 10;              // 1레벨 업그레이드 비용
    public int costIncreasePerLevel = 5;   // 레벨당 증가하는 비용

    void Start()
    {
        UpdateUI();

        // 버튼 클릭 이벤트 자동 연결
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
    }

    void Update()
    {
        // 실시간으로 레벨이나 재화 변동 사항 반영
        UpdateUI();
    }

    public void OnUpgradeButtonClicked()
    {
        if (GameManager.Instance == null) return;

        int currentLevel = GetCurrentLevel();
        int cost = GetCurrentCost(currentLevel);

        // 최대 레벨(10레벨) 제한 확인
        if (currentLevel >= 10)
        {
            Debug.Log("[상점] 이미 최고 레벨(10레벨)입니다!");
            return;
        }

        // 재화(캡슐)가 충분한지 확인 후 소모
        bool success = GameManager.Instance.ConsumeCapsules(cost);
        if (success)
        {
            // 레벨 상승 적용
            SetNextLevel(currentLevel + 1);

            // 데이터 저장
            SaveManager.SaveData(GameManager.Instance.currentData);

            Debug.Log($"[상점] 업그레이드 성공! 타입: {upgradeType}, 새 레벨: {currentLevel + 1}");
            UpdateUI();
        }
        else
        {
            Debug.Log("[상점] 캡슐(재화)이 부족합니다!");
        }
    }

    int GetCurrentLevel()
    {
        if (GameManager.Instance == null) return 1;

        switch (upgradeType)
        {
            case UpgradeType.MoveSpeed: return GameManager.Instance.currentData.moveSpeedLevel;
            case UpgradeType.ClawSpeed: return GameManager.Instance.currentData.clawSpeedLevel;
            case UpgradeType.ClawPower: return GameManager.Instance.currentData.clawPowerLevel;
            case UpgradeType.TimeLimit: return GameManager.Instance.currentData.timeLimitLevel;
        }
        return 1;
    }

    void SetNextLevel(int nextLevel)
    {
        if (GameManager.Instance == null) return;

        switch (upgradeType)
        {
            case UpgradeType.MoveSpeed: GameManager.Instance.currentData.moveSpeedLevel = nextLevel; break;
            case UpgradeType.ClawSpeed: GameManager.Instance.currentData.clawSpeedLevel = nextLevel; break;
            case UpgradeType.ClawPower: GameManager.Instance.currentData.clawPowerLevel = nextLevel; break;
            case UpgradeType.TimeLimit: GameManager.Instance.currentData.timeLimitLevel = nextLevel; break;
        }
    }

    int GetCurrentCost(int level)
    {
        // 레벨이 올라갈수록 비용이 점진적으로 증가
        return baseCost + (level - 1) * costIncreasePerLevel;
    }

    void UpdateUI()
    {
        int level = GetCurrentLevel();
        int cost = GetCurrentCost(level);

        if (levelText != null)
        {
            if (level >= 10) levelText.text = "LEVEL MAX (10)";
            else levelText.text = $"LEVEL {level}";
        }

        if (costText != null)
        {
            if (level >= 10) costText.text = "-";
            else costText.text = $"{cost} 캡슐";
        }

        if (upgradeButton != null)
        {
            upgradeButton.interactable = (level < 10);
        }
    }
}