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

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Mathf.Sin(elapsed * 60f) * 0.04f;
            ballTransform.localPosition = originalPos + new Vector3(offsetX, 0, 0);
            yield return null;
        }
        ballTransform.localPosition = originalPos;

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

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        TriggerRedNumberPopup(ballTransform.position);
    }

    private void TriggerRedNumberPopup(Vector3 worldPos)
    {
        Vector3 spawnPos = worldPos + new Vector3(0, 0.3f, 0);

        GameObject textObj = new GameObject("NumberPopup");
        textObj.transform.position = spawnPos;

        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = capsuleNumber.ToString();
        tmp.fontSize = 5;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.red;

        if (Camera.main != null)
        {
            textObj.transform.rotation = Camera.main.transform.rotation;
        }

        textObj.AddComponent<DraggableNumberToText>();

        StartCoroutine(PopupAnimationRoutine(textObj));
    }

    private IEnumerator PopupAnimationRoutine(GameObject targetObj)
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * 0.4f;

        targetObj.transform.localScale = startScale;

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