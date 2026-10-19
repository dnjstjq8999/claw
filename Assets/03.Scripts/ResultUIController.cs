using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필수!

public class ResultUIController : MonoBehaviour
{
    [Header("이동할 씬 이름 설정")]
    [SerializeField] private string stage1SceneName = "Stage1";     // 1스테이지 씬 이름 (실제 씬 이름과 같아야 함)
    [SerializeField] private string mainMenuSceneName = "MainMenuScene"; // 메인 메뉴 씬 이름 (실제 씬 이름과 같아야 함)

    /// <summary>
    /// [다시 시도] 또는 [다음/재시작] 버튼을 눌렀을 때 실행
    /// </summary>
    public void OnClickRestart()
    {
        Time.timeScale = 1f; // 일시정지 상태 해제
        SceneManager.LoadScene(stage1SceneName);
    }

    /// <summary>
    /// [메인 메뉴로] 버튼을 눌렀을 때 실행
    /// </summary>
    public void OnClickMainMenu()
    {
        Time.timeScale = 1f; // 일시정지 상태 해제
        SceneManager.LoadScene(mainMenuSceneName);
    }
}