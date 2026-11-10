using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }
    public int cheatCoinAmount = 10;

    [Header("--- 씬 이름 설정 ---")]
    [SerializeField] private string stageSceneName = "StageScene";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // I 키: 치트 코인(캡슐) 지급
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            GiveCheatCoins();
        }

        // O 키: 즉시 다음 스테이지로 이동 (스테이지 테스트용)
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            SkipToNextStage();
        }
    }

    void GiveCheatCoins()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCapsules(cheatCoinAmount);
        }
    }

    void SkipToNextStage()
    {
        if (GameManager.Instance != null)
        {
            Time.timeScale = 1f;

            if (GameManager.Instance.currentStage >= 10)
            {
                SceneManager.LoadScene("EndingScene");
            }
            else
            {
                GameManager.Instance.currentStage++;
                SceneManager.LoadScene(stageSceneName);
            }
        }
    }
}