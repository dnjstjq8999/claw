using UnityEngine;

public class CapsuleData : MonoBehaviour
{
    [Header("--- 공 속성 ---")]
    public bool isEven;       // true: 짝수(노란색), false: 홀수(파란색)
    public int capsuleNumber; // 캡슐 안에 들어있는 숫자

    /// <summary>
    /// 외부(스폰 매니저)에서 색상과 숫자를 정확히 지정해 줍니다.
    /// </summary>
    public void SetCapsuleData(bool even, int number)
    {
        isEven = even;
        capsuleNumber = number;

        // 태그도 안전하게 자동 동기화
        gameObject.tag = isEven ? "YellowCapsule" : "BlueCapsule";

        Debug.Log($"[캡슐 설정 완료] 색상: {(isEven ? "노란색(짝수)" : "파란색(홀수)")} / 내부 숫자: {capsuleNumber}");
    }
}