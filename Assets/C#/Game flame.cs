using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlame : MonoBehaviour
{
    private SpriteRenderer objectRenderer;
    private Color targetColor;
    private float elapsedTime;
    private readonly float fadeDuration = 0.5f;
    private bool isFading = false;

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();
        objectRenderer.sprite = Resources.Load<Sprite>("shapes2d/shaps");
        objectRenderer.color = new Color(0x6D / 255f, 0xD8 / 255f, 0xDD / 255f, 0f);
    }

    // Update is called once per frame
    private bool hasStarted = false;

    void Update()
    {
        if (!hasStarted && SceneManager.GetActiveScene().name == "game scene" && !isFading)
        {
            hasStarted = true;
            StartCoroutine(FadeInObject());
        }
    }

    IEnumerator FadeInObject()
    {
        isFading = true;

        targetColor = new Color(0x6D / 255f, 0xD8 / 255f, 0xDD / 255f, 1f);

        elapsedTime = 0f;

        yield return new WaitForSeconds(0.5f); // 0.5秒待機

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            objectRenderer.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), targetColor, t);
            yield return null;
        }

        isFading = false;
    }
}
