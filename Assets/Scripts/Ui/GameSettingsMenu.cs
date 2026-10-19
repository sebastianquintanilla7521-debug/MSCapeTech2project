using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Created by the existing Settings button. No Inspector wiring is needed.
public sealed class GameSettingsMenu : MonoBehaviour
{
    private static GameSettingsMenu instance;
    public static bool IsOpen => instance != null;
    private Action onClosed;
    private Func<bool> parentStillOpen;
    private bool parentMenuWasOpen;
    private Font font;
    private Texture2D popupTexture;
    private Sprite popupFrame;
    private RectTransform panel, body;
    private Text status;
    private readonly List<Text> bindingLabels = new List<Text>();
    private readonly List<Selectable> tabControls = new List<Selectable>();
    private int tab, capture = -1, captureFrame;
    private KeyCode[] allKeys;
    private float captureUntil;
    private Slider master, music, effects;
    private float nextVolumeFeedbackTime;
    private readonly List<Vector2Int> resolutions = new List<Vector2Int>();
    private int aspect, resolutionIndex, modeIndex;
    private readonly string[] aspectNames = { "16:9", "16:10", "4:3", "21:9" };
    private readonly float[] aspects = { 16f / 9f, 16f / 10f, 4f / 3f, 21f / 9f };
    private readonly string[] modeNames = { "Windowed", "Borderless", "Fullscreen" };
    private readonly FullScreenMode[] modes = { FullScreenMode.Windowed, FullScreenMode.FullScreenWindow, FullScreenMode.ExclusiveFullScreen };
    private Text modeLabel, aspectLabel, resolutionLabel;
    private Button keepButton, revertButton;
    private bool preview;
    private float previewUntil;
    private int previousWidth, previousHeight;
    private FullScreenMode previousMode;
    private bool createdEventSystem;
    private GameObject ownEventSystem;

    public static void Open(Texture2D popupTexture = null, Action onClosed = null, Func<bool> parentStillOpen = null)
    {
        if (instance != null) return;
        GameObject root = new GameObject("Game Settings", typeof(RectTransform));
        instance = root.AddComponent<GameSettingsMenu>();
        instance.popupTexture = popupTexture;
        instance.onClosed = onClosed;
        instance.parentStillOpen = parentStillOpen;
        instance.parentMenuWasOpen = GameSettings.MenuOpen;
        instance.Build();
    }

    private void Build()
    {
        GameSettings.MenuOpen = true;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        allKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 720);
        scaler.matchWidthOrHeight = 1f;
        gameObject.AddComponent<GraphicRaycaster>();
        if (EventSystem.current == null)
        {
            ownEventSystem = new GameObject("Settings EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            createdEventSystem = true;
        }
        RectTransform shade = Rect("Backdrop", transform);
        Stretch(shade);
        shade.gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.35f);
        panel = Rect("Settings Window", transform);
        panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(700, 600);
        BuildPopupFrame();
        Label(panel, "Settings", 14, 3, 600, 32, 20, Color.white);
        Button closeX = ButtonAt(panel, "", 672, 8, 20, 20, Close);
        // The X is drawn by the original popup artwork; this is its click target.
        closeX.GetComponent<Image>().color = Color.clear;
        foreach (Transform child in closeX.transform)
            child.gameObject.SetActive(false);
        if (popupFrame == null)
        {
            closeX.GetComponent<Image>().color = new Color32(232, 225, 213, 255);
            Label(closeX.transform, "X", 0, 0, 20, 20, 16, Color.black);
        }
        ButtonAt(panel, "Audio", 20, 58, 210, 36, () => ChangeTab(0));
        ButtonAt(panel, "Controls", 245, 58, 210, 36, () => ChangeTab(1));
        ButtonAt(panel, "Display", 470, 58, 210, 36, () => ChangeTab(2));
        body = Rect("Tab Content", panel);
        Place(body, 20, 110, 660, 365);
        status = Label(panel, "", 20, 480, 660, 65, 17, Color.black);
        ButtonAt(panel, "Close", 510, 550, 170, 34, Close);
        previousMode = Screen.fullScreenMode;
        modeIndex = Array.IndexOf(modes, previousMode);
        if (modeIndex < 0) modeIndex = 0;
        float currentAspect = (float)Screen.width / Mathf.Max(1, Screen.height);
        for (int i = 1; i < aspects.Length; i++)
            if (Mathf.Abs(aspects[i] - currentAspect) < Mathf.Abs(aspects[aspect] - currentAspect)) aspect = i;
        BuildTab();
    }

    private void BuildPopupFrame()
    {
        Image background = panel.gameObject.AddComponent<Image>();
        if (popupTexture == null)
        {
            // Can also reuse the popup art already referenced by a scene Image.
            foreach (Image image in FindObjectsByType<Image>(FindObjectsInactive.Include))
            {
                if (image.sprite != null && image.sprite.texture != null &&
                    image.sprite.texture.name == "Popuptileabletexture")
                {
                    popupTexture = image.sprite.texture;
                    break;
                }
            }
        }
        if (popupTexture != null && popupTexture.width == 256 && popupTexture.height == 256)
        {
            // Reuse the original 96 x 96 popup at x=96, y=96.
            // Stretch its middle while retaining the original corners, title, and X.
            popupFrame = Sprite.Create(popupTexture, new Rect(96, 96, 96, 96),
                new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect,
                new Vector4(32, 32, 32, 32));
            popupFrame.name = "Settings Popup Frame";
            background.sprite = popupFrame;
            background.type = Image.Type.Sliced;
            background.pixelsPerUnitMultiplier = 0.5f;
            background.color = Color.white;
        }
        else
        {
            background.color = new Color32(207, 199, 185, 255);
            Box(panel, "Title Bar", 2, 2, 696, 34, new Color32(38, 61, 101, 255));
        }
    }

    private void AddButtonBevel(RectTransform rect, float width, float height)
    {
        Color light = new Color32(255, 250, 239, 255);
        Color dark = new Color32(75, 70, 62, 255);
        Box(rect, "Top Edge", 0, 0, width, 2, light).GetComponent<Image>().raycastTarget = false;
        Box(rect, "Left Edge", 0, 0, 2, height, light).GetComponent<Image>().raycastTarget = false;
        Box(rect, "Bottom Edge", 0, height - 2, width, 2, dark).GetComponent<Image>().raycastTarget = false;
        Box(rect, "Right Edge", width - 2, 0, 2, height, dark).GetComponent<Image>().raycastTarget = false;
    }

    private void Update()
    {
        if (preview)
        {
            status.text = "Keep these display settings? Reverting in " + Mathf.CeilToInt(previewUntil - Time.unscaledTime) + " seconds.";
            if (Time.unscaledTime >= previewUntil || Input.GetKeyDown(KeyCode.Escape)) RevertDisplay();
            return;
        }
        if (capture >= 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Time.unscaledTime >= captureUntil)
            {
                EndCapture("Binding cancelled.");
                return;
            }
            // Ignore the click that opened capture, including its release frame.
            if (Time.frameCount <= captureFrame + 1) return;
            foreach (KeyCode key in allKeys)
            {
                if (!GameSettings.IsSupportedKey(key) || !Input.GetKeyDown(key)) continue;
                string message;
                if (GameSettings.TryBind((GameAction)capture, key, out message))
                {
                    Click();
                    EndCapture(message);
                }
                else status.text = message + " Choose another key, or Escape to cancel.";
                return;
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) Close();
    }

    private void ChangeTab(int index)
    {
        if (preview || capture >= 0) return;
        Click();
        tab = index;
        BuildTab();
    }

    private void BuildTab()
    {
        foreach (Transform child in body) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
        tabControls.Clear();
        bindingLabels.Clear();
        if (tab == 0) BuildAudio();
        else if (tab == 1) BuildControls();
        else BuildDisplay();
    }

    private void BuildAudio()
    {
        Label(body, "AUDIO", 0, 0, 660, 32, 23, Color.black);
        master = VolumeSlider("Master", 52, GameSettings.MasterVolume);
        music = VolumeSlider("Music", 122, GameSettings.MusicVolume);
        effects = VolumeSlider("Sound effects", 192, GameSettings.EffectsVolume);
        master.onValueChanged.AddListener(_ => SaveVolumes(true));
        music.onValueChanged.AddListener(_ => SaveVolumes());
        effects.onValueChanged.AddListener(_ => SaveVolumes(true));
        status.text = "Volume changes apply immediately. Your individual sound volumes stay as configured on GameAudio.";
    }

    private Slider VolumeSlider(string name, float y, float value)
    {
        Label(body, name, 0, y, 180, 32, 20, Color.black);
        Text percent = Label(body, Mathf.RoundToInt(value * 100) + "%", 575, y, 85, 32, 20, Color.black);
        RectTransform root = Rect(name + " Slider", body);
        Place(root, 185, y, 370, 32);
        Slider slider = root.gameObject.AddComponent<Slider>();
        RectTransform track = Box(root, "Track", 0, 11, 370, 10, Color.gray);
        RectTransform fillArea = Rect("Fill Area", root);
        Stretch(fillArea); fillArea.offsetMin = new Vector2(8, 10); fillArea.offsetMax = new Vector2(-8, -10);
        RectTransform fill = Box(fillArea, "Fill", 0, 0, 354, 12, new Color32(38, 61, 101, 255));
        Stretch(fill);
        RectTransform handleArea = Rect("Handle Area", root); Stretch(handleArea);
        handleArea.offsetMin = new Vector2(8, 0); handleArea.offsetMax = new Vector2(-8, 0);
        RectTransform handle = Rect("Handle", handleArea);
        handle.sizeDelta = new Vector2(18, 30);
        Image handleImage = handle.gameObject.AddComponent<Image>(); handleImage.color = Color.white;
        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handleImage;
        slider.minValue = 0f; slider.maxValue = 1f;
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(v => percent.text = Mathf.RoundToInt(v * 100) + "%");
        return slider;
    }

    private void SaveVolumes(bool feedback = false)
    {
        GameSettings.SetVolumes(master.value, music.value, effects.value);
        GameAudio.Instance?.RefreshVolumes();
        // A short preview while dragging Master/Effects, at the NEW volume.
        // Music changes use the actual music as their feedback.
        if (feedback && Time.unscaledTime >= nextVolumeFeedbackTime)
        {
            nextVolumeFeedbackTime = Time.unscaledTime + 0.15f;
            GameAudio.Instance?.PlayUIClick();
        }
    }

    private void BuildControls()
    {
        Label(body, "CONTROLS", 0, 0, 660, 32, 23, Color.black);
        for (int i = 0; i < 7; i++)
        {
            int action = i;
            Label(body, GameSettings.ActionLabel((GameAction)i), 0, 40 + i * 37, 300, 32, 19, Color.black);
            Button button = ButtonAt(body, GameSettings.KeyLabel(GameSettings.Key((GameAction)i)),
                320, 40 + i * 37, 340, 32, () => BeginCapture(action));
            bindingLabels.Add(button.GetComponentInChildren<Text>());
            tabControls.Add(button);
        }
        Button reset = ButtonAt(body, "Restore default keys", 0, 312, 270, 36, () => {
            Click(); GameSettings.ResetBindings(); RefreshBindings(); status.text = "Default keys restored.";
        });
        tabControls.Add(reset);
        status.text = "Click an action, then press a key or mouse button. Escape cancels. Enter and Escape stay reserved for menus.";
    }

    private void BeginCapture(int action)
    {
        if (capture >= 0) return;
        Click();
        capture = action;
        captureFrame = Time.frameCount;
        captureUntil = Time.unscaledTime + 10f;
        foreach (Selectable item in tabControls) item.interactable = false;
        bindingLabels[action].text = "Press a key...";
        status.text = "Choose a key for " + GameSettings.ActionLabel((GameAction)action) + ". Escape cancels; timeout is 10 seconds.";
        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void EndCapture(string message)
    {
        capture = -1;
        foreach (Selectable item in tabControls) item.interactable = true;
        RefreshBindings();
        status.text = message;
    }

    private void RefreshBindings()
    {
        for (int i = 0; i < bindingLabels.Count; i++)
            bindingLabels[i].text = GameSettings.KeyLabel(GameSettings.Key((GameAction)i));
    }

    private void BuildDisplay()
    {
        Label(body, "DISPLAY", 0, 0, 660, 32, 23, Color.black);
        modeLabel = CycleRow("Window mode", 50, () => { modeIndex = (modeIndex + 2) % 3; RefreshDisplay(); },
            () => { modeIndex = (modeIndex + 1) % 3; RefreshDisplay(); });
        aspectLabel = CycleRow("Aspect ratio", 105, () => { aspect = (aspect + 3) % 4; PopulateResolutions(); RefreshDisplay(); },
            () => { aspect = (aspect + 1) % 4; PopulateResolutions(); RefreshDisplay(); });
        resolutionLabel = CycleRow("Resolution", 160, () => { resolutionIndex = (resolutionIndex + resolutions.Count - 1) % resolutions.Count; RefreshDisplay(); },
            () => { resolutionIndex = (resolutionIndex + 1) % resolutions.Count; RefreshDisplay(); });
        Button apply = ButtonAt(body, "Apply display", 0, 230, 210, 38, ApplyDisplay);
        tabControls.Add(apply);
        keepButton = ButtonAt(body, "Keep changes", 225, 230, 210, 38, KeepDisplay);
        revertButton = ButtonAt(body, "Revert", 450, 230, 210, 38, RevertDisplay);
        keepButton.gameObject.SetActive(false); revertButton.gameObject.SetActive(false);
        Label(body, "The game artwork keeps its original proportions. Other screen shapes may show borders.", 0, 290, 660, 62, 18, Color.black);
        PopulateResolutions();
        RefreshDisplay();
        status.text = Application.isEditor ? "Display choices can be saved here. Actual window/fullscreen changes must be tested in a standalone build." :
            "Apply to preview. Confirm within 15 seconds, or the previous display settings return.";
    }

    private Text CycleRow(string name, float y, Action previous, Action next)
    {
        Label(body, name, 0, y, 230, 36, 20, Color.black);
        Button left = ButtonAt(body, "<", 240, y, 45, 36, () => { Click(); previous(); });
        Text value = Label(body, "", 295, y, 310, 36, 19, Color.black);
        value.alignment = TextAnchor.MiddleCenter;
        Button right = ButtonAt(body, ">", 615, y, 45, 36, () => { Click(); next(); });
        tabControls.Add(left); tabControls.Add(right);
        return value;
    }

    private void PopulateResolutions()
    {
        resolutions.Clear();
        int maxW = Mathf.Max(640, Display.main.systemWidth);
        int maxH = Mathf.Max(480, Display.main.systemHeight);
        foreach (Resolution r in Screen.resolutions)
            AddResolution(r.width, r.height, maxW, maxH);
        foreach (int h in new[] { 480, 600, 720, 768, 800, 900, 1080, 1200, 1440, 1600, 2160 })
            AddResolution(Mathf.RoundToInt(h * aspects[aspect]), h, maxW, maxH);
        if (resolutions.Count == 0) resolutions.Add(new Vector2Int(640, 480));
        resolutions.Sort((a, b) => a.x.CompareTo(b.x));
        resolutionIndex = 0;
        for (int i = 1; i < resolutions.Count; i++)
            if (Mathf.Abs(resolutions[i].x - Screen.width) < Mathf.Abs(resolutions[resolutionIndex].x - Screen.width)) resolutionIndex = i;
    }

    private void AddResolution(int w, int h, int maxW, int maxH)
    {
        Vector2Int size = new Vector2Int(w, h);
        if (w < 640 || h < 480 || w > maxW || h > maxH || Mathf.Abs((float)w / h - aspects[aspect]) > 0.025f) return;
        if (!resolutions.Contains(size)) resolutions.Add(size);
    }

    private void RefreshDisplay()
    {
        modeLabel.text = modeNames[modeIndex];
        aspectLabel.text = modeIndex == 1 ? "Desktop aspect" : aspectNames[aspect];
        Vector2Int size = resolutions[resolutionIndex];
        resolutionLabel.text = modeIndex == 1 ? Display.main.systemWidth + " x " + Display.main.systemHeight + " (desktop)" : size.x + " x " + size.y;
        for (int i = 2; i < 6; i++) tabControls[i].interactable = modeIndex != 1;
    }

    private void ApplyDisplay()
    {
        if (preview) return;
        Click();
        previousWidth = Screen.width; previousHeight = Screen.height; previousMode = Screen.fullScreenMode;
        Vector2Int size = modeIndex == 1 ? new Vector2Int(Display.main.systemWidth, Display.main.systemHeight) : resolutions[resolutionIndex];
        if (!Application.isEditor) Screen.SetResolution(size.x, size.y, modes[modeIndex]);
        preview = true;
        previewUntil = Time.unscaledTime + 15f;
        foreach (Selectable item in tabControls) item.interactable = false;
        keepButton.gameObject.SetActive(true); revertButton.gameObject.SetActive(true);
    }

    private void KeepDisplay()
    {
        if (!preview) return;
        Click();
        Vector2Int size = modeIndex == 1 ? new Vector2Int(Display.main.systemWidth, Display.main.systemHeight) : resolutions[resolutionIndex];
        GameSettings.SaveDisplay(Application.isEditor ? size.x : Screen.width,
            Application.isEditor ? size.y : Screen.height, Application.isEditor ? modes[modeIndex] : Screen.fullScreenMode);
        EndPreview("Display settings saved.");
    }

    private void RevertDisplay()
    {
        if (!preview) return;
        Click();
        RestorePreviousDisplay();
        EndPreview("Previous display settings restored.");
    }

    private void RestorePreviousDisplay()
    {
        if (!Application.isEditor) Screen.SetResolution(previousWidth, previousHeight, previousMode);
    }

    private void EndPreview(string message)
    {
        preview = false;
        foreach (Selectable item in tabControls) item.interactable = true;
        keepButton.gameObject.SetActive(false); revertButton.gameObject.SetActive(false);
        RefreshDisplay();
        status.text = message;
    }

    public void Close()
    {
        Click();
        if (preview) { RestorePreviousDisplay(); preview = false; }
        PlayerPrefs.Save();
        GameSettings.MenuOpen = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (popupFrame != null) Destroy(popupFrame);
        if (preview) RestorePreviousDisplay();
        if (instance == this)
        {
            instance = null;
            GameSettings.MenuOpen = parentMenuWasOpen && (parentStillOpen == null || parentStillOpen());
            GameSettings.BlockInputUntilNextFrame();
            Action callback = onClosed;
            onClosed = null;
            callback?.Invoke();
        }
        if (createdEventSystem && ownEventSystem != null) Destroy(ownEventSystem);
        PlayerPrefs.Save();
    }

    private static void Click() => GameAudio.Instance?.PlayUIClick();

    private RectTransform Rect(string name, Transform parent)
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

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private RectTransform Box(Transform parent, string name, float x, float y, float w, float h, Color color)
    {
        RectTransform rect = Rect(name, parent); Place(rect, x, y, w, h);
        rect.gameObject.AddComponent<Image>().color = color;
        return rect;
    }

    private Text Label(Transform parent, string text, float x, float y, float w, float h, int size, Color color)
    {
        RectTransform rect = Rect("Label", parent); Place(rect, x, y, w, h);
        Text label = rect.gameObject.AddComponent<Text>();
        label.text = text; label.font = font; label.fontSize = size; label.color = color;
        label.alignment = TextAnchor.MiddleLeft;
        label.raycastTarget = false;
        return label;
    }

    private Button ButtonAt(Transform parent, string text, float x, float y, float w, float h, Action action)
    {
        RectTransform rect = Box(parent, text, x, y, w, h, new Color32(232, 225, 213, 255));
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        AddButtonBevel(rect, w, h);
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1f, 0.98f, 0.93f);
        colors.pressedColor = new Color(0.72f, 0.70f, 0.65f);
        colors.selectedColor = new Color(0.94f, 0.91f, 0.84f);
        colors.fadeDuration = 0f;
        button.colors = colors;
        button.onClick.AddListener(() => action());
        Text label = Label(rect, text, 4, 0, w - 8, h, 19, Color.black);
        label.alignment = TextAnchor.MiddleCenter;
        return button;
    }
}
