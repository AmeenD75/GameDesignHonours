using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Flow")]
    public string backstoryScene = "Backstory";
    public string helpSceneName = "HelpScene";

    public void PlayGame()
    {
        MatchSetup.player1Race = null;
        MatchSetup.player2Race = null;
        MatchSetup.selectedDomain = null;
        SceneManager.LoadScene(backstoryScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }

    public void OpenHelp()
    {
        SceneManager.LoadScene(helpSceneName);
    }

}