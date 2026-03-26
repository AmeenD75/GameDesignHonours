using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckPreviewEntry : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;

    private CardData cardData;
    private DeckBuilderManager manager;
    private bool isLocked;

    public void Setup(CardData card, DeckBuilderManager deckBuilderManager, bool locked)
    {
        cardData = card;
        manager = deckBuilderManager;
        isLocked = locked;

        if (nameText != null)
        {
            nameText.text = locked
                ? card.Title + " (Locked)"
                : card.Title;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null || cardData == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right && !isLocked)
            manager.OnDeckEntryRightClicked(cardData);
    }
}