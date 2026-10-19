using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    public static StageUIManager Instance;

    [Header("--- UI 이미지 리소스 ---")]
    public Sprite blueCapsuleSprite;
    public Sprite yellowCapsuleSprite;

    private TextMeshProUGUI blueCountText;
    private TextMeshProUGUI yellowCountText;

    [Header("--- 획득한 캡슐 개수 ---")]
    public int blueCapsuleCount = 0;
    public int yellowCapsuleCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CreateCapsuleUI();
        UpdateCapsuleUI();
    }

    private void CreateCapsuleUI()
    {
        // ★ 수정 포인트: 겹침 방지를 위해 World Space가 아닌 화면 전용 Overlay 캔버스를 안전하게 탐색 또는 생성
        Canvas targetCanvas = null;
        foreach (Canvas c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                targetCanvas = c;
                break;
            }
        }

        // 만약 화면 전용 캔버스가 없다면 새로 생성
        if (targetCanvas == null)
        {
            GameObject canvasObj = new GameObject("StageUI_Canvas");
            targetCanvas = canvasObj.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject panelObj = new GameObject("CapsuleUI_Panel");
        panelObj.transform.SetParent(targetCanvas.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(30, -30);
        panelRect.sizeDelta = new Vector2(250, 100);

        VerticalLayoutGroup layout = panelObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        blueCountText = CreateCapsuleRow(panelObj.transform, "BlueRow", blueCapsuleSprite, Color.cyan);
        yellowCountText = CreateCapsuleRow(panelObj.transform, "YellowRow", yellowCapsuleSprite, Color.yellow);
    }

    private TextMeshProUGUI CreateCapsuleRow(Transform parent, string rowName, Sprite sprite, Color iconColor)
    {
        GameObject rowObj = new GameObject(rowName);
        rowObj.transform.SetParent(parent, false);
        RectTransform rowRect = rowObj.AddComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(150, 40);

        HorizontalLayoutGroup hLayout = rowObj.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 4;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // 아이콘 이미지 오브젝트
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(rowObj.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(35, 35);

        Image iconImage = iconObj.AddComponent<Image>();
        if (sprite != null) iconImage.sprite = sprite;
        else iconImage.color = iconColor;

        iconImage.preserveAspect = true;

        // 개수 텍스트 오브젝트
        GameObject textObj = new GameObject("CountText");
        textObj.transform.SetParent(rowObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(120, 40);
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.fontSize = 24;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.textWrappingMode = TextWrappingModes.NoWrap;

        return tmpText;
    }

    public void AddCapsule(string capsuleTag)
    {
        if (capsuleTag == "BlueCapsule") blueCapsuleCount++;
        else if (capsuleTag == "YellowCapsule") yellowCapsuleCount++;

        UpdateCapsuleUI();
    }

    private void UpdateCapsuleUI()
    {
        if (blueCountText != null) blueCountText.text = $"x {blueCapsuleCount}";
        if (yellowCountText != null) yellowCountText.text = $"x {yellowCapsuleCount}";
    }
}