using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewClickBlocker : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private DeckCardPreviewPanel previewPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (previewPanel != null)
            previewPanel.Hide();
    }
}