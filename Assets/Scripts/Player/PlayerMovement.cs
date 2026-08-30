using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // =========================
    // MOVEMENT SETTINGS
    // =========================

    public float moveSpeed = 10f;
    public float airMoveSpeed = 7.5f;


    // =========================
    // JUMP SETTINGS
    // =========================

    public float jumpForce = 15f;

    // 2 = normal jump + double jump
    public int maxJumps = 2;


    // =========================
    // GROUND CHECK
    // =========================

    public Transform groundCheck;
    public float groundCheckRadius = 0.18f;
    public LayerMask groundLayer;


    // =========================
    // DASH SETTINGS
    // =========================

    public float groundDashSpeed = 19f;

    public float airHorizontalDashSpeed = 3f;

    public float diagonalDashHorizontalSpeed = 4f;

    public float upDashForce = 16f;

    public float downwardDashSpeed = 14f;

    public float dashDuration = 0.045f;

    public float groundDashEndPause = 0.1f;

    public float airDashLandingPause = 0.1f;

    public float dashCooldown = 0.5f;


    // How long the game remembers your last
    // horizontal direction after releasing A/D
    public float dashDirectionMemory = 0.3f;


    // =========================
    // DASH INVENTORY
    // =========================

    // Final game maximum should be 3.
    // You can raise this in the Inspector while testing.
    public int maxDashCharges = 3;

    [SerializeField]
    private int dashCharges = 0;


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private float horizontal;
    private float vertical;

    private Rigidbody2D rb;

    private int jumpsRemaining;

    private bool isGrounded;
    private bool wasGrounded;

    private bool isDashing;
    private bool controlsLocked;

    // 1 = right
    // -1 = left
    private int facingDirection = 1;

    // Remembers when A or D was last held
    private float lastHorizontalInputTime = -999f;

    private bool landingPausePending = false;
    private bool landingPauseRunning = false;

    private float nextDashTime = 0f;


    // =========================
    // START
    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        jumpsRemaining = maxJumps;
    }


    // =========================
    // INPUT
    // =========================

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        // Remember most recent horizontal direction
        if (horizontal > 0)
        {
            facingDirection = 1;
            lastHorizontalInputTime = Time.time;
        }
        else if (horizontal < 0)
        {
            facingDirection = -1;
            lastHorizontalInputTime = Time.time;
        }


        // =========================
        // JUMP
        // =========================

        if (
            Input.GetKeyDown(KeyCode.Space)
            &&
            jumpsRemaining > 0
            &&
            !isDashing
            &&
            !controlsLocked
        )
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpsRemaining--;

            isGrounded = false;
        }


        // =========================
        // DASH
        // RIGHT CLICK
        // =========================

        if (
            Input.GetMouseButtonDown(1)
            &&
            dashCharges > 0
            &&
            Time.time >= nextDashTime
            &&
            !isDashing
            &&
            !controlsLocked
        )
        {
            if (isGrounded)
            {
                StartCoroutine(GroundDash());
            }
            else
            {
                StartCoroutine(AirDash());
            }
        }
    }


    // =========================
    // PHYSICS / MOVEMENT
    // =========================

    void FixedUpdate()
    {
        // First check whether the Player's feet
        // are actually touching a Ground layer.
        CheckGround();


        if (isDashing || controlsLocked)
        {
            return;
        }


        // Ground movement
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(
                horizontal * moveSpeed,
                rb.linearVelocity.y
            );
        }

        // Air movement
        else
        {
            rb.linearVelocity = new Vector2(
                horizontal * airMoveSpeed,
                rb.linearVelocity.y
            );
        }
    }


    // =========================
    // GROUND DETECTION
    // =========================

    void CheckGround()
    {
        bool touchingGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );


        // Only count as grounded if we're standing
        // still vertically or moving downward.
        //
        // This prevents the detector from instantly
        // resetting jumps while launching upward.
        bool groundedNow =
            touchingGround
            &&
            rb.linearVelocity.y <= 0.1f;


        // Detect the exact moment we land.
        if (groundedNow && !wasGrounded)
        {
            Landed();
        }


        isGrounded = groundedNow;
        wasGrounded = groundedNow;
    }


    // =========================
    // REMEMBER DASH DIRECTION
    // =========================

    int GetDashHorizontalDirection()
    {
        // Currently holding D
        if (horizontal > 0.01f)
        {
            return 1;
        }


        // Currently holding A
        if (horizontal < -0.01f)
        {
            return -1;
        }


        // A/D was recently released
        if (
            Time.time - lastHorizontalInputTime
            <= dashDirectionMemory
        )
        {
            return facingDirection;
        }


        // No recent horizontal direction
        return 0;
    }


    // =========================
    // GROUND DASH
    // =========================

    IEnumerator GroundDash()
    {
        // Spend one dash charge
        dashCharges--;


        // Start dash cooldown
        nextDashTime =
            Time.time + dashCooldown;


        isDashing = true;


        int dashDirection =
            GetDashHorizontalDirection();


        // If standing still, dash toward
        // whichever way we last faced.
        if (dashDirection == 0)
        {
            dashDirection = facingDirection;
        }


        rb.linearVelocity = new Vector2(
            dashDirection * groundDashSpeed,
            rb.linearVelocity.y
        );


        // Actual dash burst
        yield return new WaitForSeconds(
            dashDuration
        );


        // Stop horizontal movement
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );


        // Ground recovery pause
        yield return new WaitForSeconds(
            groundDashEndPause
        );


        isDashing = false;
    }


    // =========================
    // AIR DASH
    // =========================

    IEnumerator AirDash()
    {
        // Spend one dash charge
        dashCharges--;


        // Start dash cooldown
        nextDashTime =
            Time.time + dashCooldown;


        isDashing = true;


        // Remember that we need a recovery pause
        // when we eventually land.
        landingPausePending = true;


        float newX = rb.linearVelocity.x;
        float newY = rb.linearVelocity.y;


        int dashHorizontalDirection =
            GetDashHorizontalDirection();


        bool hasHorizontalDirection =
            dashHorizontalDirection != 0;


        // =========================
        // UP DASH
        // =========================

        if (vertical > 0)
        {
            newY = upDashForce;


            // Up + left/right
            if (hasHorizontalDirection)
            {
                newX =
                    dashHorizontalDirection
                    * diagonalDashHorizontalSpeed;
            }

            // Pure upward dash
            else
            {
                newX = 0f;
            }
        }


        // =========================
        // DOWN DASH
        // =========================

        else if (vertical < 0)
        {
            newY = -downwardDashSpeed;


            // Down + left/right
            if (hasHorizontalDirection)
            {
                newX =
                    dashHorizontalDirection
                    * diagonalDashHorizontalSpeed;
            }

            // Pure downward dash
            else
            {
                newX = 0f;
            }
        }


        // =========================
        // HORIZONTAL AIR DASH
        // =========================

        else if (hasHorizontalDirection)
        {
            newX =
                dashHorizontalDirection
                * airHorizontalDashSpeed;


            // Preserve whatever upward/downward
            // momentum the jump already had.
            newY = rb.linearVelocity.y;
        }


        // =========================
        // NOTHING HELD
        // =========================

        else
        {
            // Continue toward the player's
            // most recently faced direction.
            newX =
                facingDirection
                * airHorizontalDashSpeed;


            // Keep current jump/fall momentum.
            newY = rb.linearVelocity.y;
        }


        rb.linearVelocity = new Vector2(
            newX,
            newY
        );


        yield return new WaitForSeconds(
            dashDuration
        );


        isDashing = false;
    }


    // =========================
    // DASH PICKUP
    // =========================

    public bool AddDashCharge()
    {
        if (dashCharges >= maxDashCharges)
        {
            return false;
        }


        dashCharges++;


        dashCharges = Mathf.Min(
            dashCharges,
            maxDashCharges
        );


        return true;
    }


    // =========================
    // LANDED
    // =========================

    void Landed()
    {
        isGrounded = true;


        // Restore jumps.
        //
        // Dash charges DO NOT refill from landing.
        jumpsRemaining = maxJumps;


        // If we used an air dash before landing,
        // start the landing recovery pause.
        if (
            landingPausePending
            &&
            !landingPauseRunning
        )
        {
            landingPausePending = false;

            StartCoroutine(
                LandingPause()
            );
        }
    }


    // =========================
    // LANDING PAUSE
    // =========================

    IEnumerator LandingPause()
    {
        landingPauseRunning = true;

        controlsLocked = true;


        // Kill horizontal movement when landing.
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );


        yield return new WaitForSeconds(
            airDashLandingPause
        );


        controlsLocked = false;

        landingPauseRunning = false;
    }


    // =========================
    // DEBUG GROUND CHECK VISUAL
    // =========================

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}