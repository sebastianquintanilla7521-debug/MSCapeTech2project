using UnityEngine;

public class MovingPlatformPath : MonoBehaviour
{
    // ==================================================
    // LEVEL DESIGNER SETTINGS
    // ==================================================

    [Header("Path")]

    [Tooltip(
        "Parent containing Point1, Point2, Point3, etc."
    )]
    [SerializeField]
    private Transform pathParent;


    [Header("Movement")]

    [Tooltip(
        "How fast the platform travels along the path."
    )]
    [Min(0.1f)]
    [SerializeField]
    private float moveSpeed = 3f;


    [Tooltip(
        "If enabled, the platform waits until the " +
        "player lands on it before beginning."
    )]
    [SerializeField]
    private bool startWhenPlayerLands = true;


    [Header("Path Preview")]

    [Tooltip(
        "Size of the temporary path markers " +
        "shown in the Scene view."
    )]
    [Min(0.05f)]
    [SerializeField]
    private float pathPointPreviewSize = 0.4f;


    [Tooltip(
        "Show the path points and connecting lines " +
        "while designing the level."
    )]
    [SerializeField]
    private bool showPathPreview = true;


    [Header("Advanced")]

    [Tooltip(
        "How close the platform must be to a path " +
        "point before it counts as reached."
    )]
    [Min(0.001f)]
    [SerializeField]
    private float pointReachDistance = 0.05f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private Rigidbody2D rb;

    private Transform[] pathPoints;

    private int currentPoint = 0;

    private bool hasStarted = false;

    private bool hasFinished = false;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();


        if (rb == null)
        {
            Debug.LogError(
                name +
                " needs a Rigidbody2D."
            );

            enabled = false;

            return;
        }


        if (pathParent == null)
        {
            Debug.LogError(
                name +
                " needs a Path Parent."
            );

            enabled = false;

            return;
        }


        BuildPath();


        if (pathPoints.Length == 0)
        {
            Debug.LogError(
                pathParent.name +
                " contains no path points."
            );

            enabled = false;

            return;
        }


        // Start exactly at Point1.
        rb.position =
            pathPoints[0].position;


        if (pathPoints.Length > 1)
        {
            currentPoint = 1;
        }

        else
        {
            hasFinished = true;
        }


        if (!startWhenPlayerLands)
        {
            hasStarted = true;
        }
    }


    // ==================================================
    // BUILD PATH
    // ==================================================

    private void BuildPath()
    {
        int pointCount =
            pathParent.childCount;


        pathPoints =
            new Transform[pointCount];


        for (
            int i = 0;
            i < pointCount;
            i++
        )
        {
            pathPoints[i] =
                pathParent.GetChild(i);
        }
    }


    // ==================================================
    // MOVE
    // ==================================================

    private void FixedUpdate()
    {
        if (
            !hasStarted
            ||
            hasFinished
            ||
            pathPoints == null
            ||
            pathPoints.Length < 2
        )
        {
            return;
        }


        Vector2 target =
            pathPoints[currentPoint].position;


        Vector2 newPosition =
            Vector2.MoveTowards(
                rb.position,
                target,
                moveSpeed * Time.fixedDeltaTime
            );


        rb.MovePosition(newPosition);


        if (
            Vector2.Distance(
                newPosition,
                target
            )
            <=
            pointReachDistance
        )
        {
            ReachCurrentPoint();
        }
    }


    // ==================================================
    // REACHED WAYPOINT
    // ==================================================

    private void ReachCurrentPoint()
    {
        rb.position =
            pathPoints[currentPoint].position;


        currentPoint++;


        // Stop permanently at final point.
        if (currentPoint >= pathPoints.Length)
        {
            currentPoint =
                pathPoints.Length - 1;

            hasFinished = true;
        }
    }


    // ==================================================
    // PLAYER STARTS PLATFORM
    // ==================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (
            hasStarted
            ||
            hasFinished
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


        // Player must land from above.
        if (
            player.transform.position.y
            >
            transform.position.y
        )
        {
            hasStarted = true;
        }
    }


    // ==================================================
    // EDITOR PATH PREVIEW
    // ==================================================

    private void OnDrawGizmos()
    {
        if (
            !showPathPreview
            ||
            pathParent == null
        )
        {
            return;
        }


        int pointCount =
            pathParent.childCount;


        if (pointCount == 0)
        {
            return;
        }


        for (
            int i = 0;
            i < pointCount;
            i++
        )
        {
            Transform point =
                pathParent.GetChild(i);


            // ------------------------------
            // TEMPORARY POINT "SPRITE"
            // ------------------------------

            Gizmos.DrawCube(
                point.position,
                new Vector3(
                    pathPointPreviewSize,
                    pathPointPreviewSize,
                    0.01f
                )
            );


            // ------------------------------
            // PATH CONNECTION
            // ------------------------------

            if (i < pointCount - 1)
            {
                Transform nextPoint =
                    pathParent.GetChild(i + 1);


                Gizmos.DrawLine(
                    point.position,
                    nextPoint.position
                );
            }
        }
    }
}