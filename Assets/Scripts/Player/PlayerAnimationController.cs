using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // ==================================================
    // PLAYER REFERENCES
    // ==================================================

    [Header("Player References")]

    [SerializeField]
    private PlayerMovement playerMovement;


    // ==================================================
    // PLAYER VISUALS
    // ==================================================

    [Header("Player Visuals")]

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;


    // ==================================================
    // GROUNDED ANIMATION
    // ==================================================

    [Header("Grounded Animation")]

    [Tooltip(
        "Uses the same GroundCheck child as the movement script."
    )]
    [SerializeField]
    private Transform animationGroundCheck;


    [Tooltip(
        "Which layer counts as ground."
    )]
    [SerializeField]
    private LayerMask groundLayer;


    [Tooltip(
        "Slightly larger than the real movement GroundCheck."
    )]
    [SerializeField]
    private float animationGroundCheckRadius = 0.28f;


    [Tooltip(
        "Small visual grace period before playing the jump animation."
    )]
    [SerializeField]
    private float groundedGraceTime = 0.08f;


    private float groundedGraceTimer = 0f;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        InitializeDashVisuals();
        if (animationGroundCheck == null)
        {
            Transform foundGroundCheck =
                transform.Find("GroundCheck");


            if (foundGroundCheck != null)
            {
                animationGroundCheck =
                    foundGroundCheck;
            }
        }
    }


    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        float horizontalInput =
            GameSettings.Horizontal;


        UpdateRunningAnimation(
            horizontalInput
        );


        UpdateGroundedAnimation();


        UpdateGroundDashAnimation();


        UpdateFacingDirection(
            horizontalInput
        );
    }


    // ==================================================
    // RUNNING ANIMATION
    // ==================================================

    private void UpdateRunningAnimation(
        float horizontalInput
    )
    {
        if (animator == null)
        {
            return;
        }


        bool isRunning =
            Mathf.Abs(horizontalInput)
            > 0.01f;


        animator.SetBool(
            "IsRunning",
            isRunning
        );
    }


    // ==================================================
    // GROUNDED ANIMATION
    // ==================================================

    private void UpdateGroundedAnimation()
    {
        if (
            animator == null ||
            playerMovement == null
        )
        {
            return;
        }


        bool movementSaysGrounded =
            playerMovement.IsGrounded;


        bool animationCheckSaysGrounded =
            false;


        if (animationGroundCheck != null)
        {
            Collider2D groundHit =
                Physics2D.OverlapCircle(
                    animationGroundCheck.position,
                    animationGroundCheckRadius,
                    groundLayer
                );


            animationCheckSaysGrounded =
                groundHit != null;
        }


        bool groundedForAnimation =
            movementSaysGrounded ||
            animationCheckSaysGrounded;


        if (groundedForAnimation)
        {
            groundedGraceTimer =
                groundedGraceTime;
        }
        else
        {
            groundedGraceTimer -=
                Time.deltaTime;
        }


        bool animationGrounded =
            groundedGraceTimer > 0f;


        animator.SetBool(
            "IsGrounded",
            animationGrounded
        );
    }


    // ==================================================
    // GROUND DASH ANIMATION
    // ==================================================

    private void UpdateGroundDashAnimation()
    {
        if (
            animator == null ||
            playerMovement == null
        )
        {
            return;
        }


        animator.SetBool(
            "IsGroundDashing",
            dashFrames == null && playerMovement.IsGroundDashing
        );
    }


    // ==================================================
    // FACING DIRECTION
    // ==================================================

    private void UpdateFacingDirection(
        float horizontalInput
    )
    {
        if (spriteRenderer == null)
        {
            return;
        }


        if (horizontalInput > 0.01f)
        {
            spriteRenderer.flipX =
                false;
        }

        else if (horizontalInput < -0.01f)
        {
            spriteRenderer.flipX =
                true;
        }
    }


    // ==================================================
    // DEBUG GROUND CHECK
    // ==================================================

    private void OnDrawGizmosSelected()
    {
        Transform check =
            animationGroundCheck;


        if (check == null)
        {
            check =
                transform.Find("GroundCheck");
        }


        if (check == null)
        {
            return;
        }


        Gizmos.DrawWireSphere(
            check.position,
            animationGroundCheckRadius
        );
    }

    [Header("Directional Dash Sprites")]
    [Tooltip("Assign the whole 192 x 160 texture, not one sliced sprite.")]
    [SerializeField] private Texture2D dashSpriteSheet;
    [Tooltip("0 uses the normal player sprite's pixels per unit.")]
    [SerializeField, Min(0f)] private float dashPixelsPerUnit = 0f;

    [Header("Dash Visual Timing (Seconds)")]
    [SerializeField, Min(0.001f)] private float dashEntryTime = 0.025f;
    [SerializeField, Min(0.001f)] private float dashWindUpTime = 0.035f;
    [SerializeField, Min(0.001f)] private float dashApexHoldTime = 0.10f;
    [SerializeField, Min(0.001f)] private float dashWindDownTime = 0.04f;
    [SerializeField, Min(0.001f)] private float dashExitTime = 0.035f;
    [SerializeField] private bool holdUpDashUntilFalling = true;

    private Sprite[,] dashFrames;
    private Rigidbody2D dashBody;
    private int seenDashVersion;
    private int jumpVersionAtDashStart;
    private int activeDashRow;
    private bool dashFlip;
    private bool dashVisualActive;
    private bool dashExitActive;
    private float dashVisualAge;
    private float dashExitAge;
    private Sprite normalAnimationSprite;

    private void InitializeDashVisuals()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerMovement != null)
        {
            dashBody = playerMovement.GetComponent<Rigidbody2D>();
            seenDashVersion = playerMovement.DashAnimationVersion;
        }
        if (spriteRenderer != null)
            normalAnimationSprite = spriteRenderer.sprite;

        if (dashSpriteSheet == null)
            return;
        if (dashSpriteSheet.width != 192 || dashSpriteSheet.height != 160)
        {
            Debug.LogError("Dash sheet must be imported at 192 x 160 pixels.", this);
            return;
        }

        float ppu = dashPixelsPerUnit > 0f ? dashPixelsPerUnit :
            (normalAnimationSprite != null ? normalAnimationSprite.pixelsPerUnit : 32f);
        dashFrames = new Sprite[5, 5];
        // Five occupied columns; the sixth 32-pixel column is empty.
        // Rows: diagonal up, diagonal down, up, down, horizontal.
        for (int row = 0; row < 5; row++)
        {
            for (int frame = 0; frame < 5; frame++)
            {
                Sprite sprite = Sprite.Create(dashSpriteSheet,
                    new Rect(frame * 32, (4 - row) * 32, 32, 32),
                    new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.FullRect);
                sprite.name = "Dash_Row" + row + "_Frame" + frame;
                dashFrames[row, frame] = sprite;
            }
        }
    }

    // Runs after the Animator evaluates its ordinary idle/run/jump sprite.
    private void LateUpdate()
    {
        if (dashFrames == null || playerMovement == null || spriteRenderer == null)
            return;

        // Do not leave a frozen dash pose after death/level completion disables movement.
        if (!playerMovement.isActiveAndEnabled)
        {
            if (dashVisualActive)
                FinishDashVisual();
            seenDashVersion = playerMovement.DashAnimationVersion;
            return;
        }

        if (playerMovement.DashAnimationVersion != seenDashVersion)
        {
            seenDashVersion = playerMovement.DashAnimationVersion;
            Vector2 direction = playerMovement.DashAnimationDirection;
            activeDashRow = direction.y > 0f ? (direction.x == 0f ? 2 : 0) :
                direction.y < 0f ? (direction.x == 0f ? 3 : 1) : 4;
            dashFlip = direction.x == 0f ? spriteRenderer.flipX : direction.x < 0f;
            jumpVersionAtDashStart = playerMovement.JumpAnimationVersion;
            dashVisualActive = true;
            dashExitActive = false;
            dashVisualAge = 0f;
            dashExitAge = 0f;
        }

        if (!dashVisualActive)
            return;

        if (dashExitActive)
        {
            dashExitAge += Time.deltaTime;
            if (dashExitAge >= Mathf.Max(0.001f, dashExitTime))
            {
                FinishDashVisual();
                return;
            }
            ShowDashFrame(4);
            return;
        }

        float entryEnd = Mathf.Max(0.001f, dashEntryTime);
        float windUpEnd = entryEnd + Mathf.Max(0.001f, dashWindUpTime);
        float apexEnd = windUpEnd + Mathf.Max(0.001f, dashApexHoldTime);
        float windDownEnd = apexEnd + Mathf.Max(0.001f, dashWindDownTime);

        if (dashVisualAge < entryEnd)
            ShowDashFrame(0);
        else if (dashVisualAge < windUpEnd)
            ShowDashFrame(1);
        else if (dashVisualAge < apexEnd)
            ShowDashFrame(2);
        else
            ShowDashFrame(3);

        bool upward = activeDashRow == 0 || activeDashRow == 2;
        bool nextAnimationReady = !playerMovement.IsDashing;
        if (upward && holdUpDashUntilFalling)
        {
            nextAnimationReady &= playerMovement.IsGrounded ||
                (dashBody != null && dashBody.linearVelocity.y <= 0f) ||
                playerMovement.JumpAnimationVersion != jumpVersionAtDashStart;
        }

        // Frame 5 never appears during an active movement dash.
        // Short movement dashes still get the complete visual body sequence.
        if (dashVisualAge >= windDownEnd && nextAnimationReady)
        {
            dashExitActive = true;
            dashExitAge = 0f;
            ShowDashFrame(4);
        }
        dashVisualAge += Time.deltaTime;
    }

    private void ShowDashFrame(int frame)
    {
        spriteRenderer.sprite = dashFrames[activeDashRow, frame];
        spriteRenderer.flipX = dashFlip;
    }

    private void FinishDashVisual()
    {
        dashVisualActive = false;
        dashExitActive = false;
        if (spriteRenderer == null)
            return;
        spriteRenderer.sprite = normalAnimationSprite;
        if (animator != null && animator.isActiveAndEnabled && playerMovement != null)
        {
            string state = !playerMovement.IsGrounded ? "Base Layer.Player_Jump" :
                Mathf.Abs(GameSettings.Horizontal) > 0.01f ?
                "Base Layer.Player_Run" : "Base Layer.Player_Idle";
            int hash = Animator.StringToHash(state);
            if (animator.HasState(0, hash))
            {
                animator.Play(hash, 0, 0f);
                animator.Update(0f);
            }
        }
    }

    private void OnDisable()
    {
        if (dashVisualActive)
            FinishDashVisual();
    }

    private void OnDestroy()
    {
        if (dashFrames == null)
            return;
        foreach (Sprite frame in dashFrames)
            if (frame != null)
                Destroy(frame);
    }

}