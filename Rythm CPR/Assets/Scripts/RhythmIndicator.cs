using System.Collections;
using UnityEngine;

public class RhythmIndicator : MonoBehaviour
{
    [Header("UI")]
    public RectTransform needle;

    [Header("Feedback UI")]
    public GameObject feedbackPopup;
    public TMPro.TextMeshProUGUI feedbackText;

    [Header("Screen Feedback")]
    public CPRScreenFeedback screenFeedback;
    public int mistakesForFlash = 3;

    [Header("Camera Shake")]
    public CPRCameraShake cameraShake;

    [Header("Timing CPR")]
    public float idealBPM = 110f;
    public float perfectWindowPercent = 0.10f;
    public float goodWindowPercent = 0.25f;

    [Header("Angles")]
    public float badZoneAngle = -150f;        // TOO SLOW
    public float perfectCenterAngle = -300f;  // PERFECT
    public float tooFastRedAngle = -320f;     // TOO FAST

    [Header("Motion")]
    public float fallSpeed = 12f;
    public float perfectBoost = 0.4f;
    public float goodBoost = 0.25f;
    public float fastPenaltyBoost = 0.5f;

    [Header("Scoring")]
    public int scorePerfect = 2;
    public int scoreGood = 1;
    public int scoreTooFast = -1;
    public int scoreTooSlow = -1;

    public int score;

    private float currentAngle;
    private float targetAngle;
    private float lastPressTime = -999f;
    private int pressCount;
    private int mistakeStreak;

    private Coroutine feedbackRoutine;

    private enum RhythmErrorState { None, TooSlow, TooFast }
    private RhythmErrorState errorState = RhythmErrorState.None;

    private float IdealInterval => 60f / idealBPM;

    private void Start()
    {
        currentAngle = targetAngle = perfectCenterAngle;
        feedbackPopup?.SetActive(false);
        ApplyRotation();
    }

    private void Update()
    {
        if (pressCount == 0)
        {
            ApplyRotation();
            return;
        }

        float dt = Time.deltaTime;

        targetAngle = Mathf.MoveTowards(targetAngle, badZoneAngle, fallSpeed * dt);
        targetAngle = ClampByErrorState(targetAngle);

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, 5f * dt);
        currentAngle = ClampByErrorState(currentAngle);

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        needle.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public void RegisterCompression()
    {
        float now = Time.time;
        float interval = (lastPressTime > 0f) ? now - lastPressTime : IdealInterval;

        float diffPercent = (interval - IdealInterval) / IdealInterval;
        float absPercent = Mathf.Abs(diffPercent);

        if (absPercent <= perfectWindowPercent)
        {
            errorState = RhythmErrorState.None;
            mistakeStreak = 0;
            score += scorePerfect;
            targetAngle = Mathf.Lerp(targetAngle, perfectCenterAngle, perfectBoost);
            ShowFeedback("Perfect!", Color.green);
        }
        else if (absPercent <= goodWindowPercent)
        {
            errorState = RhythmErrorState.None;
            mistakeStreak = 0;
            score += scoreGood;
            targetAngle = Mathf.Lerp(targetAngle, perfectCenterAngle, goodBoost);
            ShowFeedback("Good", new Color(0.6f, 0.9f, 1f));
        }
        else if (diffPercent < 0f) // TOO FAST
        {
            if (currentAngle > perfectCenterAngle)
            {
                mistakeStreak++;                 // 🔴 CRUCIAL
                score += scoreTooFast;

                ShowFeedback("Too fast!", Color.red);
                TryTriggerScreenFlash();
                return;
            }

            mistakeStreak++;
            score += scoreTooFast;
            errorState = RhythmErrorState.TooFast;

            float strength = Mathf.Clamp01(fastPenaltyBoost + mistakeStreak * 0.15f);
            targetAngle = Mathf.Lerp(targetAngle, tooFastRedAngle, strength);

            ShowFeedback("Too fast!", Color.red);
            TryTriggerScreenFlash();
        }
        else // TOO SLOW
        {
            mistakeStreak++;
            score += scoreTooSlow;
            errorState = RhythmErrorState.TooSlow;

            float strength = Mathf.Clamp01(0.3f + mistakeStreak * 0.15f);
            targetAngle = Mathf.Lerp(targetAngle, badZoneAngle, strength);

            ShowFeedback("Too slow", new Color(1f, 0.6f, 0f));
            TryTriggerScreenFlash();
        }

        lastPressTime = now;
        pressCount++;
    }

    private float ClampByErrorState(float angle)
    {
        const float margin = 2f;

        if (errorState == RhythmErrorState.TooSlow)
            return Mathf.Max(angle, perfectCenterAngle + margin);

        if (errorState == RhythmErrorState.TooFast)
            return Mathf.Min(angle, perfectCenterAngle - margin);

        return angle;
    }

    private void TryTriggerScreenFlash()
    {
        if (mistakeStreak >= mistakesForFlash)
        {
            screenFeedback?.PulseError();
            cameraShake?.Shake();
            mistakeStreak = 0;
        }
    }

    private void ShowFeedback(string msg, Color color)
    {
        if (feedbackPopup == null || feedbackText == null) return;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(FeedbackRoutine(msg, color));
    }

    private IEnumerator FeedbackRoutine(string msg, Color color)
    {
        feedbackText.text = msg;
        feedbackText.color = color;
        feedbackText.alpha = 1f;

        RectTransform rt = feedbackPopup.GetComponent<RectTransform>();
        Vector3 startPos = new Vector3(0f, 90f, 0f);
        Vector3 endPos = startPos + Vector3.up * 80f;

        feedbackPopup.SetActive(true);
        rt.localPosition = startPos;

        float t = 0f;
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            float k = t / 0.6f;
            rt.localPosition = Vector3.Lerp(startPos, endPos, k);
            feedbackText.alpha = 1f - k;
            yield return null;
        }

        feedbackPopup.SetActive(false);
        feedbackRoutine = null;
    }

    public void ResetIndicator()
    {
        lastPressTime = -999f;
        pressCount = 0;
        score = 0;
        mistakeStreak = 0;
        errorState = RhythmErrorState.None;

        currentAngle = targetAngle = perfectCenterAngle;
        ApplyRotation();

        feedbackPopup?.SetActive(false);
    }
}
