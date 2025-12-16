using UnityEngine;
using TMPro;

public class FeedbackPopup : MonoBehaviour
{
    public TMP_Text text;
    public float duration = 0.35f;

    private float timer;

    public void Show(string message, Color color)
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        text.text = message;
        text.color = color;

        timer = duration;
        gameObject.SetActive(true);

        text.alpha = 1f;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (timer <= 0f) return;

        timer -= Time.deltaTime;

        // fade out
        float t = Mathf.Clamp01(timer / duration);
        text.alpha = t;

        if (timer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
