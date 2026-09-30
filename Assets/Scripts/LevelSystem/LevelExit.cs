using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Level Transition")]

    [Tooltip(
        "How long to wait before loading the next level."
    )]
    [Min(0f)]
    [SerializeField]
    private float transitionDelay = 0.15f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private bool levelCompleted = false;


    // ==================================================
    // PLAYER ENTERS EXIT
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (levelCompleted)
        {
            return;
        }


        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        levelCompleted = true;


        // Stop player input immediately.
        player.enabled = false;


        Rigidbody2D playerBody =
            player.GetComponent<Rigidbody2D>();


        if (playerBody != null)
        {
            playerBody.linearVelocity =
                Vector2.zero;
        }


        StartCoroutine(
            LoadNextLevel()
        );
    }


    // ==================================================
    // LOAD NEXT LEVEL
    // ==================================================

    private IEnumerator LoadNextLevel()
    {
        if (transitionDelay > 0f)
        {
            yield return new WaitForSeconds(
                transitionDelay
            );
        }


        Scene currentScene =
            SceneManager.GetActiveScene();


        int nextSceneIndex =
            currentScene.buildIndex + 1;


        // Make sure another scene actually exists.
        if (
            nextSceneIndex >=
            SceneManager.sceneCountInBuildSettings
        )
        {
            Debug.LogWarning(
                "LevelExit: There is no next scene after " +
                currentScene.name +
                "."
            );


            yield break;
        }


        Time.timeScale = 1f;


        SceneManager.LoadScene(
            nextSceneIndex
        );
    }
}