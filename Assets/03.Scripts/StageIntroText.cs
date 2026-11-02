using UnityEngine;
using TMPro;
using System.Collections;

public class StageIntroUI : MonoBehaviour
{
    private TextMeshProUGUI stageText;

    [Header("--- 연출 시간 설정 ---")]
    public float displayDuration = 1.5f;
    public float fadeDuration = 1.0f;
    void Start()
    {
        stageText = GetComponent<TextMeshProUGUI>();

        if (GameManager.Instance != null)
        {
            stageText.text = $"STAGE {GameManager.Instance.currentStage}";
        }

        StartCoroutine(PlayIntroAnimation());
    }

    IEnumerator PlayIntroAnimation()
    {
        Color textColor = stageText.color;
        textColor.a = 1f;
        stageText.color = textColor;

        yield return new WaitForSeconds(displayDuration);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            stageText.color = textColor;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}