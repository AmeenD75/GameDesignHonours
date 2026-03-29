using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Available Races")]
    public RaceData[] availableRaces;

    private List<RaceData> selectableRaces = new();

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text raceNameText;
    public TMP_Text raceDescriptionText;
    public Image racePortraitImage;
    public TMP_Text infoText;
    public TMP_Text continueButtonText;

    [Header("Scene Flow")]
    public string nextSceneName = "DeckModeSelectScreen";
    public string mainMenuSceneName = "MainMenu";

    private int currentRaceIndex = 0;
    private bool choosingPlayer1 = true;

    private void Start()
    {
        MatchSetup.player1Race = null;
        MatchSetup.player2Race = null;

        choosingPlayer1 = true;
        currentRaceIndex = 0;

        selectableRaces = new List<RaceData>(availableRaces);

        RefreshUI();
    }

    public void NextRace()
    {
        if (selectableRaces.Count == 0)
            return;

        currentRaceIndex++;
        if (currentRaceIndex >= selectableRaces.Count)
            currentRaceIndex = 0;

        RefreshUI();
    }

    public void PreviousRace()
    {
        if (selectableRaces.Count == 0)
            return;

        currentRaceIndex--;
        if (currentRaceIndex < 0)
            currentRaceIndex = selectableRaces.Count - 1;

        RefreshUI();
    }

    public void ConfirmRace()
    {
        if (selectableRaces.Count == 0)
            return;

        RaceData selectedRace = selectableRaces[currentRaceIndex];

        if (choosingPlayer1)
        {
            MatchSetup.player1Race = selectedRace;

            selectableRaces.Remove(selectedRace);

            choosingPlayer1 = false;
            currentRaceIndex = 0;

            RefreshUI();
        }
        else
        {
            MatchSetup.player2Race = selectedRace;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    private void RefreshUI()
    {
        if (selectableRaces.Count == 0)
        {
            if (titleText != null)
                titleText.text = "No races available";

            if (raceNameText != null)
                raceNameText.text = "";

            if (raceDescriptionText != null)
                raceDescriptionText.text = "";

            if (infoText != null)
                infoText.text = "";

            if (continueButtonText != null)
                continueButtonText.text = "Confirm";

            if (racePortraitImage != null)
            {
                racePortraitImage.sprite = null;
                racePortraitImage.enabled = false;
            }

            return;
        }

        RaceData race = selectableRaces[currentRaceIndex];

        if (titleText != null)
        {
            titleText.text = choosingPlayer1
                ? "Player 1 Race Selection"
                : "Player 2 Race Selection";
        }

        if (raceNameText != null)
            raceNameText.text = race.raceName;

        if (raceDescriptionText != null)
            raceDescriptionText.text = race.description;

        if (racePortraitImage != null)
        {
            racePortraitImage.sprite = race.portrait;
            racePortraitImage.enabled = race.portrait != null;
        }

        if (infoText != null)
        {
            if (choosingPlayer1)
                infoText.text = "Player 1, choose your race.";
            else
                infoText.text = "Player 1 chose " + MatchSetup.player1Race.raceName + ".";
        }

        if (continueButtonText != null)
        {
            continueButtonText.text = choosingPlayer1
                ? "Confirm Player 1"
                : "Start Battle";
        }
    }
}