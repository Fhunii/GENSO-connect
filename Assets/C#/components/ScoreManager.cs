using UnityEngine;
using TMPro; // TextMeshProを使用するための名前空間

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // TextからTextMeshProUGUIに変更

    private int currentScore = 0;

    void Start()
    {
        ResetScore();
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "" + currentScore; // "score:"を追加
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    // 現在のスコアを取得するためのメソッドを追加
    public int GetScore()
    {
        return currentScore;
    }
}
