using System;
using System.Collections;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] spinFrames;
    [SerializeField] private Sprite[] diceFaces;

    [SerializeField] private float frameRate = 0.05f;
    [SerializeField] private float spinDuration = 1.2f;

    public IEnumerator RollDice(Action<int> onComplete)
    {
        float timer = 0f;
        int frame = 0;

        while (timer < spinDuration)
        {
            spriteRenderer.sprite = spinFrames[frame];

            frame++;
            if (frame >= spinFrames.Length)
                frame = 0;

            timer += frameRate;
            yield return new WaitForSeconds(frameRate);
        }

        int result = UnityEngine.Random.Range(1, 9);

        spriteRenderer.sprite = diceFaces[result - 1];

        yield return new WaitForSeconds(0.3f);

        onComplete?.Invoke(result);
    }
}