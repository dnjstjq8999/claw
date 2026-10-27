using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("--- 이동 설정 (1~10레벨 업그레이드 연동) ---")]
    public float moveSpeed = 5f;
    private float baseMoveSpeed;
    public int minLevel = 1;
    public int maxLevel = 10;

    public float gravity = -9.81f;

    [Header("--- 시점 회전 설정 (에임 동기화) ---")]
    public Transform playerCamera;
    public float mouseSensitivity = 1.0f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    public bool canMove = true;

    void Start()
    {
        baseMoveSpeed = moveSpeed;

        controller = GetComponent<CharacterController>();
        LockCursor(true);

        ApplyPlayerSpeedUpgrade();
    }

    void Update()
    {
        ApplyPlayerSpeedUpgrade();

        if (!canMove) return;

        if (canMove && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor(true);
        }

        if (Keyboard.current == null || Mouse.current == null) return;

        // 1. 화면 중앙 에임 기준 시점 회전
        HandleMouseLook();

        // 2. 중력 및 바닥 검사
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 3. 이동 처리
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

    void ApplyPlayerSpeedUpgrade()
    {
        if (GameManager.Instance != null)
        {
            moveSpeed = GameManager.Instance.GetCurrentMoveSpeed();
        }
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float rawSensitivity = (GameManager.Instance != null) ? GameManager.Instance.mouseSensitivity : mouseSensitivity;
        float currentSensitivity = Mathf.Max(rawSensitivity, 0.1f);

        float mouseX = mouseDelta.x * currentSensitivity * 1.0f;
        float mouseY = mouseDelta.y * currentSensitivity * 1.0f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetControllable(bool controllable)
    {
        canMove = controllable;

        if (!controllable)
        {
            LockCursor(false);
        }
        else
        {
            LockCursor(true);
        }
    }

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