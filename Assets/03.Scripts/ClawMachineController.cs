using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClawMachineController : MonoBehaviour
{
    [Header("--- 대상 오브젝트 연결 ---")]
    public Transform carriageTransform;
    public Transform railTransform;
    public Transform clawTransform;
    public Transform[] fingers;

    [Header("--- 물리 잡기 시스템 연동 ---")]
    public ClawPhysicsGrip clawGrip;

    [Header("--- 방향키 수평 이동 설정 ---")]
    public float moveSpeed = 3f;
    public Vector2 minBounds = new Vector2(-2.5f, -2.5f);
    public Vector2 maxBounds = new Vector2(2.5f, 2.5f);

    [Header("--- 집게 수직 이동 설정 ---")]
    public float dropDepth = 2.5f;
    public float dropSpeed = 2f;

    [Header("--- 손가락 회전 각도 ---")]
    public float openAngle = 30f;
    public float closeAngle = -25f;
    public float fingerSpeed = 5f;

    private bool isOperating = false;
    private Vector3 initialCarriagePos;
    private Vector3 initialClawLocalPos;
    private Quaternion[] initialFingerRotations;
    private Vector3[] fingerLocalAxes;

    private Rigidbody carriageRigidbody;

    void Start()
    {
        if (carriageTransform != null)
        {
            initialCarriagePos = carriageTransform.position;
            carriageRigidbody = carriageTransform.GetComponent<Rigidbody>();
        }

        if (clawTransform != null)
            initialClawLocalPos = clawTransform.localPosition;

        if (fingers != null && fingers.Length > 0)
        {
            initialFingerRotations = new Quaternion[fingers.Length];
            fingerLocalAxes = new Vector3[fingers.Length];

            for (int i = 0; i < fingers.Length; i++)
            {
                if (fingers[i] != null)
                {
                    initialFingerRotations[i] = fingers[i].localRotation;

                    Vector3 centerPos = clawTransform != null ? clawTransform.position : transform.position;
                    Vector3 dirFromCenter = fingers[i].position - centerPos;
                    dirFromCenter.y = 0f;

                    if (dirFromCenter.sqrMagnitude < 0.0001f)
                        dirFromCenter = Vector3.forward;
                    else
                        dirFromCenter.Normalize();

                    Vector3 worldAxis = Vector3.Cross(dirFromCenter, Vector3.up);
                    fingerLocalAxes[i] = fingers[i].InverseTransformDirection(worldAxis);
                }
            }
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (!isOperating)
        {
            HandleArrowKeyMovement();

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartCoroutine(ClawSequenceRoutine());
            }
        }
    }

    void HandleArrowKeyMovement()
    {
        if (carriageTransform == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.upArrowKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.downArrowKey.isPressed) moveZ += 1f;
        if (Keyboard.current.leftArrowKey.isPressed) moveX += 1f;
        if (Keyboard.current.rightArrowKey.isPressed) moveX -= 1f;

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 newPos = carriageTransform.position + moveDir * moveSpeed * Time.deltaTime;

        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.z = Mathf.Clamp(newPos.z, minBounds.y, maxBounds.y);

        // 빠른 수평 이동을 위해 Transform 위치 직접 변경
        carriageTransform.position = newPos;

        if (railTransform != null)
        {
            Vector3 railPos = railTransform.position;
            railPos.z = newPos.z;
            railTransform.position = railPos;
        }
    }

    IEnumerator ClawSequenceRoutine()
    {
        isOperating = true;

        // 1단계: 내려가면서 집게 벌어지기
        Vector3 startLocalPos = initialClawLocalPos;
        Vector3 targetLocalPos = startLocalPos - new Vector3(0f, dropDepth, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * dropSpeed;
            clawTransform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            RotateFingers(openAngle, Time.deltaTime * fingerSpeed);
            yield return null;
        }

        // 2단계: 최저점에서 집게 모으기 & 조그만 ClawCenter 영역 내 공 잡기
        yield return new WaitForSeconds(0.2f);

        if (clawGrip != null)
        {
            clawGrip.GrabBallWithPhysics();
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fingerSpeed;
            RotateFingers(closeAngle, Time.deltaTime * fingerSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);

        // 3단계: 상승
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * dropSpeed;
            clawTransform.localPosition = Vector3.Lerp(targetLocalPos, startLocalPos, t);
            RotateFingers(closeAngle, Time.deltaTime * fingerSpeed);
            yield return null;
        }

        // 4단계: 원래 수령 위치(시작 지점)로 빠른 이동
        Vector3 startCarriagePos = carriageTransform.position;
        t = 0f;

        // 물리 주기(FixedUpdate)에 맞춰 이동하여 둔함/묵직함 현상 제거 및 스피드 복원
        while (t < 1f)
        {
            t += Time.fixedDeltaTime * (moveSpeed * 0.8f);
            Vector3 interpPos = Vector3.Lerp(startCarriagePos, initialCarriagePos, t);

            if (carriageRigidbody != null)
            {
                carriageRigidbody.MovePosition(interpPos);
            }
            else
            {
                carriageTransform.position = interpPos;
            }

            if (railTransform != null)
            {
                Vector3 railPos = railTransform.position;
                railPos.z = interpPos.z;
                railTransform.position = railPos;
            }

            // 프레임 대기 대신 물리 주기 대기!
            yield return new WaitForFixedUpdate();
        }

        // 목적지 위치 보정
        if (carriageRigidbody != null) carriageRigidbody.MovePosition(initialCarriagePos);
        else carriageTransform.position = initialCarriagePos;

        // 5단계: 집게 벌려 공 떨어뜨리기
        yield return new WaitForSeconds(0.2f);

        if (clawGrip != null)
        {
            clawGrip.ReleaseBall();
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fingerSpeed;
            RotateFingers(openAngle, Time.deltaTime * fingerSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(0.8f);

        // 6단계: 기본 상태 복구
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fingerSpeed;
            ResetFingers(Time.deltaTime * fingerSpeed);
            yield return null;
        }

        isOperating = false;
    }

    void RotateFingers(float angleAmount, float lerpFactor)
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            if (fingers[i] != null && fingerLocalAxes != null && i < fingerLocalAxes.Length)
            {
                Quaternion targetRot = initialFingerRotations[i] * Quaternion.AngleAxis(angleAmount, fingerLocalAxes[i]);
                fingers[i].localRotation = Quaternion.Slerp(fingers[i].localRotation, targetRot, lerpFactor);
            }
        }
    }

    void ResetFingers(float lerpFactor)
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            if (fingers[i] != null)
            {
                fingers[i].localRotation = Quaternion.Slerp(fingers[i].localRotation, initialFingerRotations[i], lerpFactor);
            }
        }
    }
}