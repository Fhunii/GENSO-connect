using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioClip gameBGM; // ゲームプレイ時のBGM

    private void Start()
    {
        // カウントダウンが終わり、START!が表示された後にBGMを再生
        AudioManager.Instance.PlayBGM(gameBGM);
    }

    public void OnSpriteConnected()
    {
        // スプライトがつながれたときの効果音を再生
        AudioManager.Instance.PlaySFX(AudioManager.Instance.spriteConnectedClip);
    }

    public void OnSpriteDestroyed()
    {
        // スプライトが消去されたときの効果音を再生
        AudioManager.Instance.PlaySFX(AudioManager.Instance.spriteDestroyedClip);
    }

    public void OnQuitButtonPressed()
    {
        // Quitボタンが押されたときの効果音を再生
        AudioManager.Instance.PlaySFX(AudioManager.Instance.quitButtonClip);
        AudioManager.Instance.StopBGM();
        // 他の処理...
    }

    public void OnOtherButtonPressed()
    {
        // その他のボタンが押されたときの効果音を再生
        AudioManager.Instance.PlaySFX(AudioManager.Instance.otherButtonClip);
    }
}
