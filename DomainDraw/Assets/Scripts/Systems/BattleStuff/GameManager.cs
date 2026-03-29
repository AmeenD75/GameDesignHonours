using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    [Header("Debug Race Assignment")]
    public RaceData debugPlayer1Race;
    public RaceData debugPlayer2Race;

    [Header("Debug Domain Assignment")]
    public DomainData debugDomain;

    [Header("Battle Visuals")]
    public SpriteRenderer battleBackgroundRenderer;

    [Header("End Screen")]
    public BattleEndUIController battleEndUI;

    [Header("Players")]
    public PlayerState player1State = new PlayerState { playerName = "Player 1", hp = 50, timeRemaining = 120f, diceUsesRemaining = 3 };
    public PlayerState player2State = new PlayerState { playerName = "Player 2", hp = 50, timeRemaining = 120f, diceUsesRemaining = 3 };

    [Header("Dice")]
    [SerializeField] private DiceController diceController;

    [Header("Character Visuals")]
    [SerializeField] private CharacterVisualController player1Character;
    [SerializeField] private CharacterVisualController player2Character;

    [Header("Battle Stats")]
    public int terrainHP = 40;
    private int terrainMaxHP;


    [Header("Turn State")]
    public bool player1Turn = true;
    private bool gameOver = false;
    private bool waitingForTurnConfirm = false;

    [Header("Systems")]
    public BattleUIController battleUI;
    public CardSystem cardSystem;

    [Header("Hand View")]
    public HandView handView;
    public Transform cardSpawnPoint;

    private CardView selectedCard;

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

    private void Start()
    {
        ApplySelectedRaces();
        ApplySelectedDecks();
        ApplySelectedDomain();
        ApplyCharacterVisuals();

        player1State.maxHP = player1State.hp;
        player2State.maxHP = player2State.hp;

        cardSystem.DrawStartingHands(player1State, player2State);
        UpdateUI();

        waitingForTurnConfirm = true;
        //ShowTurnOverlay();
        StartCoroutine(DelayedTurnOverlay());

    }

    private void Update()
    {
        if (gameOver || waitingForTurnConfirm)
            return;

        PlayerState currentPlayer = GetCurrentPlayer();
        currentPlayer.timeRemaining -= Time.deltaTime;

        if (currentPlayer.timeRemaining <= 0f)
        {
            currentPlayer.timeRemaining = 0f;

            if (player1Turn)
            {
                AddBattleLog("Player 1 ran out of time. Player 2 wins.");
                HandleGameOver("Player 2 Wins! (Time)");
            }
            else
            {
                AddBattleLog("Player 2 ran out of time. Player 1 wins.");
                HandleGameOver("Player 1 Wins! (Time)");
            }

            return;
        }

        battleUI.UpdateTimerUI(player1State.timeRemaining, player2State.timeRemaining);
    }
    #endregion

    #region Public Accessors
    public PlayerState GetCurrentPlayer()
    {
        return player1Turn ? player1State : player2State;
    }

    public PlayerState GetOpponentPlayer()
    {
        return player1Turn ? player2State : player1State;
    }

    public List<Card> GetCurrentHand()
    {
        return GetCurrentPlayer().hand;
    }
    #endregion

    #region Card Selection / Playing
    public void SelectCard(CardView cardView)
    {
        if (gameOver || waitingForTurnConfirm)
            return;

        if (cardView == null || cardView.Card == null)
            return;

        List<Card> currentHand = GetCurrentHand();

        if (!currentHand.Contains(cardView.Card))
            return;
        // Deselct card if selected
        if (selectedCard == cardView)
        {
            selectedCard.SetSelected(false);
            selectedCard = null;
            Debug.Log("Deselected card: " + cardView.Card.Title);
            return;
        }

        if (selectedCard != null)
            selectedCard.SetSelected(false);

        selectedCard = cardView;
        selectedCard.SetSelected(true);

        Debug.Log("Selected card: " + cardView.Card.Title);
    }

    public void ConfirmSelectedCard()
    {
        if (gameOver || waitingForTurnConfirm)
            return;

        if (selectedCard == null)
            return;

        List<Card> currentHand = GetCurrentHand();

        if (!currentHand.Contains(selectedCard.Card))
            return;

        PlayCard(selectedCard.Card);
        selectedCard = null;
    }


    private void PlayCard(Card card)
    {
        StartCoroutine(PlayCardSequence(card));
    }

    private IEnumerator PlayCardSequence(Card card)
    {
        PlayerState currentPlayer = GetCurrentPlayer();
        PlayerState opponent = GetOpponentPlayer();

        string currentPlayerName = player1Turn ? "Player 1" : "Player 2";
        string opponentName = player1Turn ? "Player 2" : "Player 1";

        AddBattleLog(currentPlayerName + " played " + card.Title + ".");

        CharacterVisualController attacker = player1Turn ? player1Character : player2Character;
        CharacterVisualController defender = player1Turn ? player2Character : player1Character;

        if (card.Type == CardType.Attack && attacker != null)
            yield return StartCoroutine(attacker.PlayAttack());

        if (card.DamageToEnemy > 0)
        {
            int dealt = ApplyDamage(opponent, opponentName, card.DamageToEnemy);

            if (defender != null)
                yield return StartCoroutine(defender.PlayHit());

            AddBattleLog(opponentName + " took " + dealt + " damage.");
        }

        if (card.HealSelf > 0)
        {
            currentPlayer.hp += card.HealSelf;

            if (attacker != null)
                yield return StartCoroutine(attacker.PlayBuff());

            AddBattleLog(currentPlayerName + " healed " + card.HealSelf + " HP.");
        }

        if (card.BlockAmount > 0)
        {
            ApplyDefense(currentPlayer, card.BlockAmount);

            if (attacker != null)
                yield return StartCoroutine(attacker.PlayBuff());
        }

        if (card.TerrainDamage != 0)
        {
            terrainHP -= card.TerrainDamage;
        }

        ClampValues();

        yield return new WaitForSeconds(0.15f);
        UpdateUI();

        yield return new WaitForSeconds(0.3f);

        currentPlayer.hand.Remove(card);

        EndTurn();
    }
    #endregion

    #region Actions
    //public void RollDice()
    //{
    //    if (gameOver || waitingForTurnConfirm)
    //        return;

    //    PlayerState currentPlayer = GetCurrentPlayer();
    //    PlayerState opponent = GetOpponentPlayer();

    //    string currentPlayerName = player1Turn ? "Player 1" : "Player 2";
    //    string opponentName = player1Turn ? "Player 2" : "Player 1";

    //    if (currentPlayer.diceUsesRemaining <= 0)
    //    {
    //        AddBattleLog(currentPlayerName + " has no dice rolls remaining.");
    //        return;
    //    }

    //    if (selectedCard == null)
    //    {
    //        AddBattleLog(currentPlayerName + " must select a card to swap before rolling dice.");
    //        return;
    //    }

    //    string swappedCardName = selectedCard.Card.Title;

    //    bool replaced = cardSystem.ReplaceCardInHand(currentPlayer, selectedCard.Card);
    //    if (!replaced)
    //        return;

    //    currentPlayer.diceUsesRemaining--;

    //    selectedCard.SetSelected(false);
    //    selectedCard = null;

    //    AddBattleLog(currentPlayerName + " swapped " + swappedCardName + " and rolled the dice.");

    //    int roll = Random.Range(1, 9);
    //    battleUI.SetDiceRollText(roll);

    //    if (roll <= 3)
    //    {
    //        int dealt = ApplyDamage(opponent, opponentName, 8);
    //        AddBattleLog(currentPlayerName + " rolled " + roll + " and dealt " + dealt + " damage to " + opponentName + ".");
    //    }
    //    else
    //    {
    //        int dealt = ApplyDamage(currentPlayer, currentPlayerName, 8);
    //        AddBattleLog(currentPlayerName + " rolled " + roll + " and took " + dealt + " damage.");
    //    }

    //    ClampValues();
    //    UpdateUI();
    //    EndTurn();
    //}

    public void RollDice()
    {
        if (gameOver || waitingForTurnConfirm)
            return;

        StartCoroutine(RollDiceSequence());
    }

    private IEnumerator RollDiceSequence()
    {
        PlayerState currentPlayer = GetCurrentPlayer();
        PlayerState opponent = GetOpponentPlayer();

        string currentPlayerName = player1Turn ? "Player 1" : "Player 2";
        string opponentName = player1Turn ? "Player 2" : "Player 1";

        if (currentPlayer.diceUsesRemaining <= 0)
        {
            AddBattleLog(currentPlayerName + " has no dice rolls remaining.");
            yield break;
        }

        if (selectedCard == null)
        {
            AddBattleLog(currentPlayerName + " must select a card to swap before rolling dice.");
            yield break;
        }

        string swappedCardName = selectedCard.Card.Title;

        bool replaced = cardSystem.ReplaceCardInHand(currentPlayer, selectedCard.Card);
        if (!replaced)
            yield break;

        currentPlayer.diceUsesRemaining--;

        selectedCard.SetSelected(false);
        selectedCard = null;

        AddBattleLog(currentPlayerName + " swapped " + swappedCardName + " and rolled the dice.");

        int roll = 0;
        bool finished = false;

        yield return StartCoroutine(diceController.RollDice((result) =>
        {
            roll = result;
            finished = true;
        }));

        battleUI.SetDiceRollText(roll);

        yield return new WaitForSeconds(0.25f);

        if (roll <= 4)
        {
            int dealt = ApplyDamage(currentPlayer, currentPlayerName, 8);
            AddBattleLog(currentPlayerName + " rolled " + roll + " and took " + dealt + " damage.");
        }
        else
        { 
            int dealt = ApplyDamage(opponent, opponentName, 8);
            AddBattleLog(currentPlayerName + " rolled " + roll + " and dealt " + dealt + " damage to " + opponentName + ".");
        }

        ClampValues();
        UpdateUI();

        yield return new WaitForSeconds(0.4f);

        EndTurn();
    }

    public void Forfeit()
    {
        if (gameOver || waitingForTurnConfirm)
            return;

        AddBattleLog(player1Turn ? "Player 1 forfeited." : "Player 2 forfeited.");

        if (player1Turn)
            HandleGameOver("Player 2 Wins! (Forfeit)");
        else
            HandleGameOver("Player 1 Wins! (Forfeit)");
    }
    #endregion

    #region Turn Flow
    private void EndTurn()
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
            battleUI.HideTurnOverlay();

            if (handView != null)
                handView.ClearHand();
            
            UpdateUI();
            return;
        }

        player1Turn = !player1Turn;

        cardSystem.RefillHand(GetCurrentPlayer());
        UpdateUI();

        waitingForTurnConfirm = true;
        //ShowTurnOverlay();
        StartCoroutine(DelayedTurnOverlay());
    }

    private void ClampValues()
    {
        player1State.hp = Mathf.Clamp(player1State.hp, 0, player1State.maxHP);
        player2State.hp = Mathf.Clamp(player2State.hp, 0, player2State.maxHP);
        terrainHP = Mathf.Clamp(terrainHP, 0, terrainMaxHP);
    }

    private void CheckWin()
    {
        if (player1State.hp <= 0)
        {
            AddBattleLog("Player 2 wins.");
            HandleGameOver("Player 2 Wins!");
            return;
        }

        if (player2State.hp <= 0)
        {
            AddBattleLog("Player 1 wins.");
            HandleGameOver("Player 1 Wins!");
            return;
        }

        if (terrainHP <= 0)
        {
            AddBattleLog("The terrain was destroyed. Draw.");
            HandleGameOver("Terrain Destroyed! It's a Draw!");
        }
    }
    #endregion

    #region Defense and Damage Logic
    private void ApplyDefense(PlayerState player, int blockAmount)
    {
        player.block = blockAmount;
        player.blockTurnsRemaining = 2;
    }

    private int ApplyDamage(PlayerState targetPlayer, string targetName, int damage)
    {
        if (damage <= 0)
            return 0;

        if (targetPlayer.blockTurnsRemaining > 0 && targetPlayer.block > 0)
        {
            if (damage <= targetPlayer.block)
            {
                AddBattleLog(targetName + " fully blocked the hit.");
                targetPlayer.block = 0;
                targetPlayer.blockTurnsRemaining = 0;
                return 0;
            }
            else
            {
                int blocked = targetPlayer.block;
                damage -= blocked;
                AddBattleLog(targetName + " blocked " + blocked + " damage.");
                targetPlayer.block = 0;
                targetPlayer.blockTurnsRemaining = 0;
            }
        }

        targetPlayer.hp -= damage;
        return damage;
    }

    private void DecrementCurrentPlayerDefenseDuration()
    {
        PlayerState currentPlayer = GetCurrentPlayer();

        if (currentPlayer.blockTurnsRemaining > 0)
        {
            currentPlayer.blockTurnsRemaining--;

            if (currentPlayer.blockTurnsRemaining <= 0)
                currentPlayer.block = 0;
        }
    }
    #endregion

    #region UI / Hand Presentation
    private void UpdateUI()
    {
        battleUI.UpdateMainUI(player1State, player2State, terrainHP, player1Turn, gameOver);
    }

    private async void ShowCurrentPlayerHand()
    {
        if (handView == null)
            return;

        handView.ClearHand();

        foreach (Card card in GetCurrentHand())
        {
            CardView view = CardViewCreator.Instance.CreateCardView(
                card,
                cardSpawnPoint.position,
                Quaternion.identity
            );

            Debug.Log("Spawned card at: " + view.transform.position);
            await System.Threading.Tasks.Task.Yield();
            StartCoroutine(handView.AddCard(view));
        }
    }

    private void ShowTurnOverlay()
    {
        if (handView != null)
            handView.ClearHand();

        battleUI.ShowTurnOverlay(player1Turn);
    }

    public void ConfirmTurnStart()
    {
        if (!waitingForTurnConfirm)
            return;

        waitingForTurnConfirm = false;
        battleUI.HideTurnOverlay();
        ShowCurrentPlayerHand();
    }
    #endregion

    #region Battle Log
    private void AddBattleLog(string message)
    {
        battleUI.AddBattleLog(message);
    }
    #endregion

    #region Character & Domain Selection
    private void ApplySelectedRaces()
    {
        RaceData p1Race = MatchSetup.player1Race != null ? MatchSetup.player1Race : debugPlayer1Race;
        RaceData p2Race = MatchSetup.player2Race != null ? MatchSetup.player2Race : debugPlayer2Race;

        if (p1Race != null)
        {
            player1State.playerName = p1Race.raceName;
            player1State.deck = new List<CardData>(p1Race.cardPool);
        }

        if (p2Race != null)
        {
            player2State.playerName = p2Race.raceName;
            player2State.deck = new List<CardData>(p2Race.cardPool);
        }
    }

    private void ApplySelectedDomain()
    {
        DomainData domain = MatchSetup.selectedDomain != null ? MatchSetup.selectedDomain : debugDomain;

        if (domain != null)
        {
            terrainHP = domain.startingTerrainHP;
            terrainMaxHP = terrainHP;

            if (battleBackgroundRenderer != null)
                battleBackgroundRenderer.sprite = domain.backgroundImage;
        }
        else
        {
            terrainMaxHP = terrainHP;
        }
    }

    private void ApplySelectedDecks()
    {
        if (MatchSetup.player1Deck != null)
            player1State.deck = new List<CardData>(MatchSetup.player1Deck);

        if (MatchSetup.player2Deck != null)
            player2State.deck = new List<CardData>(MatchSetup.player2Deck);
    }
    #endregion


    #region Character Sprite Setup
    private void ApplyCharacterVisuals()
    {
        RaceData p1Race = MatchSetup.player1Race != null ? MatchSetup.player1Race : debugPlayer1Race;
        RaceData p2Race = MatchSetup.player2Race != null ? MatchSetup.player2Race : debugPlayer2Race;

        Debug.Log("MatchSetup.player1Race = " + (MatchSetup.player1Race != null ? MatchSetup.player1Race.raceName : "NULL"));
        Debug.Log("MatchSetup.player2Race = " + (MatchSetup.player2Race != null ? MatchSetup.player2Race.raceName : "NULL"));

        Debug.Log("Applied P1 race = " + (p1Race != null ? p1Race.raceName : "NULL"));
        Debug.Log("Applied P2 race = " + (p2Race != null ? p2Race.raceName : "NULL"));

        if (player1Character != null)
            player1Character.Setup(p1Race, true);

        if (player2Character != null)
            player2Character.Setup(p2Race, false);
    }


    private void TriggerAnimation(CharacterVisualController character, CardType type)
    {
        if (character == null)
            return;

        switch (type)
        {
            case CardType.Attack:
                character.PlayAttack();
                break;

            case CardType.Defense:
            case CardType.Support:
                character.PlayBuff();
                break;
        }
    }


    #endregion

    #region Game Over
    private void HandleGameOver(string message)
    {
        gameOver = true;

        battleUI.SetTurnText(message);
        battleUI.HideTurnOverlay();

        if (handView != null)
            handView.ClearHand();

        UpdateUI();

        if (battleEndUI != null)
            battleEndUI.ShowEndScreen(message);
    }

    private IEnumerator DelayedTurnOverlay()
    {
        yield return new WaitForSeconds(0.5f);
        ShowTurnOverlay();
    }
    #endregion
}