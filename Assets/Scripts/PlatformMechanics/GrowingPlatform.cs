using UnityEngine;
using System.Collections.Generic;

public class GrowingPlatform : MonoBehaviour
{
    // ==================================================
    // LEVEL DESIGNER SETTINGS
    // ==================================================

    [Header("Platform Layout")]

    [Tooltip(
        "Parent containing the pieces the platform " +
        "starts the level with."
    )]
    [SerializeField]
    private Transform startingSegmentsParent;


    [Tooltip(
        "Parent containing the empty attachment slots " +
        "for collected pieces."
    )]
    [SerializeField]
    private Transform segmentSlotsParent;


    [Tooltip(
        "Distance between each segment. " +
        "Keep at 2 for your current platform pieces."
    )]
    [Min(0.1f)]
    [SerializeField]
    private float segmentSpacing = 2f;


    [Tooltip(
        "Automatically arranges starting pieces " +
        "and attachment slots in the editor."
    )]
    [SerializeField]
    private bool autoArrangeSegments = true;


    // ==================================================
    // INTERNAL
    // ==================================================

    private HashSet<Transform> reservedSlots =
        new HashSet<Transform>();


    // This list is also the durability order.
    //
    // Oldest piece = beginning of list.
    // Newest collected piece = end of list.
    private List<Transform> activeSegments =
        new List<Transform>();


    // ==================================================
    // UNITY SETUP
    // ==================================================

    private void Reset()
    {
        FindParentsAutomatically();
    }


    private void OnValidate()
    {
        FindParentsAutomatically();


        if (
            !Application.isPlaying
            &&
            autoArrangeSegments
        )
        {
            ArrangePlatform();
        }
    }


    private void Start()
    {
        FindParentsAutomatically();

        BuildStartingSegmentList();
    }


    // ==================================================
    // FIND HIERARCHY OBJECTS
    // ==================================================

    private void FindParentsAutomatically()
    {
        if (startingSegmentsParent == null)
        {
            startingSegmentsParent =
                transform.Find("StartingSegments");
        }


        if (segmentSlotsParent == null)
        {
            segmentSlotsParent =
                transform.Find("SegmentSlots");
        }
    }


    // ==================================================
    // AUTOMATIC LAYOUT
    // ==================================================

    [ContextMenu("Arrange Platform Segments")]
    public void ArrangePlatform()
    {
        int startingCount = 0;


        // ==============================
        // STARTING SEGMENTS
        // ==============================

        if (startingSegmentsParent != null)
        {
            startingCount =
                startingSegmentsParent.childCount;


            for (
                int i = 0;
                i < startingCount;
                i++
            )
            {
                Transform segment =
                    startingSegmentsParent.GetChild(i);


                segment.localPosition =
                    new Vector3(
                        i * segmentSpacing,
                        0f,
                        0f
                    );
            }
        }


        // ==============================
        // COLLECTIBLE SLOTS
        // ==============================

        if (segmentSlotsParent != null)
        {
            for (
                int i = 0;
                i < segmentSlotsParent.childCount;
                i++
            )
            {
                Transform slot =
                    segmentSlotsParent.GetChild(i);


                slot.localPosition =
                    new Vector3(
                        (startingCount + i)
                        * segmentSpacing,
                        0f,
                        0f
                    );
            }
        }
    }


    // ==================================================
    // BUILD STARTING DURABILITY ORDER
    // ==================================================

    private void BuildStartingSegmentList()
    {
        activeSegments.Clear();


        if (startingSegmentsParent == null)
        {
            return;
        }


        for (
            int i = 0;
            i < startingSegmentsParent.childCount;
            i++
        )
        {
            Transform segment =
                startingSegmentsParent.GetChild(i);


            activeSegments.Add(segment);
        }
    }


    // ==================================================
    // RESERVE NEXT ATTACHMENT SLOT
    // ==================================================

    public bool TryReserveNextSlot(
        out Transform availableSlot
    )
    {
        availableSlot = null;


        if (segmentSlotsParent == null)
        {
            Debug.LogWarning(
                name +
                " has no Segment Slots parent."
            );

            return false;
        }


        for (
            int i = 0;
            i < segmentSlotsParent.childCount;
            i++
        )
        {
            Transform slot =
                segmentSlotsParent.GetChild(i);


            bool occupied =
                slot.childCount > 0;


            bool reserved =
                reservedSlots.Contains(slot);


            if (!occupied && !reserved)
            {
                reservedSlots.Add(slot);

                availableSlot = slot;

                return true;
            }
        }


        return false;
    }


    // ==================================================
    // COLLECTED SEGMENT FINISHED ATTACHING
    // ==================================================

    public void ConfirmAttachment(
        Transform slot
    )
    {
        if (slot == null)
        {
            return;
        }


        reservedSlots.Remove(slot);


        // AttachablePlatformSegment already became
        // a child of this slot before calling this.
        if (slot.childCount > 0)
        {
            Transform newSegment =
                slot.GetChild(0);


            if (!activeSegments.Contains(newSegment))
            {
                // New pieces are added to the END,
                // so they are newer than existing pieces.
                activeSegments.Add(newSegment);
            }
        }
    }


    // ==================================================
    // CANCEL RESERVATION
    // ==================================================

    public void ReleaseReservation(
        Transform slot
    )
    {
        if (slot == null)
        {
            return;
        }


        reservedSlots.Remove(slot);
    }


    // ==================================================
    // CLEAN TRACKING LIST
    // ==================================================

    private void CleanActiveSegmentList()
    {
        activeSegments.RemoveAll(
            segment => segment == null
        );
    }


    // ==================================================
    // DURABILITY INFORMATION
    // ==================================================

    public int GetActiveSegmentCount()
    {
        CleanActiveSegmentList();

        return activeSegments.Count;
    }


    public Transform GetOldestSegment()
    {
        CleanActiveSegmentList();


        if (activeSegments.Count == 0)
        {
            return null;
        }


        return activeSegments[0];
    }


    public Transform GetNewestSegment()
    {
        CleanActiveSegmentList();


        if (activeSegments.Count == 0)
        {
            return null;
        }


        return activeSegments[
            activeSegments.Count - 1
        ];
    }


    public Transform GetRandomSegment()
    {
        CleanActiveSegmentList();


        if (activeSegments.Count == 0)
        {
            return null;
        }


        int randomIndex =
            Random.Range(
                0,
                activeSegments.Count
            );


        return activeSegments[randomIndex];
    }


    // ==================================================
    // REMOVE BROKEN SEGMENT
    // ==================================================

    public void RemoveActiveSegment(
        Transform segment
    )
    {
        if (segment == null)
        {
            return;
        }


        activeSegments.Remove(segment);
    }


    // ==================================================
    // EXTRA INFORMATION
    // ==================================================

    public int GetStartingSegmentCount()
    {
        if (startingSegmentsParent == null)
        {
            return 0;
        }


        return startingSegmentsParent.childCount;
    }


    public int GetCollectedSegmentCount()
    {
        if (segmentSlotsParent == null)
        {
            return 0;
        }


        int count = 0;


        for (
            int i = 0;
            i < segmentSlotsParent.childCount;
            i++
        )
        {
            Transform slot =
                segmentSlotsParent.GetChild(i);


            if (slot.childCount > 0)
            {
                count++;
            }
        }


        return count;
    }
}