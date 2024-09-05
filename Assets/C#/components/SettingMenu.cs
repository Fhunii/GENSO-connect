using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public AudioClip settingsSFX;

    private void Start()
    {
        // 音量スライダーの値をプレイヤープリファレンスから取得して設定
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // 初期化時に設定画面の音を再生
        AudioManager.Instance.InitializeSettingsAudio();
    }

    public void SetBGMVolume(float volume)
    {
        AudioManager.Instance.SetBGMVolume(volume);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void OnBackButtonPressed()
    {
        // メニューシーンに戻る処理
        AudioManager.Instance.StopSettingsAudio();
        // シーン遷移処理を追加
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
