using System.Collections.Generic;
using UnityEngine;

public class StandSpawner : MonoBehaviour
{
    public static StandSpawner Instance;

    [Header("--- 스탠드 배치 설정 ---")]
    [SerializeField] private GameObject standPrefab;
    [SerializeField] private int totalStands = 20;
    [SerializeField] private int columns = 5;
    [SerializeField] private float spacingX = 0.15f;
    [SerializeField] private float spacingZ = 0.15f;
    [SerializeField] private Vector3 startOffset = new Vector3(-0.3f, 0.01f, -0.3f);

    [Header("--- 생성된 스탠드 목록 (자동 관리) ---")]
    public List<Transform> standSlots = new List<Transform>();

    [Header("--- 캡슐 배치 추적 ---")]
    private int currentIndex = 0; // 다음에 캡슐이 놓일 스탠드 번호 (0부터 시작)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SpawnStands();
    }

    public void SpawnStands()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        standSlots.Clear();
        currentIndex = 0;

        for (int i = 0; i < totalStands; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 spawnPos = transform.position + startOffset + new Vector3(col * spacingX, 0, row * spacingZ);

            GameObject standObj = Instantiate(standPrefab, spawnPos, Quaternion.identity, transform);
            standObj.name = $"Stand_{i + 1}";

            standSlots.Add(standObj.transform);
        }
    }

    public void PlaceCapsuleOnNextSlot(bool isEven, int number, GameObject capsulePrefab)
    {
        if (currentIndex >= standSlots.Count)
        {
            Debug.LogWarning("[스탠드 가득 참] 더 이상 캡슐을 놓을 자리가 없습니다!");
            return;
        }

        Transform targetStand = standSlots[currentIndex];
        currentIndex++;

        // 스탠드 안에 있는 "Ball" 오브젝트 찾기
        Transform ballTransform = targetStand.Find("Ball");
        if (ballTransform != null && capsulePrefab != null)
        {
            MeshFilter targetMeshFilter = ballTransform.GetComponent<MeshFilter>();
            MeshRenderer targetRenderer = ballTransform.GetComponent<MeshRenderer>();

            MeshFilter sourceMeshFilter = capsulePrefab.GetComponentInChildren<MeshFilter>();
            MeshRenderer sourceRenderer = capsulePrefab.GetComponentInChildren<MeshRenderer>();

            if (sourceMeshFilter != null && targetMeshFilter != null)
            {
                targetMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
            }

            if (sourceRenderer != null && targetRenderer != null)
            {
                targetRenderer.sharedMaterials = sourceRenderer.sharedMaterials;
            }

            ballTransform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        }

        // 스탠드에 있는 스크립트를 통해 숫자와 짝수/홀수 데이터 설정
        CapsuleOpen capsuleScript = targetStand.GetComponentInChildren<CapsuleOpen>();
        if (capsuleScript != null)
        {
            capsuleScript.SetupCapsule(isEven, number);
        }

        Debug.Log($"스탠드 번호: {currentIndex}, 숫자: {number}, 짝수여부: {isEven}");
    }
}