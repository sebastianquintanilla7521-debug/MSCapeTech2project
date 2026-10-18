using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsSequenceController : MonoBehaviour
{
    // ==================================================
    // CREDITS
    // ==================================================

    [Header("Credits")]

    [SerializeField]
    private CanvasGroup creditsRoot;


    [Tooltip(
        "How long the Blue Screen shows by itself."
    )]
    [Min(0f)]
    [SerializeField]
    private float blueScreenDuration = 3f;


    // ==================================================
    // END SCREEN
    // ==================================================

    [Header("End Screen")]

    [SerializeField]
    private CanvasGroup endScreenRoot;


    [Tooltip(
        "How long the Credits remain before the final End Screen appears."
    )]
    [Min(0f)]
    [SerializeField]
    private float creditsDuration = 5f;


    // ==================================================
    // START MENU
    // ==================================================

    [Header("Return To Start")]

    [SerializeField]
    private string startMenuSceneName = "StartMenu";


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        Time.timeScale = 1f;


        HideGroup(
            creditsRoot
        );


        HideGroup(
            endScreenRoot
        );


        StartCoroutine(
            PlaySequence()
        );
    }


    // ==================================================
    // CREDITS SEQUENCE
    // ==================================================

    private IEnumerator PlaySequence()
    {
        // ----------------------------------------------
        // BLUE SCREEN
        // ----------------------------------------------

        yield return new WaitForSeconds(
            blueScreenDuration
        );


        // ----------------------------------------------
        // CREDITS
        // ----------------------------------------------

        ShowGroup(
            creditsRoot
        );


        yield return new WaitForSeconds(
            creditsDuration
        );


        // ----------------------------------------------
        // FINAL END SCREEN
        // ----------------------------------------------

        ShowGroup(
            endScreenRoot
        );
    }


    // ==================================================
    // SHOW
    // ==================================================

    private void ShowGroup(
        CanvasGroup group
    )
    {
        if (group == null)
        {
            return;
        }


        group.alpha = 1f;

        group.interactable = true;

        group.blocksRaycasts = true;
    }


    // ==================================================
    // HIDE
    // ==================================================

    private void HideGroup(
        CanvasGroup group
    )
    {
        if (group == null)
        {
            return;
        }


        group.alpha = 0f;

        group.interactable = false;

        group.blocksRaycasts = false;
    }


    // ==================================================
    // RETURN TO MENU
    // ==================================================

    public void ReturnToMenu()
    {
        GameAudio.Instance?.PlayUIClick();
        CheckpointSystem.ClearCheckpoint();


        Time.timeScale = 1f;


        SceneManager.LoadScene(
            startMenuSceneName
        );
    }
}