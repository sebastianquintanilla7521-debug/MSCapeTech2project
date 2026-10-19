using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class StartMenuController : MonoBehaviour
{
    // ==================================================
    // SCENES
    // ==================================================

    [Header("Scenes")]

    [SerializeField]
    private string firstSceneName = "Tut";

    [SerializeField]
    private string creditsSceneName = "Credits";


    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Settings")]

    [SerializeField]
    private CanvasGroup settingsPanel;

    [Header("Settings Popup Appearance")]
    [Tooltip("Assign the whole Popuptileabletexture.png texture from Assets/PixelArt.")]
    [SerializeField] private Texture2D settingsPopupTexture;


    // ==================================================
    // STARTUP
    // ==================================================

    private void Awake()
    {
        HideSettingsImmediately();
    }


    // ==================================================
    // START GAME
    // ==================================================

    public void StartGame()
    {
        GameAudio.Instance?.PlayUIClick();
        CheckpointSystem.ClearCheckpoint();

        SceneManager.LoadScene(
            firstSceneName
        );
    }


    // ==================================================
    // CREDITS
    // ==================================================

    public void OpenCredits()
    {
        GameAudio.Instance?.PlayUIClick();
        SceneManager.LoadScene(
            creditsSceneName
        );
    }


    // ==================================================
    // SETTINGS
    // ==================================================

    public void OpenSettings()
    {
        GameAudio.Instance?.PlayUIClick();
        HideSettingsImmediately();
        GameSettingsMenu.Open(settingsPopupTexture);
    }

    public void CloseSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        GameAudio.Instance?.PlayUIClick();

        settingsPanel.alpha = 0f;
        settingsPanel.interactable = false;
        settingsPanel.blocksRaycasts = false;
    }


    private void HideSettingsImmediately()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.alpha = 0f;
        settingsPanel.interactable = false;
        settingsPanel.blocksRaycasts = false;
    }


    // ==================================================
    // QUIT
    // ==================================================

    public void QuitGame()
    {
#if UNITY_EDITOR

        EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }
}