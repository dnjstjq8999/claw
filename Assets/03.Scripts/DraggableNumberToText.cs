using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DraggableNumberToText : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private Plane targetPlane;
    public Vector3 originalPosition; // 원래 생성된 자리
    public int myNumber;

    [Header("드롭 감지 설정")]
    public float dropThreshold = 0.8f;

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position; // 처음 생성된 위치 저장

        TextMeshPro tmp = GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            int.TryParse(tmp.text, out myNumber);
        }

        targetPlane = new Plane(Vector3.up, transform.position);

        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            boxCol = gameObject.AddComponent<BoxCollider>();
        }

        // 클릭하기 편하게 박스 크기 확장
        boxCol.size = new Vector3(1.5f, 1.5f, 0.2f);

        // ★ [핵심 해결 방법] 콜라이더를 트리거로 설정! 
        // 물리적 충돌은 무시하고, 마우스 클릭만 감지하게 만듭니다.
        boxCol.isTrigger = true;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                DraggableNumberToText clickedNumber = hit.collider.GetComponent<DraggableNumberToText>();
                if (clickedNumber == this)
                {
                    isDragging = true;
                }
            }
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            float enter;

            if (targetPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint + new Vector3(0, 0.15f, 0);
            }
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            CheckDropZone();
        }
    }

    private void CheckDropZone()
    {
        if (MathProblemPaper.Instance != null)
        {
            Vector3 paperPos = MathProblemPaper.Instance.transform.position;
            float distance = Vector3.Distance(new Vector3(transform.position.x, paperPos.y, transform.position.z), paperPos);

            if (distance < dropThreshold || transform.position.x < paperPos.x + 0.5f)
            {
                Debug.Log($"★ 종이에 숫자 {myNumber}를 장착했습니다.");

                // 종이에 넣으면서 자신의 원래 위치(originalPosition)를 MathProblemPaper에 함께 전달
                MathProblemPaper.Instance.SetPlayerAnswer(myNumber, originalPosition);

                // 드래그하던 빨간 숫자 오브젝트 파괴
                Destroy(gameObject);
                return;
            }
        }

        transform.position = originalPosition;
    }
}