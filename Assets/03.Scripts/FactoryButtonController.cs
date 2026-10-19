using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class FactoryButtonController : MonoBehaviour
{
    [Header("버튼 움직임 설정")]
    [Tooltip("안으로 들어갈 빨간색 버튼 윗부분 (buttonredtop-1)")]
    [SerializeField] private Transform buttonTop;

    [Header("조절 가능한 버튼 깊이 & 속도")]
    [SerializeField] private float pressDepth = 0.15f;
    [SerializeField] private float moveSpeed = 20f;

    [Header("상호작용 설정")]
    [SerializeField] private float interactionDistance = 3.0f;

    [Header("버튼 전용 UI 텍스트 연동")]
    [SerializeField] private GameObject factoryPromptUI;

    private Vector3 initialLocalPos;
    private bool isPressed = false;

    void Start()
    {
        if (buttonTop != null)
        {
            initialLocalPos = buttonTop.localPosition;
        }

        if (factoryPromptUI != null)
        {
            factoryPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Camera.main == null || isPressed) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        bool isLookingAtButton = false;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.transform.IsChildOf(transform))
            {
                isLookingAtButton = true;

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    StartCoroutine(PressButtonRoutine());
                }
            }
        }

        if (factoryPromptUI != null)
        {
            factoryPromptUI.SetActive(isLookingAtButton);
        }
    }

    private IEnumerator PressButtonRoutine()
    {
        isPressed = true;

        if (factoryPromptUI != null)
        {
            factoryPromptUI.SetActive(false);
        }

        // [핵심] X축 방향으로 누르는 연출입니다.
        // 만약 이 상태에서 바깥(오른쪽)으로 튀어나온다면, 아래의 '+ pressDepth'를 '- pressDepth'로 바꿔주세요!
        if (buttonTop != null)
        {
            Vector3 targetPos = initialLocalPos + new Vector3(pressDepth, 0, 0);

            float elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                buttonTop.localPosition = Vector3.Lerp(buttonTop.localPosition, targetPos, elapsed * moveSpeed);
                yield return null;
            }
            buttonTop.localPosition = targetPos;
        }

        // 채점 및 결과 처리 호출
        if (MathProblemPaper.Instance != null)
        {
            MathProblemPaper.Instance.OnSubmitButtonClicked();
        }

        yield return new WaitForSeconds(0.15f);

        // 원래 자리로 복구
        if (buttonTop != null)
        {
            float elapsed = 0f;
            while (elapsed < 0.12f)
            {
                elapsed += Time.deltaTime;
                buttonTop.localPosition = Vector3.Lerp(buttonTop.localPosition, initialLocalPos, elapsed * moveSpeed);
                yield return null;
            }
            buttonTop.localPosition = initialLocalPos;
        }

        isPressed = false;
    }
}