using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Inspector - Battle Stats
    [Header("Battle Stats")]
    public int player1HP = 50;
    public int player2HP = 50;
    public int terrainHP = 40;
    #endregion

    #region Inspector - Turn State
    [Header("Turn")]
    public bool player1Turn = true;
    private bool gameOver = false;
    #endregion


    #region Inspector - Pass and Play
    [Header("Pass and Play")]
    public GameObject turnOverlay;
    public TMP_Text turnOverlayText;
    private bool waitingForTurnConfirm = false;
    #endregion

    #region Inspector - UI
    [Header("UI")]
    public TMP_Text player1HPText;
    public TMP_Text player2HPText;
    public TMP_Text turnText;
    public TMP_Text terrainHPText;
    public TMP_Text diceNumText;
    public TMP_Text player1BlockText;
    public TMP_Text player2BlockText;
    #endregion

    #region Inspector - Decks / Hands
    [Header("Decks")]
    public List<CardData> player1Deck = new();
    public List<CardData> player2Deck = new();

    private readonly List<Card> player1Hand = new();
    private readonly List<Card> player2Hand = new();
    #endregion

    #region Battle - Defense State
    public int player1Block = 0;
    public int player2Block = 0;

    public int player1BlockTurnsRemaining = 0;
    public int player2BlockTurnsRemaining = 0;
    #endregion


    #region Inspector - Hand View
    [Header("Hand View")]
    public HandView handView;
    public Transform cardSpawnPoint;
    #endregion

    #region Runtime State
    private const int HandSize = 3;
    private CardView selectedCard;
    #endregion


    [Header("Battle Log")]
    public GameObject battleLogPanel;
    public TMP_Text battleLogText;
    private readonly List<string> battleLogEntries = new();
    [SerializeField] private int maxBattleLogEntries = 8;


    [Header("Chess Timer")]
    public float player1TimeRemaining = 120f;
    public float player2TimeRemaining = 120f;

    public TMP_Text player1TimerText;
    public TMP_Text player2TimerText;


    [Header("Dice Uses")]
    public int player1DiceUsesRemaining = 3;
    public int player2DiceUsesRemaining = 3;
    public TMP_Text player1DiceText;
    public TMP_Text player2DiceText;

    #region Unity Messages
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        DrawStartingHands();
        UpdateUI();

        waitingForTurnConfirm = true;
        ShowTurnOverlay();
    }
    #endregion

    #region Hand / Deck Management
    // Fills both hands at game start.
    void DrawStartingHands()
    {
        player1Hand.Clear();
        player2Hand.Clear();

        for (int i = 0; i < HandSize; i++)
        {
            DrawCardForPlayer(true);
            DrawCardForPlayer(false);
        }
    }

    // Draws a random card from the selected player's deck and adds it to their hand.
    void DrawCardForPlayer(bool isPlayer1)
    {
        List<CardData> deck = isPlayer1 ? player1Deck : player2Deck;
        List<Card> hand = isPlayer1 ? player1Hand : player2Hand;
        if (deck.Count == 0) return;
        // Build a list of cards not already in hand
        List<CardData> validChoices = new List<CardData>();

        foreach (CardData cardData in deck)
        {
            bool alreadyInHand = false;

            foreach (Card handCard in hand)
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
        // No valid unique card left to draw
        if (validChoices.Count == 0) return;

        int index = Random.Range(0, validChoices.Count);
        hand.Add(new Card(validChoices[index]));
    }

    // Returns the hand for the player whose turn it currently is.
    List<Card> GetCurrentHand()
    {
        return player1Turn ? player1Hand : player2Hand;
    }

    // Ensures the current player's hand is refilled up to HandSize.
    void RefillCurrentHand()
    {
        List<Card> hand = GetCurrentHand();

        while (hand.Count < HandSize)
        {
            DrawCardForPlayer(player1Turn);
            hand = GetCurrentHand();
        }
    }
    #endregion

    //public void TryPlayCard(CardView cardView)
    //{
    //    if (gameOver) return;

    //    List<Card> currentHand = GetCurrentHand();

    //    if (!currentHand.Contains(cardView.Card))
    //        return;

    //    PlayCard(cardView.Card);
    //}

    #region Card Selection / Playing
    // Called by the UI/input layer when a card is clicked/hover-selected.
    public void SelectCard(CardView cardView)
    {
        if (gameOver || waitingForTurnConfirm) return;

        List<Card> currentHand = GetCurrentHand();

        if (!currentHand.Contains(cardView.Card))
            return;

        //selectedCard = cardView;
        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = cardView;
        selectedCard.SetSelected(true);


        Debug.Log("Selected card: " + cardView.Card.Title);
    }


    // Called by the UI/input layer to confirm and play the currently selected card.
    public void ConfirmSelectedCard()
    {
        if (gameOver || waitingForTurnConfirm) return;
        if (selectedCard == null) return;

        List<Card> currentHand = GetCurrentHand();

        if (!currentHand.Contains(selectedCard.Card))
            return;

        PlayCard(selectedCard.Card);
        selectedCard = null;
    }

    // Applies the card effects, removes it from the hand, then ends the turn.
    void PlayCard(Card card)
    {
        string currentPlayerName = player1Turn ? "Player 1" : "Player 2";
        string opponentName = player1Turn ? "Player 2" : "Player 1";

        AddBattleLog(currentPlayerName + " played " + card.Title + ".");

        if (player1Turn)
        {
            if (card.DamageToEnemy > 0)
            {
                int dealt = ApplyDamage(false, card.DamageToEnemy);
                AddBattleLog(opponentName + " took " + dealt + " damage.");
            }

            if (card.HealSelf > 0)
            {
                player1HP += card.HealSelf;
                AddBattleLog(currentPlayerName + " healed " + card.HealSelf + " HP.");
            }

            if (card.SelfDamage > 0)
            {
                int selfDealt = ApplyDamage(true, card.SelfDamage);
                AddBattleLog(currentPlayerName + " took " + selfDealt + " self-damage.");
            }

            if (card.BlockAmount > 0)
            {
                ApplyDefense(true, card.BlockAmount);
                AddBattleLog(currentPlayerName + " gained " + card.BlockAmount + " block for 2 turns.");
            }
        }
        else
        {
            if (card.DamageToEnemy > 0)
            {
                int dealt = ApplyDamage(true, card.DamageToEnemy);
                AddBattleLog(opponentName + " took " + dealt + " damage.");
            }

            if (card.HealSelf > 0)
            {
                player2HP += card.HealSelf;
                AddBattleLog(currentPlayerName + " healed " + card.HealSelf + " HP.");
            }

            if (card.SelfDamage > 0)
            {
                int selfDealt = ApplyDamage(false, card.SelfDamage);
                AddBattleLog(currentPlayerName + " took " + selfDealt + " self-damage.");
            }

            if (card.BlockAmount > 0)
            {
                ApplyDefense(false, card.BlockAmount);
                AddBattleLog(currentPlayerName + " gained " + card.BlockAmount + " block for 2 turns.");
            }
        }

        if (card.TerrainDamage > 0)
        {
            terrainHP -= card.TerrainDamage;
            AddBattleLog("Terrain took " + card.TerrainDamage + " damage.");
        }
        else if (card.TerrainDamage < 0)
        {
            terrainHP -= card.TerrainDamage; // subtracting negative repairs
            AddBattleLog("Terrain was restored by " + (-card.TerrainDamage) + ".");
        }
        ClampValues();
        List<Card> currentHand = GetCurrentHand();
        currentHand.Remove(card);

        EndTurn();
    }
    #endregion

    #region Actions (UI Buttons)
    // Dice roll action that applies damage based on a 1-6 roll.
    public void RollDice()
    {
        if (gameOver || waitingForTurnConfirm) return;

        if (GetCurrentPlayerDiceUsesRemaining() <= 0)
        {
            AddBattleLog((player1Turn ? "Player 1" : "Player 2") + " has no dice rolls remaining.");
            return;
        }

        if (selectedCard == null)
        {
            AddBattleLog((player1Turn ? "Player 1" : "Player 2") + " must select a card to swap before rolling dice.");
            return;
        }

        string currentPlayerName = player1Turn ? "Player 1" : "Player 2";
        string opponentName = player1Turn ? "Player 2" : "Player 1";
        string swappedCardName = selectedCard.Card.Title;

        // Swap out selected card first
        ReplaceSelectedCardInHand();
        UseCurrentPlayerDiceCharge();

        AddBattleLog(currentPlayerName + " swapped " + swappedCardName + " and rolled the dice.");

        int roll = Random.Range(1, 7);
        diceNumText.text = "Dice Roll: " + roll;

        if (roll <= 3)
        {
            if (player1Turn)
            {
                int dealt = ApplyDamage(false, 12);
                AddBattleLog(currentPlayerName + " rolled " + roll + " and dealt " + dealt + " damage to " + opponentName + ".");
            }
            else
            {
                int dealt = ApplyDamage(true, 12);
                AddBattleLog(currentPlayerName + " rolled " + roll + " and dealt " + dealt + " damage to " + opponentName + ".");
            }
        }
        else
        {
            if (player1Turn)
            {
                int dealt = ApplyDamage(true, 12);
                AddBattleLog(currentPlayerName + " rolled " + roll + " and took " + dealt + " damage.");
            }
            else
            {
                int dealt = ApplyDamage(false, 12);
                AddBattleLog(currentPlayerName + " rolled " + roll + " and took " + dealt + " damage.");
            }
        }
        ClampValues();
        UpdateUI();
        EndTurn();
    }

    // Ends the game immediately, awarding win to the other player.
    public void Forfeit()
    {
        if (gameOver || waitingForTurnConfirm) return;
        AddBattleLog(player1Turn ? "Player 1 forfeited." : "Player 2 forfeited.");
        gameOver = true;

        if (player1Turn)
            turnText.text = "Player 2 Wins! (Forfeit)";
        else
            turnText.text = "Player 1 Wins! (Forfeit)";

        if (turnOverlay != null)
            turnOverlay.SetActive(false);

        if (handView != null)
            handView.ClearHand();

        UpdateUI();
    }
    #endregion

    //public void Attack()
    //{
    //    if (gameOver) return;

    //    if (player1Turn)
    //    {
    //        player2HP -= 10;
    //        terrainHP -= 3;
    //    }
    //    else
    //    {
    //        player1HP -= 10;
    //        terrainHP -= 3;
    //    }

    //    ClampValues();
    //    EndTurn();
    //}

    #region Turn Flow
    // Resolves end-of-turn state updates and switches active player.
    void EndTurn()
    {

        if (selectedCard != null)
        {
            selectedCard.SetSelected(false);
            selectedCard = null;
        }

        DecrementCurrentPlayerDefenseDuration();
        CheckWin();
        if (gameOver)
        {
            if (turnOverlay != null)
                turnOverlay.SetActive(false);

            if (handView != null)
                handView.ClearHand();
            UpdateUI();
            return;
        }

        player1Turn = !player1Turn;
        RefillCurrentHand();
        UpdateUI();

        waitingForTurnConfirm = true;
        ShowTurnOverlay();
    }

    // Prevents HP values from going below zero.
    void ClampValues()
    {
        player1HP = Mathf.Max(0, player1HP);
        player2HP = Mathf.Max(0, player2HP);
        terrainHP = Mathf.Max(0, terrainHP);
    }

    // Checks win/lose conditions and sets gameOver + UI message when met.
    void CheckWin()
    {
        if (player1HP <= 0)
        {
            gameOver = true;
            turnText.text = "Player 2 Wins!";
            AddBattleLog("Player 2 wins.");
            return;
        }
        if (player2HP <= 0)
        {
            gameOver = true;
            turnText.text = "Player 1 Wins!";
            AddBattleLog("Player 1 wins.");
            return;
        }
        if (terrainHP <= 0)
        {
            gameOver = true;
            turnText.text = "Terrain Destroyed! It's a Draw!";
            AddBattleLog("The terrain was destroyed. Draw.");
            return;
        }
    }
    #endregion

    #region Defense and Damage Logic
    // Applies a defensive shield to the specified player.
    void ApplyDefense(bool forPlayer1, int blockAmount)
    {
        if (forPlayer1)
        {
            player1Block = blockAmount;
            player1BlockTurnsRemaining = 2;
        }
        else
        {
            player2Block = blockAmount;
            player2BlockTurnsRemaining = 2;
        }
    }

    // Applies incoming damage, accounting for any active shield.
    int ApplyDamage(bool targetPlayer1, int damage)
    {
        if (damage <= 0) return 0;

        if (targetPlayer1)
        {
            if (player1BlockTurnsRemaining > 0 && player1Block > 0)
            {
                if (damage <= player1Block)
                {
                    AddBattleLog("Player 1 fully blocked the hit.");
                    player1Block = 0;
                    player1BlockTurnsRemaining = 0;
                    return 0;
                }
                else
                {
                    int blocked = player1Block;
                    damage -= blocked;
                    AddBattleLog("Player 1 blocked " + blocked + " damage.");
                    player1Block = 0;
                    player1BlockTurnsRemaining = 0;
                }
            }

            player1HP -= damage;
            return damage;
        }
        else
        {
            if (player2BlockTurnsRemaining > 0 && player2Block > 0)
            {
                if (damage <= player2Block)
                {
                    AddBattleLog("Player 2 fully blocked the hit.");
                    player2Block = 0;
                    player2BlockTurnsRemaining = 0;
                    return 0;
                }
                else
                {
                    int blocked = player2Block;
                    damage -= blocked;
                    AddBattleLog("Player 2 blocked " + blocked + " damage.");
                    player2Block = 0;
                    player2BlockTurnsRemaining = 0;
                }
            }
            player2HP -= damage;
            return damage;
        }
    }

    // Decrements shield duration for the player who just ended their turn.
    void DecrementCurrentPlayerDefenseDuration()
    {
        if (player1Turn)
        {
            if (player1BlockTurnsRemaining > 0)
            {
                player1BlockTurnsRemaining--;
                if (player1BlockTurnsRemaining <= 0)
                    player1Block = 0;
            }
        }
        else
        {
            if (player2BlockTurnsRemaining > 0)
            {
                player2BlockTurnsRemaining--;
                if (player2BlockTurnsRemaining <= 0)
                    player2Block = 0;
            }
        }
    }
    #endregion

    #region UI / Hand Presentation
    // Refreshes HUD text based on the current game state.
    void UpdateUI()
    {
        player1HPText.text = "Player 1 HP: " + player1HP;
        player2HPText.text = "Player 2 HP: " + player2HP;
        terrainHPText.text = "Terrain HP: " + terrainHP;

        if (player1BlockTurnsRemaining > 0 && player1Block > 0)
            player1BlockText.text = "Shield: " + player1Block + " (" + player1BlockTurnsRemaining + ")";
        else
            player1BlockText.text = "NO SHIELD";

        if (player2BlockTurnsRemaining > 0 && player2Block > 0)
            player2BlockText.text = "Shield: " + player2Block + " (" + player2BlockTurnsRemaining + ")";
        else
            player2BlockText.text = "NO SHIELD";

        player1DiceText.text = "Dice: " + player1DiceUsesRemaining;
        player2DiceText.text = "Dice: " + player2DiceUsesRemaining;

        if (!gameOver)
            turnText.text = player1Turn ? "Player 1 Turn" : "Player 2 Turn";
        UpdateTimerUI();
    }

    // Clears and re-spawns the current player's hand into the HandView.
    async void ShowCurrentPlayerHand()
    {
        handView.ClearHand();

        foreach (Card card in GetCurrentHand())
        {
            CardView view = CardViewCreator.Instance.CreateCardView(card, cardSpawnPoint.position, Quaternion.identity);
 
            Debug.Log("Spawned card at: " + view.transform.position);
            await System.Threading.Tasks.Task.Yield();
            StartCoroutine(handView.AddCard(view));
        }
    }

    // Shows the pass-and-play overlay and hides cards until confirmed.
    void ShowTurnOverlay()
    {
        if (handView != null)
            handView.ClearHand();

        if (turnOverlay != null)
            turnOverlay.SetActive(true);

        if (turnOverlayText != null)
        {
            turnOverlayText.text = player1Turn
                ? "Player 1 Turn\nPass the device"
                : "Player 2 Turn\nPass the device";
        }
    }


    // Called by the overlay button to start the active player's turn.
    public void ConfirmTurnStart()
    {
        if (!waitingForTurnConfirm) return;

        waitingForTurnConfirm = false;

        if (turnOverlay != null)
            turnOverlay.SetActive(false);

        ShowCurrentPlayerHand();
    }
    #endregion

    #region Battle Log and Timer
    void AddBattleLog(string message)
    {
        battleLogEntries.Add(message);

        if (battleLogEntries.Count > maxBattleLogEntries)
            battleLogEntries.RemoveAt(0);

        if (battleLogText != null)
            battleLogText.text = string.Join("\n", battleLogEntries);
    }
    void Update()
    {
        if (gameOver || waitingForTurnConfirm)
            return;

        if (player1Turn)
        {
            player1TimeRemaining -= Time.deltaTime;

            if (player1TimeRemaining <= 0f)
            {
                player1TimeRemaining = 0f;
                gameOver = true;
                turnText.text = "Player 2 Wins! (Time)";
                AddBattleLog("Player 1 ran out of time. Player 2 wins.");
                UpdateUI();
                return;
            }
        }
        else
        {
            player2TimeRemaining -= Time.deltaTime;

            if (player2TimeRemaining <= 0f)
            {
                player2TimeRemaining = 0f;
                gameOver = true;
                turnText.text = "Player 1 Wins! (Time)";
                AddBattleLog("Player 2 ran out of time. Player 1 wins.");
                UpdateUI();
                return;
            }
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        player1TimerText.text = "Time: " + FormatTime(player1TimeRemaining);
        player2TimerText.text = "Time: " + FormatTime(player2TimeRemaining);
    }

    string FormatTime(float timeInSeconds)
    {
        int totalSeconds = Mathf.CeilToInt(timeInSeconds);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    #endregion


    #region Dice Management 
    int GetCurrentPlayerDiceUsesRemaining()
    {
        return player1Turn ? player1DiceUsesRemaining : player2DiceUsesRemaining;
    }

    void UseCurrentPlayerDiceCharge()
    {
        if (player1Turn)
            player1DiceUsesRemaining--;
        else
            player2DiceUsesRemaining--;
    }
    void ReplaceSelectedCardInHand()
    {
        if (selectedCard == null) return;

        List<Card> currentHand = GetCurrentHand();

        if (!currentHand.Contains(selectedCard.Card))
            return;

        currentHand.Remove(selectedCard.Card);

        DrawCardForPlayer(player1Turn);

        selectedCard.SetSelected(false);
        selectedCard = null;

    }

    #endregion
}