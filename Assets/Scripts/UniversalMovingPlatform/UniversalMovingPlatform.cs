using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class UniversalMovingPlatform : MonoBehaviour
{
    // ==================================================
    // ENUMS
    // ==================================================

    public enum MovementType
    {
        Path,
        FerrisWheel
    }

    public enum StartMode
    {
        AlwaysMoving,
        WhenPlayerLands
    }

    public enum PathBehavior
    {
        OneWay,
        Loop,
        PingPong
    }

    public enum MovementEasing
    {
        Linear,
        Smooth,
        EaseInOut
    }

    public enum GrowDirection
    {
        After,
        Before,
        Alternate
    }

    public enum BreakOrder
    {
        OldestFirst,
        NewestFirst,
        Random
    }


    // ==================================================
    // BASIC SETUP
    // ==================================================

    [Header("Basic Setup")]

    [SerializeField]
    private MovementType movementType =
        MovementType.Path;

    [SerializeField]
    private StartMode startMode =
        StartMode.WhenPlayerLands;


    // ==================================================
    // MOVEMENT POINTS
    // ==================================================

    [Header("Movement Points")]

    [SerializeField]
    private Transform movementPointsParent;

    [Min(1)]
    [SerializeField]
    private int pointCount = 2;


    // ==================================================
    // PATH MOVEMENT
    // ==================================================

    [Header("Path Movement")]

    [SerializeField]
    private PathBehavior pathBehavior =
        PathBehavior.OneWay;

    [Min(0.01f)]
    [SerializeField]
    private float moveSpeed = 3f;

    [Min(0f)]
    [SerializeField]
    private float waitAtEachPoint = 0f;

    [SerializeField]
    private MovementEasing movementEasing =
        MovementEasing.Linear;

    [Min(0f)]
    [SerializeField]
    private float startDelay = 0f;

    [SerializeField]
    private bool startAtPoint1 = true;

    [SerializeField]
    private bool startMovingForward = true;

    [SerializeField]
    private bool loopForever = true;

    [Min(1)]
    [SerializeField]
    private int numberOfLoops = 1;


    // ==================================================
    // FERRIS WHEEL
    // ==================================================

    [Header("Ferris Wheel")]

    [SerializeField]
    private float rotationSpeed = 30f;

    [SerializeField]
    private bool clockwise = true;

    [SerializeField]
    private bool keepPlatformUpright = true;


    // ==================================================
    // STARTING PLATFORM
    // ==================================================

    [Header("Starting Platform")]

    [SerializeField]
    private Transform startingSegmentsParent;


    [Tooltip(
        "Prefab used when automatically building the starting platform."
    )]
    [SerializeField]
    private GameObject startingSegmentPrefab;


    [Min(1)]
    [SerializeField]
    private int startingSegmentCount = 3;


    [Min(0.01f)]
    [SerializeField]
    private float segmentSpacing = 1f;


    [SerializeField]
    private bool autoArrangeStartingSegments = true;


    // ==================================================
    // GROWING PLATFORM
    // ==================================================

    [Header("Growing Platform")]

    [SerializeField]
    private bool enableGrowingPlatform = false;

    [SerializeField]
    private GrowDirection growDirection =
        GrowDirection.After;

    [Tooltip("0 means unlimited.")]
    [Min(0)]
    [SerializeField]
    private int maximumAddedSegments = 0;


    // ==================================================
    // DURABILITY
    // ==================================================

    [Header("Durability")]

    [SerializeField]
    private bool enableDurability = false;

    [Min(0f)]
    [SerializeField]
    private float firstBreakDelay = 4f;

    [Min(0f)]
    [SerializeField]
    private float timeBetweenBreaks = 2.5f;

    [Min(0f)]
    [SerializeField]
    private float warningDuration = 1.2f;

    [SerializeField]
    private BreakOrder breakOrder =
        BreakOrder.OldestFirst;

    [Min(0)]
    [SerializeField]
    private int minimumSegmentsRemaining = 0;


    // ==================================================
    // DURABILITY WARNING
    // ==================================================

    [Header("Durability Warning")]

    [Min(0f)]
    [SerializeField]
    private float shakeAmount = 0.18f;

    [Min(0f)]
    [SerializeField]
    private float shakeRotation = 5f;

    [Min(1f)]
    [SerializeField]
    private float shakeSpeed = 35f;


    // ==================================================
    // BROKEN SEGMENT
    // ==================================================

    [Header("Broken Segment")]

    [Min(0f)]
    [SerializeField]
    private float brokenPieceGravity = 3.5f;

    [Min(0.1f)]
    [SerializeField]
    private float brokenPieceLifetime = 4f;


    // ==================================================
    // EDITOR QUALITY OF LIFE
    // ==================================================

    [Header("Editor Quality Of Life")]

    [SerializeField]
    private bool snapPointsToGrid = true;

    [Min(0.01f)]
    [SerializeField]
    private float gridSize = 1f;

    [SerializeField]
    private bool showPointMarkers = true;

    [SerializeField]
    private bool showPathLines = true;

    [SerializeField]
    private bool showDirectionArrows = true;

    [SerializeField]
    private bool showPointNumbers = true;

    [Min(0.05f)]
    [SerializeField]
    private float markerSize = 0.4f;


    // ==================================================
    // ADVANCED
    // ==================================================

    [Header("Advanced")]

    [Min(0.001f)]
    [SerializeField]
    private float pointReachDistance = 0.05f;


    // ==================================================
    // INTERNAL - GENERAL
    // ==================================================

    private Rigidbody2D rb;

    private bool movementStarted = false;
    private bool movementFinished = false;

    private float startDelayTimer = 0f;


    // ==================================================
    // INTERNAL - PATH
    // ==================================================

    private int currentPointIndex = -1;
    private int targetPointIndex = -1;

    private int movementDirection = 1;

    private Vector2 segmentStartPosition;
    private Vector2 segmentTargetPosition;

    private float segmentTimer = 0f;
    private float segmentDuration = 0f;

    private float waitTimer = 0f;

    private bool segmentPrepared = false;

    private int completedLoops = 0;

    private bool loopClosureSegment = false;

    private int pingPongStartingEnd = 0;
    private int pingPongOppositeEnd = 0;

    private bool reachedPingPongOppositeEnd = false;


    // ==================================================
    // INTERNAL - FERRIS
    // ==================================================

    private float ferrisRadius = 0f;
    private float ferrisAngle = 0f;

    private float ferrisStartingRotation = 0f;
    private float ferrisAccumulatedRotation = 0f;


    // ==================================================
    // INTERNAL - GROWING
    // ==================================================

    private HashSet<int> reservedGrowthPositions =
        new HashSet<int>();

    private List<Transform> addedGrowthSegments =
        new List<Transform>();

    private bool alternateGrowAfter = true;


    // ==================================================
    // INTERNAL - DURABILITY
    // ==================================================

    private bool durabilityStarted = false;


    // ==================================================
    // PUBLIC EDITOR ACCESS
    // ==================================================

    public MovementType PlatformMovementType
    {
        get { return movementType; }
    }

    public Transform MovementPointsParent
    {
        get { return movementPointsParent; }
        set { movementPointsParent = value; }
    }

    public Transform StartingSegmentsParent
    {
        get { return startingSegmentsParent; }
        set { startingSegmentsParent = value; }
    }

    public int PointCount
    {
        get
        {
            if (
                movementType ==
                MovementType.FerrisWheel
            )
            {
                return 1;
            }

            return pointCount;
        }

        set
        {
            pointCount =
                Mathf.Max(1, value);
        }
    }

    public bool SnapPointsToGrid
    {
        get { return snapPointsToGrid; }
    }

    public float GridSize
    {
        get { return gridSize; }
    }


    // ==================================================
    // RESET
    // ==================================================

    private void Reset()
    {
        FindSetupObjects();
    }


    // ==================================================
    // VALIDATE
    // ==================================================

    private void OnValidate()
    {
        FindSetupObjects();


        if (
            movementType ==
            MovementType.FerrisWheel
        )
        {
            pointCount = 1;

            enableGrowingPlatform = false;
        }


        if (
            autoArrangeStartingSegments
        )
        {
            ArrangeStartingSegments();
        }
    }


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();


        FindSetupObjects();
    }


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        SetupMovement();


        if (
            startMode ==
            StartMode.AlwaysMoving
        )
        {
            BeginMovement();
        }
    }


    // ==================================================
    // FIXED UPDATE
    // ==================================================

    private void FixedUpdate()
    {
        if (
            !movementStarted ||
            movementFinished
        )
        {
            return;
        }


        if (startDelayTimer > 0f)
        {
            startDelayTimer -=
                Time.fixedDeltaTime;

            return;
        }


        if (
            movementType ==
            MovementType.FerrisWheel
        )
        {
            MoveFerrisWheel();
        }

        else
        {
            MoveAlongPath();
        }
    }


    // ==================================================
    // FIND SETUP
    // ==================================================

    private void FindSetupObjects()
    {
        if (
            startingSegmentsParent ==
            null
        )
        {
            Transform found =
                transform.Find(
                    "StartingSegments"
                );


            if (found != null)
            {
                startingSegmentsParent =
                    found;
            }
        }
    }


    // ==================================================
    // SETUP MOVEMENT
    // ==================================================

    private void SetupMovement()
    {
        movementFinished = false;

        movementStarted = false;

        completedLoops = 0;

        segmentPrepared = false;

        waitTimer = 0f;

        loopClosureSegment = false;

        reachedPingPongOppositeEnd = false;


        if (
            movementType ==
            MovementType.FerrisWheel
        )
        {
            SetupFerrisWheel();
        }

        else
        {
            SetupPath();
        }
    }


    // ==================================================
    // BEGIN MOVEMENT
    // ==================================================

    public void BeginMovement()
    {
        if (
            movementStarted ||
            movementFinished
        )
        {
            return;
        }


        movementStarted = true;

        startDelayTimer =
            startDelay;


        if (enableDurability)
        {
            BeginDurability();
        }
    }


    // ==================================================
    // PLAYER LAND START
    // ==================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        TryStartFromPlayer(
            collision
        );
    }


    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        TryStartFromPlayer(
            collision
        );
    }


    private void TryStartFromPlayer(
        Collision2D collision
    )
    {
        if (
            movementStarted ||
            movementFinished ||
            startMode !=
            StartMode.WhenPlayerLands
        )
        {
            return;
        }


        PlayerMovement player =
            collision.gameObject
            .GetComponentInParent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        if (
            player.transform.position.y >
            transform.position.y
        )
        {
            BeginMovement();
        }
    }


    // ==================================================
    // PATH SETUP
    // ==================================================

    private void SetupPath()
    {
        if (
            movementPointsParent == null
        )
        {
            Debug.LogWarning(
                gameObject.name +
                ": No Movement Points Parent is assigned."
            );


            movementFinished = true;

            return;
        }


        int count =
            movementPointsParent.childCount;


        if (count < 2)
        {
            Debug.LogWarning(
                gameObject.name +
                ": Path movement needs at least 2 points."
            );


            movementFinished = true;

            return;
        }


        movementDirection =
            startMovingForward
            ? 1
            : -1;


        if (startAtPoint1)
        {
            currentPointIndex =
                movementDirection > 0
                ? 0
                : count - 1;


            rb.position =
                movementPointsParent
                .GetChild(
                    currentPointIndex
                )
                .position;


            targetPointIndex =
                currentPointIndex +
                movementDirection;
        }

        else
        {
            currentPointIndex = -1;


            targetPointIndex =
                movementDirection > 0
                ? 0
                : count - 1;
        }


        pingPongStartingEnd =
            movementDirection > 0
            ? 0
            : count - 1;


        pingPongOppositeEnd =
            movementDirection > 0
            ? count - 1
            : 0;
    }


    // ==================================================
    // PATH MOVEMENT
    // ==================================================

    private void MoveAlongPath()
    {
        if (
            movementPointsParent == null ||
            movementPointsParent.childCount < 2
        )
        {
            movementFinished = true;

            return;
        }


        if (waitTimer > 0f)
        {
            waitTimer -=
                Time.fixedDeltaTime;

            return;
        }


        if (!segmentPrepared)
        {
            PreparePathSegment();


            if (movementFinished)
            {
                return;
            }
        }


        segmentTimer +=
            Time.fixedDeltaTime;


        float rawT =
            segmentDuration <= 0f
            ? 1f
            : Mathf.Clamp01(
                segmentTimer /
                segmentDuration
            );


        float easedT =
            ApplyMovementEasing(
                rawT
            );


        Vector2 newPosition =
            Vector2.Lerp(
                segmentStartPosition,
                segmentTargetPosition,
                easedT
            );


        rb.MovePosition(
            newPosition
        );


        if (
            rawT >= 1f ||
            Vector2.Distance(
                newPosition,
                segmentTargetPosition
            )
            <=
            pointReachDistance
        )
        {
            rb.position =
                segmentTargetPosition;


            ReachPathPoint();
        }
    }


    // ==================================================
    // PREPARE PATH SECTION
    // ==================================================

    private void PreparePathSegment()
    {
        int count =
            movementPointsParent.childCount;


        if (
            targetPointIndex < 0 ||
            targetPointIndex >= count
        )
        {
            movementFinished = true;

            return;
        }


        segmentStartPosition =
            rb.position;


        segmentTargetPosition =
            movementPointsParent
            .GetChild(
                targetPointIndex
            )
            .position;


        float distance =
            Vector2.Distance(
                segmentStartPosition,
                segmentTargetPosition
            );


        segmentDuration =
            distance /
            Mathf.Max(
                0.01f,
                moveSpeed
            );


        segmentTimer = 0f;

        segmentPrepared = true;
    }


    // ==================================================
    // REACHED PATH POINT
    // ==================================================

    private void ReachPathPoint()
    {
        currentPointIndex =
            targetPointIndex;


        segmentPrepared = false;


        if (loopClosureSegment)
        {
            loopClosureSegment = false;

            completedLoops++;


            if (
                !loopForever &&
                completedLoops >=
                numberOfLoops
            )
            {
                movementFinished = true;

                return;
            }
        }


        ChooseNextPathPoint();


        if (!movementFinished)
        {
            waitTimer =
                waitAtEachPoint;
        }
    }


    // ==================================================
    // CHOOSE NEXT PATH POINT
    // ==================================================

    private void ChooseNextPathPoint()
    {
        int count =
            movementPointsParent.childCount;


        // ONE WAY
        if (
            pathBehavior ==
            PathBehavior.OneWay
        )
        {
            int next =
                currentPointIndex +
                movementDirection;


            if (
                next < 0 ||
                next >= count
            )
            {
                movementFinished = true;

                return;
            }


            targetPointIndex =
                next;

            return;
        }


        // LOOP
        if (
            pathBehavior ==
            PathBehavior.Loop
        )
        {
            int next =
                currentPointIndex +
                movementDirection;


            if (next >= count)
            {
                next = 0;

                loopClosureSegment = true;
            }

            else if (next < 0)
            {
                next =
                    count - 1;

                loopClosureSegment = true;
            }


            targetPointIndex =
                next;

            return;
        }


        // PING PONG
        if (
            pathBehavior ==
            PathBehavior.PingPong
        )
        {
            if (
                currentPointIndex ==
                pingPongOppositeEnd
            )
            {
                reachedPingPongOppositeEnd =
                    true;


                movementDirection *= -1;
            }

            else if (
                currentPointIndex ==
                pingPongStartingEnd &&
                reachedPingPongOppositeEnd
            )
            {
                completedLoops++;


                if (
                    !loopForever &&
                    completedLoops >=
                    numberOfLoops
                )
                {
                    movementFinished = true;

                    return;
                }


                reachedPingPongOppositeEnd =
                    false;


                movementDirection *= -1;
            }


            targetPointIndex =
                currentPointIndex +
                movementDirection;
        }
    }


    // ==================================================
    // EASING
    // ==================================================

    private float ApplyMovementEasing(
        float t
    )
    {
        if (
            movementEasing ==
            MovementEasing.Linear
        )
        {
            return t;
        }


        if (
            movementEasing ==
            MovementEasing.Smooth
        )
        {
            return Mathf.SmoothStep(
                0f,
                1f,
                t
            );
        }


        return
            t * t * t *
            (
                t *
                (
                    t * 6f - 15f
                )
                + 10f
            );
    }


    // ==================================================
    // FERRIS SETUP
    // ==================================================

    private void SetupFerrisWheel()
    {
        if (
            movementPointsParent == null ||
            movementPointsParent.childCount < 1
        )
        {
            Debug.LogWarning(
                gameObject.name +
                ": Ferris Wheel needs one pivot point."
            );


            movementFinished = true;

            return;
        }


        Transform pivot =
            movementPointsParent
            .GetChild(0);


        Vector2 offset =
            rb.position -
            (Vector2)pivot.position;


        ferrisRadius =
            offset.magnitude;


        if (ferrisRadius < 0.01f)
        {
            movementFinished = true;

            return;
        }


        ferrisAngle =
            Mathf.Atan2(
                offset.y,
                offset.x
            )
            *
            Mathf.Rad2Deg;


        ferrisStartingRotation =
            rb.rotation;


        ferrisAccumulatedRotation =
            0f;
    }


    // ==================================================
    // FERRIS MOVEMENT
    // ==================================================

    private void MoveFerrisWheel()
    {
        Transform pivot =
            movementPointsParent
            .GetChild(0);


        float direction =
            clockwise
            ? -1f
            : 1f;


        float angleChange =
            rotationSpeed *
            direction *
            Time.fixedDeltaTime;


        ferrisAngle +=
            angleChange;


        ferrisAccumulatedRotation +=
            angleChange;


        float radians =
            ferrisAngle *
            Mathf.Deg2Rad;


        Vector2 offset =
            new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            )
            *
            ferrisRadius;


        rb.MovePosition(
            (Vector2)pivot.position +
            offset
        );


        if (keepPlatformUpright)
        {
            rb.MoveRotation(
                ferrisStartingRotation
            );
        }

        else
        {
            rb.MoveRotation(
                ferrisStartingRotation +
                ferrisAccumulatedRotation
            );
        }
    }


    // ==================================================
    // ARRANGE STARTING SEGMENTS
    // ==================================================

    public void ArrangeStartingSegments()
    {
        if (
            startingSegmentsParent == null
        )
        {
            return;
        }


        for (
            int i = 0;
            i <
            startingSegmentsParent.childCount;
            i++
        )
        {
            Transform segment =
                startingSegmentsParent
                .GetChild(i);


            segment.localPosition =
                new Vector3(
                    i * segmentSpacing,
                    0f,
                    0f
                );
        }
    }


    // ==================================================
    // GROWING - CAN ACCEPT
    // ==================================================

    public bool CanAcceptGrowthSegment()
    {
        if (
            movementType ==
            MovementType.FerrisWheel
        )
        {
            return false;
        }


        if (
            !enableGrowingPlatform ||
            startingSegmentsParent == null
        )
        {
            return false;
        }


        CleanGrowthSegmentList();


        if (
            maximumAddedSegments > 0 &&
            (
                addedGrowthSegments.Count +
                reservedGrowthPositions.Count
            )
            >=
            maximumAddedSegments
        )
        {
            return false;
        }


        return true;
    }


    // ==================================================
    // GROWING - RESERVE POSITION
    // ==================================================

    public bool TryReserveGrowthPosition(
        out int reservedIndex,
        out Transform attachParent,
        out Vector3 localPosition
    )
    {
        reservedIndex = 0;

        attachParent = null;

        localPosition =
            Vector3.zero;


        if (!CanAcceptGrowthSegment())
        {
            return false;
        }


        HashSet<int> occupied =
            new HashSet<int>();


        for (
            int i = 0;
            i <
            startingSegmentsParent.childCount;
            i++
        )
        {
            Transform segment =
                startingSegmentsParent
                .GetChild(i);


            int index =
                Mathf.RoundToInt(
                    segment.localPosition.x /
                    segmentSpacing
                );


            occupied.Add(
                index
            );
        }


        foreach (
            int reservation
            in reservedGrowthPositions
        )
        {
            occupied.Add(
                reservation
            );
        }


        int minimumIndex = 0;
        int maximumIndex = 0;

        bool first = true;


        foreach (
            int index
            in occupied
        )
        {
            if (first)
            {
                minimumIndex = index;

                maximumIndex = index;

                first = false;
            }

            else
            {
                minimumIndex =
                    Mathf.Min(
                        minimumIndex,
                        index
                    );


                maximumIndex =
                    Mathf.Max(
                        maximumIndex,
                        index
                    );
            }
        }


        if (
            growDirection ==
            GrowDirection.After
        )
        {
            reservedIndex =
                maximumIndex + 1;
        }

        else if (
            growDirection ==
            GrowDirection.Before
        )
        {
            reservedIndex =
                minimumIndex - 1;
        }

        else
        {
            reservedIndex =
                alternateGrowAfter
                ? maximumIndex + 1
                : minimumIndex - 1;


            alternateGrowAfter =
                !alternateGrowAfter;
        }


        reservedGrowthPositions.Add(
            reservedIndex
        );


        attachParent =
            startingSegmentsParent;


        localPosition =
            new Vector3(
                reservedIndex *
                segmentSpacing,

                0f,
                0f
            );


        return true;
    }


    // ==================================================
    // GROWING - CONFIRM
    // ==================================================

    public void ConfirmGrowthAttachment(
        int reservedIndex,
        Transform segment
    )
    {
        reservedGrowthPositions.Remove(
            reservedIndex
        );


        if (
            segment != null &&
            !addedGrowthSegments.Contains(
                segment
            )
        )
        {
            addedGrowthSegments.Add(
                segment
            );
        }
    }


    // ==================================================
    // GROWING - CANCEL
    // ==================================================

    public void CancelGrowthReservation(
        int reservedIndex
    )
    {
        reservedGrowthPositions.Remove(
            reservedIndex
        );
    }


    // ==================================================
    // GROWING - REMOVED
    // ==================================================

    public void NotifyGrowthSegmentRemoved(
        Transform segment
    )
    {
        if (segment == null)
        {
            return;
        }


        addedGrowthSegments.Remove(
            segment
        );
    }


    private void CleanGrowthSegmentList()
    {
        addedGrowthSegments.RemoveAll(
            segment =>
            segment == null
        );
    }


    // ==================================================
    // DURABILITY START
    // ==================================================

    private void BeginDurability()
    {
        if (
            durabilityStarted ||
            !enableDurability
        )
        {
            return;
        }


        durabilityStarted =
            true;


        StartCoroutine(
            DurabilityLoop()
        );
    }


    // ==================================================
    // DURABILITY LOOP
    // ==================================================

    private IEnumerator DurabilityLoop()
    {
        // Start Delay belongs to the platform itself,
        // so durability waits through that as well.
        float initialDelay =
            startDelay +
            firstBreakDelay;


        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(
                initialDelay
            );
        }


        while (enableDurability)
        {
            // If we've reached the minimum,
            // don't permanently end durability.
            // Wait in case the player grows the platform again.
            while (
                GetActiveSegmentCount()
                <=
                minimumSegmentsRemaining
            )
            {
                if (!enableDurability)
                {
                    yield break;
                }


                yield return null;
            }


            Transform segment =
                ChooseSegmentToBreak();


            if (segment == null)
            {
                yield return null;

                continue;
            }


            yield return StartCoroutine(
                WarnAndBreak(
                    segment
                )
            );


            if (!enableDurability)
            {
                yield break;
            }


            if (
                timeBetweenBreaks > 0f
            )
            {
                yield return new WaitForSeconds(
                    timeBetweenBreaks
                );
            }
        }
    }


    // ==================================================
    // DURABILITY - COUNT SEGMENTS
    // ==================================================

    private int GetActiveSegmentCount()
    {
        if (
            startingSegmentsParent == null
        )
        {
            return 0;
        }


        return
            startingSegmentsParent.childCount;
    }


    // ==================================================
    // DURABILITY - CHOOSE SEGMENT
    // ==================================================

    private Transform ChooseSegmentToBreak()
    {
        if (
            startingSegmentsParent == null ||
            startingSegmentsParent.childCount == 0
        )
        {
            return null;
        }


        int count =
            startingSegmentsParent.childCount;


        if (
            breakOrder ==
            BreakOrder.NewestFirst
        )
        {
            return
                startingSegmentsParent
                .GetChild(
                    count - 1
                );
        }


        if (
            breakOrder ==
            BreakOrder.Random
        )
        {
            return
                startingSegmentsParent
                .GetChild(
                    Random.Range(
                        0,
                        count
                    )
                );
        }


        // Oldest first.
        return
            startingSegmentsParent
            .GetChild(0);
    }


    // ==================================================
    // DURABILITY WARNING
    // ==================================================

    private IEnumerator WarnAndBreak(
        Transform segment
    )
    {
        if (segment == null)
        {
            yield break;
        }


        Vector3 originalPosition =
            segment.localPosition;


        Quaternion originalRotation =
            segment.localRotation;


        float elapsed = 0f;


        while (
            elapsed < warningDuration &&
            segment != null &&
            enableDurability
        )
        {
            float progress =
                warningDuration <= 0f
                ? 1f
                : elapsed /
                  warningDuration;


            float currentShake =
                Mathf.Lerp(
                    shakeAmount * 0.3f,
                    shakeAmount,
                    progress
                );


            float currentRotation =
                Mathf.Lerp(
                    shakeRotation * 0.3f,
                    shakeRotation,
                    progress
                );


            float shakeX =
                Mathf.Sin(
                    Time.time *
                    shakeSpeed
                )
                *
                currentShake;


            float shakeY =
                Mathf.Cos(
                    Time.time *
                    shakeSpeed *
                    1.37f
                )
                *
                currentShake;


            float rotation =
                Mathf.Sin(
                    Time.time *
                    shakeSpeed *
                    0.85f
                )
                *
                currentRotation;


            segment.localPosition =
                originalPosition +
                new Vector3(
                    shakeX,
                    shakeY,
                    0f
                );


            segment.localRotation =
                originalRotation *
                Quaternion.Euler(
                    0f,
                    0f,
                    rotation
                );


            elapsed +=
                Time.deltaTime;


            yield return null;
        }


        if (segment == null)
        {
            yield break;
        }


        segment.localPosition =
            originalPosition;


        segment.localRotation =
            originalRotation;


        if (!enableDurability)
        {
            yield break;
        }


        BreakSegment(
            segment
        );
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


        // If this was a grown segment,
        // free up one of the growth count slots.
        NotifyGrowthSegmentRemoved(
            segment
        );


        // ==============================================
        // DISABLE ATTACHABLE BEHAVIOR
        // ==============================================

        UniversalAttachablePlatformSegment
            attachable =
            segment.GetComponent<
                UniversalAttachablePlatformSegment
            >();


        if (attachable != null)
        {
            attachable.enabled =
                false;
        }


        // ==============================================
        // DISABLE COLOR PLATFORM LOGIC
        // ==============================================

        ColorPlatform colorPlatform =
            segment.GetComponent<
                ColorPlatform
            >();


        if (colorPlatform == null)
        {
            colorPlatform =
                segment
                .GetComponentInChildren<
                    ColorPlatform
                >();
        }


        if (colorPlatform != null)
        {
            colorPlatform.enabled =
                false;
        }


        // ==============================================
        // DISABLE COLLISION
        // ==============================================

        Collider2D[] colliders =
            segment
            .GetComponentsInChildren<
                Collider2D
            >(
                true
            );


        foreach (
            Collider2D segmentCollider
            in colliders
        )
        {
            segmentCollider.enabled =
                false;
        }


        // ==============================================
        // DETACH FROM PLATFORM
        // ==============================================

        segment.SetParent(
            null,
            true
        );


        // ==============================================
        // MAKE PIECE FALL
        // ==============================================

        Rigidbody2D fallingBody =
            segment.GetComponent<
                Rigidbody2D
            >();


        if (fallingBody == null)
        {
            fallingBody =
                segment.gameObject
                .AddComponent<
                    Rigidbody2D
                >();
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