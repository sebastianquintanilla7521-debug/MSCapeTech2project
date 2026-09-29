using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // =========================
    // MOVEMENT SETTINGS
    // =========================

    public float moveSpeed = 10f;
    public float airMoveSpeed = 8f;


    // =========================
    // JUMP SETTINGS
    // =========================

    public float jumpForce = 15f;
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

    // Ground horizontal dash
    public float groundDashSpeed = 19f;

    // How long the ground dash accelerates/eases
    public float groundDashMoveTime = 0.14f;


    // Air horizontal dash
    public float airHorizontalDashSpeed = 24f;

    // Small upward kick on a pure sideways air dash
    public float airHorizontalDashLift = 3f;

    // How long the sideways air dash accelerates/eases
    public float airHorizontalDashMoveTime = 0.14f;


    // Diagonal / vertical dash values
    public float diagonalDashHorizontalSpeed = 4f;
    public float upDashForce = 14f;
    public float downwardDashSpeed = 14f;

    // Used for W, W+D, S, etc.
    public float dashDuration = 0.045f;


    // =========================
    // DASH RECOVERY
    // =========================

    public float groundDashEndPause = 0.1f;
    public float airDashLandingPause = 0.1f;


    // =========================
    // DASH COOLDOWN
    // =========================

    public float dashCooldown = 0.5f;


    // =========================
    // DASH DIRECTION MEMORY
    // =========================

    // Your current value was 3 seconds.
    public float dashDirectionMemory = 3f;


    // =========================
    // DASH INVENTORY
    // =========================

    // Huge values for testing.
    // Later you can put this back to 3.
    public int maxDashCharges = 9999;

    [SerializeField]
    private int dashCharges = 9999;


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private float horizontal;
    private float vertical;

    private Rigidbody2D rb;

    private int jumpsRemaining;

    private bool isGrounded;
    private bool wasGrounded;

    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    private bool isDashing;
    private bool controlsLocked;

    private bool isGroundDashing;

    public bool IsGroundDashing
    {
        get { return isGroundDashing; }
    }

    // 1 = right
    // -1 = left
    private int facingDirection = 1;

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


        // Remember the most recent horizontal direction
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
    // NORMAL MOVEMENT
    // =========================

    void FixedUpdate()
    {
        CheckGround();


        // Normal movement should not overwrite
        // dash velocity or recovery pauses.
        if (isDashing || controlsLocked)
        {
            return;
        }


        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(
                horizontal * moveSpeed,
                rb.linearVelocity.y
            );
        }

        else
        {
            rb.linearVelocity = new Vector2(
                horizontal * airMoveSpeed,
                rb.linearVelocity.y
            );
        }
    }


    // =========================
    // GROUND CHECK
    // =========================

    void CheckGround()
    {
        if (groundCheck == null)
        {
            return;
        }


        bool touchingGround =
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );


        bool groundedNow =
            touchingGround
            &&
            rb.linearVelocity.y <= 0.1f;


        // We were in the air and just landed.
        if (groundedNow && !wasGrounded)
        {
            Landed();
        }


        isGrounded = groundedNow;
        wasGrounded = groundedNow;
    }


    // =========================
    // DASH DIRECTION MEMORY
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


        // Recently released A or D
        if (
            Time.time - lastHorizontalInputTime
            <= dashDirectionMemory
        )
        {
            return facingDirection;
        }


        return 0;
    }


    // =========================
    // GROUND DASH
    // =========================

    IEnumerator GroundDash()
    {
        dashCharges--;

        nextDashTime =
            Time.time + dashCooldown;

        isDashing = true;
        isGroundDashing = true;


        int direction =
            GetDashHorizontalDirection();


        if (direction == 0)
        {
            direction = facingDirection;
        }


        float startX =
            rb.linearVelocity.x;

        float peakX =
            direction * groundDashSpeed;


        // If still holding that direction,
        // ease back into normal running speed.
        // Otherwise ease toward zero.
        float endX = 0f;


        if (
            Mathf.Abs(horizontal) > 0.01f
            &&
            Mathf.Sign(horizontal) == direction
        )
        {
            endX =
                direction * moveSpeed;
        }


        // First portion accelerates into the dash.
        float accelerationTime =
            groundDashMoveTime * 0.3f;

        // Rest of the time eases back out.
        float decelerationTime =
            groundDashMoveTime - accelerationTime;


        float elapsed = 0f;


        // =========================
        // ACCELERATE
        // =========================

        while (elapsed < accelerationTime)
        {
            float t =
                elapsed / accelerationTime;

            t = Smooth(t);


            float newX =
                Mathf.Lerp(
                    startX,
                    peakX,
                    t
                );


            rb.linearVelocity = new Vector2(
                newX,
                rb.linearVelocity.y
            );


            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }


        elapsed = 0f;


        // =========================
        // EASE OUT
        // =========================

        while (elapsed < decelerationTime)
        {
            float t =
                elapsed / decelerationTime;

            t = Smooth(t);


            float newX =
                Mathf.Lerp(
                    peakX,
                    endX,
                    t
                );


            rb.linearVelocity = new Vector2(
                newX,
                rb.linearVelocity.y
            );


            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }


        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );


        // Small recovery after ground dash
        yield return new WaitForSeconds(
            groundDashEndPause
        );

        isGroundDashing = false;
        isDashing = false;
    }


    // =========================
    // AIR DASH
    // =========================

    IEnumerator AirDash()
    {
        dashCharges--;

        nextDashTime =
            Time.time + dashCooldown;

        isDashing = true;

        landingPausePending = true;


        int horizontalDirection =
            GetDashHorizontalDirection();


        bool hasHorizontalDirection =
            horizontalDirection != 0;


        // =========================
        // UP DASH
        // W + RIGHT CLICK
        // =========================

        if (vertical > 0)
        {
            float newX = 0f;


            if (hasHorizontalDirection)
            {
                // W + A/D
                newX =
                    horizontalDirection
                    * diagonalDashHorizontalSpeed;
            }


            rb.linearVelocity = new Vector2(
                newX,
                upDashForce
            );


            yield return new WaitForSeconds(
                dashDuration
            );


            isDashing = false;

            yield break;
        }


        // =========================
        // DOWN DASH
        // S + RIGHT CLICK
        // =========================

        if (vertical < 0)
        {
            float newX = 0f;


            if (hasHorizontalDirection)
            {
                newX =
                    horizontalDirection
                    * diagonalDashHorizontalSpeed;
            }


            rb.linearVelocity = new Vector2(
                newX,
                -downwardDashSpeed
            );


            yield return new WaitForSeconds(
                dashDuration
            );


            isDashing = false;

            yield break;
        }


        // =========================
        // SIDEWAYS AIR DASH
        // A/D + RIGHT CLICK
        // =========================

        if (!hasHorizontalDirection)
        {
            horizontalDirection =
                facingDirection;
        }


        yield return StartCoroutine(
            HorizontalAirDash(
                horizontalDirection
            )
        );


        isDashing = false;
    }


    // =========================
    // SMOOTH SIDEWAYS AIR DASH
    // =========================

    IEnumerator HorizontalAirDash(
        int direction
    )
    {
        float startX =
            rb.linearVelocity.x;


        float peakX =
            direction
            * airHorizontalDashSpeed;


        // Give the sideways dash a small upward
        // boost so it feels like an aerial movement
        // rather than a sideways teleport.
        float newY =
            Mathf.Max(
                rb.linearVelocity.y,
                airHorizontalDashLift
            );


        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            newY
        );


        // If A/D is still held,
        // finish at normal air movement speed.
        //
        // Otherwise ease toward zero.
        float endX = 0f;


        if (
            Mathf.Abs(horizontal) > 0.01f
            &&
            Mathf.Sign(horizontal) == direction
        )
        {
            endX =
                direction * airMoveSpeed;
        }


        float accelerationTime =
            airHorizontalDashMoveTime * 0.3f;


        float decelerationTime =
            airHorizontalDashMoveTime
            - accelerationTime;


        float elapsed = 0f;


        // =========================
        // ACCELERATE
        // =========================

        while (elapsed < accelerationTime)
        {
            float t =
                elapsed / accelerationTime;

            t = Smooth(t);


            float newX =
                Mathf.Lerp(
                    startX,
                    peakX,
                    t
                );


            rb.linearVelocity = new Vector2(
                newX,
                rb.linearVelocity.y
            );


            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }


        elapsed = 0f;


        // =========================
        // EASE OUT
        // =========================

        while (elapsed < decelerationTime)
        {
            float t =
                elapsed / decelerationTime;

            t = Smooth(t);


            float newX =
                Mathf.Lerp(
                    peakX,
                    endX,
                    t
                );


            rb.linearVelocity = new Vector2(
                newX,
                rb.linearVelocity.y
            );


            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }


        rb.linearVelocity = new Vector2(
            endX,
            rb.linearVelocity.y
        );
    }


    // =========================
    // DASH PICKUP SUPPORT
    // =========================

    public bool AddDashCharge()
    {
        if (dashCharges >= maxDashCharges)
        {
            return false;
        }


        dashCharges++;


        dashCharges =
            Mathf.Min(
                dashCharges,
                maxDashCharges
            );


        return true;
    }


    // =========================
    // LANDING
    // =========================

    void Landed()
    {
        isGrounded = true;

        // Restore jumps.
        jumpsRemaining = maxJumps;


        // Landing does NOT restore dash charges.
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
    // SMOOTHING
    // =========================

    float Smooth(float value)
    {
        return value
            * value
            * (3f - 2f * value);
    }


    // =========================
    // DEBUG GROUND CHECK
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