using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class GameViewport : MonoBehaviour
{
    // ==================================================
    // REFERENCE SCREEN
    // ==================================================

    [Header("Reference Resolution")]

    [SerializeField]
    private float referenceWidth = 740f;

    [SerializeField]
    private float referenceHeight = 416f;


    // ==================================================
    // GAMEPLAY WINDOW INSIDE CAMERA FRAME
    // ==================================================

    [Header("Gameplay Window")]

    [SerializeField]
    private float viewportLeft = 80f;

    [SerializeField]
    private float viewportBottom = 28f;

    [SerializeField]
    private float viewportWidth = 629f;

    [SerializeField]
    private float viewportHeight = 335f;


    // ==================================================
    // CAMERA ZOOM
    // ==================================================

    [Header("Camera Zoom")]

    [Tooltip(
        "How many 1-unit tiles should be visible vertically inside the gameplay window."
    )]
    [SerializeField]
    private float visibleWorldHeight = 13f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private Camera targetCamera;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;


    // ==================================================
    // SETUP
    // ==================================================

    private void OnEnable()
    {
        targetCamera =
            GetComponent<Camera>();

        UpdateViewport();
    }


    private void Update()
    {
        if (
            Screen.width != lastScreenWidth ||
            Screen.height != lastScreenHeight
        )
        {
            UpdateViewport();
        }
    }


    private void OnValidate()
    {
        targetCamera =
            GetComponent<Camera>();

        UpdateViewport();
    }


    // ==================================================
    // UPDATE CAMERA VIEWPORT
    // ==================================================

    private void UpdateViewport()
    {
        if (targetCamera == null)
        {
            return;
        }


        if (
            Screen.width <= 0 ||
            Screen.height <= 0
        )
        {
            return;
        }


        lastScreenWidth =
            Screen.width;

        lastScreenHeight =
            Screen.height;


        // ==================================================
        // PRESERVE THE 740 x 416 PRESENTATION AREA
        // ==================================================

        float referenceAspect =
            referenceWidth /
            referenceHeight;


        float screenAspect =
            (float)Screen.width /
            Screen.height;


        Rect presentationRect;


        // Wider screen:
        // leave space on left and right.
        if (screenAspect > referenceAspect)
        {
            float width =
                referenceAspect /
                screenAspect;


            presentationRect =
                new Rect(
                    (1f - width) * 0.5f,
                    0f,
                    width,
                    1f
                );
        }

        // Taller / narrower screen:
        // leave space on top and bottom.
        else
        {
            float height =
                screenAspect /
                referenceAspect;


            presentationRect =
                new Rect(
                    0f,
                    (1f - height) * 0.5f,
                    1f,
                    height
                );
        }


        // ==================================================
        // GAME WINDOW INSIDE PRESENTATION
        // ==================================================

        float normalizedLeft =
            viewportLeft /
            referenceWidth;


        float normalizedBottom =
            viewportBottom /
            referenceHeight;


        float normalizedWidth =
            viewportWidth /
            referenceWidth;


        float normalizedHeight =
            viewportHeight /
            referenceHeight;


        targetCamera.rect =
            new Rect(
                presentationRect.x +
                normalizedLeft *
                presentationRect.width,

                presentationRect.y +
                normalizedBottom *
                presentationRect.height,

                normalizedWidth *
                presentationRect.width,

                normalizedHeight *
                presentationRect.height
            );


        // ==================================================
        // CAMERA ZOOM
        // ==================================================

        if (targetCamera.orthographic)
        {
            targetCamera.orthographicSize =
                visibleWorldHeight / 2f;
        }
    }
}