using UnityEngine;

[CreateAssetMenu(fileName = "DeckBuildSettings", menuName = "Game/Deck Build Settings")]
public class DeckBuildSettings : ScriptableObject
{
    [Header("Deck Size")]
    public int totalDeckSize = 15;

    [Header("General Card Limits")]
    public int maxPerType = 5;
}