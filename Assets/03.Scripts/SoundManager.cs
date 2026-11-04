using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("--- 오디오 소스 설정 ---")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("--- 배경음악 클립 ---")]
    public AudioClip mainMenuBGM;
    public AudioClip stageBGM;

    [Header("--- 배경음악 개별 볼륨 배율 ---")]
    [Range(0f, 1f)] public float mainMenuVolumeScale = 0.8f;

    [Header("--- 씬 이름 설정 ---")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private string stageSceneName = "StageScene";

    [Header("--- 기본 효과음 및 기계음 ---")]
    public AudioClip defaultClickSFX;
    public AudioClip clawMoveSFX; // 인형뽑기 기계 작동음

    private float baseBgmVolume = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 게임 켜지자마자 저장된 볼륨 불러오기
            baseBgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
            float savedSfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            SetSFXVolume(savedSfxVol);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopSFX();

        if (scene.name == mainMenuSceneName)
        {
            PlayBGM(mainMenuBGM, true);
        }
        else if (scene.name == stageSceneName)
        {
            PlayBGM(stageBGM, true);
        }

        RegisterAllButtons();
    }

    private void RegisterAllButtons()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayButtonClick);
            btn.onClick.AddListener(PlayButtonClick);
        }
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            ApplyCurrentBGMVolume();
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = loop;

        ApplyCurrentBGMVolume();

        bgmSource.Play();
    }

    private void ApplyCurrentBGMVolume()
    {
        if (bgmSource == null) return;

        float scale = (bgmSource.clip == mainMenuBGM) ? mainMenuVolumeScale : 1.0f;
        bgmSource.volume = baseBgmVolume * scale;
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

    // 모든 효과음(루프 포함)을 즉시 정지하는 함수
    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }

    public void PlayButtonClick()
    {
        if (defaultClickSFX != null)
        {
            PlaySFX(defaultClickSFX);
        }
    }

    public void PlayClawMoveSFX()
    {
        if (clawMoveSFX == null || sfxSource == null) return;

        if (!sfxSource.isPlaying || sfxSource.clip != clawMoveSFX)
        {
            sfxSource.clip = clawMoveSFX;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopClawSFX()
    {
        if (sfxSource != null && sfxSource.clip == clawMoveSFX)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }

    public void SetBGMVolume(float volume)
    {
        baseBgmVolume = volume;
        ApplyCurrentBGMVolume();
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }
}