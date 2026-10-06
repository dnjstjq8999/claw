using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCapsuleManager : MonoBehaviour
{
    [Header("Capsule Prefabs")]
    [SerializeField] private GameObject blueCapsulePrefab;
    [SerializeField] private GameObject yellowCapsulePrefab;

    [Header("Spawn Settings")]
    [Tooltip("공이 떨어질 시작 위치 (인형뽑기 통 내부 상공)")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnRadius = 0.5f;

    [Header("Stage Settings")]
    [SerializeField] private int blueCount = 10;
    [SerializeField] private int yellowCount = 10;

    private void Start()
    {
        // 스테이지 시작 시 공 무작위 배치 실행
        GenerateStageCapsules();
    }

    public void GenerateStageCapsules()
    {
        // 기존에 남아있는 공이 있다면 제거 (재시작 대비)
        ClearExistingCapsules();

        // 파란 공/노란 공 리스트 생성 및 무작위 셔플
        List<GameObject> capsuleList = new List<GameObject>();
        for (int i = 0; i < blueCount; i++) capsuleList.Add(blueCapsulePrefab);
        for (int i = 0; i < yellowCount; i++) capsuleList.Add(yellowCapsulePrefab);

        for (int i = 0; i < capsuleList.Count; i++)
        {
            GameObject temp = capsuleList[i];
            int randomIndex = Random.Range(i, capsuleList.Count);
            capsuleList[i] = capsuleList[randomIndex];
            capsuleList[randomIndex] = temp;
        }

        // 약간의 위치 오프셋을 주며 순간적으로 연속 생성
        StartCoroutine(SpawnRoutine(capsuleList));
    }

    private IEnumerator SpawnRoutine(List<GameObject> capsuleList)
    {
        int spawnIndex = 0;

        foreach (GameObject prefab in capsuleList)
        {
            // 공이 스폰될 때마다 Y축 높이를 조금씩 올려서 위에서 순차적으로 떨어지게 함
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                spawnIndex * 0.15f, // 높이에 차등을 주어 겹침 방지
                Random.Range(-spawnRadius, spawnRadius)
            );

            Quaternion randomRotation = Random.rotation;

            GameObject capsule = Instantiate(prefab, spawnPoint.position + randomOffset, randomRotation, transform);


            spawnIndex++;

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void ClearExistingCapsules()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
        }
    }
}
