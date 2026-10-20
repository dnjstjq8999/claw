using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("--- 오디오 소스 설정 ---")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [Header("--- 기본 버튼 효과음 (선택) ---")]
    public AudioClip defaultClickSFX;

    private void Awake()
    {
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

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick()
    {
        if (defaultClickSFX != null)
        {
            PlaySFX(defaultClickSFX);
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }
}
