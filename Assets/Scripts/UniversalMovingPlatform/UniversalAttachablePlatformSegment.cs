using UnityEngine;
using System.Collections;

public class UniversalAttachablePlatformSegment : MonoBehaviour
{
    // ==================================================
    // TARGET PLATFORM
    // ==================================================

    [Header("Target Platform")]

    [Tooltip(
        "Optional. Leave empty to automatically use " +
        "the nearest compatible Universal Moving Platform."
    )]
    [SerializeField]
    private UniversalMovingPlatform targetPlatform;


    [SerializeField]
    private bool automaticallyFindNearestPlatform = true;


    // ==================================================
    // ATTACHMENT
    // ==================================================

    [Header("Attachment")]

    [Min(0.1f)]
    [SerializeField]
    private float attachSpeed = 12f;


    [Min(0.001f)]
    [SerializeField]
    private float arrivalDistance = 0.05f;


    [SerializeField]
    private bool rotateToPlatform = true;


    [Min(0.1f)]
    [SerializeField]
    private float rotationFollowSpeed = 12f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private bool isFlying = false;

    private bool isAttached = false;


    private UniversalMovingPlatform reservedPlatform;

    private Transform reservedParent;

    private int reservedIndex;

    private Vector3 reservedLocalPosition;


    private Collider2D[] segmentColliders;

    private bool[] originalColliderStates;


    private ColorPlatform colorPlatform;

    private bool colorPlatformWasEnabled;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        CacheComponents();
    }


    // ==================================================
    // CACHE COMPONENTS
    // ==================================================

    private void CacheComponents()
    {
        segmentColliders =
            GetComponentsInChildren<Collider2D>(
                true
            );


        originalColliderStates =
            new bool[
                segmentColliders.Length
            ];


        for (
            int i = 0;
            i < segmentColliders.Length;
            i++
        )
        {
            originalColliderStates[i] =
                segmentColliders[i].enabled;
        }


        colorPlatform =
            GetComponent<ColorPlatform>();


        if (colorPlatform == null)
        {
            colorPlatform =
                GetComponentInChildren<ColorPlatform>();
        }


        if (colorPlatform != null)
        {
            colorPlatformWasEnabled =
                colorPlatform.enabled;
        }
    }


    // ==================================================
    // BONK
    // ==================================================

    public void Bonk()
    {
        if (
            isFlying ||
            isAttached
        )
        {
            return;
        }


        TryBeginAttachment();
    }


    // ==================================================
    // BEGIN ATTACHMENT
    // ==================================================

    private void TryBeginAttachment()
    {
        UniversalMovingPlatform platform =
            FindTargetPlatform();


        if (platform == null)
        {
            Debug.Log(
                gameObject.name +
                ": No compatible growing platform found."
            );

            return;
        }


        bool gotReservation =
            platform.TryReserveGrowthPosition(
                out reservedIndex,
                out reservedParent,
                out reservedLocalPosition
            );


        if (!gotReservation)
        {
            Debug.Log(
                gameObject.name +
                ": Platform could not reserve a growth position."
            );

            return;
        }


        reservedPlatform =
            platform;


        isFlying = true;


        PrepareForFlight();


        StartCoroutine(
            FlyToPlatform()
        );
    }


    // ==================================================
    // FIND TARGET PLATFORM
    // ==================================================

    private UniversalMovingPlatform FindTargetPlatform()
    {
        // Manual target gets priority.
        if (
            targetPlatform != null &&
            targetPlatform.CanAcceptGrowthSegment()
        )
        {
            return targetPlatform;
        }


        if (!automaticallyFindNearestPlatform)
        {
            return null;
        }


        UniversalMovingPlatform[] platforms =
            FindObjectsByType<UniversalMovingPlatform>(
                FindObjectsSortMode.None
            );


        UniversalMovingPlatform closestPlatform =
            null;


        float closestDistance =
            float.MaxValue;


        foreach (
            UniversalMovingPlatform platform
            in platforms
        )
        {
            if (
                platform == null ||
                !platform.CanAcceptGrowthSegment()
            )
            {
                continue;
            }


            float distance =
                (
                    platform.transform.position -
                    transform.position
                )
                .sqrMagnitude;


            if (distance < closestDistance)
            {
                closestDistance =
                    distance;


                closestPlatform =
                    platform;
            }
        }


        return closestPlatform;
    }


    // ==================================================
    // PREPARE FOR FLIGHT
    // ==================================================

    private void PrepareForFlight()
    {
        // Turn off collision while the piece
        // is flying toward the platform.
        for (
            int i = 0;
            i < segmentColliders.Length;
            i++
        )
        {
            segmentColliders[i].enabled =
                false;
        }


        // Prevent ColorPlatform from turning
        // colliders back on during flight.
        if (colorPlatform != null)
        {
            colorPlatform.enabled =
                false;
        }
    }


    // ==================================================
    // FLY TO PLATFORM
    // ==================================================

    private IEnumerator FlyToPlatform()
    {
        while (
            reservedPlatform != null &&
            reservedParent != null
        )
        {
            // Recalculate every frame because
            // the platform may itself be moving.
            Vector3 targetWorldPosition =
                reservedParent.TransformPoint(
                    reservedLocalPosition
                );


            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    targetWorldPosition,
                    attachSpeed *
                    Time.deltaTime
                );


            if (rotateToPlatform)
            {
                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        reservedParent.rotation,
                        rotationFollowSpeed *
                        Time.deltaTime
                    );
            }


            float distance =
                Vector3.Distance(
                    transform.position,
                    targetWorldPosition
                );


            if (
                distance <=
                arrivalDistance
            )
            {
                CompleteAttachment();

                yield break;
            }


            yield return null;
        }


        // Target disappeared while flying.
        if (reservedPlatform != null)
        {
            reservedPlatform.CancelGrowthReservation(
                reservedIndex
            );
        }


        isFlying = false;


        RestoreComponents();
    }


    // ==================================================
    // COMPLETE ATTACHMENT
    // ==================================================

    private void CompleteAttachment()
    {
        if (
            reservedPlatform == null ||
            reservedParent == null
        )
        {
            return;
        }


        transform.SetParent(
            reservedParent,
            true
        );


        transform.localPosition =
            reservedLocalPosition;


        if (rotateToPlatform)
        {
            transform.localRotation =
                Quaternion.identity;
        }


        isFlying = false;

        isAttached = true;


        RestoreComponents();


        reservedPlatform.ConfirmGrowthAttachment(
            reservedIndex,
            transform
        );


        targetPlatform =
            reservedPlatform;
    }


    // ==================================================
    // RESTORE COMPONENTS
    // ==================================================

    private void RestoreComponents()
    {
        if (colorPlatform != null)
        {
            colorPlatform.enabled =
                colorPlatformWasEnabled;
        }


        for (
            int i = 0;
            i < segmentColliders.Length;
            i++
        )
        {
            if (
                segmentColliders[i] != null
            )
            {
                segmentColliders[i].enabled =
                    originalColliderStates[i];
            }
        }
    }
}