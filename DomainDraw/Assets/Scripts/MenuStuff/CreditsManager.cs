using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("Scene Flow")]
    public string previousScene = "MainMenu";

    public void BackToMenu()
    {
        SceneManager.LoadScene(previousScene);
    }
}
