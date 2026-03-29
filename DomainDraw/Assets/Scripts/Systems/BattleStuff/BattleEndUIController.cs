using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEndUIController : MonoBehaviour
{
    [Header("End Screen")]
    public GameObject endPanel;
    public TMP_Text resultText;

    [Header("Scene Flow")]
    public string battleSceneName = "Battle1";
    public string mainMenuSceneName = "MainMenu";

    [Header("Audio")]
    public AudioSource endScreenAudioSource;
    public AudioClip winClip;  
    public AudioClip loseClip;  

    private void Start()
    {
        HideEndScreen();
    }

    public void ShowEndScreen(string message, bool isWin)
    {
        if (endPanel != null)
            endPanel.SetActive(true);

        if (resultText != null)
            resultText.text = message;

        if (endScreenAudioSource != null)
        {
            endScreenAudioSource.clip = isWin ? winClip : loseClip;
            endScreenAudioSource.Play();
        }
    }

    public void HideEndScreen()
    {
        if (endPanel != null)
            endPanel.SetActive(false);
    }

    public void RestartBattle()
    {
        SceneManager.LoadScene(battleSceneName);
    }

    public void ReturnToMainMenu()
    {
        MatchSetup.player1Race = null;
        MatchSetup.player2Race = null;
        MatchSetup.selectedDomain = null;

        SceneManager.LoadScene(mainMenuSceneName);

    }
}