using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attached To: Player 오브젝트
/// 역할: 화면 중앙 에임점 중심 완벽 동기화 마우스 Look 및 WASD 이동
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("--- 이동 설정 ---")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("--- 시점 회전 설정 (에임 동기화) ---")]
    [Tooltip("Player 하위의 Main Camera Transform")]
    public Transform playerCamera;
    [Tooltip("마우스 회전 감도 (필요에 따라 0.1~2.0 조절)")]
    public float mouseSensitivity = 1.0f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f; // 카메라 상하 각도

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 시작 시 커서를 화면 중앙에 완벽 고정 및 숨김
        LockCursor(true);
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        // 1. 화면 중앙 에임 기준 시점 회전
        HandleMouseLook();

        // 2. 중력 및 바닥 검사
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 3. 이동 처리 (캐릭터가 바라보는 정면 기준)
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;

        Vector3 moveInput = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 4. 수직 중력
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// 마우스 이동과 카메라/플레이어 회전을 1:1로 정확히 동기화
    /// </summary>
    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * 0.1f;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.1f;

        // 카메라 상하 회전 (X축)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // 플레이어 몸통 좌우 회전 (Y축)
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// 마우스 커서 잠금/해제 제어
    /// </summary>
    public void LockCursor(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}