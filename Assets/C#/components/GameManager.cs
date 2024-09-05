using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioClip gameBGM; // ゲームプレイ時のBGM

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager instance is not found!");
            return;
        }

        // カウントダウンが終わり、START!が表示された後にBGMを再生
        AudioManager.Instance.PlayBGM(gameBGM);
    }

    public void OnSpriteConnected()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.spriteConnectedClip);
        }
    }

    public void OnSpriteDestroyed()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.spriteDestroyedClip);
        }
    }

    public void OnQuitButtonPressed()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.quitButtonClip);
            AudioManager.Instance.StopBGM();
        }
        // 他の処理...
    }

    public void OnOtherButtonPressed()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.otherButtonClip);
        }
    }
}
