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

    // Remember the designer's Tilemap Color alpha separately from temporary hiding.
    private float visibleOpacity = 1f;
    private bool opacityCaptured;
    private float lastAppliedAlpha;
    private bool hasAppliedColor;


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

        if (tilemap != null && !opacityCaptured)
        {
            visibleOpacity = Mathf.Clamp01(tilemap.color.a);
            opacityCaptured = true;
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
        // Also accept Inspector alpha changes while testing, without mistaking
        // our own hidden alpha (0) for a new designer setting.
        if (hasAppliedColor && !Mathf.Approximately(tilemap.color.a, lastAppliedAlpha))
            visibleOpacity = Mathf.Clamp01(tilemap.color.a);

        Color platformTint =
            colorManager.GetUnityColor(
                platformColor
            );


        // Completely hide the Tilemap when
        // it matches the background.
        platformTint.a =
            matchesBackground
            ? 0f
            : visibleOpacity;


        tilemap.color =
            platformTint;
        lastAppliedAlpha = platformTint.a;
        hasAppliedColor = true;
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