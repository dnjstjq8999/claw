using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [Header("--- Ä¸½¶ »ö»óº° ÇÁ¸®ÆÕ ¼³Á¤ ---")]
    [SerializeField] private GameObject blueCapsulePrefab;
    [SerializeField] private GameObject yellowCapsulePrefab;

    private void OnTriggerEnter(Collider other)
    {
        CapsuleData capsule = other.GetComponent<CapsuleData>();

        if (capsule != null)
        {

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

            Destroy(other.gameObject);
        }
    }
}