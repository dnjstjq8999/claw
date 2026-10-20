using UnityEngine;
using UnityEngine.InputSystem;

public class CapsuleOpenManager : MonoBehaviour
{
    [Header("클릭 감지 설정")]
    public float clickDistance = 5.0f;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    void Update()
    {
        if (Camera.main == null) return;

        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, clickDistance))
            {
                if (hit.collider != null)
                {
                    CapsuleOpen capsule = hit.collider.GetComponentInParent<CapsuleOpen>();

                    if (capsule != null)
                    {
                        float timeSinceLastClick = Time.time - lastClickTime;

                        if (timeSinceLastClick <= doubleClickThreshold)
                        {
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