using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public AudioClip settingsSFX;

    private Coroutine sfxCoroutine;

    private void Start()
    {
        // 音量スライダーの値をプレイヤープリファレンスから取得して設定
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        StartSFXCoroutine();
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

    private void StartSFXCoroutine()
    {
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
        }
        sfxCoroutine = StartCoroutine(PlaySettingsSFX());
    }

    private IEnumerator PlaySettingsSFX()
    {
        while (SceneManager.GetActiveScene().name == "settings scene")
        {
            AudioManager.Instance.PlaySFX(settingsSFX);
            yield return new WaitForSeconds(1f);
        }
    }

    public void OnBackButtonPressed()
    {
        // メニューシーンに戻る処理
        AudioManager.Instance.StopBGM();
        if (sfxCoroutine != null)
        {
            StopCoroutine(sfxCoroutine);
        }
        // 他の処理...
    }
}
