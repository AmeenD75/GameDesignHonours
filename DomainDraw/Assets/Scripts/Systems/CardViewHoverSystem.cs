using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    [SerializeField] private CardView cardViewHover;
    [SerializeField] private Vector3 hoverOffset;


    public void Show(Card card, Vector3 position)
    {
        cardViewHover.gameObject.SetActive(true);
        cardViewHover.Setup(card);
        cardViewHover.transform.position = position + hoverOffset;

    }

    public void Hide()
    {
        cardViewHover.gameObject.SetActive(false);
    }
}
