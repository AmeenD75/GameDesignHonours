using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // <-- Required for the new Input System

public class BackStoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text storyTextDisplay;
    public CanvasGroup screenFadeGroup; // Attach to the Canvas or a full-screen Panel

    [Header("Story Settings")]
    [TextArea(3, 5)]
    public string[] storyChunks;
    public float typingSpeed = 0.03f; // Speed of the typewriter effect

    [Header("Scene Transition")]
    public string characterSelection = "CharacterSelection";
    public float fadeDuration = 1.5f;

    private int currentIndex = 0;
    private bool isTransitioning = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(characterSelection);
    }

    private void Start()
    {
        // Ensure the screen is fully visible at the start
        if (screenFadeGroup != null)
            screenFadeGroup.alpha = 1f;

        // Display the first chunk of text with the typewriter effect
        if (storyChunks.Length > 0 && storyTextDisplay != null)
        {
            typingCoroutine = StartCoroutine(TypeText(storyChunks[0]));
        }
    }

    private void Update()
    {
        // Listen for a left mouse click using the new Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !isTransitioning)
        {
            if (isTyping)
            {
                // If the text is still spelling out, click to skip to the end of the paragraph
                StopCoroutine(typingCoroutine);
                storyTextDisplay.text = storyChunks[currentIndex];
                isTyping = false;
            }
            else
            {
                // If the text is fully displayed, click to advance to the next story chunk
                AdvanceStory();
            }
        }
    }

    private void AdvanceStory()
    {
        currentIndex++;

        // If we still have story left to show, type out the next text chunk
        if (currentIndex < storyChunks.Length)
        {
            typingCoroutine = StartCoroutine(TypeText(storyChunks[currentIndex]));
        }
        // If we reached the end of the text array, start the fade out
        else
        {
            StartCoroutine(FadeOutAndChangeScene());
        }
    }

    // Coroutine to spell out the text letter by letter
    private IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        storyTextDisplay.text = "";

        // Loop through each character and add it to the display
        foreach (char letter in textToType.ToCharArray())
        {
            storyTextDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private IEnumerator FadeOutAndChangeScene()
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        // Gradually fade out by dropping the alpha to 0
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            if (screenFadeGroup != null)
            {
                screenFadeGroup.alpha = 1f - (elapsedTime / fadeDuration);
            }

            yield return null;
        }

        // Once the fade is done, load the character selection scene
        SceneManager.LoadScene(characterSelection);
    }
}