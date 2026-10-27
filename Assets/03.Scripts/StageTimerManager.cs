using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageTimerManager : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("--- 시간 설정 (1~10레벨 업그레이드 연동) ---")]
    [SerializeField] private float baseTimeLimit = 60f;
    [SerializeField] private float extraTimePerLevel = 10f;
    public int minLevel = 1;
    public int maxLevel = 10;

    private float currentTime;
    private bool isGameOver = false;

    private void Start()
    {
        // 씬 시작 시 시간과 마우스 커서 상태 정상화
        Time.timeScale = 1.0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        float extraTime = 0f;
        if (GameManager.Instance != null)
        {
            int timeLevel = GameManager.Instance.currentData.timeLimitLevel;

            timeLevel = Mathf.Clamp(timeLevel, minLevel, maxLevel);

            extraTime = (timeLevel - minLevel) * extraTimePerLevel;

            Debug.Log($"[시간 업그레이드 연동] 시간 레벨: {timeLevel}, 추가된 시간: {extraTime}초 (총 {baseTimeLimit + extraTime}초)");
        }

        currentTime = baseTimeLimit + extraTime;
        isGameOver = false;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0) currentTime = 0;

            UpdateTimerUI();

            if (currentTime <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = seconds.ToString();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("[게임오버] 시간이 종료되었습니다!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.SetControllable(false);
        }

        // 게임오버 순간 정지
        Time.timeScale = 0f;

        StartCoroutine(ReturnToMainMenuRoutine());
    }

    private IEnumerator ReturnToMainMenuRoutine()
    {
        yield return new WaitForSecondsRealtime(2.0f);

        Time.timeScale = 1.0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenuScene");
    }
}