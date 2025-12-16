using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CPRScreenFeedback : MonoBehaviour
{
    [Header("Overlay")]
    public Image overlayImage;

    [Header("Pulse Flash Settings")]
    [Range(0f, 1f)] public float maxAlpha = 0.35f;
    public float fadeInTime = 0.1f;
    public float fadeOutTime = 0.3f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.15f;
    public float shakeStrength = 12f; // pixeli

    private Coroutine pulseRoutine;
    private RectTransform rt;
    private Vector2 originalPos;   // <-- Vector2, nu Vector3

    private void Awake()
    {
        if (overlayImage == null)
            overlayImage = GetComponent<Image>();

        if (overlayImage == null)
        {
            Debug.LogError("CPRScreenFeedback: overlayImage nu este setat!");
            enabled = false;
            return;
        }

        rt = overlayImage.GetComponent<RectTransform>();
        originalPos = rt.anchoredPosition;

        // fullscreen stretch în canvas
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // pornește transparent
        var c = overlayImage.color;
        c.a = 0f;
        overlayImage.color = c;
    }

    /// <summary>
    /// Apelează asta când utilizatorul greșește (too fast / too slow).
    /// Face flash roșu + shake simultan.
    /// </summary>
    public void PulseError()
    {
        if (overlayImage == null)
            return;

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(FlashAndShake());
    }

    private IEnumerator FlashAndShake()
    {
        Color c = overlayImage.color;
        float timer = 0f;

        while (timer < fadeInTime + fadeOutTime)
        {
            timer += Time.unscaledDeltaTime;

            // ----- FADE -----
            float k;
            if (timer < fadeInTime)
                k = timer / Mathf.Max(0.0001f, fadeInTime);
            else
                k = 1f - Mathf.Clamp01((timer - fadeInTime) / Mathf.Max(0.0001f, fadeOutTime));

            c.a = k * maxAlpha;
            overlayImage.color = c;

            // ----- SHAKE -----
            float shakeT = timer / Mathf.Max(0.0001f, shakeDuration);
            if (shakeT < 1f)
            {
                Vector2 shakeOffset = Random.insideUnitCircle * shakeStrength;
                rt.anchoredPosition = originalPos + shakeOffset;
            }
            else
            {
                rt.anchoredPosition = originalPos;
            }

            yield return null;
        }

        // reset la final
        c.a = 0f;
        overlayImage.color = c;
        rt.anchoredPosition = originalPos;
        pulseRoutine = null;
    }
}
