using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    // ==================================================
    // TRANSITION
    // ==================================================

    [Header("Level Transition")]

    [Min(0f)]
    [SerializeField]
    private float transitionDelay = 0.15f;


    [Tooltip(
        "Leave this empty to load the next scene normally. " +
        "Enter a scene name to load a specific scene instead."
    )]
    [SerializeField]
    private string targetSceneName = "";


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
        GameAudio.Instance?.PlayLevelComplete();


        player.enabled = false;


        Rigidbody2D playerBody =
            player.GetComponent<Rigidbody2D>();


        if (playerBody != null)
        {
            playerBody.linearVelocity =
                Vector2.zero;
        }


        StartCoroutine(
            LoadDestination()
        );
    }


    // ==================================================
    // LOAD DESTINATION
    // ==================================================

    private IEnumerator LoadDestination()
    {
        if (transitionDelay > 0f)
        {
            yield return new WaitForSeconds(
                transitionDelay
            );
        }


        Time.timeScale = 1f;


        // --------------------------------------------------
        // SPECIFIC SCENE
        // --------------------------------------------------

        if (
            !string.IsNullOrWhiteSpace(
                targetSceneName
            )
        )
        {
            SceneManager.LoadScene(
                targetSceneName
            );

            yield break;
        }


        // --------------------------------------------------
        // NEXT SCENE
        // --------------------------------------------------

        int currentSceneIndex =
            SceneManager
            .GetActiveScene()
            .buildIndex;


        int nextSceneIndex =
            currentSceneIndex + 1;


        if (
            nextSceneIndex >=
            SceneManager.sceneCountInBuildSettings
        )
        {
            Debug.LogWarning(
                "LevelExit: There is no next scene."
            );

            yield break;
        }


        SceneManager.LoadScene(
            nextSceneIndex
        );
    }
}