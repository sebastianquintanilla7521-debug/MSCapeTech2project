using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelResetManager : MonoBehaviour
{
    // ==================================================
    // LEVEL DESIGNER SETTINGS
    // ==================================================

    [Header("Reset Settings")]

    [Tooltip(
        "How long the game waits after the player fails " +
        "before restarting the level."
    )]
    [Min(0f)]
    [SerializeField]
    private float resetDelay = 0.5f;


    [Header("Testing")]

    [Tooltip(
        "Allows the level designer to press R " +
        "to quickly restart while testing."
    )]
    [SerializeField]
    private bool allowRestartKey = true;


    [SerializeField]
    private KeyCode restartKey = KeyCode.R;


    // ==================================================
    // INTERNAL
    // ==================================================

    private bool isResetting = false;


    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        if (
            allowRestartKey
            &&
            Input.GetKeyDown(restartKey)
        )
        {
            ResetLevel();
        }
    }


    // ==================================================
    // PLAYER FAILED
    // ==================================================

    public void FailLevel()
    {
        if (isResetting)
        {
            return;
        }


        isResetting = true;


        StartCoroutine(
            FailSequence()
        );
    }


    // ==================================================
    // FAIL SEQUENCE
    // ==================================================

    private IEnumerator FailSequence()
    {
        yield return new WaitForSeconds(
            resetDelay
        );


        ResetLevel();
    }


    // ==================================================
    // RESET LEVEL
    // ==================================================

    public void ResetLevel()
    {
        // Safety in case we later use
        // slow motion or pause effects.
        Time.timeScale = 1f;


        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }
}