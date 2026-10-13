using UnityEngine;

public class GoalArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CapsuleData capsule = other.GetComponent<CapsuleData>();

        if (capsule != null)
        {
            // 1. 기존 UI 개수 증가 (필요한 경우)
            if (StageUIManager.Instance != null)
            {
                StageUIManager.Instance.AddCapsule(other.tag);
            }

            // 2. 테이블 위 20개 스탠드에 캡슐 배치 (스탠드 매니저가 있을 때만 작동)
            if (StandSpawner.Instance != null)
            {
                // TODO: 스탠드 매니저와 연동할 데이터 전달
            }

            // 바닥에 들어온 공 제거
            Destroy(other.gameObject, 0.3f);
        }
    }
}