using UnityEngine;
using System.Collections;

public class TileAnimation : MonoBehaviour
{
    public void AnimateTile(float delay)
    {
        StartCoroutine(AnimateTileCoroutine(delay));
    }

    private IEnumerator AnimateTileCoroutine(float delay)
    {
        transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(delay);

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}