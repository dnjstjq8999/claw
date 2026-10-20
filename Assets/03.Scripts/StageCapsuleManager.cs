using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCapsuleManager : MonoBehaviour
{
    public static StageCapsuleManager Instance;

    [Header("Capsule Prefabs")]
    [SerializeField] private GameObject blueCapsulePrefab;
    [SerializeField] private GameObject yellowCapsulePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnRadius = 0.5f;

    [Header("Stage Settings")]
    [SerializeField] private int blueCount = 10;
    [SerializeField] private int yellowCount = 10;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerateStageCapsules();
    }

    public void GenerateStageCapsules()
    {
        ClearExistingCapsules();

        // 노란 공(짝수: 0,2,4,6,8) 각 2개씩 총 10개 풀 생성 및 셔플
        List<int> yellowNumbers = new List<int> { 0, 0, 2, 2, 4, 4, 6, 6, 8, 8 };
        ShuffleList(yellowNumbers);

        // 파란 공(홀수: 1,3,5,7,9) 각 2개씩 총 10개 풀 생성 및 셔플
        List<int> blueNumbers = new List<int> { 1, 1, 3, 3, 5, 5, 7, 7, 9, 9 };
        ShuffleList(blueNumbers);

        // 공 프리팹 리스트 구성 (순서 셔플)
        List<GameObject> capsulePrefabList = new List<GameObject>();
        for (int i = 0; i < blueCount; i++) capsulePrefabList.Add(blueCapsulePrefab);
        for (int i = 0; i < yellowCount; i++) capsulePrefabList.Add(yellowCapsulePrefab);
        ShuffleList(capsulePrefabList);

        // 연속 생성 및 숫자 데이터 주입
        StartCoroutine(SpawnRoutine(capsulePrefabList, yellowNumbers, blueNumbers));
    }

    private IEnumerator SpawnRoutine(List<GameObject> prefabList, List<int> yellowNums, List<int> blueNums)
    {
        int spawnIndex = 0;
        int yellowIndex = 0;
        int blueIndex = 0;

        foreach (GameObject prefab in prefabList)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                spawnIndex * 0.15f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Quaternion randomRotation = Random.rotation;
            GameObject capsuleObj = Instantiate(prefab, spawnPoint.position + randomOffset, randomRotation, transform);

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
}