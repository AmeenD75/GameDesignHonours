//using UnityEngine;

//public class Card 
//{
//    private readonly CardData data;

//    public string Title => data.name;
//    public string ctype => data.Type;
//    public string description => data.Description;
//    public Sprite image => data.Image;



//    public Card(CardData data)
//    {
//        this.data = data;
//    }
//}


using UnityEngine;

public class Card
{
    private readonly CardData data;

    public string Title => data.Title;
    public CardType Type => data.Type;
    public string Description => data.Description;
    public Sprite Image => data.Image;

    public int DamageToEnemy => data.DamageToEnemy;
    public int HealSelf => data.HealSelf;
    public int TerrainDamage => data.TerrainDamage;
    public int SelfDamage => data.SelfDamage;

    public int BlockAmount => data.BlockAmount;

    public Card(CardData data)
    {
        this.data = data;
    }
}