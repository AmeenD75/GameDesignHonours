using UnityEngine;

[CreateAssetMenu(fileName = "NewDomainData", menuName = "Data/Domain")]
public class DomainData : ScriptableObject
{
    [Header("Basic Info")]
    public string domainName;

    [TextArea(2, 5)]
    public string description;

    public Sprite backgroundImage;

    [Header("Battle Settings")]
    public int startingTerrainHP = 40;
}