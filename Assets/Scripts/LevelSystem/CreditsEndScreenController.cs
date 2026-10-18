using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsEndScreenController : MonoBehaviour
{
    // ==================================================
    // END SCREEN
    // ==================================================

    [Header("End Screen")]

    [SerializeField]
    private CanvasGroup endScreenPanel;


    [Tooltip(
        "How many seconds the player can explore the credits scene " +
        "before the end screen appears."
    )]
    [Min(0f)]
    [SerializeField]
    private float endScreenDelay = 10f;


    // ==================================================
    // RETURN TO MENU
    // ==================================================

    [Header("Return To Menu")]

    [SerializeField]
    private string startMenuSceneName =
        "StartMenu";


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        HideEndScreen();

        StartCoroutine(
            EndScreenTimer()
        );
    }


    // ==================================================
    // TIMER
    // ==================================================

    private IEnumerator EndScreenTimer()
    {
        yield return new WaitForSeconds(
            endScreenDelay
        );


        ShowEndScreen();
    }


    // ==================================================
    // SHOW END SCREEN
    // ==================================================

    private void ShowEndScreen()
    {
        if (endScreenPanel == null)
        {
            return;
        }


        endScreenPanel.alpha = 1f;

        endScreenPanel.interactable = true;

        endScreenPanel.blocksRaycasts = true;
    }


    // ==================================================
    // HIDE END SCREEN
    // ==================================================

    private void HideEndScreen()
    {
        if (endScreenPanel == null)
        {
            return;
        }


        endScreenPanel.alpha = 0f;

        endScreenPanel.interactable = false;

        endScreenPanel.blocksRaycasts = false;
    }


    // ==================================================
    // RETURN TO START MENU
    // ==================================================

    public void ReturnToMenu()
    {
        CheckpointSystem.ClearCheckpoint();

        Time.timeScale = 1f;


        SceneManager.LoadScene(
            startMenuSceneName
        );
    }
}