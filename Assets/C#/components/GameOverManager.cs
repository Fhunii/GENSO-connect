using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Button quitButton;
    public Button returnToTitleButton;
    public TextMeshProUGUI scoreText;  // 既存のScoreTextを使用
    public TextMeshProUGUI highScoreText;
    public GameObject scorePanel;
    public RectTransform scoreTextTransform;  // ScoreTextのRectTransform

    private int score;
    private int highScore;
    private Vector2 originalScorePosition; // 元の位置を保存

    private void Start()
    {
        // 元のスコアテキストの位置を保存
        originalScorePosition = scoreTextTransform.anchoredPosition;

        // スコアとパネルを非表示にする
        scorePanel.SetActive(false);
        returnToTitleButton.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(false);

        // ハイスコアをロード
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore.ToString();

        // 終了ボタンにイベントを追加
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        returnToTitleButton.onClick.AddListener(OnReturnToTitleButtonClicked);
    }

    public void OnQuitButtonClicked()
    {
        // ゲーム終了時のスコアの表示とUIの設定
        score = FindObjectOfType<ScoreManager>().GetScore(); // スコア取得
        scoreText.text = "Score\n" + score.ToString();

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            highScoreText.text = "High Score: " + highScore.ToString();
        }

        // スコアの位置を中央に移動
        scoreTextTransform.anchoredPosition = new Vector2(0, 30);

        // スコアとパネルを表示
        scorePanel.SetActive(true);
        highScoreText.gameObject.SetActive(true);
        returnToTitleButton.gameObject.SetActive(true);
    }

    public void OnReturnToTitleButtonClicked()
    {
        // スコアテキストの位置を元に戻す
        scoreTextTransform.anchoredPosition = originalScorePosition;
        SceneManager.LoadScene("menu scene");
    }
}
