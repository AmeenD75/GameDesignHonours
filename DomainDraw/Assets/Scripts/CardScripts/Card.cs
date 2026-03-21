using System.Collections.Generic;
using UnityEngine;

public class Card
{
    private readonly CardData data;

    public string Title => data.Title;
    public CardType Type => data.Type;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Sprite CardBack => data.CardBack;

    public int DamageToEnemy => data.DamageToEnemy;
    public int HealSelf => data.HealSelf;
    public int TerrainDamage => data.TerrainDamage;
    public int SelfDamage => data.SelfDamage;
    public int BlockAmount => data.BlockAmount;

    public string EffectText => BuildEffectText();

    public Card(CardData data)
    {
        this.data = data;
    }

    private string BuildEffectText()
    {
        List<string> effects = new();

        if (DamageToEnemy > 0)
            effects.Add("Deal " + DamageToEnemy + " damage");

        if (HealSelf > 0)
            effects.Add("Heal " + HealSelf);

        if (SelfDamage > 0)
            effects.Add("Take " + SelfDamage + " self-damage");

        if (BlockAmount > 0)
            effects.Add("Gain " + BlockAmount + " block");

        if (TerrainDamage > 0)
            effects.Add("Damage terrain by " + TerrainDamage);
        else if (TerrainDamage < 0)
            effects.Add("Restore terrain by " + (-TerrainDamage));

        if (effects.Count == 0)
            return "No effect";

        return string.Join("\n", effects);
    }
}