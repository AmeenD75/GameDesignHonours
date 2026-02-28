using UnityEngine;
using UnityEngine.InputSystem;

public class CardHoverRaycaster : MonoBehaviour
{
    private CardView current;

    void Update()
    {
        if (Camera.main == null || Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));

        RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

        CardView hovered = hit.collider ? hit.collider.GetComponentInParent<CardView>() : null;

        if (hovered == current) return;

        // exit old
        if (current != null) current.HoverExit();

        current = hovered;

        // enter new
        if (current != null) current.HoverEnter();
    }
}