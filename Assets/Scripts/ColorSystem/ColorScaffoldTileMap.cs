using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorScaffoldTileMap : MonoBehaviour
{
    // =========================
    // COLOR SOURCE
    // =========================

    [Header("Color Source")]

    [Tooltip(
        "Drag the matching colored PlatformTop Tilemap here."
    )]
    [SerializeField]
    private ColorPlatformTileMap platformColorSource;


    // =========================
    // SCAFFOLD APPEARANCE
    // =========================

    [Header("Scaffold Appearance")]

    [Tooltip(
        "0.5 = 50% opacity."
    )]
    [Range(0f, 1f)]
    [SerializeField]
    private float opacity = 0.5f;


    // =========================
    // INTERNAL
    // =========================

    private ColorManager colorManager;

    private Tilemap tilemap;


    // =========================
    // START
    // =========================

    private void Start()
    {
        colorManager =
            FindAnyObjectByType<ColorManager>();


        tilemap =
            GetComponent<Tilemap>();


        ApplyColor();
    }


    // =========================
    // APPLY COLOR
    // =========================

    private void ApplyColor()
    {
        if (
            colorManager == null ||
            tilemap == null ||
            platformColorSource == null
        )
        {
            return;
        }


        Color scaffoldColor =
            colorManager.GetUnityColor(
                platformColorSource.PlatformColor
            );


        // Keep the same RGB color as the
        // platform, but make it translucent.
        scaffoldColor.a =
            opacity;


        tilemap.color =
            scaffoldColor;
    }
}