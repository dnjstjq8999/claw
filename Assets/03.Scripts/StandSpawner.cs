using System.Collections.Generic;
using UnityEngine;

public class StandSpawner : MonoBehaviour
{
    public static StandSpawner Instance;

    [Header("--- 스탠드 배치 설정 ---")]
    [SerializeField] private GameObject standPrefab; // 복사할 스탠드 프리팹
    [SerializeField] private int totalStands = 20;     // 총 20개
    [SerializeField] private int columns = 5;          // 한 줄에 5개씩 (5개 x 4줄 = 20개)
    [SerializeField] private float spacingX = 0.15f;   // 가로 간격
    [SerializeField] private float spacingZ = 0.15f;   // 세로 간격
    [SerializeField] private Vector3 startOffset = new Vector3(-0.3f, 0.01f, -0.3f); // 테이블 위 시작 위치 오프셋

    [Header("--- 생성된 스탠드 목록 (자동 관리) ---")]
    public List<Transform> standSlots = new List<Transform>();

    void Start()
    {
        SpawnStands();
    }

    public void SpawnStands()
    {
        // 기존에 생성된 게 있다면 초기화
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        standSlots.Clear();

        // 20개 스탠드 생성 및 격자 배치
        for (int i = 0; i < totalStands; i++)
        {
            int row = i / columns; // 몇 번째 줄
            int col = i % columns; // 줄 안에서 몇 번째 칸

            // 위치 계산
            Vector3 spawnPos = transform.position + startOffset + new Vector3(col * spacingX, 0, row * spacingZ);

            // 스탠드 생성 (현재 오브젝트의 자식으로 배치)
            GameObject standObj = Instantiate(standPrefab, spawnPos, Quaternion.identity, transform);
            standObj.name = $"Stand_{i + 1}";

            // 리스트에 등록 (나중에 1번부터 순서대로 캡슐을 꽂기 위함)
            standSlots.Add(standObj.transform);
        }

    }
}