using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DomainSelectManager : MonoBehaviour
{
    [Header("Available Domains")]
    public DomainData[] availableDomains;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text domainNameText;
    public TMP_Text domainDescriptionText;
    public TMP_Text terrainHPText;
    public Image domainImage;
    public TMP_Text infoText;

    [Header("Scene Flow")]
    public string nextSceneName = "Battle1";

    private int currentDomainIndex = 0;

    private void Start()
    {
        MatchSetup.selectedDomain = null;
        currentDomainIndex = 0;
        RefreshUI();
    }

    public void NextDomain()
    {
        if (availableDomains == null || availableDomains.Length == 0)
            return;

        currentDomainIndex++;
        if (currentDomainIndex >= availableDomains.Length)
            currentDomainIndex = 0;

        RefreshUI();
    }

    public void PreviousDomain()
    {
        if (availableDomains == null || availableDomains.Length == 0)
            return;

        currentDomainIndex--;
        if (currentDomainIndex < 0)
            currentDomainIndex = availableDomains.Length - 1;

        RefreshUI();
    }

    public void ConfirmDomain()
    {
        if (availableDomains == null || availableDomains.Length == 0)
            return;

        MatchSetup.selectedDomain = availableDomains[currentDomainIndex];
        SceneManager.LoadScene(nextSceneName);
    }

    private void RefreshUI()
    {
        if (availableDomains == null || availableDomains.Length == 0)
        {
            if (titleText != null)
                titleText.text = "No domains available";

            if (domainNameText != null)
                domainNameText.text = "";

            if (domainDescriptionText != null)
                domainDescriptionText.text = "";

            if (terrainHPText != null)
                terrainHPText.text = "";

            if (infoText != null)
                infoText.text = "";

            if (domainImage != null)
            {
                domainImage.sprite = null;
                domainImage.enabled = false;
            }

            return;
        }

        DomainData domain = availableDomains[currentDomainIndex];

        if (titleText != null)
            titleText.text = "Choose Domain";

        if (domainNameText != null)
            domainNameText.text = domain.domainName;

        if (domainDescriptionText != null)
            domainDescriptionText.text = domain.description;

        if (terrainHPText != null)
            terrainHPText.text = "Domain HP: " + domain.startingTerrainHP;

        if (infoText != null)
        {
            string p1 = MatchSetup.player1Race != null ? MatchSetup.player1Race.raceName : "None";
            string p2 = MatchSetup.player2Race != null ? MatchSetup.player2Race.raceName : "None";
            infoText.text = "Player 1: " + p1 + "\nPlayer 2: " + p2;
        }

        if (domainImage != null)
        {
            domainImage.sprite = domain.backgroundImage;
            domainImage.enabled = domain.backgroundImage != null;
        }
    }
}