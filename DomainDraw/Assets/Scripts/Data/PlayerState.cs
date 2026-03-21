using System.Collections.Generic;

[System.Serializable]
public class PlayerState
{
    public string playerName = "Player";
    public int hp = 50;
    public int maxHP = 50;

    public int block = 0;
    public int blockTurnsRemaining = 0;

    public float timeRemaining = 120f;
    public int diceUsesRemaining = 3;

    public List<CardData> deck = new();
    public List<Card> hand = new();
}