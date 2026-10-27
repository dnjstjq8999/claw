using UnityEngine;
using TMPro;

public class ReinforceManager : MonoBehaviour
{
    [Header("--- 4개의 업그레이드 패널 (순서대로 0~3) ---")]
    public GameObject[] upgradePanels;
    public GameObject reinforcePanel;
    public GameObject homePanel;

    [Header("--- 전체 재화(공) UI 텍스트 ---")]
    [SerializeField] private TextMeshProUGUI totalCapsuleText;

    [Header("--- 각 패널별 텍스트 연결 (4개 크기로 설정) ---")]
    [SerializeField] private TextMeshProUGUI[] currentLevelTextArray; // Current Level 표시용 텍스트 배열
    [SerializeField] private TextMeshProUGUI[] nowTextArray;          // Now 옆 텍스트 (현재 능력치)
    [SerializeField] private TextMeshProUGUI[] nextTextArray;         // Next 옆 텍스트 (다음 능력치)
    [SerializeField] private TextMeshProUGUI[] requiredTextArray;     // Required Capsules 옆 텍스트 (필요 공 개수)

    [Header("--- 강화 밸런스 설정 ---")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private int baseCost = 5;

    private void Start()
    {
        ShowPanel(0);
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        if (totalCapsuleText != null)
        {
            totalCapsuleText.text = GameManager.Instance.currentData.totalCapsules.ToString();
        }

        for (int i = 0; i < upgradePanels.Length; i++)
        {
            int currentLevel = GetLevel(i);
            bool isMax = (currentLevel >= maxLevel);

            // 1. Current Level 텍스트 업데이트 (만렙일 때도 10으로 제대로 뜨도록 반영)
            if (currentLevelTextArray != null && i < currentLevelTextArray.Length && currentLevelTextArray[i] != null)
            {
                currentLevelTextArray[i].text = $"Current Level : {currentLevel}";
            }

            // 2. Now 텍스트 업데이트
            if (nowTextArray != null && i < nowTextArray.Length && nowTextArray[i] != null)
            {
                nowTextArray[i].text = GetStatText(i, currentLevel);
            }

            // 3. Next 텍스트 업데이트 (만렙이면 "-", 아니면 다음 레벨 스탯)
            if (nextTextArray != null && i < nextTextArray.Length && nextTextArray[i] != null)
            {
                if (isMax)
                    nextTextArray[i].text = "-";
                else
                    nextTextArray[i].text = GetStatText(i, currentLevel + 1);
            }

            // 4. Required Capsules 텍스트 업데이트 (만렙이면 "-", 아니면 비용 표시)
            if (requiredTextArray != null && i < requiredTextArray.Length && requiredTextArray[i] != null)
            {
                if (isMax)
                {
                    requiredTextArray[i].text = "-";
                }
                else
                {
                    int currentCost = baseCost + (currentLevel - 1);
                    requiredTextArray[i].text = currentCost.ToString();
                }
            }
        }
    }

    private string GetStatText(int index, int level)
    {
        switch (index)
        {
            case 0: // 0번: 이동 속도
                {
                    float moveSpeed = 5.0f + (level - 1) * (5.0f / 9f);
                    return $"{moveSpeed:F1}"; // 예: 5.0 ~ 10.0
                }
            case 1: // 1번: 집게 속도
                {
                    float clawSpeed = 2.0f + (level - 1) * (2.0f / 9f);
                    return $"{clawSpeed:F1}";
                }
            case 2: // 2번: 뽑기 실력
                {
                    return $"Skill Lv.{level}";
                }
            case 3: // 3번: 시간 강화
                {
                    float extraTime = (level - 1) * 10f;
                    return $"+{extraTime}s";
                }
            default:
                return $"{level}";
        }
    }

    private int GetLevel(int index)
    {
        if (GameManager.Instance == null) return 1;

        switch (index)
        {
            case 0: return GameManager.Instance.currentData.moveSpeedLevel;
            case 1: return GameManager.Instance.currentData.clawSpeedLevel;
            case 2: return GameManager.Instance.currentData.clawPowerLevel;
            case 3: return GameManager.Instance.currentData.timeLimitLevel;
            default: return 1;
        }
    }

    public void OnClickUpgrade(int index)
    {
        if (GameManager.Instance == null) return;

        int currentLevel = GetLevel(index);
        if (currentLevel >= maxLevel) return;

        int cost = baseCost + (currentLevel - 1);

        if (GameManager.Instance.ConsumeCapsules(cost))
        {
            switch (index)
            {
                case 0: GameManager.Instance.currentData.moveSpeedLevel++; break;
                case 1: GameManager.Instance.currentData.clawSpeedLevel++; break;
                case 2: GameManager.Instance.currentData.clawPowerLevel++; break;
                case 3: GameManager.Instance.currentData.timeLimitLevel++; break;
            }

            SaveManager.SaveData(GameManager.Instance.currentData);
        }
    }

    public void ShowPanel(int index)
    {
        for (int i = 0; i < upgradePanels.Length; i++)
        {
            if (upgradePanels[i] != null)
            {
                upgradePanels[i].SetActive(i == index);
            }
        }
    }

    public void OnClickBackButton()
    {
        if (reinforcePanel != null) reinforcePanel.SetActive(false);
        if (homePanel != null) homePanel.SetActive(true);
    }
}