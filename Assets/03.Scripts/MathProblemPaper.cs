using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MathProblemPaper : MonoBehaviour
{
    public static MathProblemPaper Instance;

    [System.Serializable]
    public class ProblemUIGroup
    {
        public TextMeshProUGUI problemText;
        public TextMeshProUGUI[] slotTexts;
    }

    [Header("UI Reference")]
    [SerializeField] private ProblemUIGroup[] problemUIgroups;

    [Header("--- 결과 패널 ---")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("--- 씬 이동 및 설정 ---")]
    [SerializeField] private string stageSceneName = "StageScene";
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private float delayTime = 3.0f;

    public GameObject circleMarkObj;
    public GameObject crossMarkObj;

    [System.Serializable]
    public struct ProblemData
    {
        public int operand1;
        public int operand2;
        public int correctAnswer;
        public int?[] slots;
        public GameObject[] slotPlacedObjects;

        public ProblemData(int a, int b)
        {
            operand1 = a;
            operand2 = b;
            correctAnswer = a * b;

            int count = 1;
            if (correctAnswer >= 100) count = 3;
            else if (correctAnswer >= 10) count = 2;

            slots = new int?[count];
            slotPlacedObjects = new GameObject[count];
        }

        public bool IsComplete
        {
            get
            {
                foreach (var s in slots)
                {
                    if (!s.HasValue) return false;
                }
                return true;
            }
        }

        public int? GetPlayerAnswerValue()
        {
            if (!IsComplete) return null;

            int val = 0;
            foreach (var s in slots)
            {
                if (!s.HasValue) return null;
                val = val * 10 + s.Value;
            }
            return val;
        }
    }

    [SerializeField] private List<ProblemData> currentProblems = new List<ProblemData>();
    private int requiredProblemCount = 1;

    private int previewProblemIndex = -1;
    private int previewSlotIndex = -1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (circleMarkObj != null) circleMarkObj.SetActive(false);
        if (crossMarkObj != null) crossMarkObj.SetActive(false);

        int stage = (GameManager.Instance != null) ? GameManager.Instance.currentStage : 1;

        if (stage == 1) requiredProblemCount = 1;
        else if (stage == 2) requiredProblemCount = 1;
        else if (stage >= 3 && stage <= 5) requiredProblemCount = 1;
        else if (stage >= 6 && stage <= 8) requiredProblemCount = 2;
        else requiredProblemCount = 3;

        GenerateProblemsStrict(stage);
    }

    public void GenerateProblemsStrict(int currentStage)
    {
        bool isStageValid = false;
        List<ProblemData> generatedList = new List<ProblemData>();

        while (!isStageValid)
        {
            generatedList.Clear();
            List<ProblemData> stageGeneratedProblems = new List<ProblemData>();
            bool batchSuccess = true;

            for (int i = 0; i < requiredProblemCount; i++)
            {
                int num1 = 1, num2 = 1;
                int correctAnswer = 0;
                bool isValid = false;
                int safetyCounter = 0;

                while (!isValid)
                {
                    safetyCounter++;
                    if (safetyCounter > 1000) { batchSuccess = false; break; }

                    if (currentStage == 1)
                    {
                        num1 = Random.Range(1, 4);
                        num2 = Random.Range(2, 4);
                    }
                    else if (currentStage == 2)
                    {
                        num1 = Random.Range(6, 10);
                        num2 = Random.Range(6, 10);
                    }
                    else if (currentStage >= 3 && currentStage <= 5)
                    {
                        num1 = Random.Range(8, 20);
                        num2 = Random.Range(8, 20);
                    }
                    else if (currentStage >= 6 && currentStage <= 8)
                    {
                        num1 = Random.Range(5, 12);
                        num2 = Random.Range(5, 12);
                    }
                    else
                    {
                        num1 = Random.Range(6, 13);
                        num2 = Random.Range(6, 13);
                    }

                    correctAnswer = num1 * num2;

                    if (!CheckNumberFrequencyValid(correctAnswer)) continue;

                    if (currentStage == 1 && correctAnswer >= 10) continue;
                    if (currentStage == 2 && (correctAnswer < 10 || correctAnswer >= 100)) continue;
                    if (currentStage >= 3 && correctAnswer < 10) continue;

                    bool isDuplicate = false;
                    foreach (var p in stageGeneratedProblems)
                    {
                        if ((p.operand1 == num1 && p.operand2 == num2) || (p.operand1 == num2 && p.operand2 == num1))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (isDuplicate) continue;

                    isValid = true;
                }

                if (!batchSuccess) break;

                ProblemData newProblem = new ProblemData(num1, num2);
                stageGeneratedProblems.Add(newProblem);
            }

            if (batchSuccess)
            {
                if (CheckStageTotalDigitFrequencyValid(stageGeneratedProblems))
                {
                    generatedList = stageGeneratedProblems;
                    isStageValid = true;
                }
            }
        }

        currentProblems = generatedList;
        UpdateAllProblemUI();
        SetGamePlayState(true);
    }

    private bool CheckNumberFrequencyValid(int answer)
    {
        string answerStr = answer.ToString();
        Dictionary<char, int> digitCounts = new Dictionary<char, int>();

        foreach (char c in answerStr)
        {
            if (!digitCounts.ContainsKey(c))
            {
                digitCounts[c] = 0;
            }
            digitCounts[c]++;

            if (digitCounts[c] >= 3)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckStageTotalDigitFrequencyValid(List<ProblemData> problems)
    {
        Dictionary<char, int> totalDigitCounts = new Dictionary<char, int>();

        foreach (var p in problems)
        {
            string answerStr = p.correctAnswer.ToString();
            foreach (char c in answerStr)
            {
                if (!totalDigitCounts.ContainsKey(c))
                {
                    totalDigitCounts[c] = 0;
                }
                totalDigitCounts[c]++;

                if (totalDigitCounts[c] > 2)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void UpdateAllProblemUI()
    {
        for (int i = 0; i < problemUIgroups.Length; i++)
        {
            var group = problemUIgroups[i];
            if (group == null || group.problemText == null) continue;

            if (i < currentProblems.Count)
            {
                group.problemText.gameObject.SetActive(true);
                var p = currentProblems[i];

                group.problemText.fontSize = 14f;
                group.problemText.textWrappingMode = TextWrappingModes.NoWrap;
                group.problemText.text = $"{p.operand1} × {p.operand2} =";

                for (int d = 0; d < group.slotTexts.Length; d++)
                {
                    if (group.slotTexts[d] == null) continue;

                    if (d < p.slots.Length)
                    {
                        group.slotTexts[d].gameObject.SetActive(true);
                        group.slotTexts[d].fontSize = 14f;
                        group.slotTexts[d].textWrappingMode = TextWrappingModes.NoWrap;

                        if (i == previewProblemIndex && d == previewSlotIndex)
                        {
                            group.slotTexts[d].text = "<color=red><b>?</b></color>";
                        }
                        else if (p.slots[d].HasValue)
                        {
                            group.slotTexts[d].text = $"<color=red>{p.slots[d].Value}</color>";
                        }
                        else
                        {
                            group.slotTexts[d].text = "?";
                        }
                    }
                    else
                    {
                        group.slotTexts[d].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                group.problemText.gameObject.SetActive(false);
                if (group.slotTexts != null)
                {
                    foreach (var slot in group.slotTexts)
                    {
                        if (slot != null) slot.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // ?? 3D 거리 계산 대신 UI RectTransform 영역(ScreenPoint) 포함 여부로 변경
    public void UpdateDragPreview(Vector3 currentPosition)
    {
        int bestProblemIndex = -1;
        int bestSlotIndex = -1;
        Camera cam = Camera.main;
        if (cam == null) return;

        // 드래그 중인 오브젝트의 월드 좌표를 화면 스크린 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, currentPosition);

        for (int i = 0; i < currentProblems.Count; i++)
        {
            if (i >= problemUIgroups.Length) break;
            var group = problemUIgroups[i];
            var p = currentProblems[i];

            for (int d = 0; d < p.slots.Length; d++)
            {
                if (d >= group.slotTexts.Length || group.slotTexts[d] == null) continue;
                if (!group.slotTexts[d].gameObject.activeSelf) continue;

                RectTransform rectTransform = group.slotTexts[d].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // 슬롯 UI의 사각형 영역 안에 마우스/숫자의 스크린 좌표가 포함되는지 검사 (약간의 여유 패딩 30픽셀 부여 가능)
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPos, cam))
                    {
                        bestProblemIndex = i;
                        bestSlotIndex = d;
                        break; // 정확히 들어온 슬롯을 찾으면 즉시 중단
                    }
                }
            }
            if (bestProblemIndex != -1) break;
        }

        if (bestProblemIndex != -1 && bestSlotIndex != -1)
        {
            if (previewProblemIndex != bestProblemIndex || previewSlotIndex != bestSlotIndex)
            {
                previewProblemIndex = bestProblemIndex;
                previewSlotIndex = bestSlotIndex;
                UpdateAllProblemUI();
            }
        }
        else
        {
            ClearDragPreview();
        }
    }

    public void ClearDragPreview()
    {
        if (previewProblemIndex != -1 || previewSlotIndex != -1)
        {
            previewProblemIndex = -1;
            previewSlotIndex = -1;
            UpdateAllProblemUI();
        }
    }

    public bool TryDropNumberToNearestSlot(Vector3 dropPosition, int number, GameObject draggedObject)
    {
        int bestProblemIndex = -1;
        int bestSlotIndex = -1;
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector2 dropScreenPos = RectTransformUtility.WorldToScreenPoint(cam, dropPosition);
        ClearDragPreview();

        for (int i = 0; i < currentProblems.Count; i++)
        {
            if (i >= problemUIgroups.Length) break;
            var group = problemUIgroups[i];
            var p = currentProblems[i];

            for (int d = 0; d < p.slots.Length; d++)
            {
                if (d >= group.slotTexts.Length || group.slotTexts[d] == null) continue;
                if (!group.slotTexts[d].gameObject.activeSelf) continue;

                RectTransform rectTransform = group.slotTexts[d].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, dropScreenPos, cam))
                    {
                        bestProblemIndex = i;
                        bestSlotIndex = d;
                        break;
                    }
                }
            }
            if (bestProblemIndex != -1) break;
        }

        if (bestProblemIndex != -1 && bestSlotIndex != -1)
        {
            var p = currentProblems[bestProblemIndex];

            if (p.slots[bestSlotIndex].HasValue && p.slotPlacedObjects[bestSlotIndex] != null)
            {
                GameObject oldObj = p.slotPlacedObjects[bestSlotIndex];

                DraggableNumberToText draggableComp = oldObj.GetComponent<DraggableNumberToText>();
                if (draggableComp != null)
                {
                    oldObj.transform.position = draggableComp.originalPosition;
                }
                oldObj.SetActive(true);
            }

            p.slots[bestSlotIndex] = number;
            p.slotPlacedObjects[bestSlotIndex] = draggedObject;
            draggedObject.SetActive(false);

            currentProblems[bestProblemIndex] = p;

            UpdateAllProblemUI();
            return true;
        }

        return false;
    }

    public void OnSubmitButtonClicked()
    {
        foreach (var p in currentProblems)
        {
            if (!p.IsComplete) return;
        }

        CheckAllAnswers();
    }

    private void CheckAllAnswers()
    {
        bool allCorrect = true;
        foreach (var p in currentProblems)
        {
            int? finalAns = p.GetPlayerAnswerValue();
            if (!finalAns.HasValue || finalAns.Value != p.correctAnswer)
            {
                allCorrect = false;
                break;
            }
        }

        StartCoroutine(PlayMarkAnimationAndShowUI(allCorrect));
    }

    private IEnumerator PlayMarkAnimationAndShowUI(bool isCorrect)
    {
        GameObject targetMark = isCorrect ? circleMarkObj : crossMarkObj;
        if (targetMark != null)
        {
            targetMark.SetActive(true);
        }

        yield return new WaitForSeconds(0.8f);

        SetGamePlayState(false);
        Time.timeScale = 0f;

        if (isCorrect)
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(delayTime);
            Time.timeScale = 1f;

            if (GameManager.Instance.currentStage >= 10)
            {
                SceneManager.LoadScene("EndingScene");
            }
            else
            {
                GameManager.Instance.currentStage++;
                SceneManager.LoadScene(stageSceneName);
            }
        }
        else
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(delayTime);
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
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
}