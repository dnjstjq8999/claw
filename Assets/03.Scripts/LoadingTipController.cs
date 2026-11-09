using UnityEngine;
using TMPro;

public class LoadingTipController : MonoBehaviour
{
    [Header("--- 팁 텍스트 UI 연결 ---")]
    [SerializeField] private TextMeshProUGUI tipText;

    // LocalizationManager에 등록된 팁 키 목록
    private string[] tipKeys = new string[]
    {
        "TIP_1",
        "TIP_2",
        "TIP_3"
    };

    void Start()
    {
        ShowRandomTip();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += ShowRandomTip;
        }
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= ShowRandomTip;
        }
    }

    void ShowRandomTip()
    {
        if (tipText == null) return;

        if (tipKeys != null && tipKeys.Length > 0)
        {
            int randomIndex = Random.Range(0, tipKeys.Length);
            string selectedKey = tipKeys[randomIndex];

            if (LocalizationManager.Instance != null)
            {
                tipText.text = LocalizationManager.Instance.GetLocalizedValue(selectedKey);
            }
            else
            {
                tipText.text = selectedKey;
            }
        }
        else
        {
            if (LocalizationManager.Instance != null)
            {
                tipText.text = LocalizationManager.Instance.GetLocalizedValue("TIP_DEFAULT");
            }
            else
            {
                tipText.text = "Tip: Enjoy the game!";
            }
        }
    }
}