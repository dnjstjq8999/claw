using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("--- 씬 이름 설정 ---")]
    public string stage1SceneName = "Stage1";

    [Header("--- UI 팝업 패널 ---")]
    public GameObject homePanel;
    public GameObject optionsPanel;
    public GameObject reinforcePanel;
    public GameObject helpPanel;

    [Header("--- 사운드 설정 ---")]
    public AudioSource audioSource;
    public AudioClip clickSFX;

    private void Start()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (reinforcePanel != null) reinforcePanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    public void OnClickStartGame()
    {
        PlayClickSound();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentStage = 1;
        }

        LoadingSceneController.nextSceneName = stage1SceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    public void OnClickOptions()
    {
        PlayClickSound();
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void OnClickReinforce()
    {
        PlayClickSound();
        if (homePanel != null) homePanel.SetActive(false);
        if (reinforcePanel != null)
        {
            reinforcePanel.SetActive(true);
        }
    }

    public void OnClickHelp()
    {
        PlayClickSound();
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
        }
    }

    public void OnClickClosePanel(GameObject panel)
    {
        PlayClickSound();
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void OnClickQuitGame()
    {
        PlayClickSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSFX != null)
        {
            audioSource.PlayOneShot(clickSFX);
        }
    }
}