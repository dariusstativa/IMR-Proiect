using System.Collections;
using UnityEngine;

public class CPRCameraShake : MonoBehaviour
{
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.05f; 

    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float t = 0f;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;

           
            Vector3 offset = Random.insideUnitSphere * shakeStrength;
            offset.z = 0; 

            transform.localPosition = originalPos + offset;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
