using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    // ==================================================
    // CHECKPOINT
    // ==================================================

    [Header("Checkpoint")]

    [Tooltip(
        "Every checkpoint in a scene needs a unique ID."
    )]
    [SerializeField]
    private string checkpointID =
        "Checkpoint_01";


    [Tooltip(
        "Where the player respawns."
    )]
    [SerializeField]
    private Transform respawnPoint;


    [Header("Visual")]

    [SerializeField]
    private SavePointVisual savePointVisual;


    // ==================================================
    // INTERNAL
    // ==================================================

    private bool activated = false;

    private string sceneName;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        sceneName =
            SceneManager
            .GetActiveScene()
            .name;


        // If this checkpoint was already collected
        // before a scene reload, keep it hidden.
        if (
            CheckpointSystem
            .IsCheckpointCollected(
                sceneName,
                checkpointID
            )
        )
        {
            activated = true;


            if (savePointVisual != null)
            {
                savePointVisual
                    .HideImmediately();
            }
        }
    }


    // ==================================================
    // PLAYER ENTERS
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (activated)
        {
            return;
        }


        PlayerMovement player =
            other
            .GetComponentInParent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        activated = true;


        Vector3 savePosition =
            respawnPoint != null
            ? respawnPoint.position
            : transform.position;


        // ----------------------------------------------
        // SAVE
        // ----------------------------------------------

        CheckpointSystem.SetCheckpoint(
            sceneName,
            checkpointID,
            savePosition
        );

        GameAudio.Instance?.PlayCheckpoint();


        // ----------------------------------------------
        // VISUAL FEEDBACK
        // ----------------------------------------------

        if (savePointVisual != null)
        {
            savePointVisual
                .PlayCollectedAnimation();
        }
    }
}