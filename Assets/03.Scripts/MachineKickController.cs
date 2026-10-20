using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineKickController : MonoBehaviour
{
    [Header("--- 충격 물리 설정 ---")]
    public Transform kickCenter;
    public float kickRadius = 5.0f;
    public float kickForce = 15.0f;
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

            if (rb != null && !rb.isKinematic)
            {
                rb.WakeUp();
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