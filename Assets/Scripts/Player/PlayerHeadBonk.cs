using UnityEngine;

public class PlayerHeadBonk : MonoBehaviour
{
    // =========================
    // HEAD CHECK
    // =========================

    // Drag the HeadCheck child here.
    public Transform headCheck;

    // Size of the invisible area
    // above the player's head.
    public Vector2 headCheckSize =
        new Vector2(0.7f, 0.15f);


    // =========================
    // INTERNAL
    // =========================

    private Rigidbody2D rb;


    // =========================
    // START
    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // =========================
    // CHECK FOR HEAD BONK
    // =========================

    void FixedUpdate()
    {
        if (
            headCheck == null ||
            rb == null
        )
        {
            return;
        }


        // Only bonk things while
        // the Player is moving upward.
        if (rb.linearVelocity.y <= 0.1f)
        {
            return;
        }


        Collider2D[] hits =
            Physics2D.OverlapBoxAll(
                headCheck.position,
                headCheckSize,
                0f
            );


        foreach (Collider2D hit in hits)
        {
            AttachablePlatformSegment segment =
                hit.GetComponent<AttachablePlatformSegment>();


            if (segment == null)
            {
                segment =
                    hit.GetComponentInParent<
                        AttachablePlatformSegment
                    >();
            }


            if (segment != null)
            {
                segment.TryCollect();
            }
        }
    }


    // =========================
    // EDITOR VISUAL
    // =========================

    void OnDrawGizmosSelected()
    {
        if (headCheck == null)
        {
            return;
        }


        Gizmos.DrawWireCube(
            headCheck.position,
            headCheckSize
        );
    }
}