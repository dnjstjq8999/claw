using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUIController : MonoBehaviour
{
    [Header("이동할 씬 이름 설정")]
    [SerializeField] private string stage1SceneName = "Stage1";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";


    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(stage1SceneName);
    }


    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}