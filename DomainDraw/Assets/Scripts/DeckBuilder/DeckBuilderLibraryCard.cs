using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckBuilderLibraryCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private Image frameImage;

    private CardData cardData;
    private DeckBuilderManager manager;
    private bool canAdd;

    public void Setup(CardData card, DeckBuilderManager deckBuilderManager, bool isInDeck, bool canAddCard, bool typeCapReached)
    {
        cardData = card;
        manager = deckBuilderManager;
        canAdd = canAddCard;

        if (cardImage != null)
            cardImage.sprite = card.Image;

        if (titleText != null)
            titleText.text = card.Title;

        if (stateText != null)
        {
            if (isInDeck)
                stateText.text = "In Deck";
            else if (typeCapReached)
                stateText.text = "Type Max";
            else if (!canAddCard)
                stateText.text = "Unavailable";
            else
                stateText.text = "";
        }

        if (frameImage != null)
        {
            Color color = Color.white;

            if (isInDeck)
                color = new Color(0.7f, 1f, 0.7f, 1f);
            else if (typeCapReached)
                color = new Color(1f, 0.8f, 0.8f, 1f);

            frameImage.color = color;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null || cardData == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            manager.OnLibraryCardLeftClicked(cardData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (canAdd)
                manager.OnLibraryCardRightClicked(cardData);
        }
    }
}