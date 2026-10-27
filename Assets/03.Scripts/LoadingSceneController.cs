using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("--- UI 연결 ---")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("--- 로딩 디테일 설정 ---")]
    [SerializeField] private float minLoadingTime = 1.5f;

    public static string nextSceneName = "Stage1";

    void Start()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            nextSceneName = "Stage1";
        }

        StartCoroutine(LoadSceneAsyncProcess());
    }

    IEnumerator LoadSceneAsyncProcess()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);

            float timeProgress = Mathf.Clamp01(timer / minLoadingTime);

            float displayProgress = Mathf.Min(loadProgress, timeProgress);

            if (progressBar != null) progressBar.value = displayProgress;
            if (progressText != null) progressText.text = (displayProgress * 100f).ToString("F0") + "%";

            if (operation.progress >= 0.9f && timer >= minLoadingTime)
            {
                if (progressBar != null) progressBar.value = 1f;
                if (progressText != null) progressText.text = "100%";

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}