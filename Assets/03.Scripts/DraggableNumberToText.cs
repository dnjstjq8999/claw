using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DraggableNumberToText : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private Plane targetPlane;
    public Vector3 originalPosition;
    public int myNumber;

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;

        TextMeshPro tmp = GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            int.TryParse(tmp.text, out myNumber);
        }

        targetPlane = new Plane(Vector3.up, transform.position);

        BoxCollider boxCol = GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            boxCol = gameObject.AddComponent<BoxCollider>();
        }

        boxCol.size = new Vector3(1.5f, 1.5f, 0.2f);
        boxCol.isTrigger = true;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                DraggableNumberToText clickedNumber = hit.collider.GetComponent<DraggableNumberToText>();
                if (clickedNumber == this)
                {
                    isDragging = true;
                }
            }
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            float enter;

            if (targetPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint + new Vector3(0, 0.15f, 0);
            }
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            CheckDropZone();
        }
    }

    private void CheckDropZone()
    {
        if (MathProblemPaper.Instance != null)
        {
            Vector3 paperPos = MathProblemPaper.Instance.transform.position;
            float distance = Vector3.Distance(new Vector3(transform.position.x, paperPos.y, transform.position.z), paperPos);

            if (distance < 1.0f) // ³Ë³ËÇÑ ±âº» ÀÎ½Ä °Å¸®
            {
                MathProblemPaper.Instance.SetPlayerAnswer(myNumber, originalPosition);
                Destroy(gameObject);
                return;
            }
        }

        transform.position = originalPosition;
    }
}