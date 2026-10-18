using UnityEngine;

public class CameraYZone : MonoBehaviour
{
    public enum CameraVerticalMode
    {
        FixedPosition,
        FollowPlayer,
        RestoreDefault
    }


    // ==================================================
    // CAMERA
    // ==================================================

    [Header("Camera")]

    [SerializeField]
    private CameraFollow2D cameraFollow;


    // ==================================================
    // MODE
    // ==================================================

    [Header("Vertical Behavior")]

    [SerializeField]
    private CameraVerticalMode mode =
        CameraVerticalMode.FixedPosition;


    [Tooltip(
        "Used when Mode is Fixed Position."
    )]
    [SerializeField]
    private float cameraY = 0f;


    [Tooltip(
        "Used when Mode is Follow Player."
    )]
    [SerializeField]
    private float playerYOffset = 0f;


    [Tooltip(
        "Return the camera to its original Y when the player leaves this zone."
    )]
    [SerializeField]
    private bool restoreOnExit = false;


    // ==================================================
    // SETUP
    // ==================================================

    private void Awake()
    {
        if (cameraFollow == null)
        {
            Camera mainCamera =
                Camera.main;


            if (mainCamera != null)
            {
                cameraFollow =
                    mainCamera
                    .GetComponent<CameraFollow2D>();
            }
        }
    }


    // ==================================================
    // PLAYER ENTERS
    // ==================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        PlayerMovement player =
            other
            .GetComponentInParent<PlayerMovement>();


        if (
            player == null ||
            cameraFollow == null
        )
        {
            return;
        }


        switch (mode)
        {
            case CameraVerticalMode.FixedPosition:

                cameraFollow
                    .SetVerticalPosition(
                        cameraY
                    );

                break;


            case CameraVerticalMode.FollowPlayer:

                cameraFollow
                    .FollowPlayerVertically(
                        playerYOffset
                    );

                break;


            case CameraVerticalMode.RestoreDefault:

                cameraFollow
                    .RestoreDefaultVerticalPosition();

                break;
        }
    }


    // ==================================================
    // PLAYER LEAVES
    // ==================================================

    private void OnTriggerExit2D(
        Collider2D other
    )
    {
        if (!restoreOnExit)
        {
            return;
        }


        PlayerMovement player =
            other
            .GetComponentInParent<PlayerMovement>();


        if (
            player == null ||
            cameraFollow == null
        )
        {
            return;
        }


        cameraFollow
            .RestoreDefaultVerticalPosition();
    }
}