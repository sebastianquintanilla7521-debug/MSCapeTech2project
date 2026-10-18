using UnityEngine;

public class DangerTilemap : MonoBehaviour
{
    // ==================================================
    // LEVEL RESET
    // ==================================================

    private LevelResetManager levelResetManager;

    private bool hasTriggered = false;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        levelResetManager =
            FindAnyObjectByType<LevelResetManager>();


        if (levelResetManager == null)
        {
            Debug.LogWarning(
                gameObject.name +
                ": No LevelResetManager was found."
            );
        }
    }


    // ==================================================
    // PLAYER TOUCHES DANGER
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (hasTriggered)
        {
            return;
        }


        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        hasTriggered = true;


        // Immediately stop player input.
        player.enabled = false;


        // Immediately stop all player movement.
        Rigidbody2D playerBody =
            player.GetComponent<Rigidbody2D>();


        if (playerBody != null)
        {
            playerBody.linearVelocity =
                Vector2.zero;

            playerBody.simulated =
                false;
        }


        // Spikes reset instantly instead of
        // using the delayed FailLevel sequence.
        if (levelResetManager != null)
        {
            levelResetManager.FailLevelImmediately();
        }
    }
}