using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckBuilderManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private DeckBuildSettings deckBuildSettings;

    [Header("Top UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text raceText;
    [SerializeField] private TMP_Text remainingText;

    [Header("Library Parents")]
    [SerializeField] private Transform attackContent;
    [SerializeField] private Transform defenseContent;
    [SerializeField] private Transform supportContent;

    [Header("Deck Preview Parent")]
    [SerializeField] private Transform deckPreviewContent;

    [Header("Prefabs")]
    [SerializeField] private DeckBuilderLibraryCard libraryCardPrefab;
    [SerializeField] private DeckPreviewEntry deckPreviewEntryPrefab;

    [Header("Preview Panel")]
    [SerializeField] private DeckCardPreviewPanel previewPanel;

    [Header("Scene Flow")]
    [SerializeField] private string nextSceneName = "DomainSelectScene";

    private bool buildingPlayer1;
    private RaceData currentRace;

    private readonly List<CardData> currentDeck = new();
    private readonly List<CardData> lockedRaceCards = new();

    private readonly List<DeckBuilderLibraryCard> spawnedLibraryCards = new();
    private readonly List<DeckPreviewEntry> spawnedDeckEntries = new();

    private void Start()
    {
        if (!MatchSetup.player1UsesPreset)
        {
            buildingPlayer1 = true;
        }
        else if (!MatchSetup.player2UsesPreset)
        {
            buildingPlayer1 = false;
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        StartBuildForCurrentPlayer();
    }

    private void StartBuildForCurrentPlayer()
    {
        currentDeck.Clear();
        lockedRaceCards.Clear();

        currentRace = buildingPlayer1 ? MatchSetup.player1Race : MatchSetup.player2Race;

        if (currentRace != null)
        {
            foreach (CardData card in currentRace.cardPool)
            {
                if (card == null)
                    continue;

                lockedRaceCards.Add(card);
                currentDeck.Add(card);
            }
        }

        RefreshAll();
    }

    public void OnLibraryCardLeftClicked(CardData card)
    {
        if (card == null || previewPanel == null)
            return;

        bool isLocked = lockedRaceCards.Contains(card);
        bool isInDeck = currentDeck.Contains(card);
        previewPanel.Show(card, isLocked, isInDeck);
    }

    public void OnLibraryCardRightClicked(CardData card)
    {
        AddCardToDeck(card);
    }

    public void OnDeckEntryRightClicked(CardData card)
    {
        RemoveCardFromDeck(card);
    }

    public void AddCardToDeck(CardData card)
    {
        if (card == null)
            return;

        if (lockedRaceCards.Contains(card))
            return;

        if (currentDeck.Count >= deckBuildSettings.totalDeckSize)
            return;

        if (currentDeck.Contains(card))
            return;

        if (GetTypeCount(currentDeck, card.Type) >= deckBuildSettings.maxPerType)
            return;

        currentDeck.Add(card);
        RefreshAll();
    }

    public void RemoveCardFromDeck(CardData card)
    {
        if (card == null)
            return;

        if (lockedRaceCards.Contains(card))
            return;

        if (!currentDeck.Contains(card))
            return;

        currentDeck.Remove(card);
        RefreshAll();
    }

    public void ConfirmDeck()
    {
        if (currentDeck.Count != deckBuildSettings.totalDeckSize)
            return;

        if (buildingPlayer1)
        {
            MatchSetup.player1Deck = new List<CardData>(currentDeck);

            if (!MatchSetup.player2UsesPreset)
            {
                buildingPlayer1 = false;
                StartBuildForCurrentPlayer();
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        else
        {
            MatchSetup.player2Deck = new List<CardData>(currentDeck);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void RefreshAll()
    {
        RefreshTopText();
        RefreshLibrary();
        RefreshDeckPreview();

        if (previewPanel != null)
            previewPanel.Hide();
    }

    private void RefreshTopText()
    {
        if (titleText != null)
            titleText.text = "Deck Builder";

        if (playerText != null)
            playerText.text = buildingPlayer1 ? "Player 1" : "Player 2";

        if (raceText != null)
            raceText.text = currentRace != null ? currentRace.raceName : "Race: None";

        if (remainingText != null)
        {
            int remaining = deckBuildSettings.totalDeckSize - currentDeck.Count;
            remainingText.text = remaining + " more cards";
        }
    }

    private void RefreshLibrary()
    {
        ClearLibraryCards();

        if (cardLibrary == null || libraryCardPrefab == null)
            return;

        foreach (CardData card in cardLibrary.generalCards)
        {
            if (card == null)
                continue;

            Transform parent = GetParentForType(card.Type);
            if (parent == null)
                continue;

            DeckBuilderLibraryCard instance = Instantiate(libraryCardPrefab, parent);

            bool isInDeck = currentDeck.Contains(card);
            bool typeCapReached = !isInDeck && GetTypeCount(currentDeck, card.Type) >= deckBuildSettings.maxPerType;
            bool deckFull = currentDeck.Count >= deckBuildSettings.totalDeckSize;
            bool canAdd = !isInDeck && !typeCapReached && !deckFull;

            instance.Setup(card, this, isInDeck, canAdd, typeCapReached);
            spawnedLibraryCards.Add(instance);
        }
    }

    private void RefreshDeckPreview()
    {
        ClearDeckEntries();

        if (deckPreviewEntryPrefab == null || deckPreviewContent == null)
            return;

        foreach (CardData card in currentDeck)
        {
            if (card == null)
                continue;

            DeckPreviewEntry instance = Instantiate(deckPreviewEntryPrefab, deckPreviewContent);
            bool isLocked = lockedRaceCards.Contains(card);
            instance.Setup(card, this, isLocked);
            spawnedDeckEntries.Add(instance);
        }
    }

    private Transform GetParentForType(CardType type)
    {
        switch (type)
        {
            case CardType.Attack:
                return attackContent;
            case CardType.Defense:
                return defenseContent;
            case CardType.Support:
                return supportContent;
            default:
                return null;
        }
    }

    private int GetTypeCount(List<CardData> deck, CardType type)
    {
        int count = 0;

        foreach (CardData card in deck)
        {
            if (card != null && card.Type == type)
                count++;
        }

        return count;
    }

    private void ClearLibraryCards()
    {
        for (int i = 0; i < spawnedLibraryCards.Count; i++)
        {
            if (spawnedLibraryCards[i] != null)
                Destroy(spawnedLibraryCards[i].gameObject);
        }

        spawnedLibraryCards.Clear();
    }

    private void ClearDeckEntries()
    {
        for (int i = 0; i < spawnedDeckEntries.Count; i++)
        {
            if (spawnedDeckEntries[i] != null)
                Destroy(spawnedDeckEntries[i].gameObject);
        }

        spawnedDeckEntries.Clear();
    }
}