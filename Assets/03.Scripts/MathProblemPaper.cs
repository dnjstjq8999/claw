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

    // 플레이어가 드래그해서 넣은 임시 답안과 그때의 원래 위치 정보
    private int? currentPlayerAnswer = null;
    private Vector3 lastOriginalPosition; // 이전에 넣었던 숫자의 원래 자리

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
                if (i * j <= 9)
                {
                    validProblems.Add(new ProblemPair(i, j));
                }
            }
        }
    }

    public void GenerateNewProblem()
    {
        if (validProblems.Count == 0) InitializeValidProblems();

        int randomIndex = Random.Range(0, validProblems.Count);
        ProblemPair selected = validProblems[randomIndex];

        operand1 = selected.num1;
        operand2 = selected.num2;
        correctAnswer = operand1 * operand2;

        currentPlayerAnswer = null;
        UpdateProblemUI();

        Debug.Log($"[스테이지 1 문제 출제] {operand1} × {operand2} = ? (정답: {correctAnswer})");
    }

    /// <summary>
    /// 숫자를 종이에 놓았을 때 호출 (원래 위치 정보도 함께 받음)
    /// </summary>
    public void SetPlayerAnswer(int answer, Vector3 spawnPosition)
    {
        // 만약 이미 다른 숫자가 들어가 있었다면, 그 숫자를 '원래 있던 자리'로 되돌려 보냄
        if (currentPlayerAnswer.HasValue)
        {
            int oldAnswer = currentPlayerAnswer.Value;
            SpawnReturnedNumber(oldAnswer, lastOriginalPosition);
            Debug.Log($"[숫자 교체] 기존에 있던 {oldAnswer}가 원래 자리로 돌아갑니다.");
        }

        currentPlayerAnswer = answer;
        lastOriginalPosition = spawnPosition; // 이번에 넣은 숫자의 원래 위치를 백업해 둠
        UpdateProblemUI();
        Debug.Log($"[답안 임시 입력] {answer}");
    }

    /// <summary>
    /// 교체되어 빠져나온 숫자를 본래의 캡슐 위치로 되돌려주는 함수
    /// </summary>
    private void SpawnReturnedNumber(int numberValue, Vector3 returnPos)
    {
        GameObject textObj = new GameObject("ReturnedNumber");
        textObj.transform.position = returnPos; // 정확히 처음에 튀어나왔던 그 위치로 설정!

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = numberValue.ToString();
        tmp.fontSize = 5;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.red;

        if (Camera.main != null)
        {
            textObj.transform.rotation = Camera.main.transform.rotation;
        }

        // 다시 드래그할 수 있도록 컴포넌트 부착
        DraggableNumberToText draggable = textObj.AddComponent<DraggableNumberToText>();
        draggable.originalPosition = returnPos; // 원래 자리 지정
    }

    private void UpdateProblemUI()
    {
        if (problemText != null)
        {
            string answerStr = currentPlayerAnswer.HasValue ? $"<color=red>{currentPlayerAnswer.Value}</color>" : "?";
            problemText.text = $"{operand1} × {operand2} = {answerStr}";
        }
    }

    public void OnSubmitButtonClicked()
    {
        if (!currentPlayerAnswer.HasValue)
        {
            Debug.Log("<color=yellow>[경고]</color> 숫자를 먼저 드래그해서 넣어주세요!");
            return;
        }

        CheckAnswer(currentPlayerAnswer.Value);
    }

    public void CheckAnswer(int playerAnswer)
    {
        if (playerAnswer == correctAnswer)
        {
            Debug.Log("<color=green>[정답!]</color> 축하합니다! 정답입니다.");
        }
        else
        {
            Debug.Log("<color=red>[오답]</color> 다시 시도해보세요.");
        }
    }
}