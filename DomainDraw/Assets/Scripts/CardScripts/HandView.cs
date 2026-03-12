using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    private readonly List<CardView> cards = new();

    [SerializeField] private Vector3 handCenter = new Vector3(0f, -3.5f, 0f);
    [SerializeField] private float spacing = 2.5f;

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        yield return UpdateCardPositions(0f);
    }

    public void ClearHand()
    {
        foreach (CardView card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
    }

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0)
            yield break;

        float startX = handCenter.x - ((cards.Count - 1) * spacing / 2f);

        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 targetPos = new Vector3(startX + i * spacing, handCenter.y, handCenter.z);

            cards[i].transform.position = targetPos;
            cards[i].transform.rotation = Quaternion.identity;
        }

        yield return null;
    }
}