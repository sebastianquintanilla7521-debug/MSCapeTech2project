using UnityEngine;

public class ColorTransitionBackground : MonoBehaviour
{
    // =========================
    // REFERENCES
    // =========================

    public ColorManager colorManager;


    // =========================
    // ANIMATION SETTINGS
    // =========================

    // How many seconds after the warning begins
    // before the next color actually starts appearing
    public float transitionDelay = 0.15f;

    // How far the next color peeks down
    public float peekAmount = 0.6f;

    // First bounce upward
    public float firstBounceAmount = 0.20f;

    // Second bounce downward
    public float secondBounceAmount = 0.35f;


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private float screenHeight;
    private float screenWidth;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        SetupSize();

        HideAboveScreen();
    }


    void Update()
    {
        if (colorManager == null || mainCamera == null)
        {
            return;
        }


        SetupSize();


        // =========================
        // NO WARNING
        // =========================

        if (!colorManager.IsWarning)
        {
            HideAboveScreen();

            return;
        }


        // =========================
        // NEXT COLOR
        // =========================

        spriteRenderer.color =
            colorManager.GetUnityColor(
                colorManager.nextColor
            );


        // Convert our delay from seconds
        // into part of the warning duration
        float delayProgress =
            transitionDelay /
            colorManager.warningDuration;


        // Before the delay is finished,
        // keep the next color COMPLETELY hidden
        if (
            colorManager.WarningProgress
            <= delayProgress
        )
        {
            HideAboveScreen();

            return;
        }


        // Remap the remaining warning time
        // back to a clean 0 -> 1 animation
        float animationProgress =
            Mathf.InverseLerp(
                delayProgress,
                1f,
                colorManager.WarningProgress
            );


        AnimateTransition(
            animationProgress
        );
    }


    // =========================
    // SIZE BACKGROUND
    // =========================

    void SetupSize()
    {
        screenHeight =
            mainCamera.orthographicSize * 2f;

        screenWidth =
            screenHeight * mainCamera.aspect;


        // Slight horizontal overscan prevents
        // tiny gaps on the left/right.
        //
        // IMPORTANT:
        // Vertical size is EXACTLY screenHeight.
        // This prevents the top edge from
        // accidentally being visible while hidden.
        transform.localScale = new Vector3(
            screenWidth + 0.5f,
            screenHeight,
            1f
        );
    }


    // =========================
    // TRANSITION ANIMATION
    // =========================

    void AnimateTransition(float progress)
    {
        float cameraCenterY =
            mainCamera.transform.position.y;


        // Position where the square is
        // completely above the screen
        float hiddenY =
            cameraCenterY + screenHeight;


        // Position where just a small amount
        // is peeking into the screen
        float peekY =
            hiddenY - peekAmount;


        float currentY;


        // =========================
        // PHASE 1
        // FIRST PEEK
        // =========================

        if (progress < 0.15f)
        {
            float t = Mathf.InverseLerp(
                0f,
                0.15f,
                progress
            );

            t = Smooth(t);


            currentY = Mathf.Lerp(
                hiddenY,
                peekY,
                t
            );
        }


        // =========================
        // PHASE 2
        // BOUNCE UP
        // =========================

        else if (progress < 0.25f)
        {
            float t = Mathf.InverseLerp(
                0.15f,
                0.25f,
                progress
            );

            t = Smooth(t);


            currentY = Mathf.Lerp(
                peekY,
                peekY + firstBounceAmount,
                t
            );
        }


        // =========================
        // PHASE 3
        // SECOND PEEK
        // =========================

        else if (progress < 0.38f)
        {
            float t = Mathf.InverseLerp(
                0.25f,
                0.38f,
                progress
            );

            t = Smooth(t);


            currentY = Mathf.Lerp(
                peekY + firstBounceAmount,
                peekY - secondBounceAmount,
                t
            );
        }


        // =========================
        // PHASE 4
        // SMALL RECOIL
        // =========================

        else if (progress < 0.48f)
        {
            float t = Mathf.InverseLerp(
                0.38f,
                0.48f,
                progress
            );

            t = Smooth(t);


            currentY = Mathf.Lerp(
                peekY - secondBounceAmount,
                peekY,
                t
            );
        }


        // =========================
        // PHASE 5
        // FULL SLIDE DOWN
        // =========================

        else
        {
            float t = Mathf.InverseLerp(
                0.48f,
                1f,
                progress
            );

            t = Smooth(t);


            currentY = Mathf.Lerp(
                peekY,
                cameraCenterY,
                t
            );
        }


        transform.position = new Vector3(
            mainCamera.transform.position.x,
            currentY,
            transform.position.z
        );
    }


    // =========================
    // COMPLETELY HIDE BACKGROUND
    // =========================

    void HideAboveScreen()
    {
        if (mainCamera == null)
        {
            return;
        }


        float cameraCenterY =
            mainCamera.transform.position.y;


        // Since the transition square is exactly
        // one screen tall, moving its center up
        // by one complete screen height means
        // its bottom edge sits exactly at the
        // top edge of the camera.
        float hiddenY =
            cameraCenterY + screenHeight;


        transform.position = new Vector3(
            mainCamera.transform.position.x,
            hiddenY,
            transform.position.z
        );
    }


    // =========================
    // SMOOTH ANIMATION
    // =========================

    float Smooth(float value)
    {
        return value
            * value
            * (3f - 2f * value);
    }
}