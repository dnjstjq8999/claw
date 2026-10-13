using UnityEngine;

public class CapsuleData : MonoBehaviour
{
    [Header("--- 공 속성 ---")]
    public int capsuleNumber;
    public bool isEven;

    // StageCapsuleManager에서 호출하여 숫자를 세팅해주는 함수
    public void SetCapsuleData(bool isYellow, int number)
    {
        capsuleNumber = number;
        isEven = (capsuleNumber % 2 == 0);

        Debug.Log($"[캡슐 설정 완료] 숫자: {capsuleNumber} (짝수: {isEven})");
    }
}