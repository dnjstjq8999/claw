using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("--- 텍스트 키값 ---")]
    [SerializeField] private string localizationKey;

    private TextMeshProUGUI textComponent;
    private bool isSubscribed = false;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        TrySubscribeAndInitialize();
    }

    private void OnEnable()
    {
        UpdateText();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void TrySubscribeAndInitialize()
    {
        if (isSubscribed) return;

        LocalizationManager manager = LocalizationManager.Instance;
        if (manager == null)
        {
            manager = FindAnyObjectByType<LocalizationManager>();
        }

        if (manager != null)
        {
            manager.OnLanguageChanged -= UpdateText;
            manager.OnLanguageChanged += UpdateText;
            isSubscribed = true;
        }

        UpdateText();
    }

    private void Unsubscribe()
    {
        if (!isSubscribed) return;

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
        isSubscribed = false;
    }

    public void UpdateText()
    {
        if (textComponent == null || string.IsNullOrEmpty(localizationKey)) return;

        LocalizationManager manager = LocalizationManager.Instance;
        if (manager == null)
        {
            manager = FindAnyObjectByType<LocalizationManager>();
        }

        if (manager != null)
        {
            textComponent.text = manager.GetLocalizedValue(localizationKey);
        }
        else
        {
            textComponent.text = localizationKey;
        }
    }

    public void SetKey(string newKey)
    {
        localizationKey = newKey;
        UpdateText();
    }
}