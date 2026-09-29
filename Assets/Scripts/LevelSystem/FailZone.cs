using UnityEngine;

public class FailZone : MonoBehaviour
{
    // ==================================================
    // LEVEL RESET
    // ==================================================

    [Tooltip(
        "Leave empty to automatically find " +
        "the Level Reset Manager."
    )]
    [SerializeField]
    private LevelResetManager levelResetManager;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        if (levelResetManager == null)
        {
            levelResetManager =
                FindAnyObjectByType<
                    LevelResetManager
                >();
        }
    }


    // ==================================================
    // SOMETHING ENTERS FAIL ZONE
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        PlayerMovement player =
            other.GetComponent<PlayerMovement>();


        if (player == null)
        {
            player =
                other.GetComponentInParent<
                    PlayerMovement
                >();
        }


        // Ignore everything except the Player.
        if (player == null)
        {
            return;
        }


        if (levelResetManager != null)
        {
            levelResetManager.FailLevel();
        }
    }
}