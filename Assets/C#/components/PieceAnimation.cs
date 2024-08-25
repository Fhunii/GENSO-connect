using UnityEngine;
using System.Collections;

public class PieceAnimation : MonoBehaviour
{
    public void FadeIn(float delay)
    {
        StartCoroutine(FadeInCoroutine(delay));
    }

    private IEnumerator FadeInCoroutine(float delay)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color startColor = sr.color;
        startColor.a = 0;
        sr.color = startColor;

        yield return new WaitForSeconds(delay);

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Color newColor = sr.color;
            newColor.a = Mathf.Lerp(0, 1, t);
            sr.color = newColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Color finalColor = sr.color;
        finalColor.a = 1;
        sr.color = finalColor;
    }
}