using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MathProblemPaper : MonoBehaviour
{
    public static MathProblemPaper Instance;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI problemText;

    [Header("결과 팝업 UI 패널 설정")]
    public GameObject resultUIPanel;
    public TextMeshProUGUI resultMessageText;
    public Button nextStageButton;
    public Button mainMenuButton;

    [Header("종이 위 마크 (동그라미 / 빗금)")]
    public GameObject circleMarkObj;
    public GameObject crossMarkObj;

    [Header("Current Problem Data")]
    public int operand1;
    public int operand2;
    public int correctAnswer;

    private int? currentPlayerAnswer = null;
    private Vector3 lastOriginalPosition;

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
        if (resultUIPanel != null) resultUIPanel.SetActive(false);
        if (circleMarkObj != null) circleMarkObj.SetActive(false);
        if (crossMarkObj != null) crossMarkObj.SetActive(false);

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

        if (resultUIPanel != null) resultUIPanel.SetActive(false);
        if (circleMarkObj != null) circleMarkObj.SetActive(false);
        if (crossMarkObj != null) crossMarkObj.SetActive(false);

        // 새 문제 출제 시 게임 플레이 상태로 복구 (시점 이동 켜기)
        SetGamePlayState(true);

        UpdateProblemUI();
    }

    public void SetPlayerAnswer(int answer, Vector3 spawnPosition)
    {
        if (currentPlayerAnswer.HasValue)
        {
            int oldAnswer = currentPlayerAnswer.Value;
            SpawnReturnedNumber(oldAnswer, lastOriginalPosition);
        }

        currentPlayerAnswer = answer;
        lastOriginalPosition = spawnPosition;
        UpdateProblemUI();
    }

    private void SpawnReturnedNumber(int numberValue, Vector3 returnPos)
    {
        GameObject textObj = new GameObject("ReturnedNumber");
        textObj.transform.position = returnPos;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = numberValue.ToString();
        tmp.fontSize = 5;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.red;

        if (Camera.main != null)
        {
            textObj.transform.rotation = Camera.main.transform.rotation;
        }

        DraggableNumberToText draggable = textObj.AddComponent<DraggableNumberToText>();
        draggable.originalPosition = returnPos;
    }

    private void UpdateProblemUI()
    {
        if (problemText != null)
        {
            string answerStr = currentPlayerAnswer.HasValue ? $"<color=red>{currentPlayerAnswer.Value}</color>" : "?";
            problemText.text = $"{operand1} × {operand2} = {answerStr}";
        }
    }

    public bool TryDropNumber(Vector3 dropPosition, int number, Vector3 spawnPos)
    {
        Vector3 paperPos = transform.position;
        float distX = dropPosition.x - paperPos.x;
        float distZ = dropPosition.z - paperPos.z;

        if (Mathf.Abs(distX) < 0.8f && Mathf.Abs(distZ) < 0.6f)
        {
            SetPlayerAnswer(number, spawnPos);
            return true;
        }

        return false;
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
        if (resultUIPanel != null) resultUIPanel.SetActive(false);

        bool isCorrect = (playerAnswer == correctAnswer);

        if (isCorrect)
        {
            if (crossMarkObj != null) crossMarkObj.SetActive(false);
            if (circleMarkObj != null) StartCoroutine(PlayMarkAnimationAndShowUI(circleMarkObj, true));
        }
        else
        {
            if (circleMarkObj != null) circleMarkObj.SetActive(false);
            if (crossMarkObj != null) StartCoroutine(PlayMarkAnimationAndShowUI(crossMarkObj, false));
        }
    }

    /// <summary>
    /// 종이 위에 O/X 연출을 먼저 보여준 뒤, 결과 UI를 띄우는 코루틴 (애니메이션 스케일 보완)
    /// </summary>
    private IEnumerator PlayMarkAnimationAndShowUI(GameObject targetMark, bool isCorrect)
    {
        targetMark.SetActive(true);
        Transform markTransform = targetMark.transform;

        // [중요] 오브젝트 원래의 스케일을 기억해 둡니다 (유니티 인스펙터에서 설정한 크기 유지)
        Vector3 originalScale = markTransform.localScale;

        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startScale = Vector3.zero;
        markTransform.localScale = startScale;

        // 1. O 또는 X 마크가 0에서 원래 크기(originalScale)로 팡 커지는 연출 재생
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 부드러운 연출을 위해 서서히 커지게 설정
            markTransform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }
        markTransform.localScale = originalScale;

        // 2. 연출이 끝난 후 0.4초 대기
        yield return new WaitForSeconds(0.4f);

        // 3. 결과 UI 활성화 및 텍스트/버튼 설정
        if (resultUIPanel != null) resultUIPanel.SetActive(true);

        if (isCorrect)
        {
            if (resultMessageText != null) resultMessageText.text = "CLEAR!";
            if (nextStageButton != null) nextStageButton.gameObject.SetActive(true);
            if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(true);
        }
        else
        {
            if (resultMessageText != null) resultMessageText.text = "FAIL!";
            if (nextStageButton != null) nextStageButton.gameObject.SetActive(false);
            if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(true);
        }

        // 4. 결과 UI가 떴으므로 캐릭터 시점 회전 정지 및 마우스 해제
        SetGamePlayState(false);
    }

    /// <summary>
    /// 게임 플레이 중일 때는 시점 회전을 켜고, 결과 UI가 떴을 때는 시점 회전을 끄는 함수
    /// </summary>
    private void SetGamePlayState(bool isPlaying)
    {
        if (isPlaying)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // 플레이어 시점 이동 스크립트 활성화 (사용 중이신 스크립트 이름으로 켜기)
            // 예시: PlayerController 또는 MouseLook 컴포넌트 찾아서 켜주기
            var playerScript = FindFirstObjectByType<PlayerController>();
            if (playerScript != null) playerScript.enabled = true;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // 플레이어 시점 이동 스크립트 비활성화
            var playerScript = FindFirstObjectByType<PlayerController>();
            if (playerScript != null) playerScript.enabled = false;
        }
    }

    public void OnClickNextStage()
    {
        GenerateNewProblem();
    }

    public void OnClickMainMenu()
    {
        Debug.Log("메인 메뉴로 이동!");
    }
}