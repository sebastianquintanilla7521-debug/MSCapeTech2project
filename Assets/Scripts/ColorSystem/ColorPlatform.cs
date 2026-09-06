using UnityEngine;

public class ColorPlatform : MonoBehaviour
{
    // The ColorManager controlling the background
    public ColorManager colorManager;

    // The color assigned to THIS platform
    public ColorManager.GameColor platformColor =
        ColorManager.GameColor.Red;


    // Store every collider and sprite renderer
    // belonging to this platform
    private Collider2D[] platformColliders;
    private SpriteRenderer[] spriteRenderers;


    void Start()
    {
        // Find ALL colliders on this object
        // and any children underneath it
        platformColliders =
            GetComponentsInChildren<Collider2D>(true);

        // Find all sprite renderers too
        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(true);


        ApplyPlatformColor();
    }


    void Update()
    {
        if (colorManager == null)
        {
            return;
        }


        bool matchesBackground =
            colorManager.currentColor
            == platformColor;


        // If background matches the platform:
        // disable EVERY collider.
        //
        // Otherwise:
        // turn them all back on.
        foreach (Collider2D platformCollider in platformColliders)
        {
            platformCollider.enabled =
                !matchesBackground;
        }
    }


    void ApplyPlatformColor()
    {
        Color chosenColor = Color.white;


        if (platformColor ==
            ColorManager.GameColor.Red)
        {
            chosenColor = Color.red;
        }

        else if (platformColor ==
                 ColorManager.GameColor.Yellow)
        {
            chosenColor = Color.yellow;
        }

        else if (platformColor ==
                 ColorManager.GameColor.Blue)
        {
            chosenColor = Color.blue;
        }


        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            sprite.color = chosenColor;
        }
    }
}