using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attached To: ClawMachine 오브젝트
/// 역할: 손잡이 에임 감지, 인형뽑기 시작/종료 처리, 카메라 시점 전환 및 기계 조작권 이전
/// </summary>
public class ClawMachineInteraction : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera characterCamera;      // 플레이어 메인 카메라
    public GameObject frontCamera;      // 인형뽑기 정면 카메라
    public GameObject sideCamera;       // 인형뽑기 측면 카메라

    [Header("플레이어 및 기계 스크립트 참조")]
    public PlayerController playerController;     // 플레이어 조작 스크립트
    public ClawMachineController clawMachineController; // 집게 이동/다운 조작 스크립트

    [Header("UI 설정")]
    public GameObject interactionUI;    // [E] 상호작용 UI
    public GameObject crosshairUI;      // 화면 중앙 에임점 UI

    [Header("Aim 조준 상호작용 설정")]
    public Transform handleTransform;   // 조준 대상 (손잡이 Transform)
    public float interactDistance = 3.0f; // 최대 감지 거리
    public LayerMask handleLayer;       // 손잡이 레이어

    private bool isAimingHandle = false; // 현재 손잡이를 바라보는 중인지
    private bool isPlaying = false;      // 인형뽑기 진행 중인지

    private void Start()
    {
        // 초기 카메라 및 UI 상태 정돈
        if (frontCamera != null) frontCamera.SetActive(false);
        if (sideCamera != null) sideCamera.SetActive(false);
        if (interactionUI != null) interactionUI.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(true);

        // 게임 시작 시 기계 직접 조작 스크립트는 끔
        if (clawMachineController != null) clawMachineController.enabled = false;
    }

    private void Update()
    {
        if (!isPlaying)
        {
            CheckHandleAim();

            if (isAimingHandle && IsEPressed())
            {
                EnterClawMachine();
            }
        }
        else
        {
            HandleCameraSwitchInput();

            if (IsEscPressed())
            {
                ExitClawMachine();
            }
        }
    }

    private void CheckHandleAim()
    {
        if (characterCamera == null) return;

        Ray ray = characterCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        int mask = handleLayer.value == 0 ? Physics.DefaultRaycastLayers : handleLayer.value;

        if (Physics.Raycast(ray, out hit, interactDistance, mask))
        {
            bool isHandle = false;

            // 1. 인스펙터에 지정된 handleTransform과 직접 일치하는지 우선 검사 (가장 안전)
            if (handleTransform != null && hit.transform == handleTransform)
            {
                isHandle = true;
            }
            // 2. 오브젝트 이름 또는 CompareTag 예외 안전 검사
            else if (hit.collider.gameObject.name.Contains("InteractionTrigger") || hit.collider.gameObject.name.Contains("Handle"))
            {
                isHandle = true;
            }

            if (isHandle)
            {
                if (!isAimingHandle)
                {
                    isAimingHandle = true;
                    if (interactionUI != null) interactionUI.SetActive(true);
                }
                return;
            }
        }

        if (isAimingHandle)
        {
            isAimingHandle = false;
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }

    private void EnterClawMachine()
    {
        isPlaying = true;
        isAimingHandle = false;

        if (interactionUI != null) interactionUI.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(false);

        // 플레이어 제어 비활성화 및 커서 풀기
        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.LockCursor(false);
        }

        // 인형뽑기 집게 조작 스크립트 활성화
        if (clawMachineController != null)
        {
            clawMachineController.enabled = true;
        }

        // 정면 카메라로 시점 전환
        if (characterCamera != null) characterCamera.gameObject.SetActive(false);
        if (frontCamera != null) frontCamera.SetActive(true);
        if (sideCamera != null) sideCamera.SetActive(false);
    }

    private void ExitClawMachine()
    {
        isPlaying = false;

        if (frontCamera != null) frontCamera.SetActive(false);
        if (sideCamera != null) sideCamera.SetActive(false);
        if (characterCamera != null) characterCamera.gameObject.SetActive(true);

        if (crosshairUI != null) crosshairUI.SetActive(true);

        // 인형뽑기 기계 조작 비활성화
        if (clawMachineController != null)
        {
            clawMachineController.enabled = false;
        }

        // 플레이어 제어권 다시 복구
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.LockCursor(true);
        }
    }

    private void HandleCameraSwitchInput()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftAltKey.wasPressedThisFrame) SwitchToSideView();
            if (Keyboard.current.leftAltKey.wasReleasedThisFrame) SwitchToFrontView();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt)) SwitchToSideView();
            if (Input.GetKeyUp(KeyCode.LeftAlt)) SwitchToFrontView();
        }
    }

    private void SwitchToSideView()
    {
        if (frontCamera != null) frontCamera.SetActive(false);
        if (sideCamera != null) sideCamera.SetActive(true);
    }

    private void SwitchToFrontView()
    {
        if (sideCamera != null) sideCamera.SetActive(false);
        if (frontCamera != null) frontCamera.SetActive(true);
    }

    private bool IsEPressed()
    {
        if (Keyboard.current != null) return Keyboard.current.eKey.wasPressedThisFrame;
        return Input.GetKeyDown(KeyCode.E);
    }

    private bool IsEscPressed()
    {
        if (Keyboard.current != null) return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Input.GetKeyDown(KeyCode.Escape);
    }
}