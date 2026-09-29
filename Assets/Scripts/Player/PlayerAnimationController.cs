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
            Input.GetAxisRaw("Horizontal");


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
            playerMovement.IsGroundDashing
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
}