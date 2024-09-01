using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI startText;
    public float countdownDuration = 3f; // カウントダウンの時間（秒）
    public float startTextDisplayDuration = 1f; // START!の表示時間（秒）

    private bool isCountdownActive = false;

    void Start()
    {
        startText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        isCountdownActive = true;
        float elapsedTime = 0f;

        while (elapsedTime < countdownDuration)
        {
            float remainingTime = countdownDuration - elapsedTime;
            countdownText.text = Mathf.Ceil(remainingTime).ToString();
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        countdownText.gameObject.SetActive(false);
        startText.gameObject.SetActive(true);

        // START!の表示
        
        yield return new WaitForSeconds(startTextDisplayDuration);

        startText.gameObject.SetActive(false);
        isCountdownActive = false;
    }

    public bool IsCountdownActive()
    {
        return isCountdownActive;
    }
}
