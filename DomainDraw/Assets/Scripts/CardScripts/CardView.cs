//using TMPro;
//using UnityEngine;
//public class CardView : MonoBehaviour
//{
//    [SerializeField] private TMP_Text title;
//    [SerializeField] private TMP_Text type;
//    [SerializeField] private TMP_Text description;
//    [SerializeField] private SpriteRenderer imageSR;
//    [SerializeField] private GameObject wrapper;

//    public Card Card { get; private set; }

//    public void Setup(Card card)
//    {
//        Card = card;
//        title.text = card.Title;
//        description.text = card.Description;
//        type.text = card.Type.ToString();
//        imageSR.sprite = card.Image;
//    }

//    public void HoverEnter()
//    {
//        if (wrapper != null)
//            wrapper.SetActive(false);

//        if (CardViewHoverSystem.Instance != null && Card != null)
//        {
//            Vector3 pos = new(transform.position.x, -2, 0);
//            CardViewHoverSystem.Instance.Show(Card, pos);
//        }
//    }

//    public void HoverExit()
//    {
//        if (CardViewHoverSystem.Instance != null)
//            CardViewHoverSystem.Instance.Hide();

//        if (wrapper != null)
//            wrapper.SetActive(true);
//    }

//    public void SetSelected(bool selected)
//    {
//        transform.localScale = selected ? Vector3.one * 1.2f : Vector3.one;
//    }
//}


using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text effectText;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private SpriteRenderer wrapperBackSR;
    [SerializeField] private SpriteRenderer typeIconSR;

    [Header("Type Icons")]
    [SerializeField] private Sprite attackIcon;
    [SerializeField] private Sprite defenseIcon;
    [SerializeField] private Sprite supportIcon;

    [Header("Hover / Selection")]
    [SerializeField] private GameObject wrapper;
    [SerializeField] private float selectedScale = 1.2f;

    public Card Card { get; private set; }

    private Vector3 defaultScale;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void Setup(Card card)
    {
        Card = card;

        if (description != null)
            title.text = card.Title;

        if (description != null)
            description.text = card.Description;

        if (effectText != null)
            effectText.text = card.EffectText;

        if (imageSR != null)
            imageSR.sprite = card.Image;

        if (wrapperBackSR != null)
            wrapperBackSR.sprite = card.CardBack;

        if (typeIconSR != null)
            typeIconSR.sprite = GetTypeIcon(card.Type);

        SetSelected(false);
    }

    public void HoverEnter()
    {
        if (wrapper != null)
            wrapper.SetActive(false);

        if (CardViewHoverSystem.Instance != null && Card != null)
        {
            Vector3 pos = new Vector3(transform.position.x, -2f, 0f);
            CardViewHoverSystem.Instance.Show(Card, pos);
        }
    }

    public void HoverExit()
    {
        if (CardViewHoverSystem.Instance != null)
            CardViewHoverSystem.Instance.Hide();

        if (wrapper != null)
            wrapper.SetActive(true);
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? defaultScale * selectedScale : defaultScale;
    }

    private Sprite GetTypeIcon(CardType type)
    {
        switch (type)
        {
            case CardType.Attack:
                return attackIcon;

            case CardType.Defense:
                return defenseIcon;

            case CardType.Support:
                return supportIcon;

            default:
                return null;
        }
    }
}