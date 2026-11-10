using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    [SerializeField] private TextMeshProUGUI congratulationText;
    [SerializeField] private GameObject clickToMenuText;

    [Header("--- 텍스트 컬러 변환 설정 ---")]
    [SerializeField] private float colorChangeSpeed = 2.0f;

    [Header("--- 씬 이동 설정 ---")]
    [SerializeField] private float delaySeconds = 3.0f;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    private bool canClick = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (clickToMenuText != null)
        {
            clickToMenuText.SetActive(false);
        }

        StartCoroutine(EnableClickRoutine());
    }

    void Update()
    {
        if (congratulationText != null)
        {
            float hue = Mathf.Repeat(Time.time * colorChangeSpeed * 0.1f, 1f);
            congratulationText.color = Color.HSVToRGB(hue, 0.8f, 1.0f);
        }

        if (canClick && Input.GetMouseButtonDown(0))
        {
            GoToMainMenu();
        }
    }

    IEnumerator EnableClickRoutine()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (clickToMenuText != null)
        {
            clickToMenuText.SetActive(true);
        }

        canClick = true;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}