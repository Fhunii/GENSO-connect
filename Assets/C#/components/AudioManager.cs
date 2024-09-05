using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip gameBGM;
    public AudioClip settingsBGM;
    public AudioClip spriteConnectedClip;
    public AudioClip spriteDestroyedClip;
    public AudioClip quitButtonClip;
    public AudioClip otherButtonClip;

    public float sfxVolume = 1f; // 音量設定
    public float bgmVolume = 1f;

    private Coroutine sfxRoutine;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmSource.isPlaying)
        {
            bgmSource.volume = bgmVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    public void InitializeSettingsAudio()
    {
        PlayBGM(settingsBGM);
        StartSFXRoutine(spriteConnectedClip, 1f);
    }

    private void StartSFXRoutine(AudioClip clip, float interval)
    {
        if (sfxRoutine != null)
        {
            StopCoroutine(sfxRoutine);
        }
        sfxRoutine = StartCoroutine(PlaySFXRepeatedly(clip, interval));
    }

    private IEnumerator PlaySFXRepeatedly(AudioClip clip, float interval)
    {
        while (true)
        {
            PlaySFX(clip);
            yield return new WaitForSeconds(interval);
        }
    }

    public void StopSettingsAudio()
    {
        StopBGM();
        if (sfxRoutine != null)
        {
            StopCoroutine(sfxRoutine);
        }
    }
}
