using UnityEngine;

public class CapsuleData : MonoBehaviour
{
    [Header("--- 공 속성 ---")]
    public bool isEven;       // true: 짝수(노란색), false: 홀수(파란색)
    public int capsuleNumber; // 캡슐 안에 들어있는 0~9 사이의 숫자

    void Start()
    {
        InitializeCapsule();
    }

    /// <summary>
    /// 공의 색상에 맞춰 내부에 0~9 사이의 홀수/짝수 데이터 부여
    /// </summary>
    public void InitializeCapsule()
    {
        if (gameObject.CompareTag("YellowCapsule"))
        {
            isEven = true; // 짝수
            int[] evenNumbers = { 0, 2, 4, 6, 8 };
            capsuleNumber = evenNumbers[Random.Range(0, evenNumbers.Length)];
        }
        else if (gameObject.CompareTag("BlueCapsule"))
        {
            isEven = false; // 홀수
            int[] oddNumbers = { 1, 3, 5, 7, 9 };
            capsuleNumber = oddNumbers[Random.Range(0, oddNumbers.Length)];
        }

        Debug.Log($"[캡슐 생성] 색상: {(isEven ? "노란색(짝수)" : "파란색(홀수)")} / 내부 숫자: {capsuleNumber}");
    }
}