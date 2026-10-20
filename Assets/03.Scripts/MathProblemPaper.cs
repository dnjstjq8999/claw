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

    private IEnumerator PlayMarkAnimationAndShowUI(GameObject targetMark, bool isCorrect)
    {
        targetMark.SetActive(true);
        Transform markTransform = targetMark.transform;

        Vector3 originalScale = markTransform.localScale;

        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startScale = Vector3.zero;
        markTransform.localScale = startScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            markTransform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }
        markTransform.localScale = originalScale;

        yield return new WaitForSeconds(0.4f);

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

        SetGamePlayState(false);
    }


    private void SetGamePlayState(bool isPlaying)
    {
        if (isPlaying)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var playerScript = FindFirstObjectByType<PlayerController>();
            if (playerScript != null) playerScript.enabled = true;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

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