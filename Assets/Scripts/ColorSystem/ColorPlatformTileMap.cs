using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorPlatformTileMap : MonoBehaviour
{
    // ==================================================
    // PLATFORM COLOR
    // ==================================================

    [Header("Platform Color")]

    [SerializeField]
    private ColorManager.GameColor platformColor =
        ColorManager.GameColor.Red;


    public ColorManager.GameColor PlatformColor
    {
        get { return platformColor; }
    }


    // ==================================================
    // REFERENCES
    // ==================================================

    private ColorManager colorManager;

    private Tilemap tilemap;

    private Collider2D[] tilemapColliders;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        Setup();

        UpdatePlatform();
    }


    // ==================================================
    // ENABLE
    // ==================================================

    private void OnEnable()
    {
        Setup();
    }


    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        UpdatePlatform();
    }


    // ==================================================
    // SETUP
    // ==================================================

    private void Setup()
    {
        if (tilemap == null)
        {
            tilemap =
                GetComponent<Tilemap>();
        }


        if (
            tilemapColliders == null ||
            tilemapColliders.Length == 0
        )
        {
            tilemapColliders =
                GetComponents<Collider2D>();
        }


        if (colorManager == null)
        {
            colorManager =
                FindAnyObjectByType<ColorManager>();
        }
    }


    // ==================================================
    // UPDATE PLATFORM
    // ==================================================

    private void UpdatePlatform()
    {
        if (
            tilemap == null ||
            colorManager == null
        )
        {
            Setup();


            if (
                tilemap == null ||
                colorManager == null
            )
            {
                return;
            }
        }


        bool matchesBackground =
            colorManager.ShouldPlatformBeHidden(
                platformColor
            );


        ApplyColor(
            matchesBackground
        );


        UpdateCollision(
            matchesBackground
        );
    }


    // ==================================================
    // VISUAL
    // ==================================================

    private void ApplyColor(
        bool matchesBackground
    )
    {
        Color platformTint =
            colorManager.GetUnityColor(
                platformColor
            );


        // Completely hide the Tilemap when
        // it matches the background.
        platformTint.a =
            matchesBackground
            ? 0f
            : 1f;


        tilemap.color =
            platformTint;
    }


    // ==================================================
    // COLLISION
    // ==================================================

    private void UpdateCollision(
        bool matchesBackground
    )
    {
        if (tilemapColliders == null)
        {
            return;
        }


        foreach (
            Collider2D tilemapCollider
            in tilemapColliders
        )
        {
            if (tilemapCollider == null)
            {
                continue;
            }


            tilemapCollider.enabled =
                !matchesBackground;
        }
    }


    // ==================================================
    // EDITOR
    // ==================================================

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            return;
        }


        Setup();

        UpdatePlatform();
    }
}