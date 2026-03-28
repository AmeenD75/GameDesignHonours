using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text hpText;

    [Header("Animation")]
    [SerializeField] private float animationSpeed = 5f;

    private float targetFill = 1f;
    private Coroutine animationRoutine;

    public void SetInstant(int current, int max)
    {
        float value = (float)current / max;
        targetFill = value;

        if (fillImage != null)
            fillImage.fillAmount = value;

        UpdateText(current, max);
    }

    public void SetAnimated(int current, int max)
    {
        float value = (float)current / max;
        targetFill = value;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(AnimateFill());
        fillImage.color = Color.Lerp(Color.red, Color.green, targetFill);
        UpdateText(current, max);
    }

    private IEnumerator AnimateFill()
    {
        while (Mathf.Abs(fillImage.fillAmount - targetFill) > 0.001f)
        {
            fillImage.fillAmount = Mathf.Lerp(
                fillImage.fillAmount,
                targetFill,
                Time.deltaTime * animationSpeed
            );

            yield return null;
        }

        fillImage.fillAmount = targetFill;
    }

    private void UpdateText(int current, int max)
    {
        if (hpText != null)
            hpText.text = current + " / " + max;
    }
}