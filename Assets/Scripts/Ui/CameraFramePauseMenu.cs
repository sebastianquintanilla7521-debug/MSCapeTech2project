using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Add to the CameraFrame Image in the shared Canvas prefab.
[RequireComponent(typeof(RectTransform))]
public sealed class CameraFramePauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private string startMenuSceneName = "StartMenu";
    [Tooltip("Whole Popuptileabletexture.png; auto-found from scene UI if empty.")]
    [SerializeField] private Texture2D popupTexture;
    [SerializeField] private bool escapeAlsoPauses = true;

    private GameObject frameButton, popupRoot;
    private CanvasGroup popupGroup;
    private Sprite popupSprite;
    private Font font;
    private Text message;
    private bool paused, quitting;
    private float previousTimeScale;
    private bool previousAudioPause;
    private int settingsClosedFrame = -1;

    private void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (popupTexture == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                foreach (Image image in canvas.GetComponentsInChildren<Image>(true))
                    if (image.sprite != null && image.sprite.texture != null &&
                        image.sprite.texture.name == "Popuptileabletexture")
                    { popupTexture = image.sprite.texture; break; }
        }
        BuildFrameButton();
    }

    private void BuildFrameButton()
    {
        RectTransform rect = NewRect("Pause X Click Area", transform);
        // The artwork is 740 x 416. Anchor directly over its top-right X.
        // Normalized anchors follow the same frame through aspect/resolution changes.
        rect.anchorMin = new Vector2(717f / 740f, 397f / 416f);
        rect.anchorMax = new Vector2(737f / 740f, 413f / 416f);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        Image target = rect.gameObject.AddComponent<Image>();
        target.color = Color.clear;
        target.raycastTarget = true;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = target;
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(OpenPause);
        frameButton = rect.gameObject;
    }

    private void Update()
    {
        if (quitting || GameSettingsMenu.IsOpen || Time.frameCount <= settingsClosedFrame) return;
        if (escapeAlsoPauses && Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Resume();
            else OpenPause();
        }
    }

    public void OpenPause()
    {
        if (!isActiveAndEnabled || paused || quitting || GameSettingsMenu.IsOpen || GameSettings.MenuOpen) return;
        if (SceneManager.GetActiveScene().name == startMenuSceneName) return;
        previousTimeScale = Time.timeScale;
        previousAudioPause = AudioListener.pause;
        paused = true;
        GameSettings.MenuOpen = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Click();
        BuildPopup();
        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void BuildPopup()
    {
        popupRoot = new GameObject("Pause Popup", typeof(RectTransform));
        Canvas canvas = popupRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 250;
        CanvasScaler scaler = popupRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 720);
        scaler.matchWidthOrHeight = 1f;
        popupRoot.AddComponent<GraphicRaycaster>();
        popupGroup = popupRoot.AddComponent<CanvasGroup>();
        if (EventSystem.current == null)
        {
            GameObject events = new GameObject("Pause EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            events.transform.SetParent(popupRoot.transform, false);
        }

        RectTransform shade = NewRect("Backdrop", popupRoot.transform);
        shade.anchorMin = Vector2.zero; shade.anchorMax = Vector2.one;
        shade.offsetMin = shade.offsetMax = Vector2.zero;
        shade.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.35f);

        RectTransform panel = NewRect("Pause Window", popupRoot.transform);
        panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(440, 365);
        Image background = panel.gameObject.AddComponent<Image>();
        if (popupTexture != null && popupTexture.width == 256 && popupTexture.height == 256)
        {
            popupSprite = Sprite.Create(popupTexture, new Rect(96, 96, 96, 96),
                new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(32, 32, 32, 32));
            background.sprite = popupSprite;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 0.5f;
        }
        else
        {
            background.color = new Color32(207, 199, 185, 255);
            Box(panel, "Title Bar", 2, 2, 436, 34, new Color32(38, 61, 101, 255));
        }
        Label(panel, "Paused", 14, 3, 350, 32, 20, Color.white);
        Button close = MakeButton(panel, "", 412, 8, 20, 20, Resume);
        close.GetComponent<Image>().color = popupSprite != null ? Color.clear : Color.white;
        foreach (Transform child in close.transform) child.gameObject.SetActive(false);
        if (popupSprite == null) Label(close.transform, "X", 0, 0, 20, 20, 16, Color.black);

        MakeButton(panel, "Resume", 35, 62, 370, 42, Resume);
        MakeButton(panel, "Settings", 35, 116, 370, 42, OpenSettings);
        MakeButton(panel, "Back to Menu", 35, 170, 370, 42, BackToMenu);
        MakeButton(panel, "Quit", 35, 224, 370, 42, Quit);
        message = Label(panel, "Game paused", 35, 283, 370, 60, 17, Color.black);
        message.alignment = TextAnchor.MiddleCenter;
    }

    public void OpenSettings()
    {
        if (!paused || quitting || GameSettingsMenu.IsOpen) return;
        Click();
        // Keep the pause Canvas active so its EventSystem, if any, keeps running.
        popupGroup.alpha = 0f;
        popupGroup.interactable = false;
        popupGroup.blocksRaycasts = false;
        GameSettingsMenu.Open(popupTexture, SettingsClosed,
            () => this != null && paused && isActiveAndEnabled);
    }

    private void SettingsClosed()
    {
        if (this == null || !paused || popupGroup == null || !isActiveAndEnabled) return;
        popupGroup.alpha = 1f;
        popupGroup.interactable = true;
        popupGroup.blocksRaycasts = true;
        GameSettings.MenuOpen = true;
        settingsClosedFrame = Time.frameCount;
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        if (!paused || quitting || GameSettingsMenu.IsOpen) return;
        Click();
        RestoreGame();
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void BackToMenu()
    {
        if (!paused || quitting || GameSettingsMenu.IsOpen) return;
        if (!Application.CanStreamedLevelBeLoaded(startMenuSceneName))
        {
            message.text = "Add " + startMenuSceneName + " to the build's Scene List first.";
            return;
        }
        Click();
        RestoreGame();
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(startMenuSceneName);
    }

    public void Quit()
    {
        if (!paused || quitting || GameSettingsMenu.IsOpen) return;
        quitting = true;
        Click();
        popupGroup.interactable = false;
        StartCoroutine(QuitAfterClick());
    }

    private IEnumerator QuitAfterClick()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        PlayerPrefs.Save();
        RestoreGame();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RestoreGame()
    {
        if (!paused) return;
        paused = false;
        GameSettings.MenuOpen = false;
        GameSettings.BlockInputUntilNextFrame();
        Time.timeScale = previousTimeScale;
        AudioListener.pause = previousAudioPause;
        if (popupRoot != null) { popupRoot.SetActive(false); Destroy(popupRoot); }
        popupRoot = null; popupGroup = null;
        if (popupSprite != null) Destroy(popupSprite);
        popupSprite = null;
    }

    private void OnDisable() => RestoreGame();
    private void OnDestroy()
    {
        RestoreGame();
        if (frameButton != null) Destroy(frameButton);
    }

    private static void Click() => GameAudio.Instance?.PlayUIClick();

    private RectTransform NewRect(string name, Transform parent)
    {
        RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }
    private static void Place(RectTransform rect, float x, float y, float w, float h)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(w, h);
    }
    private RectTransform Box(Transform parent, string name, float x, float y, float w, float h, Color color)
    {
        RectTransform rect = NewRect(name, parent); Place(rect, x, y, w, h);
        rect.gameObject.AddComponent<Image>().color = color;
        return rect;
    }
    private Text Label(Transform parent, string text, float x, float y, float w, float h, int size, Color color)
    {
        RectTransform rect = NewRect("Label", parent); Place(rect, x, y, w, h);
        Text label = rect.gameObject.AddComponent<Text>();
        label.font = font; label.text = text; label.fontSize = size; label.color = color;
        label.alignment = TextAnchor.MiddleLeft; label.raycastTarget = false;
        return label;
    }
    private Button MakeButton(Transform parent, string text, float x, float y, float w, float h, UnityEngine.Events.UnityAction action)
    {
        RectTransform rect = Box(parent, text, x, y, w, h, new Color32(232, 225, 213, 255));
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        button.onClick.AddListener(action);
        Color light = new Color32(255, 250, 239, 255), dark = new Color32(75, 70, 62, 255);
        Box(rect, "Top Edge", 0, 0, w, 2, light).GetComponent<Image>().raycastTarget = false;
        Box(rect, "Left Edge", 0, 0, 2, h, light).GetComponent<Image>().raycastTarget = false;
        Box(rect, "Bottom Edge", 0, h - 2, w, 2, dark).GetComponent<Image>().raycastTarget = false;
        Box(rect, "Right Edge", w - 2, 0, 2, h, dark).GetComponent<Image>().raycastTarget = false;
        Label(rect, text, 4, 0, w - 8, h, 20, Color.black).alignment = TextAnchor.MiddleCenter;
        return button;
    }
}
