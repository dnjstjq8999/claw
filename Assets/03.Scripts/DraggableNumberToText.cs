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
                transform.position = hitPoint;

                if (MathProblemPaper.Instance != null)
                {
                    MathProblemPaper.Instance.UpdateDragPreview(transform.position);
                }
            }
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;

            if (MathProblemPaper.Instance != null)
            {
                MathProblemPaper.Instance.ClearDragPreview();
            }

            CheckDropZone();
        }
    }

    private void CheckDropZone()
    {
        if (MathProblemPaper.Instance != null)
        {
            bool success = MathProblemPaper.Instance.TryDropNumberToNearestSlot(transform.position, myNumber, gameObject);

            if (success)
            {
                return;
            }
        }

        transform.position = originalPosition;
    }
}