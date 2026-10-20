using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class FactoryButtonController : MonoBehaviour
{
    [Header("버튼 움직임 설정")]
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

        if (MathProblemPaper.Instance != null)
        {
            MathProblemPaper.Instance.OnSubmitButtonClicked();
        }

        yield return new WaitForSeconds(0.15f);

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