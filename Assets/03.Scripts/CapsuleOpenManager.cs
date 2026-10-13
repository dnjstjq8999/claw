using UnityEngine;
using UnityEngine.InputSystem;

public class CapsuleOpenManager : MonoBehaviour
{
    [Header("클릭 감지 설정")]
    public float clickDistance = 5.0f; // 클릭할 수 있는 최대 거리

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f; // 0.3초 이내 두 번 클릭 시 더블클릭

    void Update()
    {
        // 1. 카메라가 없으면 에러 안 나게 안전 복귀
        if (Camera.main == null) return;

        // 2. 마우스 입력 장치가 없으면 안전 복귀
        if (Mouse.current == null) return;

        // 마우스 좌클릭 감지
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, clickDistance))
            {
                // 3. 충돌한 오브젝트와 콤포넌트가 확실히 존재할 때만 실행
                if (hit.collider != null)
                {
                    CapsuleOpen capsule = hit.collider.GetComponentInParent<CapsuleOpen>();

                    if (capsule != null)
                    {
                        float timeSinceLastClick = Time.time - lastClickTime;

                        if (timeSinceLastClick <= doubleClickThreshold)
                        {
                            // ★ 더블클릭 성공! 캡슐 오픈 함수 호출
                            capsule.OnPlayerClicked();
                            lastClickTime = 0f;
                        }
                        else
                        {
                            lastClickTime = Time.time;
                        }
                    }
                }
            }
        }
    }
}