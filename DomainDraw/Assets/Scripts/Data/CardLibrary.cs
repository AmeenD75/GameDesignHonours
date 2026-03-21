using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardLibrary", menuName = "Game/Card Library")]
public class CardLibrary : ScriptableObject
{
    [Header("General Cards")]
    public List<CardData> generalCards = new();
}