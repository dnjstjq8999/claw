using UnityEngine;

public class GoalArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string objectTag = other.tag;

        // 공이 들어오면 StageUIManager에 전달
        if (objectTag == "BlueCapsule" || objectTag == "YellowCapsule")
        {
            if (StageUIManager.Instance != null)
            {
                StageUIManager.Instance.AddCapsule(objectTag);
            }

            // 배출구로 들어온 공 제거
            Destroy(other.gameObject, 0.3f);
        }
    }
}