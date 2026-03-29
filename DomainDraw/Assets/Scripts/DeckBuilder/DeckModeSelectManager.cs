using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckModeSelectManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private DeckBuildSettings deckBuildSettings;

    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text infoText;

    [Header("Scene Flow")]
    [SerializeField] private string deckBuilderSceneName = "DeckBuilderScene";
    [SerializeField] private string domainSelectSceneName = "DomainSelection";

    private bool choosingPlayer1 = true;

    private void Start()
    {
        choosingPlayer1 = true;

        MatchSetup.player1UsesPreset = false;
        MatchSetup.player2UsesPreset = false;
        MatchSetup.player1Deck = null;
        MatchSetup.player2Deck = null;

        RefreshUI();
    }

    public void ChoosePreset()
    {
        if (choosingPlayer1)
        {
            MatchSetup.player1UsesPreset = true;
            choosingPlayer1 = false;
            RefreshUI();
        }
        else
        {
            MatchSetup.player2UsesPreset = true;
            FinalizeAndContinue();
        }
    }

    public void ChooseBuildOwn()
    {
        if (choosingPlayer1)
        {
            MatchSetup.player1UsesPreset = false;
            choosingPlayer1 = false;
            RefreshUI();
        }
        else
        {
            MatchSetup.player2UsesPreset = false;
            FinalizeAndContinue();
        }
    }

    private void FinalizeAndContinue()
    {
        if (MatchSetup.player1UsesPreset)
            MatchSetup.player1Deck = BuildPresetDeck(MatchSetup.player1Race);

        if (MatchSetup.player2UsesPreset)
            MatchSetup.player2Deck = BuildPresetDeck(MatchSetup.player2Race);

        bool anyCustom = !MatchSetup.player1UsesPreset || !MatchSetup.player2UsesPreset;

        if (anyCustom)
            SceneManager.LoadScene(deckBuilderSceneName);
        else
            SceneManager.LoadScene(domainSelectSceneName);
    }

    private List<CardData> BuildPresetDeck(RaceData race)
    {
        List<CardData> deck = new List<CardData>();

        if (race != null)
            deck.AddRange(race.cardPool);

        if (cardLibrary == null || deckBuildSettings == null)
            return deck;

        foreach (CardData card in cardLibrary.generalCards)
        {
            if (card == null)
                continue;

            if (deck.Count >= deckBuildSettings.totalDeckSize)
                break;

            if (deck.Contains(card))
                continue;

            // NEW: Check the specific limit based on the card type
            if (IsTypeLimitReached(deck, card.Type))
                continue;

            deck.Add(card);
        }

        return deck;
    }

    // NEW HELPER METHOD: Compares current count against the specific setting
    private bool IsTypeLimitReached(List<CardData> deck, CardType type)
    {
        int currentCount = GetTypeCount(deck, type);

        // Note: Make sure these enum names match exactly what is in your CardType enum!
        switch (type)
        {
            case CardType.Attack:
                return currentCount >= deckBuildSettings.maxAttackCards;
            case CardType.Defense:
                return currentCount >= deckBuildSettings.maxDefenseCards;
            case CardType.Support:
                return currentCount >= deckBuildSettings.maxSupportCards;
            default:
                return false;
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

    private void RefreshUI()
    {
        if (titleText != null)
            titleText.text = choosingPlayer1 ? "Player 1 - Choose Deck Mode" : "Player 2 - Choose Deck Mode";

        if (infoText != null)
        {
            RaceData race = choosingPlayer1 ? MatchSetup.player1Race : MatchSetup.player2Race;
            string raceName = race != null ? race.raceName : "No Race";
            infoText.text =
                "Race: " + raceName + "\n" +
                "Each deck has 15 cards total.\n" +
                "5 race cards are locked in.\n" +
                "The remaining 10 come from the general library.\n" +
                "Preset auto-builds the deck.\n" +
                "Build Deck lets you choose the remaining cards yourself.";
        }
    }

    public void ReturnToCharacterSelect()
    {
        MatchSetup.player1Deck = null;
        MatchSetup.player2Deck = null;
        SceneManager.LoadScene("CharacterSelection");
    }
}