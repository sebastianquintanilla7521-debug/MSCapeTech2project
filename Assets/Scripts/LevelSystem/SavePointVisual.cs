using UnityEngine;
using System.Collections;

public class SavePointVisual : MonoBehaviour
{
    // ==================================================
    // COLLECTION EFFECT
    // ==================================================

    [Header("Collection Animation")]

    [SerializeField]
    private float animationDuration = 0.45f;

    [SerializeField]
    private float popScale = 1.4f;

    [SerializeField]
    private float rotationAmount = 25f;

    [SerializeField]
    private float riseAmount = 0.35f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private SpriteRenderer spriteRenderer;

    private Vector3 startingScale;
    private Vector3 startingPosition;

    private bool collected = false;


    // ==================================================
    // SETUP
    // ==================================================

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();


        startingScale =
            transform.localScale;


        startingPosition =
            transform.localPosition;
    }


    // ==================================================
    // COLLECT
    // ==================================================

    public void PlayCollectedAnimation()
    {
        if (collected)
        {
            return;
        }


        collected = true;


        StartCoroutine(
            CollectedAnimation()
        );
    }


    // ==================================================
    // ANIMATION
    // ==================================================

    private IEnumerator CollectedAnimation()
    {
        float timer = 0f;


        Color startingColor =
            spriteRenderer != null
            ? spriteRenderer.color
            : Color.white;


        while (timer < animationDuration)
        {
            timer += Time.deltaTime;


            float progress =
                Mathf.Clamp01(
                    timer /
                    animationDuration
                );


            // ------------------------------------------
            // POP UP, THEN SHRINK AWAY
            // ------------------------------------------

            float scaleMultiplier;


            if (progress < 0.35f)
            {
                scaleMultiplier =
                    Mathf.Lerp(
                        1f,
                        popScale,
                        progress / 0.35f
                    );
            }

            else
            {
                scaleMultiplier =
                    Mathf.Lerp(
                        popScale,
                        0f,
                        (progress - 0.35f) / 0.65f
                    );
            }


            transform.localScale =
                startingScale *
                scaleMultiplier;


            // ------------------------------------------
            // LITTLE SPIN / WOBBLE
            // ------------------------------------------

            float rotation =
                Mathf.Sin(
                    progress *
                    Mathf.PI *
                    3f
                )
                *
                rotationAmount;


            transform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    rotation
                );


            // ------------------------------------------
            // FLOAT UP
            // ------------------------------------------

            transform.localPosition =
                startingPosition +
                Vector3.up *
                riseAmount *
                progress;


            // ------------------------------------------
            // FADE OUT
            // ------------------------------------------

            if (spriteRenderer != null)
            {
                Color color =
                    startingColor;


                color.a =
                    1f - progress;


                spriteRenderer.color =
                    color;
            }


            yield return null;
        }


        gameObject.SetActive(false);
    }


    // ==================================================
    // IMMEDIATE HIDE
    // ==================================================

    public void HideImmediately()
    {
        collected = true;

        gameObject.SetActive(false);
    }
}