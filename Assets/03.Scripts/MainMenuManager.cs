using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("--- 이동할 스테이지 씬 이름 ---")]
    public string stage1SceneName = "Stage1"; // Build Settings에 등록된 씬 이름

    /// <summary>
    /// 게임 시작 버튼 클릭 시 호출
    /// </summary>
    public void OnClickStartGame()
    {
        Debug.Log("Stage 1 진입 중...");
        SceneManager.LoadScene(stage1SceneName);
    }

    /// <summary>
    /// 게임 종료 버튼 클릭 시 호출 (선택 사항)
    /// </summary>
    public void OnClickQuitGame()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}