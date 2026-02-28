using UnityEngine;

public class Card 
{
    private readonly CardData data;

    public string Title => data.name;
    public string ctype => data.Type;
    public string description => data.Description;
    public Sprite image => data.Image;



    public Card(CardData data)
    {
        this.data = data;
    }
}
