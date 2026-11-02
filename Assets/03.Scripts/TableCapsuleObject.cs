using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TableCapsuleObject : MonoBehaviour, IPointerClickHandler
{
    [Header("--- Ä¸½¶ µ¥ÀÌÅÍ ---")]
    public int capsuleNumber;
    public bool isEven;
    private bool isOpen = false;
    private bool isInitialized = false;

    [Header("--- UI / ·»´õ¸µ Âü°í ---")]
    [SerializeField] private GameObject closedVisual;
    [SerializeField] private TextMeshPro numberText;

    void Start()
    {
        if (numberText != null) numberText.gameObject.SetActive(false);
    }

    public void SetupCapsule(bool even, int num)
    {
        isEven = even;
        capsuleNumber = num;
        isInitialized = true;

        if (numberText != null) numberText.text = num.ToString();
    }

    private float lastClickTime = 0f;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInitialized || isOpen) return;

        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick < 0.3f)
        {
            OpenCapsule();
        }
        lastClickTime = Time.time;
    }

    void OpenCapsule()
    {
        isOpen = true;
        if (closedVisual != null) closedVisual.SetActive(false);
        if (numberText != null)
        {
            numberText.gameObject.SetActive(true);
            numberText.text = capsuleNumber.ToString();
        }
    }
}