//using UnityEngine;

//public enum CardType { Attack, Defense, Support }


//[CreateAssetMenu(menuName = "Data/Card")]
//public class CardData : ScriptableObject
//{
//    //public string cardName;
//    //[TextArea] public string description;
//    //public CardType type;

//    //public int damageToEnemy;     // reduces opponent HP
//    //public int healSelf;          // restores your HP
//    //public int terrainDamage;     // reduces terrain HP (negative can restore)

//    //// Optional: later you can add cost, status effects, etc.
//    ///

//    //[field: SerializeField] public string name { get; private set; }
//    [field: SerializeField] public string Description { get; private set; }
//    [field : SerializeField] public string Type { get; private set; }
//    [field: SerializeField] public Sprite Image { get; private set; }

//}

using UnityEngine;

public enum CardType { Attack, Defense, Support }

[CreateAssetMenu(menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public CardType Type { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }

    [field: SerializeField] public int DamageToEnemy { get; private set; }
    [field: SerializeField] public int HealSelf { get; private set; }
    [field: SerializeField] public int TerrainDamage { get; private set; }
    [field: SerializeField] public int SelfDamage { get; private set; }
}