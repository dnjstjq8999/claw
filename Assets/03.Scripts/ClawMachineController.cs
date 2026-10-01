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

    [Header("--- WASD 수평 이동 설정 ---")]
    public float moveSpeed = 3f;
    public Vector2 minBounds = new Vector2(-2.5f, -2.5f); // X, Z 최소 이동 범위
    public Vector2 maxBounds = new Vector2(2.5f, 2.5f);   // X, Z 최대 이동 범위

    [Header("--- 집게 수직 이동 설정 ---")]
    public float dropDepth = 2.5f;     // 하강 깊이 (Y축)
    public float dropSpeed = 2f;       // 하강/상승 속도

    [Header("--- 손가락 회전 각도 ---")]
    public float openAngle = 30f;
    public float closeAngle = -25f;
    public float fingerSpeed = 5f;     // 손가락 동작 속도

    private bool isOperating = false;
    private Vector3 initialCarriagePos;
    private Vector3 initialClawLocalPos;
    private Quaternion[] initialFingerRotations;
    private Vector3[] fingerLocalAxes; // 각 힌지별 중심 방향 전용 회전축

    void Start()
    {
        if (carriageTransform != null)
            initialCarriagePos = carriageTransform.position;

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

                    // 1. 집게 중심(clawTransform)에서 힌지로 향하는 방위각 벡터 계산
                    Vector3 centerPos = clawTransform != null ? clawTransform.position : transform.position;
                    Vector3 dirFromCenter = fingers[i].position - centerPos;
                    dirFromCenter.y = 0f; // 수평 위치만 계산

                    if (dirFromCenter.sqrMagnitude < 0.0001f)
                        dirFromCenter = Vector3.forward;
                    else
                        dirFromCenter.Normalize();

                    // 2. 중심점을 향해 안쪽으로 오므라드는 월드 회전축 계산 (방위각 x Y축 외적)
                    Vector3 worldAxis = Vector3.Cross(dirFromCenter, Vector3.up);

                    // 3. 해당 힌지의 Local 좌표계 축으로 변환하여 기억
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
            HandleWASDMovement();

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartCoroutine(ClawSequenceRoutine());
            }
        }
    }

    void HandleWASDMovement()
    {
        if (carriageTransform == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ -= 1f; // W: 앞으로
        if (Keyboard.current.sKey.isPressed) moveZ += 1f; // S: 뒤로
        if (Keyboard.current.aKey.isPressed) moveX += 1f; // A: 왼쪽
        if (Keyboard.current.dKey.isPressed) moveX -= 1f; // D: 오른쪽

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 newPos = carriageTransform.position + moveDir * moveSpeed * Time.deltaTime;

        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.z = Mathf.Clamp(newPos.z, minBounds.y, maxBounds.y);

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

        // 2단계: 최저점에서 집게 모으기 (잡기)
        yield return new WaitForSeconds(0.2f);
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

        // 4단계: 원래 수령 위치(시작 지점)로 이동
        Vector3 currentCarriagePos = carriageTransform.position;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed * 0.8f);
            Vector3 interpPos = Vector3.Lerp(currentCarriagePos, initialCarriagePos, t);
            carriageTransform.position = interpPos;

            if (railTransform != null)
            {
                Vector3 railPos = railTransform.position;
                railPos.z = interpPos.z;
                railTransform.position = railPos;
            }
            yield return null;
        }

        // 5단계: 집게 넓게 벌려 상품 떨어뜨리기
        yield return new WaitForSeconds(0.2f);
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
                // 각 힌지가 중심점을 향해 접히도록 계산된 고유 회전축 적용
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