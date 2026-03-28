using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TerrainUtils;

public class BattleUIController : MonoBehaviour
{
    [Header("Main UI")]
    public TMP_Text player1HPText;
    public TMP_Text player2HPText;
    public TMP_Text turnText;
    public TMP_Text terrainHPText;
    public TMP_Text diceNumText;
    public TMP_Text player1BlockText;
    public TMP_Text player2BlockText;

    [Header("Timer UI")]
    public TMP_Text player1TimerText;
    public TMP_Text player2TimerText;

    [Header("Dice UI")]
    public TMP_Text player1DiceText;
    public TMP_Text player2DiceText;

    [Header("Pass and Play")]
    public GameObject turnOverlay;
    public TMP_Text turnOverlayText;

    [Header("Battle Log")]
    public GameObject battleLogPanel;
    public TMP_Text battleLogText;
    [SerializeField] private int maxBattleLogEntries = 8;

    [Header("HP Bars")]
    public HealthBarUI player1HPBar;
    public HealthBarUI player2HPBar;
    public HealthBarUI terrainHPBar;


    private readonly List<string> battleLogEntries = new();

    public void UpdateMainUI(PlayerState player1, PlayerState player2, int terrainHP, bool player1Turn, bool gameOver)
    {
        if (player1HPText != null)
            player1HPText.text = "Player 1 HP: " + player1.hp + " / " + player1.maxHP;
            player1HPBar.SetAnimated(player1.hp, player1.maxHP);

        if (player2HPText != null)
            player2HPText.text = "Player 2 HP: " + player2.hp + " / " + player2.maxHP;
            player2HPBar.SetAnimated(player2.hp, player2.maxHP);

        if (terrainHPText != null)
            terrainHPText.text = "Terrain HP: " + terrainHP;

        if (player1BlockText != null)
        {
            if (player1.blockTurnsRemaining > 0 && player1.block > 0)
                player1BlockText.text = "Shield: " + player1.block + " (" + player1.blockTurnsRemaining + ")";
            else
                player1BlockText.text = "NO SHIELD";
        }

        if (player2BlockText != null)
        {
            if (player2.blockTurnsRemaining > 0 && player2.block > 0)
                player2BlockText.text = "Shield: " + player2.block + " (" + player2.blockTurnsRemaining + ")";
            else
                player2BlockText.text = "NO SHIELD";
        }

        if (player1DiceText != null)
            player1DiceText.text = "Dice: " + player1.diceUsesRemaining;

        if (player2DiceText != null)
            player2DiceText.text = "Dice: " + player2.diceUsesRemaining;

        if (!gameOver && turnText != null)
            turnText.text = player1Turn ? "Player 1 Turn" : "Player 2 Turn";

        UpdateTimerUI(player1.timeRemaining, player2.timeRemaining);
    }

    public void SetTurnText(string message)
    {
        if (turnText != null)
            turnText.text = message;
    }

    public void SetDiceRollText(int roll)
    {
        if (diceNumText != null)
            diceNumText.text = "Dice Roll: " + roll;
    }

    public void ShowTurnOverlay(bool player1Turn)
    {
        if (turnOverlay != null)
            turnOverlay.SetActive(true);

        if (turnOverlayText != null)
        {
            turnOverlayText.text = player1Turn
                ? "Player 1 Turn\nPass the device"
                : "Player 2 Turn\nPass the device";
        }
    }

    public void HideTurnOverlay()
    {
        if (turnOverlay != null)
            turnOverlay.SetActive(false);
    }

    public void AddBattleLog(string message)
    {
        battleLogEntries.Add(message);

        if (battleLogEntries.Count > maxBattleLogEntries)
            battleLogEntries.RemoveAt(0);

        if (battleLogText != null)
            battleLogText.text = string.Join("\n", battleLogEntries);
    }

    public void UpdateTimerUI(float player1Time, float player2Time)
    {
        if (player1TimerText != null)
            player1TimerText.text = "Time: " + FormatTime(player1Time);

        if (player2TimerText != null)
            player2TimerText.text = "Time: " + FormatTime(player2Time);
    }

    private string FormatTime(float timeInSeconds)
    {
        int totalSeconds = Mathf.CeilToInt(timeInSeconds);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}