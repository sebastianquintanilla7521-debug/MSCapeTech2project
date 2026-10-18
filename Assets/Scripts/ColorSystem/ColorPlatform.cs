using UnityEngine;

public class ColorPlatform : MonoBehaviour
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

    private Collider2D[] platformColliders;

    private SpriteRenderer[] spriteRenderers;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        FindColorManager();

        GetPlatformParts();

        UpdatePlatform();
    }


    // ==================================================
    // ENABLE
    // ==================================================

    private void OnEnable()
    {
        FindColorManager();

        GetPlatformParts();
    }


    // ==================================================
    // UPDATE
    // ==================================================

    private void Update()
    {
        UpdatePlatform();
    }


    // ==================================================
    // FIND COLOR MANAGER
    // ==================================================

    private void FindColorManager()
    {
        if (colorManager != null)
        {
            return;
        }


        colorManager =
            FindFirstObjectByType<ColorManager>();
    }


    // ==================================================
    // GET PLATFORM PARTS
    // ==================================================

    private void GetPlatformParts()
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


    // ==================================================
    // UPDATE PLATFORM
    // ==================================================

    private void UpdatePlatform()
    {
        if (colorManager == null)
        {
            FindColorManager();


            if (colorManager == null)
            {
                return;
            }
        }


        bool matchesBackground =
            colorManager.ShouldPlatformBeHidden(
                platformColor
            );


        ApplyPlatformColor(
            matchesBackground
        );


        UpdateCollision(
            matchesBackground
        );
    }


    // ==================================================
    // VISUAL
    // ==================================================

    private void ApplyPlatformColor(
        bool matchesBackground
    )
    {
        if (
            spriteRenderers == null ||
            spriteRenderers.Length == 0
        )
        {
            GetPlatformParts();
        }


        Color platformTint =
            colorManager.GetUnityColor(
                platformColor
            );


        // IMPORTANT:
        // Matching background = completely invisible.
        //
        // Non-matching = completely visible.
        platformTint.a =
            matchesBackground
            ? 0f
            : 1f;


        foreach (
            SpriteRenderer spriteRenderer
            in spriteRenderers
        )
        {
            if (spriteRenderer == null)
            {
                continue;
            }


            spriteRenderer.color =
                platformTint;
        }
    }


    // ==================================================
    // COLLISION
    // ==================================================

    private void UpdateCollision(
        bool matchesBackground
    )
    {
        if (
            platformColliders == null ||
            platformColliders.Length == 0
        )
        {
            GetPlatformParts();
        }


        foreach (
            Collider2D platformCollider
            in platformColliders
        )
        {
            if (platformCollider == null)
            {
                continue;
            }


            platformCollider.enabled =
                !matchesBackground;
        }
    }


    // ==================================================
    // EDITOR
    // ==================================================

    private void OnValidate()
    {
        GetPlatformParts();


        if (!Application.isPlaying)
        {
            return;
        }


        FindColorManager();

        UpdatePlatform();
    }
}