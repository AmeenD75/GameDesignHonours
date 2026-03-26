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

    [Header("Card Pool")]
    public List<CardData> cardPool = new();
}