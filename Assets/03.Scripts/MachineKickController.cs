using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attached To: ClawMachine 오브젝트
/// 역할: [Z] 키 입력 시 기계 내부의 공들에게 쿵! 하고 직관적인 물리 충격을 가함
/// </summary>
public class MachineKickController : MonoBehaviour
{
    [Header("--- 충격 물리 설정 ---")]
    [Tooltip("충격파가 터질 중심점 (InteractionTrigger 위치 또는 기계 바닥)")]
    public Transform kickCenter;
    [Tooltip("충격파 감지 범위")]
    public float kickRadius = 5.0f;
    [Tooltip("발로 차는 힘의 크기")]
    public float kickForce = 15.0f;
    [Tooltip("위로 튀어오르는 힘의 비율")]
    public float upwardModifier = 2.0f;

    [Header("--- 카메라 셰이크 ---")]
    public Transform[] camerasToShake;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.12f;

    [Header("--- 쿨타임 ---")]
    public float cooldown = 1.0f;

    private bool isCooldown = false;

    void Update()
    {
        if (IsZPressed() && !isCooldown)
        {
            KickMachine();
        }
    }

    private void KickMachine()
    {
        StartCoroutine(CooldownRoutine());

        // 1. 카메라 흔들림 연출
        foreach (Transform cam in camerasToShake)
        {
            if (cam != null && cam.gameObject.activeInHierarchy)
            {
                StartCoroutine(ShakeCameraRoutine(cam));
            }
        }

        // 2. 중심점 위치 계산
        Vector3 centerPosition = kickCenter != null ? kickCenter.position : transform.position;

        // 3. 충격파 범위 안의 콜라이더 탐색
        Collider[] hitColliders = Physics.OverlapSphere(centerPosition, kickRadius);
        int movedCount = 0;

        foreach (Collider hit in hitColliders)
        {
            Rigidbody rb = hit.GetComponentInParent<Rigidbody>();
            if (rb == null) rb = hit.GetComponent<Rigidbody>();

            // Rigidbody가 부착되어 있고 IsKinematic이 꺼진 공들만 충격 가하기
            if (rb != null && !rb.isKinematic)
            {
                rb.WakeUp(); // 물리 엔진 깨우기
                rb.AddExplosionForce(kickForce, centerPosition, kickRadius, upwardModifier, ForceMode.Impulse);
                movedCount++;
            }
        }
    }

    private IEnumerator ShakeCameraRoutine(Transform camTransform)
    {
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            camTransform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPos;
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }

    private bool IsZPressed()
    {
        if (Keyboard.current != null) return Keyboard.current.zKey.wasPressedThisFrame;
        return Input.GetKeyDown(KeyCode.Z);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centerPosition = kickCenter != null ? kickCenter.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPosition, kickRadius);
    }
}