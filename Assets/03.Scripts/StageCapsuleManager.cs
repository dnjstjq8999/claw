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

        // 1. 노란 공(짝수: 0,2,4,6,8) 각 2개씩 총 10개 풀 생성 및 셔플
        List<int> yellowNumbers = new List<int> { 0, 0, 2, 2, 4, 4, 6, 6, 8, 8 };
        ShuffleList(yellowNumbers);

        // 2. 파란 공(홀수: 1,3,5,7,9) 각 2개씩 총 10개 풀 생성 및 셔플
        List<int> blueNumbers = new List<int> { 1, 1, 3, 3, 5, 5, 7, 7, 9, 9 };
        ShuffleList(blueNumbers);

        // 3. 공 프리팹 리스트 구성 (어떤 공이 스폰될지 순서 셔플)
        List<GameObject> capsulePrefabList = new List<GameObject>();
        for (int i = 0; i < blueCount; i++) capsulePrefabList.Add(blueCapsulePrefab);
        for (int i = 0; i < yellowCount; i++) capsulePrefabList.Add(yellowCapsulePrefab);
        ShuffleList(capsulePrefabList);

        // 4. 약간의 위치 오프셋을 주며 순간적으로 연속 생성 및 숫자 데이터 주입
        StartCoroutine(SpawnRoutine(capsulePrefabList, yellowNumbers, blueNumbers));
    }

    private IEnumerator SpawnRoutine(List<GameObject> prefabList, List<int> yellowNums, List<int> blueNums)
    {
        int spawnIndex = 0;
        int yellowIndex = 0;
        int blueIndex = 0;

        foreach (GameObject prefab in prefabList)
        {
            // 공이 스폰될 때마다 Y축 높이를 조금씩 올려서 위에서 순차적으로 떨어지게 함
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                spawnIndex * 0.15f, // 높이에 차등을 주어 겹침 방지
                Random.Range(-spawnRadius, spawnRadius)
            );

            Quaternion randomRotation = Random.rotation;

            GameObject capsuleObj = Instantiate(prefab, spawnPoint.position + randomOffset, randomRotation, transform);

            // 5. 생성된 공에 정확한 중복 없는 숫자 데이터 주입
            CapsuleData capsuleData = capsuleObj.GetComponent<CapsuleData>();
            if (capsuleData != null)
            {
                bool isYellow = capsuleObj.CompareTag("YellowCapsule");
                int assignedNum = 0;

                if (isYellow)
                {
                    assignedNum = yellowNums[yellowIndex];
                    yellowIndex++;
                }
                else
                {
                    assignedNum = blueNums[blueIndex];
                    blueIndex++;
                }

                capsuleData.SetCapsuleData(isYellow, assignedNum);
            }

            spawnIndex++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 리스트를 무작위로 섞는 함수 (Fisher-Yates Shuffle)
    /// </summary>
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
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