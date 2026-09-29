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
            allowRestartKey &&
            Input.GetKeyDown(restartKey)
        )
        {
            ResetLevel();
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


        StartCoroutine(
            FailSequence()
        );
    }


    // ==================================================
    // FAIL DELAY
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
        Time.timeScale = 1f;


        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.name
        );
    }
}