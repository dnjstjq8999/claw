using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("--- ¾À ÀÌ¸§ ¼³Á¤ ---")]
    public string stage1SceneName = "Stage1";

    [Header("--- UI ÆË¾÷ ÆÐ³Î ---")]
    public GameObject homePanel;
    public GameObject optionsPanel;
    public GameObject reinforcePanel;

    [Header("--- »ç¿îµå ¼³Á¤ ---")]
    public AudioSource audioSource;
    public AudioClip clickSFX;

    private void Start()
    {
        // ¾À ½ÃÀÛ ½Ã ÆË¾÷Ã¢µéÀº ´Ý¾ÆµÒ
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (reinforcePanel != null) reinforcePanel.SetActive(false);
    }

    public void OnClickStartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene(stage1SceneName);
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