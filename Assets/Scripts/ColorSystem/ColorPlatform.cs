using UnityEngine;

public class ColorPlatform : MonoBehaviour
{
    // =========================
    // PLATFORM COLOR
    // =========================

    public ColorManager.GameColor platformColor =
        ColorManager.GameColor.Red;


    // =========================
    // INTERNAL VARIABLES
    // =========================

    private ColorManager colorManager;

    private Collider2D[] platformColliders;

    private SpriteRenderer[] spriteRenderers;


    // =========================
    // START
    // =========================

    void Start()
    {
        FindColorManager();

        GetPlatformParts();

        ApplyPlatformColor();

        UpdateCollision();
    }


    // =========================
    // WHEN SCRIPT IS ENABLED
    // =========================

    void OnEnable()
    {
        FindColorManager();

        GetPlatformParts();

        ApplyPlatformColor();

        UpdateCollision();
    }


    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        UpdateCollision();
    }


    // =========================
    // FIND COLOR MANAGER
    // =========================

    void FindColorManager()
    {
        if (colorManager == null)
        {
            colorManager =
                FindAnyObjectByType<ColorManager>();
        }
    }


    // =========================
    // GET PLATFORM PARTS
    // =========================

    void GetPlatformParts()
    {
        platformColliders =
            GetComponentsInChildren<Collider2D>(
                true
            );


        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(
                true
            );
    }


    // =========================
    // APPLY VISUAL COLOR
    // =========================

    void ApplyPlatformColor()
    {
        if (colorManager == null)
        {
            return;
        }


        Color chosenColor =
            colorManager.GetUnityColor(
                platformColor
            );


        foreach (
            SpriteRenderer sprite
            in spriteRenderers
        )
        {
            sprite.color =
                chosenColor;
        }
    }


    // =========================
    // COLOR COLLISION
    // =========================

    void UpdateCollision()
    {
        if (
            colorManager == null
            ||
            platformColliders == null
        )
        {
            return;
        }


        bool matchesBackground =
            colorManager.currentColor
            ==
            platformColor;


        foreach (
            Collider2D platformCollider
            in platformColliders
        )
        {
            platformCollider.enabled =
                !matchesBackground;
        }
    }


    // =========================
    // EDITOR COLOR PREVIEW
    // =========================

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }


        ColorManager manager =
            FindAnyObjectByType<ColorManager>();


        if (manager == null)
        {
            return;
        }


        SpriteRenderer[] sprites =
            GetComponentsInChildren<SpriteRenderer>(
                true
            );


        Color chosenColor =
            manager.GetUnityColor(
                platformColor
            );


        foreach (
            SpriteRenderer sprite
            in sprites
        )
        {
            sprite.color =
                chosenColor;
        }
    }
}