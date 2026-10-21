using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterRotator : MonoBehaviour, IDragHandler
{
    public Transform targetCharacter;
    public float rotationSpeed = 5f;

    public void OnDrag(PointerEventData eventData)
    {
        if (targetCharacter != null)
        {
            float xRotation = -eventData.delta.x * rotationSpeed * Time.deltaTime;
            targetCharacter.Rotate(Vector3.up, xRotation, Space.World);
        }
    }
}