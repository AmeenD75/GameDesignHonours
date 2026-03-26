using System.Collections;
using UnityEngine;

public class CharacterVisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private Sprite buffSprite;

    [Header("Animation Settings")]
    [SerializeField] private float moveAmount = 0.5f;
    [SerializeField] private float duration = 0.2f;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    public void Setup(RaceData race)
    {
        if (race == null || spriteRenderer == null)
            return;

        spriteRenderer.sprite = race.battleSprite;

        // Optional: set idle = battle sprite
        idleSprite = race.battleSprite;
    }

    public void PlayAttack()
    {
        StartCoroutine(AttackAnimation());
    }

    public void PlayHit()
    {
        StartCoroutine(HitAnimation());
    }

    public void PlayBuff()
    {
        StartCoroutine(BuffAnimation());
    }

    private IEnumerator AttackAnimation()
    {
        spriteRenderer.sprite = attackSprite != null ? attackSprite : idleSprite;

        yield return Move(Vector3.right * moveAmount);

        yield return Move(Vector3.left * moveAmount);

        ResetToIdle();
    }

    private IEnumerator HitAnimation()
    {
        spriteRenderer.sprite = hitSprite != null ? hitSprite : idleSprite;

        yield return Move(Vector3.left * moveAmount * 0.5f);
        yield return Move(Vector3.right * moveAmount * 0.5f);

        ResetToIdle();
    }

    private IEnumerator BuffAnimation()
    {
        spriteRenderer.sprite = buffSprite != null ? buffSprite : idleSprite;

        transform.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(duration);
        transform.localScale = Vector3.one;

        ResetToIdle();
    }

    private IEnumerator Move(Vector3 offset)
    {
        Vector3 start = transform.position;
        Vector3 target = start + offset;

        float t = 0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private void ResetToIdle()
    {
        transform.position = originalPosition;
        spriteRenderer.sprite = idleSprite;
    }
}