using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Flow")]
    public string characterSelectSceneName = "CharacterSelection";

    public void PlayGame()
    {
        MatchSetup.player1Race = null;
        MatchSetup.player2Race = null;
        MatchSetup.selectedDomain = null;
        SceneManager.LoadScene(characterSelectSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}