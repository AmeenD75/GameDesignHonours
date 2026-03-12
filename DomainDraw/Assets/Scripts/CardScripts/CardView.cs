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
//        description.text = card.description;
//        type.text = card.ctype;
//        imageSR.sprite = card.image;

//    }

//    //void OnMouseEnter()
//    //{
//    //    wrapper.SetActive(false);
//    //    Vector3 pos = new(transform.position.x, -2, 0);
//    //    CardViewHoverSystem.Instance.Show(Card, pos);

//    //    Debug.Log("Card hover ENTER: " + name);
//    //    Debug.Assert(CardViewHoverSystem.Instance != null, "No CardViewHoverSystem in the scene");
//    //    Debug.Assert(Card != null, "Card is null on hover (Setup not run?)");

//    //}

//    //void OnMouseExit()
//    //{
//    //    CardViewHoverSystem.Instance.Hide();
//    //    wrapper.SetActive(true);
//    //}

//   public void HoverEnter()
//    {
//        // DO NOT disable the object containing the collider
//        wrapper.SetActive(false);  // risky if wrapper includes collider/visuals
//        Vector3 pos = new(transform.position.x, -2, 0);
//        CardViewHoverSystem.Instance.Show(Card, pos);


//    }

//    public void HoverExit()
//    {
//        CardViewHoverSystem.Instance.Hide();
//         wrapper.SetActive(true);
//    }

//}

using TMPro;
using UnityEngine;
public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text type;
    [SerializeField] private TMP_Text description;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;

    public Card Card { get; private set; }

    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        type.text = card.Type.ToString();
        imageSR.sprite = card.Image;
    }

    public void HoverEnter()
    {
        if (wrapper != null)
            wrapper.SetActive(false);

        if (CardViewHoverSystem.Instance != null && Card != null)
        {
            Vector3 pos = new(transform.position.x, -2, 0);
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
        transform.localScale = selected ? Vector3.one * 1.2f : Vector3.one;
    }
}