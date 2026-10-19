using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelResetManager : MonoBehaviour
{
    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Reset Settings")]

    [SerializeField]
    private float resetDelay = 0.5f;


    [Header("Testing")]

    [SerializeField]
    private bool allowRestartKey = true;


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
            allowRestartKey &&
            GameSettings.Pressed(GameAction.Restart)
        )
        {
            FailLevelImmediately();
        }
    }


    // ==================================================
    // FAIL LEVEL
    // ==================================================

    public void FailLevel()
    {
        if (isResetting)
        {
            return;
        }


        isResetting = true;
        GameAudio.Instance?.PlayDeath();


        StartCoroutine(
            FailSequence()
        );
    }


    // ==================================================
    // FAIL DELAY
    // ==================================================

    // Instant lethal reset, sharing the same guard as delayed deaths.
    public void FailLevelImmediately()
    {
        if (isResetting)
        {
            return;
        }

        isResetting = true;
        GameAudio.Instance?.PlayDeath();
        ResetLevel();
    }

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
        Time.timeScale = 1f;


        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.name
        );
    }
}