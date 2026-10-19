using UnityEngine;

public class MovingPlatformTrackVisual : MonoBehaviour
{
    // ==================================================
    // LINKED PLATFORM
    // ==================================================

    [Header("Linked Platform")]

    [Tooltip("Drag the moving platform that this track belongs to here.")]
    [SerializeField]
    private Transform linkedPlatform;
    private UniversalMovingPlatform generatedDirectionSource;


    // ==================================================
    // TRACK ANIMATION
    // ==================================================

    [Header("Track Animation")]

    [SerializeField]
    private SpriteRenderer trackRenderer;

    [Tooltip("Put the sliced animation frames here in playback order.")]
    [SerializeField]
    private Sprite[] animationFrames;

    [Tooltip("How many animation frames play per second.")]
    [Min(0.1f)]
    [SerializeField]
    private float framesPerSecond = 8f;


    // ==================================================
    // ACTIVATION
    // ==================================================

    [Header("Activation")]

    [Tooltip(
        "Enable this if the linked platform begins moving automatically."
    )]
    [SerializeField]
    private bool alwaysActive = false;


    [Tooltip(
        "Once activated, keep the track at full opacity."
    )]
    [SerializeField]
    private bool stayActiveAfterTriggered = true;


    [Tooltip(
        "If checked, the track animation only plays while " +
        "the linked platform is physically moving."
    )]
    [SerializeField]
    private bool stopAnimationWhenPlatformStops = false;


    [Range(0f, 1f)]
    [SerializeField]
    private float inactiveAlpha = 0.5f;


    [Range(0f, 1f)]
    [SerializeField]
    private float activeAlpha = 1f;


    // ==================================================
    // REVERSE ANIMATION
    // ==================================================

    [Header("Reverse Animation")]

    [Tooltip(
        "Reverse the track animation when the platform " +
        "travels back in the opposite direction."
    )]
    [SerializeField]
    private bool reverseWithPlatformDirection = true;


    [Tooltip(
        "Enable this if the track animation visually moves " +
        "opposite the platform."
    )]
    [SerializeField]
    private bool invertAnimationDirection = false;


    // ==================================================
    // MOVEMENT DETECTION
    // ==================================================

    [Header("Movement Detection")]

    [SerializeField]
    [Min(0.00001f)]
    private float movementThreshold = 0.001f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private Vector3 lastPlatformPosition;

    private Vector3 originalTravelDirection;

    private bool hasOriginalTravelDirection = false;

    private bool hasBeenActivated = false;

    private bool isCurrentlyMoving = false;

    private int animationDirection = 1;

    private float animationTimer = 0f;

    private int currentFrame = 0;


    // ==================================================
    // SETUP
    // ==================================================

    private void Awake()
    {
        if (trackRenderer == null)
        {
            trackRenderer =
                GetComponent<SpriteRenderer>();
        }


        if (linkedPlatform != null)
        {
            lastPlatformPosition =
                linkedPlatform.position;
        }


        if (
            animationFrames != null &&
            animationFrames.Length > 0 &&
            trackRenderer != null
        )
        {
            trackRenderer.sprite =
                animationFrames[0];
        }


        if (alwaysActive)
        {
            hasBeenActivated = true;
        }


        UpdateOpacity();
    }


    // ==================================================
    // UPDATE
    // ==================================================

    private void LateUpdate()
    {
        if (
            linkedPlatform == null ||
            trackRenderer == null
        )
        {
            return;
        }


        DetectPlatformMovement();

        UpdateActivation();

        UpdateOpacity();

        UpdateAnimation();
    }


    // ==================================================
    // MOVEMENT DETECTION
    // ==================================================

    private void DetectPlatformMovement()
    {
        Vector3 movement =
            linkedPlatform.position -
            lastPlatformPosition;


        isCurrentlyMoving =
            movement.sqrMagnitude >
            movementThreshold * movementThreshold;


        if (isCurrentlyMoving)
        {
            Vector3 currentDirection =
                movement.normalized;


            if (!hasOriginalTravelDirection)
            {
                originalTravelDirection =
                    currentDirection;

                hasOriginalTravelDirection =
                    true;
            }


            if (
                reverseWithPlatformDirection &&
                hasOriginalTravelDirection
            )
            {
                float directionDot =
                    Vector3.Dot(
                        currentDirection,
                        originalTravelDirection
                    );


                if (directionDot > 0.25f)
                {
                    animationDirection = 1;
                }


                else if (directionDot < -0.25f)
                {
                    animationDirection = -1;
                }
            }
        }


        if (generatedDirectionSource != null && reverseWithPlatformDirection)
            animationDirection = generatedDirectionSource.TrackPlaybackDirection;

        lastPlatformPosition =
            linkedPlatform.position;
    }


    // ==================================================
    // ACTIVATION
    // ==================================================

    private void UpdateActivation()
    {
        if (alwaysActive)
        {
            hasBeenActivated = true;

            return;
        }


        if (isCurrentlyMoving)
        {
            hasBeenActivated = true;
        }


        if (!stayActiveAfterTriggered)
        {
            hasBeenActivated =
                isCurrentlyMoving;
        }
    }


    // ==================================================
    // OPACITY
    // ==================================================

    private void UpdateOpacity()
    {
        if (trackRenderer == null)
        {
            return;
        }


        bool active =
            alwaysActive ||
            hasBeenActivated;


        Color color =
            trackRenderer.color;


        color.a =
            active
            ? activeAlpha
            : inactiveAlpha;


        trackRenderer.color =
            color;
    }


    // ==================================================
    // ANIMATION
    // ==================================================

    private void UpdateAnimation()
    {
        if (
            animationFrames == null ||
            animationFrames.Length == 0
        )
        {
            return;
        }


        bool visuallyActive =
            alwaysActive ||
            hasBeenActivated;


        if (!visuallyActive)
        {
            return;
        }


        bool shouldAnimate;


        if (stopAnimationWhenPlatformStops)
        {
            shouldAnimate =
                isCurrentlyMoving;
        }

        else
        {
            shouldAnimate =
                visuallyActive;
        }


        if (!shouldAnimate)
        {
            return;
        }


        animationTimer +=
            Time.deltaTime;


        float frameDuration =
            1f /
            framesPerSecond;


        while (
            animationTimer >=
            frameDuration
        )
        {
            animationTimer -=
                frameDuration;


            int direction =
                animationDirection;


            if (invertAnimationDirection)
            {
                direction *= -1;
            }


            currentFrame +=
                direction;


            if (
                currentFrame >=
                animationFrames.Length
            )
            {
                currentFrame = 0;
            }


            else if (currentFrame < 0)
            {
                currentFrame =
                    animationFrames.Length - 1;
            }


            trackRenderer.sprite =
                animationFrames[currentFrame];
        }
    }
    public Sprite FirstTrackSprite => animationFrames != null && animationFrames.Length > 0 ?
        animationFrames[0] : (trackRenderer != null ? trackRenderer.sprite : GetComponent<SpriteRenderer>()?.sprite);

    public void ConfigureGeneratedTrack(UniversalMovingPlatform platform)
    {
        linkedPlatform = platform.transform;
        generatedDirectionSource = platform;
        trackRenderer = GetComponent<SpriteRenderer>();
        lastPlatformPosition = linkedPlatform.position;
        hasOriginalTravelDirection = false;
        hasBeenActivated = alwaysActive;
        animationTimer = 0f;
        currentFrame = 0;
        isCurrentlyMoving = false;
        if (animationFrames != null && animationFrames.Length > 0 && trackRenderer != null)
            trackRenderer.sprite = animationFrames[0];
        UpdateOpacity();
    }
}