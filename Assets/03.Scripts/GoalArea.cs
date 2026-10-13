using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [Header("--- 캡슐 색상별 프리팹 설정 ---")]
    [SerializeField] private GameObject blueCapsulePrefab;
    [SerializeField] private GameObject yellowCapsulePrefab;

    private void OnTriggerEnter(Collider other)
    {
        // 무언가 닿았는지 확인하는 디버그 로그
        Debug.Log($"[GoalArea 감지됨] 닿은 오브젝트 이름: {other.gameObject.name} / 태그: {other.tag}");

        CapsuleData capsule = other.GetComponent<CapsuleData>();

        if (capsule != null)
        {
            Debug.Log("[GoalArea] CapsuleData 컴포넌트 발견!");

            if (StageUIManager.Instance != null)
            {
                StageUIManager.Instance.AddCapsule(other.tag);
            }

            GameObject prefabToSpawn = null;
            if (other.CompareTag("BlueCapsule"))
            {
                prefabToSpawn = blueCapsulePrefab;
            }
            else if (other.CompareTag("YellowCapsule"))
            {
                prefabToSpawn = yellowCapsulePrefab;
            }
            else
            {
                prefabToSpawn = blueCapsulePrefab != null ? blueCapsulePrefab : yellowCapsulePrefab;
            }

            if (StandSpawner.Instance != null && prefabToSpawn != null)
            {
                StandSpawner.Instance.PlaceCapsuleOnNextSlot(capsule.isEven, capsule.capsuleNumber, prefabToSpawn);
            }
            else
            {
                Debug.LogWarning("[GoalArea] StandSpawner 또는 생성할 프리팹이 연결되지 않았습니다!");
            }

            Destroy(other.gameObject);
        }
        else
        {
            Debug.LogWarning("[GoalArea] 닿은 오브젝트에 CapsuleData 스크립트가 없습니다!");
        }
    }
}