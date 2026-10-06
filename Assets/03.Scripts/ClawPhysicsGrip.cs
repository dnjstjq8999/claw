using System.Collections;
using UnityEngine;

/// <summary>
/// Attached To: 집게 내부 중앙 오브젝트 (ClawCenter)
/// 역할: 집게가 공을 잡았을 때 FixedJoint를 연결하여 상승 시 빠지지 않도록 견고하게 고정
/// </summary>
public class ClawPhysicsGrip : MonoBehaviour
{
    [Header("--- 잡기 상태 ---")]
    public Rigidbody currentTargetBall;

    // ConfigurableJoint 대신 견고한 FixedJoint 사용
    private FixedJoint activeJoint;

    private void OnTriggerEnter(Collider other)
    {
        // 이미 잡은 공이 있다면 추가 감지 안 함
        if (activeJoint != null) return;

        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb == null) rb = other.GetComponent<Rigidbody>();

        if (rb != null && !rb.isKinematic)
        {
            currentTargetBall = rb;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 이미 조인트로 잡힌 상태라면 트리거 영역을 벗어나도 target을 놓치지 않음
        if (activeJoint != null) return;

        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb == null) rb = other.GetComponent<Rigidbody>();

        if (rb != null && rb == currentTargetBall)
        {
            currentTargetBall = null;
        }
    }

    /// <summary>
    /// 집게가 최하단에서 오무려질 때 호출
    /// </summary>
    public void GrabBallWithPhysics()
    {
        if (currentTargetBall != null && activeJoint == null)
        {
            // 1. 공의 순간적인 물리 반발력을 줄이기 위해 속도 초기화
            currentTargetBall.linearVelocity = Vector3.zero;
            currentTargetBall.angularVelocity = Vector3.zero;

            // 2. FixedJoint 동적 생성 및 연결
            activeJoint = gameObject.AddComponent<FixedJoint>();
            activeJoint.connectedBody = currentTargetBall;

            // 3. 끊어지는 힘(Break Force)을 무한대로 설정하여 상승 관성에 떨어지지 않게 고정
            activeJoint.breakForce = float.PositiveInfinity;
            activeJoint.breakTorque = float.PositiveInfinity;
        }
    }

    /// <summary>
    /// 배출구 위에서 집게가 열릴 때 호출
    /// </summary>
    public void ReleaseBall()
    {
        if (activeJoint != null)
        {
            Destroy(activeJoint);
            activeJoint = null;
        }

        currentTargetBall = null;
    }
}