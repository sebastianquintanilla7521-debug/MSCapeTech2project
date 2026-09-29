using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorPlatformTileMap : MonoBehaviour
{
    // =========================
    // PLATFORM COLOR
    // =========================

    [Header("Platform Color")]

    [SerializeField]
    private ColorManager.GameColor platformColor =
        ColorManager.GameColor.Blue;


    public ColorManager.GameColor PlatformColor
    {
        get { return platformColor; }
    }


    // =========================
    // INTERNAL
    // =========================

    private ColorManager colorManager;

    private Tilemap tilemap;

    private Collider2D[] tilemapColliders;


    // =========================
    // START
    // =========================

    private void Start()
    {
        colorManager =
            FindAnyObjectByType<ColorManager>();


        tilemap =
            GetComponent<Tilemap>();


        tilemapColliders =
            GetComponents<Collider2D>();


        ApplyColor();

        UpdateCollision();
    }


    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        UpdateCollision();
    }


    // =========================
    // APPLY COLOR
    // =========================

    private void ApplyColor()
    {
        if (
            colorManager == null ||
            tilemap == null
        )
        {
            return;
        }


        Color chosenColor =
            colorManager.GetUnityColor(
                platformColor
            );


        chosenColor.a = 1f;


        tilemap.color =
            chosenColor;
    }


    // =========================
    // COLOR COLLISION
    // =========================

    private void UpdateCollision()
    {
        if (
            colorManager == null ||
            tilemapColliders == null
        )
        {
            return;
        }


        bool matchesBackground =
            colorManager.currentColor
            ==
            platformColor;


        foreach (
            Collider2D tileCollider
            in tilemapColliders
        )
        {
            tileCollider.enabled =
                !matchesBackground;
        }
    }
}