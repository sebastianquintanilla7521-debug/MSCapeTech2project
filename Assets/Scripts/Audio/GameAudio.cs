using UnityEngine;

// Put this on a dedicated root GameObject, not on the player or Canvas.
[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public sealed class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [Header("Dash")]
    [SerializeField] private AudioClip dashSfx;
    [SerializeField, Range(0f, 1f)] private float dashVolume = 1f;

    [Header("Jump")]
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField, Range(0f, 1f)] private float jumpVolume = 1f;

    [Header("UI Click")]
    [SerializeField] private AudioClip uiClickSfx;
    [SerializeField, Range(0f, 1f)] private float uiClickVolume = 1f;

    [Header("Death")]
    [SerializeField] private AudioClip deathSfx;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 1f;

    [Header("Checkpoint / Level Completed")]
    [Tooltip("The same clip is used for checkpoint activation and level completion.")]
    [SerializeField] private AudioClip checkpointAndLevelCompleteSfx;
    [SerializeField, Range(0f, 1f)] private float checkpointVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float levelCompleteVolume = 1f;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.3f;
    [SerializeField] private bool playMusicOnStart = true;

    private AudioSource musicSource;
    private AudioSource gameplaySource;
    private AudioSource uiSource;
    private AudioSource deathSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        gameplaySource = CreateSource(false);
        uiSource = CreateSource(true);
        deathSource = CreateSource(true);

        musicSource = CreateSource(false);
        musicSource.loop = true;
        ApplyEffectsVolume();
        musicSource.volume = Mathf.Clamp01(musicVolume) * GameSettings.MasterVolume * GameSettings.MusicVolume;

        if (playMusicOnStart)
            PlayMusic();
    }

    private AudioSource CreateSource(bool ignoreListenerPause)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = 1f;
        source.ignoreListenerPause = ignoreListenerPause;
        return source;
    }

    private void Update()
    {
        RefreshVolumes();
    }

    // Called immediately by the settings UI, and each frame for already-playing audio.
    public void RefreshVolumes()
    {
        ApplyEffectsVolume();
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(musicVolume) * GameSettings.MasterVolume * GameSettings.MusicVolume;
    }

    private void ApplyEffectsVolume()
    {
        float gain = GameSettings.MasterVolume * GameSettings.EffectsVolume;
        if (gameplaySource != null) gameplaySource.volume = gain;
        if (uiSource != null) uiSource.volume = gain;
        if (deathSource != null) deathSource.volume = gain;
    }

    public void PlayMusic()
    {
        if (musicSource == null || backgroundMusic == null)
            return;

        // Repeated calls for the current track keep its playback position.
        if (musicSource.clip == backgroundMusic && musicSource.isPlaying)
            return;

        musicSource.clip = backgroundMusic;
        musicSource.volume = Mathf.Clamp01(musicVolume) * GameSettings.MasterVolume * GameSettings.MusicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayDash() => Play(gameplaySource, dashSfx, dashVolume);
    public void PlayJump() => Play(gameplaySource, jumpSfx, jumpVolume);
    public void PlayUIClick() => Play(uiSource, uiClickSfx, uiClickVolume);
    // The clip belongs to the popup prefab; playback uses the persistent UI source.
    public void PlayPopup(AudioClip clip, float volume = 1f) => Play(uiSource, clip, volume);
    public void PlayDeath() => Play(deathSource, deathSfx, deathVolume);
    public void PlayCheckpoint() => Play(uiSource, checkpointAndLevelCompleteSfx, checkpointVolume);
    public void PlayLevelComplete() => Play(uiSource, checkpointAndLevelCompleteSfx, levelCompleteVolume);

    private void Play(AudioSource source, AudioClip clip, float volume)
    {
        ApplyEffectsVolume();
        if (source != null && clip != null)
            source.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
