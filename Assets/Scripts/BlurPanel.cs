using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("UI/Blur Panel")]
public class BlurPanel : Image
{
    public bool animate;
    public float time = 0.5f;

    private CanvasGroup cv;
    private Coroutine animationCoroutine;


    protected override void Reset()
    {
        base.Reset();
        color = new Color(.5f, .5f, .5f, .05f);
    }


    protected override void Awake()
    {
        cv = GetComponent<CanvasGroup>();
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isEditor && animate)
        {
            material.SetFloat("_size", 0);
            cv.alpha = 0;

            // Stop any existing animation
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            // Start new animation
            animationCoroutine = StartCoroutine(AnimateBlur(0, 1, time));
        }
    }

    private IEnumerator AnimateBlur(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Use smoothstep interpolation for more natural easing
            t = t * t * (3f - 2f * t);

            float currentValue = Mathf.Lerp(startValue, endValue, t);
            UpdateBlur(currentValue);

            yield return null;
        }

        // Ensure we end up exactly at the target value
        UpdateBlur(endValue);
    }

    private void UpdateBlur(float value)
    {
        material.SetFloat("_size", value);
        cv.alpha = value;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
    }
}