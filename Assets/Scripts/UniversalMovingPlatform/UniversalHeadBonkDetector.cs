using UnityEngine;

public class UniversalHeadBonkDetector : MonoBehaviour
{
    // ==================================================
    // REFERENCES
    // ==================================================

    [Header("Head Check")]

    [SerializeField]
    private Transform headCheck;


    [SerializeField]
    private Vector2 headCheckSize =
        new Vector2(
            0.7f,
            0.15f
        );


    [Header("Bonk Settings")]

    [Tooltip(
        "Player must be moving upward this fast " +
        "for the hit to count as a bonk."
    )]
    [SerializeField]
    private float minimumUpwardVelocity = 0.1f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private Rigidbody2D rb;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();


        if (headCheck == null)
        {
            Transform found =
                transform.Find(
                    "HeadCheck"
                );


            if (found != null)
            {
                headCheck =
                    found;
            }
        }
    }


    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        if (
            rb == null ||
            headCheck == null
        )
        {
            return;
        }


        // Only bonk while traveling upward.
        if (
            rb.linearVelocity.y <=
            minimumUpwardVelocity
        )
        {
            return;
        }


        CheckForUniversalSegment();
    }


    // ==================================================
    // BONK CHECK
    // ==================================================

    private void CheckForUniversalSegment()
    {
        Collider2D[] hits =
            Physics2D.OverlapBoxAll(
                headCheck.position,
                headCheckSize,
                0f
            );


        foreach (
            Collider2D hit
            in hits
        )
        {
            if (hit == null)
            {
                continue;
            }


            UniversalAttachablePlatformSegment segment =
                hit.GetComponentInParent<
                    UniversalAttachablePlatformSegment
                >();


            if (segment == null)
            {
                continue;
            }


            segment.Bonk();


            // One bonk per frame is enough.
            return;
        }
    }


    // ==================================================
    // DEBUG
    // ==================================================

    private void OnDrawGizmosSelected()
    {
        Transform check =
            headCheck;


        if (check == null)
        {
            check =
                transform.Find(
                    "HeadCheck"
                );
        }


        if (check == null)
        {
            return;
        }


        Gizmos.DrawWireCube(
            check.position,
            headCheckSize
        );
    }
}