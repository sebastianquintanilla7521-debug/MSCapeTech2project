using UnityEngine;

public class TutorialPopupTrigger : MonoBehaviour
{
    // ==================================================
    // POPUP
    // ==================================================

    [Header("Tutorial Popup")]

    [SerializeField]
    private TutorialPopupUI popup;


    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Trigger Settings")]

    [Tooltip(
        "If enabled, this trigger will only show its popup once."
    )]
    [SerializeField]
    private bool triggerOnlyOnce = true;


    // ==================================================
    // INTERNAL
    // ==================================================

    private bool hasTriggered = false;


    // ==================================================
    // PLAYER ENTERS
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (
            triggerOnlyOnce &&
            hasTriggered
        )
        {
            return;
        }


        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        if (popup == null)
        {
            Debug.LogWarning(
                gameObject.name +
                ": No tutorial popup has been assigned."
            );

            return;
        }


        hasTriggered = true;

        popup.ShowPopup();
    }
}