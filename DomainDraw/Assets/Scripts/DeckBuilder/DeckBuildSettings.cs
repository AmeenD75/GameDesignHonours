using UnityEngine;

[CreateAssetMenu(fileName = "DeckBuildSettings", menuName = "Game/Deck Build Settings")]
public class DeckBuildSettings : ScriptableObject
{
    [Header("Deck Size")]
    public int totalDeckSize = 15;

    [Header("Specific Card Limits")]
    public int maxAttackCards = 5;
    public int maxDefenseCards = 5;
    public int maxSupportCards = 5;
}