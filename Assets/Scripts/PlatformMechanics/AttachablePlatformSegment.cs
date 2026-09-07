using UnityEngine;
using System.Collections;

public class AttachablePlatformSegment : MonoBehaviour
{
    // ==================================================
    // LEVEL DESIGNER SETTINGS
    // ==================================================

    [Header("Target Platform")]

    [Tooltip(
        "Optional. Leave empty if there is only one " +
        "Growing Platform in the scene."
    )]
    [SerializeField]
    private GrowingPlatform targetPlatform;


    [Header("Collection Animation")]

    [Tooltip(
        "How quickly this segment flies toward " +
        "the moving platform after being bonked."
    )]
    [Min(0.1f)]
    [SerializeField]
    private float attachSpeed = 12f;


    [Tooltip(
        "How close the segment gets before snapping " +
        "perfectly into its slot."
    )]
    [Min(0.001f)]
    [SerializeField]
    private float snapDistance = 0.05f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private Collider2D[] segmentColliders;

    private ColorPlatform colorPlatform;

    private bool hasBeenCollected = false;

    private Transform reservedSlot;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        segmentColliders =
            GetComponentsInChildren<Collider2D>(
                true
            );


        colorPlatform =
            GetComponent<ColorPlatform>();


        if (colorPlatform == null)
        {
            colorPlatform =
                GetComponentInChildren<ColorPlatform>();
        }


        // Automatically find the mover if
        // there is only one in the scene.
        if (targetPlatform == null)
        {
            targetPlatform =
                FindAnyObjectByType<GrowingPlatform>();
        }
    }


    // ==================================================
    // PLAYER BONKS SEGMENT
    // ==================================================

    public void TryCollect()
    {
        if (hasBeenCollected)
        {
            return;
        }


        if (targetPlatform == null)
        {
            Debug.LogWarning(
                name +
                " has no Growing Platform target."
            );

            return;
        }


        if (
            !targetPlatform.TryReserveNextSlot(
                out reservedSlot
            )
        )
        {
            // Platform is full.
            return;
        }


        hasBeenCollected = true;


        StartCoroutine(
            FlyToPlatform()
        );
    }


    // ==================================================
    // FLY TO PLATFORM
    // ==================================================

    private IEnumerator FlyToPlatform()
    {
        // Stop ColorPlatform from changing
        // collision while the piece is flying.
        if (colorPlatform != null)
        {
            colorPlatform.enabled = false;
        }


        SetCollision(false);


        // Chase the slot even while the
        // moving platform is traveling.
        while (reservedSlot != null)
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    reservedSlot.position,
                    attachSpeed * Time.deltaTime
                );


            if (
                Vector2.Distance(
                    transform.position,
                    reservedSlot.position
                )
                <=
                snapDistance
            )
            {
                break;
            }


            yield return null;
        }


        if (reservedSlot == null)
        {
            yield break;
        }


        AttachToSlot();
    }


    // ==================================================
    // ATTACH
    // ==================================================

    private void AttachToSlot()
    {
        transform.SetParent(
            reservedSlot
        );


        transform.localPosition =
            Vector3.zero;


        transform.localRotation =
            Quaternion.identity;


        targetPlatform.ConfirmAttachment(
            reservedSlot
        );


        if (colorPlatform != null)
        {
            colorPlatform.enabled = true;
        }

        else
        {
            SetCollision(true);
        }
    }


    // ==================================================
    // COLLISION HELPER
    // ==================================================

    private void SetCollision(
        bool enabledState
    )
    {
        foreach (
            Collider2D segmentCollider
            in segmentColliders
        )
        {
            segmentCollider.enabled =
                enabledState;
        }
    }
}