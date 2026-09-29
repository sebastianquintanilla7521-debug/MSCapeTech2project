using UnityEngine;
using System.Collections;

public class PlatformDurability : MonoBehaviour
{
    // ==================================================
    // BREAK ORDER
    // ==================================================

    public enum BreakOrder
    {
        OldestFirst,
        NewestFirst,
        Random
    }


    // ==================================================
    // ENABLE / DISABLE
    // ==================================================

    [Header("Enable Durability")]

    [Tooltip(
        "Turn this off if this moving platform " +
        "should never lose segments."
    )]
    [SerializeField]
    private bool enableDurability = true;


    // ==================================================
    // DURABILITY TIMING
    // ==================================================

    [Header("Durability Timing")]

    [Tooltip(
        "How long after the player first lands " +
        "before the first segment begins warning."
    )]
    [Min(0f)]
    [SerializeField]
    private float firstBreakDelay = 4f;


    [Tooltip(
        "How long after one piece breaks before " +
        "the next piece begins warning."
    )]
    [Min(0f)]
    [SerializeField]
    private float timeBetweenBreaks = 2.5f;


    [Tooltip(
        "How long the segment shakes before breaking."
    )]
    [Min(0f)]
    [SerializeField]
    private float warningDuration = 1.5f;


    // ==================================================
    // BREAK BEHAVIOR
    // ==================================================

    [Header("Break Behavior")]

    [Tooltip(
        "Determines which segment breaks next."
    )]
    [SerializeField]
    private BreakOrder breakOrder =
        BreakOrder.OldestFirst;


    [Tooltip(
        "Number of segments that can never be destroyed. " +
        "Set to 0 if the entire platform can disappear."
    )]
    [Min(0)]
    [SerializeField]
    private int minimumSegmentsRemaining = 0;


    // ==================================================
    // WARNING EFFECT
    // ==================================================

    [Header("Warning Effect")]

    [Tooltip(
        "Maximum distance the segment shakes " +
        "right before breaking."
    )]
    [Min(0f)]
    [SerializeField]
    private float shakeAmount = 0.18f;


    [Tooltip(
        "How much the segment twists during the warning."
    )]
    [Min(0f)]
    [SerializeField]
    private float shakeRotation = 5f;


    [Tooltip(
        "How quickly the shaking changes direction."
    )]
    [Min(1f)]
    [SerializeField]
    private float shakeSpeed = 35f;


    // ==================================================
    // FALLING PIECE
    // ==================================================

    [Header("Falling Piece")]

    [Tooltip(
        "Gravity applied after the segment breaks."
    )]
    [Min(0f)]
    [SerializeField]
    private float brokenPieceGravity = 3.5f;


    [Tooltip(
        "How long broken debris remains before deletion."
    )]
    [Min(0.1f)]
    [SerializeField]
    private float brokenPieceLifetime = 4f;


    // ==================================================
    // START SETTINGS
    // ==================================================

    [Header("Start")]

    [Tooltip(
        "If enabled, durability begins when the " +
        "player first lands on this moving platform."
    )]
    [SerializeField]
    private bool startWhenPlayerLands = true;


    // ==================================================
    // INTERNAL
    // ==================================================

    private GrowingPlatform growingPlatform;

    private bool durabilityStarted = false;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        growingPlatform =
            GetComponent<GrowingPlatform>();


        if (growingPlatform == null)
        {
            Debug.LogError(
                name +
                " needs a GrowingPlatform component."
            );

            enabled = false;

            return;
        }


        // Durability completely disabled.
        if (!enableDurability)
        {
            return;
        }


        // Optional immediate-start mode.
        if (!startWhenPlayerLands)
        {
            BeginDurability();
        }
    }


    // ==================================================
    // PLAYER LANDS
    // ==================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (!enableDurability)
        {
            return;
        }


        if (
            durabilityStarted
            ||
            !startWhenPlayerLands
        )
        {
            return;
        }


        PlayerMovement player =
            collision.gameObject
            .GetComponent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        // Player must be above the platform.
        if (
            player.transform.position.y
            >
            transform.position.y + 0.1f
        )
        {
            BeginDurability();
        }
    }


    // ==================================================
    // BEGIN DURABILITY
    // ==================================================

    public void BeginDurability()
    {
        if (!enableDurability)
        {
            return;
        }


        if (durabilityStarted)
        {
            return;
        }


        durabilityStarted = true;


        StartCoroutine(
            DurabilityLoop()
        );
    }


    // ==================================================
    // MAIN DURABILITY LOOP
    // ==================================================

    private IEnumerator DurabilityLoop()
    {
        yield return new WaitForSeconds(
            firstBreakDelay
        );


        while (enableDurability)
        {
            int currentCount =
                growingPlatform
                .GetActiveSegmentCount();


            if (
                currentCount
                <=
                minimumSegmentsRemaining
            )
            {
                yield break;
            }


            Transform segment =
                ChooseNextSegment();


            if (segment == null)
            {
                yield break;
            }


            yield return StartCoroutine(
                WarnAndBreak(segment)
            );


            if (!enableDurability)
            {
                yield break;
            }


            yield return new WaitForSeconds(
                timeBetweenBreaks
            );
        }
    }


    // ==================================================
    // CHOOSE SEGMENT
    // ==================================================

    private Transform ChooseNextSegment()
    {
        if (
            breakOrder ==
            BreakOrder.NewestFirst
        )
        {
            return growingPlatform
                .GetNewestSegment();
        }


        if (
            breakOrder ==
            BreakOrder.Random
        )
        {
            return growingPlatform
                .GetRandomSegment();
        }


        return growingPlatform
            .GetOldestSegment();
    }


    // ==================================================
    // WARNING + BREAK
    // ==================================================

    private IEnumerator WarnAndBreak(
        Transform segment
    )
    {
        if (segment == null)
        {
            yield break;
        }


        Vector3 originalLocalPosition =
            segment.localPosition;


        Quaternion originalLocalRotation =
            segment.localRotation;


        float elapsed = 0f;


        while (
            elapsed < warningDuration
            &&
            segment != null
            &&
            enableDurability
        )
        {
            float progress =
                elapsed / warningDuration;


            // Starts smaller and becomes increasingly
            // violent as the break gets closer.
            float currentShakeAmount =
                Mathf.Lerp(
                    shakeAmount * 0.35f,
                    shakeAmount,
                    progress
                );


            float currentRotationAmount =
                Mathf.Lerp(
                    shakeRotation * 0.35f,
                    shakeRotation,
                    progress
                );


            // Change the shake rapidly enough that
            // it is very obvious visually.
            float shakeX =
                Mathf.Sin(
                    Time.time * shakeSpeed
                )
                * currentShakeAmount;


            float shakeY =
                Mathf.Cos(
                    Time.time
                    * shakeSpeed
                    * 1.37f
                )
                * currentShakeAmount;


            float rotation =
                Mathf.Sin(
                    Time.time
                    * shakeSpeed
                    * 0.85f
                )
                * currentRotationAmount;


            segment.localPosition =
                originalLocalPosition
                +
                new Vector3(
                    shakeX,
                    shakeY,
                    0f
                );


            segment.localRotation =
                originalLocalRotation
                *
                Quaternion.Euler(
                    0f,
                    0f,
                    rotation
                );


            elapsed += Time.deltaTime;


            yield return null;
        }


        if (segment == null)
        {
            yield break;
        }


        // Restore its exact placement before
        // either breaking or cancelling.
        segment.localPosition =
            originalLocalPosition;


        segment.localRotation =
            originalLocalRotation;


        // Durability was switched off
        // while the segment was warning.
        if (!enableDurability)
        {
            yield break;
        }


        BreakSegment(segment);
    }


    // ==================================================
    // BREAK SEGMENT
    // ==================================================

    private void BreakSegment(
        Transform segment
    )
    {
        if (segment == null)
        {
            return;
        }


        growingPlatform.RemoveActiveSegment(
            segment
        );


        // ==============================
        // DISABLE ATTACHMENT LOGIC
        // ==============================

        AttachablePlatformSegment attachable =
            segment.GetComponent<
                AttachablePlatformSegment
            >();


        if (attachable != null)
        {
            attachable.enabled = false;
        }


        // ==============================
        // DISABLE COLOR COLLISION LOGIC
        // ==============================

        ColorPlatform colorPlatform =
            segment.GetComponent<ColorPlatform>();


        if (colorPlatform == null)
        {
            colorPlatform =
                segment.GetComponentInChildren<
                    ColorPlatform
                >();
        }


        if (colorPlatform != null)
        {
            colorPlatform.enabled = false;
        }


        // ==============================
        // DISABLE COLLISION
        // ==============================

        Collider2D[] colliders =
            segment.GetComponentsInChildren<
                Collider2D
            >(
                true
            );


        foreach (
            Collider2D segmentCollider
            in colliders
        )
        {
            segmentCollider.enabled = false;
        }


        // ==============================
        // DETACH
        // ==============================

        segment.SetParent(
            null,
            true
        );


        // ==============================
        // FALL
        // ==============================

        Rigidbody2D fallingBody =
            segment.GetComponent<Rigidbody2D>();


        if (fallingBody == null)
        {
            fallingBody =
                segment.gameObject
                .AddComponent<Rigidbody2D>();
        }


        fallingBody.bodyType =
            RigidbodyType2D.Dynamic;


        fallingBody.gravityScale =
            brokenPieceGravity;


        fallingBody.freezeRotation =
            false;


        fallingBody.linearVelocity =
            new Vector2(
                Random.Range(
                    -0.5f,
                    0.5f
                ),
                -1f
            );


        fallingBody.angularVelocity =
            Random.Range(
                -150f,
                150f
            );


        Destroy(
            segment.gameObject,
            brokenPieceLifetime
        );
    }
}