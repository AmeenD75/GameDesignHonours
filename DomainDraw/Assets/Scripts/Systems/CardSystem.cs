using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    private const int HandSize = 3;
    public void DrawStartingHands(PlayerState player1, PlayerState player2)
    {
        player1.hand.Clear();
        player2.hand.Clear();

        for (int i = 0; i < HandSize; i++)
        {
            DrawCardForPlayer(player1);
            DrawCardForPlayer(player2);
        }
    }
    public void DrawCardForPlayer(PlayerState player)
    {
        if (player.deck == null || player.deck.Count == 0)
            return;

        List<CardData> validChoices = new();

        foreach (CardData cardData in player.deck)
        {
            bool alreadyInHand = false;

            foreach (Card handCard in player.hand)
            {
                if (handCard.Title == cardData.Title)
                {
                    alreadyInHand = true;
                    break;
                }
            }

            if (!alreadyInHand)
                validChoices.Add(cardData);
        }

        if (validChoices.Count == 0)
            return;

        int index = Random.Range(0, validChoices.Count);
        player.hand.Add(new Card(validChoices[index]));
    }

    public void RefillHand(PlayerState player)
    {
        while (player.hand.Count < HandSize)
        {
            int beforeCount = player.hand.Count;
            DrawCardForPlayer(player);

            // Prevent infinite loop if no unique cards remain
            if (player.hand.Count == beforeCount)
                break;
        }
    }
    public bool ReplaceCardInHand(PlayerState player, Card cardToReplace)
    {
        if (cardToReplace == null)
            return false;

        if (!player.hand.Contains(cardToReplace))
            return false;

        player.hand.Remove(cardToReplace);
        DrawCardForPlayer(player);
        return true;
    }

    public List<Card> GetHand(PlayerState player)
    {
        return player.hand;
    }
}