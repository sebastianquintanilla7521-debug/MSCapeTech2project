using UnityEngine;
using System.Collections.Generic;

public class MovingPlatformPassengerCarry : MonoBehaviour
{
    // ==================================================
    // SETTINGS
    // ==================================================

    [Header("Player Carry")]

    [Tooltip(
        "Checked = a player standing on this platform " +
        "moves with the platform automatically."
    )]
    [SerializeField]
    private bool carryPlayerWithPlatform = true;


    [Tooltip(
        "Carry the player when the platform moves left/right."
    )]
    [SerializeField]
    private bool carryHorizontal = true;


    [Tooltip(
        "Carry the player when the platform moves up/down."
    )]
    [SerializeField]
    private bool carryVertical = true;


    [Tooltip(
        "If the platform itself rotates, keep the player's position " +
        "attached to that rotation without rotating the player sprite."
    )]
    [SerializeField]
    private bool carryWithPlatformRotation = true;


    // ==================================================
    // INTERNAL
    // ==================================================

    private Vector3 previousPlatformPosition;

    private Quaternion previousPlatformRotation;


    private HashSet<Rigidbody2D> passengers =
        new HashSet<Rigidbody2D>();


    // ==================================================
    // START
    // ==================================================

    private void OnEnable()
    {
        previousPlatformPosition =
            transform.position;


        previousPlatformRotation =
            transform.rotation;
    }


    // ==================================================
    // CARRY PLAYER
    // ==================================================

    private void LateUpdate()
    {
        Vector3 currentPosition =
            transform.position;


        Quaternion currentRotation =
            transform.rotation;


        if (!carryPlayerWithPlatform)
        {
            previousPlatformPosition =
                currentPosition;


            previousPlatformRotation =
                currentRotation;


            passengers.Clear();

            return;
        }


        // Make a temporary list so destroyed
        // players can safely be removed.
        List<Rigidbody2D> invalidPassengers =
            null;


        foreach (
            Rigidbody2D passenger
            in passengers
        )
        {
            if (passenger == null)
            {
                if (invalidPassengers == null)
                {
                    invalidPassengers =
                        new List<Rigidbody2D>();
                }


                invalidPassengers.Add(
                    passenger
                );


                continue;
            }


            Vector3 targetPosition;


            // ==============================================
            // FOLLOW PLATFORM ROTATION
            // ==============================================

            if (carryWithPlatformRotation)
            {
                Vector3 oldOffset =
                    (Vector3)passenger.position -
                    previousPlatformPosition;


                Quaternion rotationDifference =
                    currentRotation *
                    Quaternion.Inverse(
                        previousPlatformRotation
                    );


                Vector3 rotatedOffset =
                    rotationDifference *
                    oldOffset;


                targetPosition =
                    currentPosition +
                    rotatedOffset;
            }


            // ==============================================
            // TRANSLATION ONLY
            // ==============================================

            else
            {
                Vector3 platformMovement =
                    currentPosition -
                    previousPlatformPosition;


                targetPosition =
                    (Vector3)passenger.position +
                    platformMovement;
            }


            Vector3 playerMovement =
                targetPosition -
                (Vector3)passenger.position;


            // Designer can independently disable
            // horizontal or vertical carrying.
            if (!carryHorizontal)
            {
                playerMovement.x = 0f;
            }


            if (!carryVertical)
            {
                playerMovement.y = 0f;
            }


            passenger.position +=
                (Vector2)playerMovement;
        }


        if (invalidPassengers != null)
        {
            foreach (
                Rigidbody2D invalidPassenger
                in invalidPassengers
            )
            {
                passengers.Remove(
                    invalidPassenger
                );
            }
        }


        previousPlatformPosition =
            currentPosition;


        previousPlatformRotation =
            currentRotation;
    }


    // ==================================================
    // PLAYER IS STANDING ON PLATFORM
    // ==================================================

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        if (!carryPlayerWithPlatform)
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


        Rigidbody2D playerBody =
            player.GetComponent<Rigidbody2D>();


        if (playerBody == null)
        {
            return;
        }


        // Check the player's position relative
        // to the platform instead of world direction.
        Vector3 localPlayerPosition =
            transform.InverseTransformPoint(
                playerBody.worldCenterOfMass
            );


        // Player must be above the platform.
        if (localPlayerPosition.y <= 0f)
        {
            passengers.Remove(
                playerBody
            );

            return;
        }


        bool standingOnPlatform =
            false;


        for (
            int i = 0;
            i < collision.contactCount;
            i++
        )
        {
            ContactPoint2D contact =
                collision.GetContact(i);


            Vector3 localNormal =
                transform.InverseTransformDirection(
                    contact.normal
                );


            // Mostly vertical contact =
            // player is on a horizontal surface,
            // not merely touching its side.
            if (
                Mathf.Abs(localNormal.y) >
                0.5f
            )
            {
                standingOnPlatform =
                    true;

                break;
            }
        }


        if (standingOnPlatform)
        {
            passengers.Add(
                playerBody
            );
        }

        else
        {
            passengers.Remove(
                playerBody
            );
        }
    }


    // ==================================================
    // PLAYER LEAVES PLATFORM
    // ==================================================

    private void OnCollisionExit2D(
        Collision2D collision
    )
    {
        PlayerMovement player =
            collision.gameObject
            .GetComponentInParent<PlayerMovement>();


        if (player == null)
        {
            return;
        }


        Rigidbody2D playerBody =
            player.GetComponent<Rigidbody2D>();


        if (playerBody != null)
        {
            passengers.Remove(
                playerBody
            );
        }
    }


    // ==================================================
    // CLEANUP
    // ==================================================

    private void OnDisable()
    {
        passengers.Clear();
    }
}