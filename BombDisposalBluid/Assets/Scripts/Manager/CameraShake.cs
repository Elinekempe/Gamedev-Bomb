using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    private void Awake()
    {
        // De camera keert na het schudden terug naar deze positie.
        originalPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        // Een lopende shake wordt vervangen zodat effecten niet opstapelen.
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        // Willekeurige kleine offsets simuleren een korte impact op de camera.
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Herstel de exacte beginpositie om blijvende cameradrift te voorkomen.
        transform.localPosition = originalPos;
    }
}