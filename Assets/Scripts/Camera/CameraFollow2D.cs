using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    // ==================================================
    // PLAYER
    // ==================================================

    [Header("Player")]

    [SerializeField]
    private Transform player;


    // ==================================================
    // HORIZONTAL FOLLOW
    // ==================================================

    [Header("Horizontal Follow")]

    [SerializeField]
    private float horizontalOffset = 0f;

    [Tooltip(
        "0 follows immediately. Increase for horizontal camera lag."
    )]
    [Min(0f)]
    [SerializeField]
    private float horizontalSmoothTime = 0f;


    // ==================================================
    // AUTOMATIC VERTICAL CAMERA
    // ==================================================

    [Header("Automatic Vertical Camera")]

    [Tooltip(
        "How many tiles ABOVE the camera center the player can move " +
        "before the camera starts moving upward."
    )]
    [Min(0f)]
    [SerializeField]
    private float upwardDeadZoneTiles = 3f;


    [Tooltip(
        "How many tiles BELOW the camera center the player can move " +
        "before the camera starts moving downward."
    )]
    [Min(0f)]
    [SerializeField]
    private float downwardDeadZoneTiles = 1.25f;


    [Tooltip(
        "How smoothly the camera moves vertically."
    )]
    [Min(0f)]
    [SerializeField]
    private float verticalSmoothTime = 0.3f;


    // ==================================================
    // GROUNDED RECENTER
    // ==================================================

    [Header("Grounded Downward Recenter")]

    [Tooltip(
        "When grounded below the camera center, gently move the camera " +
        "down even if the player has not fully crossed the lower dead zone."
    )]
    [SerializeField]
    private bool recenterDownWhenGrounded = true;


    [Tooltip(
        "How far below the camera center the player should normally sit " +
        "when the camera settles."
    )]
    [Min(0f)]
    [SerializeField]
    private float groundedPreferredOffset = 0.5f;


    // ==================================================
    // PIXEL ART
    // ==================================================

    [Header("Pixel Art Stabilization")]

    [SerializeField]
    private bool pixelSnapCamera = true;


    // ==================================================
    // INTERNAL
    // ==================================================

    private enum VerticalMode
    {
        Automatic,
        Fixed,
        FollowPlayer
    }


    private VerticalMode verticalMode =
        VerticalMode.Automatic;


    private Camera targetCamera;
    private PlayerMovement playerMovement;

    private float targetY;
    private float fixedY;
    private float verticalPlayerOffset;

    private float horizontalVelocity;
    private float verticalVelocity;


    // ==================================================
    // SETUP
    // ==================================================

    private void Awake()
    {
        targetCamera =
            GetComponent<Camera>();


        if (player == null)
        {
            PlayerMovement foundPlayer =
                FindAnyObjectByType<PlayerMovement>();


            if (foundPlayer != null)
            {
                player =
                    foundPlayer.transform;

                playerMovement =
                    foundPlayer;
            }
        }
        else
        {
            playerMovement =
                player.GetComponent<PlayerMovement>();
        }


        targetY =
            transform.position.y;


        fixedY =
            targetY;
    }


    // ==================================================
    // CAMERA MOVEMENT
    // ==================================================

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }


        // ==================================================
        // HORIZONTAL
        // ==================================================

        float desiredX =
            player.position.x +
            horizontalOffset;


        float newX;


        if (horizontalSmoothTime <= 0.001f)
        {
            newX =
                desiredX;
        }
        else
        {
            newX =
                Mathf.SmoothDamp(
                    transform.position.x,
                    desiredX,
                    ref horizontalVelocity,
                    horizontalSmoothTime
                );
        }


        // ==================================================
        // VERTICAL
        // ==================================================

        UpdateVerticalTarget();


        float newY;


        if (verticalSmoothTime <= 0.001f)
        {
            newY =
                targetY;
        }
        else
        {
            newY =
                Mathf.SmoothDamp(
                    transform.position.y,
                    targetY,
                    ref verticalVelocity,
                    verticalSmoothTime
                );
        }


        // ==================================================
        // PIXEL SNAP
        // ==================================================

        if (
            pixelSnapCamera &&
            targetCamera != null &&
            targetCamera.orthographic &&
            targetCamera.pixelHeight > 0
        )
        {
            float worldUnitsPerPixel =
                (targetCamera.orthographicSize * 2f) /
                targetCamera.pixelHeight;


            newX =
                Mathf.Round(
                    newX /
                    worldUnitsPerPixel
                ) *
                worldUnitsPerPixel;


            newY =
                Mathf.Round(
                    newY /
                    worldUnitsPerPixel
                ) *
                worldUnitsPerPixel;
        }


        // ==================================================
        // APPLY
        // ==================================================

        transform.position =
            new Vector3(
                newX,
                newY,
                transform.position.z
            );
    }


    // ==================================================
    // VERTICAL LOGIC
    // ==================================================

    private void UpdateVerticalTarget()
    {
        switch (verticalMode)
        {
            // ==========================================
            // AUTOMATIC
            // ==========================================

            case VerticalMode.Automatic:
                {
                    float cameraY =
                        transform.position.y;


                    float upperEdge =
                        cameraY +
                        upwardDeadZoneTiles;


                    float lowerEdge =
                        cameraY -
                        downwardDeadZoneTiles;


                    // --------------------------------------
                    // PLAYER TOO HIGH
                    // --------------------------------------

                    if (player.position.y > upperEdge)
                    {
                        targetY =
                            player.position.y -
                            upwardDeadZoneTiles;
                    }


                    // --------------------------------------
                    // PLAYER TOO LOW
                    // --------------------------------------

                    else if (player.position.y < lowerEdge)
                    {
                        targetY =
                            player.position.y +
                            downwardDeadZoneTiles;
                    }


                    // --------------------------------------
                    // GROUNDED DOWNWARD RECENTER
                    // --------------------------------------

                    else if (
                        recenterDownWhenGrounded &&
                        playerMovement != null &&
                        playerMovement.IsGrounded &&
                        player.position.y <
                        cameraY - groundedPreferredOffset
                    )
                    {
                        targetY =
                            player.position.y +
                            groundedPreferredOffset;
                    }


                    break;
                }


            // ==========================================
            // FIXED Y
            // ==========================================

            case VerticalMode.Fixed:

                targetY =
                    fixedY;

                break;


            // ==========================================
            // FOLLOW PLAYER DIRECTLY
            // ==========================================

            case VerticalMode.FollowPlayer:

                targetY =
                    player.position.y +
                    verticalPlayerOffset;

                break;
        }
    }


    // ==================================================
    // CAMERA ZONE SUPPORT
    // ==================================================

    public void SetVerticalPosition(
        float newY
    )
    {
        verticalMode =
            VerticalMode.Fixed;


        fixedY =
            newY;


        targetY =
            newY;
    }


    public void FollowPlayerVertically(
        float offset
    )
    {
        verticalMode =
            VerticalMode.FollowPlayer;


        verticalPlayerOffset =
            offset;
    }


    public void RestoreDefaultVerticalPosition()
    {
        verticalMode =
            VerticalMode.Automatic;


        targetY =
            transform.position.y;
    }


    // ==================================================
    // EDITOR VISUALIZATION
    // ==================================================

    private void OnDrawGizmosSelected()
    {
        float centerY =
            transform.position.y;


        float lineWidth =
            30f;


        // Upper boundary
        Gizmos.DrawLine(
            new Vector3(
                transform.position.x - lineWidth,
                centerY + upwardDeadZoneTiles,
                0f
            ),
            new Vector3(
                transform.position.x + lineWidth,
                centerY + upwardDeadZoneTiles,
                0f
            )
        );


        // Lower boundary
        Gizmos.DrawLine(
            new Vector3(
                transform.position.x - lineWidth,
                centerY - downwardDeadZoneTiles,
                0f
            ),
            new Vector3(
                transform.position.x + lineWidth,
                centerY - downwardDeadZoneTiles,
                0f
            )
        );
    }
}