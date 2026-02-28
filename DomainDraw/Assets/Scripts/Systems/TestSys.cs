using UnityEngine;
using UnityEngine.InputSystem;   // ADD THIS

public class TestSys : MonoBehaviour
{
    [SerializeField] private HandView handView;
    [SerializeField] private CardData cardData;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Card card = new(cardData);
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(cardView));
        }
    }
}