using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FactoryButtonController : MonoBehaviour
{
    [Header("버튼 부품 연결")]
    [Tooltip("누를 때 움직일 빨간색 윗부분 오브젝트 (buttonredtop-1)를 여기에 드래그 해주세요.")]
    public Transform buttonTop;

    [Header("움직임 설정")]
    public float pressDepth = 0.05f; // 들어가는 깊이
    public float moveSpeed = 15f;    // 움직이는 속도

    private Vector3 initialLocalPos;
    private bool isPressed = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // 버튼 탑이 지정되지 않았다면 자식 오브젝트 중에서 'buttonredtop-1'을 이름으로 자동 찾기
        if (buttonTop == null)
        {
            Transform found = transform.Find("339c2c49a714400887ae6fba0c7ae19f/RootNode/buttonredtop-1"); // 경로에 맞춰 찾기
            if (found == null)
            {
                // 혹은 이름으로 검색
                foreach (Transform child in GetComponentsInChildren<Transform>())
                {
                    if (child.name.Contains("buttonredtop"))
                    {
                        buttonTop = child;
                        break;
                    }
                }
            }
        }

        if (buttonTop != null)
        {
            initialLocalPos = buttonTop.localPosition;
        }

        // 전체 모델이나 루트에 클릭을 감지할 콜라이더가 없다면 BoxCollider 추가
        if (GetComponent<Collider>() == null)
        {
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(0.3f, 0.3f, 0.3f);
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (Mouse.current == null || buttonTop == null) return;

        // 마우스 클릭 시 버튼을 눌렀는지 체크
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭한 오브젝트가 이 버튼이거나, 버튼의 자식(빨간 윗부분 등)인 경우
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    StartCoroutine(PressButtonRoutine());
                }
            }
        }
    }

    private IEnumerator PressButtonRoutine()
    {
        if (isPressed) yield break;
        isPressed = true;

        // 1. 채점 기능 실행! (아까 만든 MathProblemPaper의 제출 버튼 함수 호출)
        if (MathProblemPaper.Instance != null)
        {
            MathProblemPaper.Instance.OnSubmitButtonClicked();
        }

        // 2. 버튼이 쑥 들어가는 연출 (로컬 좌표 기준 안쪽으로 이동)
        // (보통 로컬 Z축이나 Y축 방향이 누르는 방향이므로 상황에 맞게 조절 필요, 여기서는 Z축 기준 예시)
        Vector3 pressedLocalPos = initialLocalPos - new Vector3(0, 0, pressDepth);

        float elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            buttonTop.localPosition = Vector3.Lerp(buttonTop.localPosition, pressedLocalPos, elapsed * moveSpeed);
            yield return null;
        }

        // 3. 다시 원래 자리로 튀어나오는 연출
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            buttonTop.localPosition = Vector3.Lerp(buttonTop.localPosition, initialLocalPos, elapsed * moveSpeed);
            yield return null;
        }

        buttonTop.localPosition = initialLocalPos;
        isPressed = false;
    }
}