using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Battle Stats")]
    public int player1HP = 50;
    public int player2HP = 50;
    public int terrainHP = 40;

    [Header("Turn")]
    public bool player1Turn = true;
    private bool gameOver = false;


    [Header("Pass and Play")]
    public GameObject turnOverlay;
    public TMP_Text turnOverlayText;
    private bool waitingForTurnConfirm = false;

    [Header("UI")]
    public TMP_Text player1HPText;
    public TMP_Text player2HPText;
    public TMP_Text turnText;
    public TMP_Text terrainHPText;
    public TMP_Text diceNumText;
    public TMP_Text player1BlockText;
    public TMP_Text player2BlockText;

    [Header("Decks")]
    public List<CardData> player1Deck = new();
    public List<CardData> player2Deck = new();

    private readonly List<Card> player1Hand = new();
    private readonly List<Card> player2Hand = new();

    public int player1Block = 0;
    public int player2Block = 0;

    public int player1BlockTurnsRemaining = 0;
    public int player2BlockTurnsRemaining = 0;


    [Header("Hand View")]
    public HandView handView;
    public Transform cardSpawnPoint;

    private const int HandSize = 3;
    private CardView selectedCard;

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

    void DrawCardForPlayer(bool isPlayer1)
    {
        List<CardData> deck = isPlayer1 ? player1Deck : player2Deck;
        List<Card> hand = isPlayer1 ? player1Hand : player2Hand;

        if (deck.Count == 0) return;

        int index = Random.Range(0, deck.Count);
        hand.Add(new Card(deck[index]));
    }

    List<Card> GetCurrentHand()
    {
        return player1Turn ? player1Hand : player2Hand;
    }

    void RefillCurrentHand()
    {
        List<Card> hand = GetCurrentHand();

        while (hand.Count < HandSize)
        {
            DrawCardForPlayer(player1Turn);
            hand = GetCurrentHand();
        }
    }

    //public void TryPlayCard(CardView cardView)
    //{
    //    if (gameOver) return;

    //    List<Card> currentHand = GetCurrentHand();

    //    if (!currentHand.Contains(cardView.Card))
    //        return;

    //    PlayCard(cardView.Card);
    //}

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

    void PlayCard(Card card)
    {
        if (player1Turn)
        {
            ApplyDamage(false, card.DamageToEnemy);
            player1HP += card.HealSelf;
            ApplyDamage(true, card.SelfDamage);
            ApplyDefense(true, card.BlockAmount);
        }
        else
        {
            ApplyDamage(true, card.DamageToEnemy);
            player2HP += card.HealSelf;
            ApplyDamage(false, card.SelfDamage);
            ApplyDefense(false, card.BlockAmount);
        }

        terrainHP -= card.TerrainDamage;

        ClampValues();

        List<Card> currentHand = GetCurrentHand();
        currentHand.Remove(card);

        EndTurn();
    }

    public void RollDice()
    {

        if (gameOver || waitingForTurnConfirm) return;
        Debug.Log("RollDice pressed");

        if (gameOver) return;

        int roll = Random.Range(1, 7);
        diceNumText.text = "Dice Roll: " + roll;

        if (roll <= 3)
        {
            if (player1Turn) player2HP -= 12;
            else player1HP -= 12;
        }
        else
        {
            if (player1Turn) player1HP -= 12;
            else player2HP -= 12;
        }

        ClampValues();
        EndTurn();
    }

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
            UpdateUI();
            return;
        }

        player1Turn = !player1Turn;
        RefillCurrentHand();
        UpdateUI();

        waitingForTurnConfirm = true;
        ShowTurnOverlay();
    }

    void ClampValues()
    {
        player1HP = Mathf.Max(0, player1HP);
        player2HP = Mathf.Max(0, player2HP);
        terrainHP = Mathf.Max(0, terrainHP);
    }

    void CheckWin()
    {
        if (terrainHP <= 0)
        {
            gameOver = true;
            turnText.text = "Terrain Destroyed! It's a Draw!";
            return;
        }

        if (player1HP <= 0)
        {
            gameOver = true;
            turnText.text = "Player 2 Wins!";
            return;
        }

        if (player2HP <= 0)
        {
            gameOver = true;
            turnText.text = "Player 1 Wins!";
            return;
        }

    }

    #region Defense and Damage Logic
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

    void ApplyDamage(bool targetPlayer1, int damage)
    {
        if (targetPlayer1)
        {
            if (player1BlockTurnsRemaining > 0 && player1Block > 0)
            {
                if (damage <= player1Block)
                {
                    // Full block
                    player1Block = 0;
                    player1BlockTurnsRemaining = 0;
                    return;
                }
                else
                {
                    // Partial mitigation, then defense expires
                    damage -= player1Block;
                    player1Block = 0;
                    player1BlockTurnsRemaining = 0;
                }
            }

            player1HP -= damage;
        }
        else
        {
            if (player2BlockTurnsRemaining > 0 && player2Block > 0)
            {
                if (damage <= player2Block)
                {
                    // Full block
                    player2Block = 0;
                    player2BlockTurnsRemaining = 0;
                    return;
                }
                else
                {
                    // Partial mitigation, then defense expires
                    damage -= player2Block;
                    player2Block = 0;
                    player2BlockTurnsRemaining = 0;
                }
            }

            player2HP -= damage;
        }
    }

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

        if (!gameOver)
            turnText.text = player1Turn ? "Player 1 Turn" : "Player 2 Turn";
    }

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

    public void ConfirmTurnStart()
    {
        if (!waitingForTurnConfirm) return;

        waitingForTurnConfirm = false;

        if (turnOverlay != null)
            turnOverlay.SetActive(false);

        ShowCurrentPlayerHand();
    }
}