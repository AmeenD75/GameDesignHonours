using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardPreviewPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image artImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private TMP_Text stateText;

    private void Start()
    {
        Hide();
    }

    public void Show(CardData card, bool isLocked, bool isInDeck)
    {
        if (card == null)
            return;

        if (root != null)
            root.SetActive(true);

        if (artImage != null)
            artImage.sprite = card.Image;

        if (titleText != null)
            titleText.text = card.Title;

        if (typeText != null)
            typeText.text = card.Type.ToString();

        if (descriptionText != null)
            descriptionText.text = card.Description;

        if (effectText != null)
            effectText.text = BuildEffectText(card);

        if (stateText != null)
        {
            if (isLocked)
                stateText.text = "Locked race card";
            else if (isInDeck)
                stateText.text = "Already in deck";
            else
                stateText.text = "Right click in library to add";
        }
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private string BuildEffectText(CardData card)
    {
        string result = "";

        if (card.DamageToEnemy > 0)
            result += "Deal " + card.DamageToEnemy + " damage\n";

        if (card.HealSelf > 0)
            result += "Heal " + card.HealSelf + "\n";

        if (card.BlockAmount > 0)
            result += "Gain " + card.BlockAmount + " block\n";

        if (card.SelfDamage > 0)
            result += "Take " + card.SelfDamage + " self-damage\n";

        if (card.TerrainDamage > 0)
            result += "Damage terrain by " + card.TerrainDamage + "\n";
        else if (card.TerrainDamage < 0)
            result += "Restore terrain by " + (-card.TerrainDamage) + "\n";

        if (string.IsNullOrWhiteSpace(result))
            result = "No effect";

        return result.TrimEnd();
    }
}