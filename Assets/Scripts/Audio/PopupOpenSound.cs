using UnityEngine;

// Add to the actual popup root, rather than the Canvas containing several popups.
[DisallowMultipleComponent]
public sealed class PopupOpenSound : MonoBehaviour
{
    [SerializeField] private AudioClip popupAppearSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    private bool wasVisible;

    private void OnEnable() { wasVisible = false; }
    private void OnDisable() { wasVisible = false; }

    private void LateUpdate()
    {
        // TutorialPopupUI hides with CanvasGroup alpha, without disabling its object.
        // Check parents too, so enabling a child inside a hidden popup stays silent.
        float alpha = 1f;
        foreach (CanvasGroup group in GetComponentsInParent<CanvasGroup>())
        {
            if (!group.enabled) continue;
            alpha *= group.alpha;
            if (group.ignoreParentGroups) break;
        }
        bool visible = alpha > 0.001f;
        if (visible && !wasVisible)
            GameAudio.Instance?.PlayPopup(popupAppearSound, volume);
        wasVisible = visible;
    }
}
