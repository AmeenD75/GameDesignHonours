using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int player1HP = 50;
    public int player2HP = 50;

    public int terrainHP = 40;

    public bool player1Turn = true;

    public TMP_Text player1HPText;
    public TMP_Text player2HPText;
    public TMP_Text turnText;
    public TMP_Text terrainHPText;
    public TMP_Text DiceNum;

    void Start()
    {
        UpdateUI();
    }

    public void Attack()
    {
        if (player1Turn)
        {
            player2HP -= 10;
            terrainHP -= 3;
        }
        else
        {
            player1HP -= 10;
            terrainHP -= 3;
        }

        player1Turn = !player1Turn;

        //CheckWin();

        UpdateUI();
    }

    void CheckWin()
    {
        if (player1HP <= 0)
        {
            turnText.text = "Player 2 Wins!";
            return;
        }

        if (player2HP <= 0)
        {
            turnText.text = "Player 1 Wins!";
            return;
        }

        if (terrainHP <= 0)
        {
            turnText.text = "Terrain Destroyed! It's a Draw!";
            return;
        }
    }

    void UpdateUI()
    {
        player1HPText.text = "Player 1 HP: " + player1HP;
        player2HPText.text = "Player 2 HP: " + player2HP;

        terrainHPText.text = "Terrain HP: " + terrainHP;

        if (player1Turn)
            turnText.text = "Player 1 Turn";
        else
            turnText.text = "Player 2 Turn";
        CheckWin();
    }


    public void RollDice()
    {
        int roll = Random.Range(0, 2);

        DiceNum.text = "Dice Roll: " + roll;

        if (player1Turn)
        {
            if (roll == 0)
                player2HP -= 12;
            else
                player1HP -= 12;
        }
        else
        {
            if (roll == 0)
                player1HP -= 12;
            else
                player2HP -= 12;
        }

        player1Turn = !player1Turn;

        UpdateUI();
    }
}