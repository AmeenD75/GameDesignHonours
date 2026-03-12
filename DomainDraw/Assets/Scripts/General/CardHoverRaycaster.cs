using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CardHoverRaycaster : MonoBehaviour
{
    private CardView current;

    void Update()
    {
        if (Camera.main == null || Mouse.current == null) return;


        // IMPORTANT: ignore clicks if mouse is over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (current != null)
            {
                current.HoverExit();
                current = null;
            }
            return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        world.z = 0f;

        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

        CardView hovered = hit.collider ? hit.collider.GetComponentInParent<CardView>() : null;

        if (hovered != current)
        {
            if (current != null) current.HoverExit();

            current = hovered;

            if (current != null) current.HoverEnter();
        }

        // Click handling
        if (current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Clicked card: " + current.Card.Title);
            GameManager.Instance.TryPlayCard(current);
        }
    }
}