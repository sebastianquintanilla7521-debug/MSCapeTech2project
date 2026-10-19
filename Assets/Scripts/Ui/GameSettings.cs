using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameAction { Left, Right, Up, Down, Jump, Dash, Restart }

// Shared saved preferences. No scene object is required.
public static class GameSettings
{
    private const string Prefix = "MSCape.Settings.";
    private static readonly KeyCode[] Defaults = {
        KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,
        KeyCode.Space, KeyCode.Mouse1, KeyCode.R
    };
    private static readonly KeyCode[] Bindings = new KeyCode[7];
    private static bool loaded;
    private static int blockedInputFrame = -1;
    private static float master, music, effects;
    public static bool MenuOpen { get; set; }

    public static float MasterVolume { get { Load(); return master; } }
    public static float MusicVolume { get { Load(); return music; } }
    public static float EffectsVolume { get { Load(); return effects; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        loaded = false;
        blockedInputFrame = -1;
        MenuOpen = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Load();
        if (!Application.isEditor && PlayerPrefs.HasKey(Prefix + "Width"))
        {
            int w = PlayerPrefs.GetInt(Prefix + "Width", 1280);
            int h = PlayerPrefs.GetInt(Prefix + "Height", 720);
            FullScreenMode mode = (FullScreenMode)PlayerPrefs.GetInt(Prefix + "Mode", (int)FullScreenMode.Windowed);
            if (mode != FullScreenMode.Windowed && mode != FullScreenMode.FullScreenWindow &&
                mode != FullScreenMode.ExclusiveFullScreen)
                mode = FullScreenMode.Windowed;
            int maxW = Mathf.Max(640, Display.main.systemWidth);
            int maxH = Mathf.Max(480, Display.main.systemHeight);
            if (w >= 640 && h >= 480 && w <= maxW && h <= maxH)
                Screen.SetResolution(w, h, mode);
        }
    }

    private static void Load()
    {
        if (loaded) return;
        loaded = true;
        master = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Master", 1f));
        music = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Music", 1f));
        effects = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Effects", 1f));
        for (int i = 0; i < Bindings.Length; i++)
        {
            KeyCode key = (KeyCode)PlayerPrefs.GetInt(Prefix + "Key." + i, (int)Defaults[i]);
            Bindings[i] = IsSupportedKey(key) ? key : Defaults[i];
        }
        // Repair invalid/duplicate saved mappings as one complete set.
        for (int i = 0; i < Bindings.Length; i++)
            for (int j = 0; j < i; j++)
                if (Bindings[i] == Bindings[j]) { ResetBindings(); return; }
    }

    public static void SetVolumes(float masterValue, float musicValue, float effectsValue)
    {
        Load();
        master = Mathf.Clamp01(masterValue);
        music = Mathf.Clamp01(musicValue);
        effects = Mathf.Clamp01(effectsValue);
        PlayerPrefs.SetFloat(Prefix + "Master", master);
        PlayerPrefs.SetFloat(Prefix + "Music", music);
        PlayerPrefs.SetFloat(Prefix + "Effects", effects);
    }

    public static KeyCode Key(GameAction action) { Load(); return Bindings[(int)action]; }
    public static bool Held(GameAction action) => !MenuOpen && Time.frameCount > blockedInputFrame && Input.GetKey(Key(action));
    public static void BlockInputUntilNextFrame() => blockedInputFrame = Time.frameCount + 1;
    public static bool Pressed(GameAction action)
    {
        if (MenuOpen || Time.frameCount <= blockedInputFrame) return false;
        KeyCode key = Key(action);
        // A mouse-bound dash/jump/restart must not fire through a UI button click.
        if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6 &&
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return false;
        return Input.GetKeyDown(key);
    }
    public static float Horizontal => (Held(GameAction.Right) ? 1f : 0f) - (Held(GameAction.Left) ? 1f : 0f);
    public static float Vertical => (Held(GameAction.Up) ? 1f : 0f) - (Held(GameAction.Down) ? 1f : 0f);

    public static bool IsSupportedKey(KeyCode key)
    {
        // Escape always cancels/closes the menu; Enter is reserved for UI navigation.
        return Enum.IsDefined(typeof(KeyCode), key) && key != KeyCode.None &&
            key != KeyCode.Escape && key != KeyCode.Return && key != KeyCode.KeypadEnter &&
            (int)key < (int)KeyCode.JoystickButton0;
    }

    public static bool TryBind(GameAction action, KeyCode key, out string message)
    {
        Load();
        if (!IsSupportedKey(key)) { message = "Choose a keyboard key or mouse button. Escape / Enter are reserved."; return false; }
        for (int i = 0; i < Bindings.Length; i++)
            if (i != (int)action && Bindings[i] == key)
            {
                message = KeyLabel(key) + " is already assigned to " + ActionLabel((GameAction)i) + ".";
                return false;
            }
        Bindings[(int)action] = key;
        PlayerPrefs.SetInt(Prefix + "Key." + (int)action, (int)key);
        PlayerPrefs.Save();
        message = "Binding saved.";
        return true;
    }

    public static void ResetBindings()
    {
        loaded = true;
        for (int i = 0; i < Bindings.Length; i++)
        {
            Bindings[i] = Defaults[i];
            PlayerPrefs.SetInt(Prefix + "Key." + i, (int)Defaults[i]);
        }
        PlayerPrefs.Save();
    }

    public static void SaveDisplay(int width, int height, FullScreenMode mode)
    {
        PlayerPrefs.SetInt(Prefix + "Width", width);
        PlayerPrefs.SetInt(Prefix + "Height", height);
        PlayerPrefs.SetInt(Prefix + "Mode", (int)mode);
        PlayerPrefs.Save();
    }

    public static string ActionLabel(GameAction action)
    {
        switch (action)
        {
            case GameAction.Left: return "Move left";
            case GameAction.Right: return "Move right";
            case GameAction.Up: return "Aim dash up";
            case GameAction.Down: return "Aim dash down";
            default: return action.ToString();
        }
    }

    public static string KeyLabel(KeyCode key)
    {
        if (key == KeyCode.Mouse0) return "Left mouse";
        if (key == KeyCode.Mouse1) return "Right mouse";
        if (key == KeyCode.Mouse2) return "Middle mouse";
        return key.ToString();
    }
}
