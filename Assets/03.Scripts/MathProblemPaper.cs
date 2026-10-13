using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MathProblemPaper : MonoBehaviour
{
    public static MathProblemPaper Instance;

    [Header("UI Reference")]
    [Tooltip("테이블 위 종이에 문제를 표시할 텍스트 컴포넌트")]
    [SerializeField] private TextMeshProUGUI problemText;

    [Header("Current Problem Data")]
    public int operand1;
    public int operand2;
    public int correctAnswer;

    // 곱한 결과가 1자리 수(9 이하)인 조합들을 담을 구조체와 리스트
    private struct ProblemPair
    {
        public int num1;
        public int num2;
        public ProblemPair(int a, int b) { num1 = a; num2 = b; }
    }

    private List<ProblemPair> validProblems = new List<ProblemPair>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 게임 시작 시 결과가 1자리인 곱셈 조합을 미리 세팅
        InitializeValidProblems();
    }

    void Start()
    {
        GenerateNewProblem();
    }

    private void InitializeValidProblems()
    {
        validProblems.Clear();

        for (int i = 1; i <= 9; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                if (i * j <= 9) // 결과가 1자리(9 이하)인 경우만 추가
                {
                    validProblems.Add(new ProblemPair(i, j));
                }
            }
        }
    }

 
    public void GenerateNewProblem()
    {
        if (validProblems.Count == 0) InitializeValidProblems();

        // 리스트에서 무작위로 하나 선택
        int randomIndex = Random.Range(0, validProblems.Count);
        ProblemPair selected = validProblems[randomIndex];

        operand1 = selected.num1;
        operand2 = selected.num2;
        correctAnswer = operand1 * operand2;

        // 문제 텍스트 갱신 (예: "2 × 4 = ?")
        if (problemText != null)
        {
            problemText.text = $"{operand1} × {operand2} = ?";
        }

        Debug.Log($"[스테이지 1 문제 출제] {operand1} × {operand2} = ? (정답: {correctAnswer})");
    }

    public void CheckAnswer(int playerAnswer)
    {
        if (playerAnswer == correctAnswer)
        {
            Debug.Log("<color=green>[정답!]</color> 축하합니다! 정답입니다.");
            // 나중에 정답 맞췄을 때의 연출(다음 문제 출제 등)을 여기에 넣으면 됩니다.
        }
        else
        {
            Debug.Log("<color=red>[오답]</color> 다시 시도해보세요.");
        }
    }

}