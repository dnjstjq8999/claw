using UnityEngine;
using UnityEngine.InputSystem;


public class ClawMachineInteraction : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera characterCamera;
    public GameObject frontCamera;
    public GameObject sideCamera;
    [Header("플레이어 및 기계 스크립트 참조")]
    public PlayerController playerController;
    public ClawMachineController clawMachineController;

    [Header("UI 설정")]
    public GameObject interactionUI;
    public GameObject crosshairUI;

    [Header("Aim 조준 상호작용 설정")]
    public Transform handleTransform;
    public float interactDistance = 3.0f;
    public LayerMask handleLayer;

    private bool isAimingHandle = false;
    private bool isPlaying = false;

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

            if (handleTransform != null && hit.transform == handleTransform)
            {
                isHandle = true;
            }
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

        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.LockCursor(false);
        }

        if (clawMachineController != null)
        {
            clawMachineController.enabled = true;
        }

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

        if (clawMachineController != null)
        {
            clawMachineController.enabled = false;
        }

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