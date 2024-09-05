using UnityEngine;

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
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Initialize BGM for settings scene
    public void InitializeSettingsAudio()
    {
        PlayBGM(settingsBGM);
        PlaySFX(spriteConnectedClip); // Example effect, adjust as needed
    }
}
