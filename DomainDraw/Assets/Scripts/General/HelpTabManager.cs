using UnityEngine;
using UnityEngine.UI;

public class HelpTabManager : MonoBehaviour
{
    [Header("Tabs and Pages")]
    public GameObject[] pages;
    public Button[] tabButtons;

    [Header("Visual Feedback")]
    public Color activeTabColor = Color.white;
    public Color inactiveTabColor = Color.gray;
    private void Start()
    {
        // When the scene loads, automatically open the first tab (Index 0)
        SwitchTab(0);
    }

    public void SwitchTab(int tabIndex)
    {
        // Loop through all our pages
        for (int i = 0; i < pages.Length; i++)
        {
            // If the current index matches the tab we clicked, turn the page ON (true). Otherwise, turn it OFF (false).
            pages[i].SetActive(i == tabIndex);

            // Update the button colors so the player knows which tab they are on
            if (tabButtons.Length > i && tabButtons[i] != null)
            {
                tabButtons[i].GetComponent<Image>().color = (i == tabIndex) ? activeTabColor : inactiveTabColor;
            }
        }
    }
}