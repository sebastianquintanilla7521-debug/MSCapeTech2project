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

    public void PlayDash() => Play(gameplaySource, dashSfx, dashVolume);
    public void PlayJump() => Play(gameplaySource, jumpSfx, jumpVolume);
    public void PlayUIClick() => Play(uiSource, uiClickSfx, uiClickVolume);
    public void PlayDeath() => Play(deathSource, deathSfx, deathVolume);

    private void Play(AudioSource source, AudioClip clip, float volume)
    {
        if (source != null && clip != null)
            source.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
