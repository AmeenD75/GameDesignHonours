using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRaceData", menuName = "Data/Race")]
public class RaceData : ScriptableObject
{
    [Header("Basic Info")]
    public string raceName;

    [TextArea(2, 5)]
    public string description;

    public Sprite portrait;

    [Header("Player 1 Visuals (Faces Right)")]
    public Sprite player1Idle;
    public Sprite[] player1AttackFrames;

    [Header("Player 2 Visuals (Faces Left)")]
    public Sprite player2Idle;
    public Sprite[] player2AttackFrames;

    [Header("Card Pool")]
    public List<CardData> cardPool = new();
}