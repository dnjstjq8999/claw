using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TableCapsuleObject : MonoBehaviour, IPointerClickHandler
{
    [Header("--- 캡슐 데이터 ---")]
    public int capsuleNumber;
    public bool isEven;
    private bool isOpen = false; // 열렸는지 여부

    [Header("--- UI / 렌더링 참고 ---")]
    [SerializeField] private GameObject closedVisual; // 닫힌 캡슐 모양 오브젝트
    [SerializeField] private TextMeshPro numberText;  // 열렸을 때 숫자를 보여줄 3D 텍스트

    void Start()
    {
        if (numberText != null) numberText.gameObject.SetActive(false); // 처음엔 숨김
    }

    /// <summary>
    /// 스탠드 매니저에서 캡슐을 생성할 때 데이터를 전달해주는 함수
    /// </summary>
    public void SetupCapsule(bool even, int num)
    {
        isEven = even;
        capsuleNumber = num;
        if (numberText != null) numberText.text = num.ToString();
    }

    // 마우스 클릭 (더블클릭 감지)
    private float lastClickTime = 0f;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOpen) return; // 이미 열렸으면 패스

        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick < 0.3f) // 0.3초 이내에 두 번 클릭하면 더블클릭
        {
            OpenCapsule();
        }
        lastClickTime = Time.time;
    }

    /// <summary>
    /// 캡슐을 열어 내부 숫자를 공개합니다.
    /// </summary>
    void OpenCapsule()
    {
        isOpen = true;
        if (closedVisual != null) closedVisual.SetActive(false); // 껍질 숨기기
        if (numberText != null)
        {
            numberText.gameObject.SetActive(true);
            numberText.text = capsuleNumber.ToString();
        }
        Debug.Log($"[캡슐 오픈 완료] 숨겨진 숫자: {capsuleNumber}");
    }
}