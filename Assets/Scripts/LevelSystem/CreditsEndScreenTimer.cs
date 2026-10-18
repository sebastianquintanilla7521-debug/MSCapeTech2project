using UnityEngine;
using System.Collections;

public class CreditsEndScreenTimer : MonoBehaviour
{
    // ==================================================
    // END SCREEN
    // ==================================================

    [Header("End Screen")]

    [SerializeField]
    private CanvasGroup endScreen;


    [Tooltip(
        "How many seconds before the End Screen appears."
    )]
    [Min(0f)]
    [SerializeField]
    private float delayBeforeEndScreen = 10f;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        HideEndScreen();


        StartCoroutine(
            EndScreenCountdown()
        );
    }


    // ==================================================
    // TIMER
    // ==================================================

    private IEnumerator EndScreenCountdown()
    {
        yield return new WaitForSeconds(
            delayBeforeEndScreen
        );


        ShowEndScreen();
    }


    // ==================================================
    // SHOW
    // ==================================================

    private void ShowEndScreen()
    {
        if (endScreen == null)
        {
            return;
        }


        endScreen.alpha = 1f;

        endScreen.interactable = true;

        endScreen.blocksRaycasts = true;
    }


    // ==================================================
    // HIDE
    // ==================================================

    private void HideEndScreen()
    {
        if (endScreen == null)
        {
            return;
        }


        endScreen.alpha = 0f;

        endScreen.interactable = false;

        endScreen.blocksRaycasts = false;
    }
}