using System.Collections;
using UnityEngine;

public class CharacterVisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Animation Settings")]
    [SerializeField] private float frameRate = 0.07f;
    [SerializeField] private float moveDistance = 0.6f;

    private Sprite idleSprite;
    private Sprite[] attackFrames;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }
    public void Setup(RaceData race, bool isPlayer1)
    {
        if (race == null || spriteRenderer == null)
            return;

        if (isPlayer1)
        {
            idleSprite = race.player1Idle;
            attackFrames = race.player1AttackFrames;
        }
        else
        {
            idleSprite = race.player2Idle;
            attackFrames = race.player2AttackFrames;
        }

        spriteRenderer.sprite = idleSprite;
    }

    public IEnumerator PlayAttack()
    {
        yield return Move(Vector3.right * moveDistance);

        if (attackFrames != null && attackFrames.Length > 0)
        {
            for (int i = 0; i < attackFrames.Length; i++)
            {
                spriteRenderer.sprite = attackFrames[i];
                yield return new WaitForSeconds(frameRate);
            }
        }

        yield return Move(Vector3.left * moveDistance);

        ResetIdle();
    }

    public IEnumerator PlayHit()
    {
        yield return Shake();
    }

    public IEnumerator PlayBuff()
    {
        transform.localScale = Vector3.one * 1.15f;
        yield return new WaitForSeconds(0.15f);
        transform.localScale = Vector3.one;
    }

    private IEnumerator Move(Vector3 offset)
    {
        Vector3 start = transform.position;
        Vector3 target = start + offset;

        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator Shake()
    {
        float duration = 0.2f;
        float magnitude = 0.1f;

        Vector3 original = transform.position;

        float t = 0f;

        while (t < duration)
        {
            transform.position = original + Random.insideUnitSphere * magnitude;
            transform.position = new Vector3(transform.position.x, original.y, original.z);

            t += Time.deltaTime;
            yield return null;
        }

        transform.position = original;
    }

    private void ResetIdle()
    {
        spriteRenderer.sprite = idleSprite;
        transform.position = originalPosition;
    }
}