using UnityEngine;
using UnityEngine.SceneManagement;


public class HelpSceneManager : MonoBehaviour
{
    [Header("Scene Flow")]
    public string mainMenuScene = "MainMenu";

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
