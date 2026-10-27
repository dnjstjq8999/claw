using System.Collections;
using UnityEngine;

public class ClawPhysicsGrip : MonoBehaviour
{
    [Header("--- 잡기 상태 ---")]
    public Rigidbody currentTargetBall;

    private FixedJoint activeJoint;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            float targetY = GameManager.Instance.GetCurrentClawCenterY();

            Vector3 localPos = transform.localPosition;
            localPos.y = targetY;
            transform.localPosition = localPos;

            Debug.Log($"[집게 파워 연동] 적용된 Y 위치: {targetY}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        if (activeJoint != null) return;

        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb == null) rb = other.GetComponent<Rigidbody>();

        if (rb != null && rb == currentTargetBall)
        {
            currentTargetBall = null;
        }
    }

    public void GrabBallWithPhysics()
    {
        if (currentTargetBall != null && activeJoint == null)
        {
            currentTargetBall.linearVelocity = Vector3.zero;
            currentTargetBall.angularVelocity = Vector3.zero;

            activeJoint = gameObject.AddComponent<FixedJoint>();
            activeJoint.connectedBody = currentTargetBall;

            activeJoint.breakForce = float.PositiveInfinity;
            activeJoint.breakTorque = float.PositiveInfinity;
        }
    }

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