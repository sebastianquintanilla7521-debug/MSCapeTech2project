using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialPopupUI : MonoBehaviour
{
    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Popup Settings")]

    [SerializeField]
    private bool startHidden = true;


    // ==================================================
    // INTERNAL
    // ==================================================

    private CanvasGroup canvasGroup;

    private bool isOpen = false;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        canvasGroup =
            GetComponent<CanvasGroup>();


        if (startHidden)
        {
            HideImmediately();
        }

        else
        {
            ShowImmediately();
        }
    }


    // ==================================================
    // SHOW
    // ==================================================

    public void ShowPopup()
    {
        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }


        canvasGroup.alpha = 1f;

        canvasGroup.interactable = true;

        canvasGroup.blocksRaycasts = true;


        isOpen = true;
    }


    // ==================================================
    // HIDE
    // ==================================================

    public void HidePopup()
    {
        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }


        canvasGroup.alpha = 0f;

        canvasGroup.interactable = false;

        canvasGroup.blocksRaycasts = false;


        isOpen = false;
    }


    // ==================================================
    // STARTUP HELPERS
    // ==================================================

    private void HideImmediately()
    {
        canvasGroup.alpha = 0f;

        canvasGroup.interactable = false;

        canvasGroup.blocksRaycasts = false;


        isOpen = false;
    }


    private void ShowImmediately()
    {
        canvasGroup.alpha = 1f;

        canvasGroup.interactable = true;

        canvasGroup.blocksRaycasts = true;


        isOpen = true;
    }


    // ==================================================
    // PUBLIC STATE
    // ==================================================

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
    }
}