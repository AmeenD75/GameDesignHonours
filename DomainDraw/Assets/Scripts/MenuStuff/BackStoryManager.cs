using UnityEngine;
using UnityEngine.SceneManagement;



public class BackStoryManager : MonoBehaviour
{
    [Header("Scene Flow")]
    public string characterSelection = "CharacterSelection";

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(characterSelection);
    }
}
