using UnityEngine;
using TMPro;
using System.Collections;

public class CapsuleOpen : MonoBehaviour
{
    [Header("캡슐 데이터")]
    public int capsuleNumber;
    public bool isEven;

    [Header("더블클릭 설정")]
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;
    private bool isInteracted = false;

    public void SetupCapsule(bool even, int num)
    {
        isEven = even;
        capsuleNumber = num;
        isInteracted = false;

        // 공이 다시 세팅될 때 렌더러와 콜라이더 복구
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = true;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }

    public void OnPlayerClicked()
    {
        if (isInteracted) return;

        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            // ★ 더블클릭 성공! 흔들림 + 사라짐 + 빨간색 숫자 팝업
            isInteracted = true;
            StartCoroutine(ShakeAndDisappearRoutine());
            lastClickTime = 0f;
        }
        else
        {
            lastClickTime = Time.time;
        }
    }

    private IEnumerator ShakeAndDisappearRoutine()
    {
        Transform ballTransform = transform;

        Vector3 originalPos = ballTransform.localPosition;
        float shakeDuration = 0.4f;
        float elapsed = 0f;

        // 1. 좌우 흔들림 연출
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Mathf.Sin(elapsed * 60f) * 0.04f;
            ballTransform.localPosition = originalPos + new Vector3(offsetX, 0, 0);
            yield return null;
        }
        ballTransform.localPosition = originalPos;

        // 2. 서서히 작아지며 사라지는 연출
        float shrinkDuration = 0.3f;
        elapsed = 0f;
        Vector3 startScale = ballTransform.localScale;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkDuration;
            ballTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        ballTransform.localScale = Vector3.zero;

        // 공의 렌더러와 콜라이더 끄기
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 3. 선명한 빨간색 숫자 팝업 생성 (사라지지 않고 영구 유지)
        TriggerRedNumberPopup(ballTransform.position);
    }

    private void TriggerRedNumberPopup(Vector3 worldPos)
    {
        Vector3 spawnPos = worldPos + new Vector3(0, 0.3f, 0);

        GameObject textObj = new GameObject("NumberPopup");
        textObj.transform.position = spawnPos;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = capsuleNumber.ToString();
        tmp.fontSize = 5; // 숫자가 적당히 보이도록 설정
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.red; // 무조건 선명한 빨간색

        if (Camera.main != null)
        {
            textObj.transform.rotation = Camera.main.transform.rotation;
        }

        StartCoroutine(PopupAnimationRoutine(textObj));
    }

    private IEnumerator PopupAnimationRoutine(GameObject targetObj)
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * 0.4f;

        targetObj.transform.localScale = startScale;

        // 텍스트가 뿅 커지는 연출만 수행하고 끝 (Destroy 없음)
        while (elapsed < 0.25f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.25f;
            targetObj.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        targetObj.transform.localScale = targetScale;
    }
}