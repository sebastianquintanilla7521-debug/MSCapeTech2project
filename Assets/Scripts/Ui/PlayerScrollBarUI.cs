using UnityEngine;

public class PlayerScrollbarUI : MonoBehaviour
{
    // ==================================================
    // PLAYER
    // ==================================================

    [Header("Player")]

    [SerializeField]
    private Transform player;


    // ==================================================
    // LEVEL BOUNDS
    // ==================================================

    [Header("Level Bounds")]

    [Tooltip("Bottom-left playable point of the level.")]
    [SerializeField]
    private Transform levelMin;

    [Tooltip("Top-right playable point of the level.")]
    [SerializeField]
    private Transform levelMax;


    // ==================================================
    // HORIZONTAL SCROLLBAR
    // ==================================================

    [Header("Horizontal Scrollbar")]

    [SerializeField]
    private RectTransform horizontalTrack;

    [SerializeField]
    private RectTransform horizontalThumb;


    // ==================================================
    // VERTICAL SCROLLBAR
    // ==================================================

    [Header("Vertical Scrollbar")]

    [SerializeField]
    private RectTransform verticalTrack;

    [SerializeField]
    private RectTransform verticalThumb;


    // ==================================================
    // UPDATE
    // ==================================================

    private void LateUpdate()
    {
        if (
            player == null ||
            levelMin == null ||
            levelMax == null
        )
        {
            return;
        }


        UpdateHorizontalScrollbar();
        UpdateVerticalScrollbar();
    }


    // ==================================================
    // HORIZONTAL
    // ==================================================

    private void UpdateHorizontalScrollbar()
    {
        if (
            horizontalTrack == null ||
            horizontalThumb == null
        )
        {
            return;
        }


        float normalizedX =
            Mathf.InverseLerp(
                levelMin.position.x,
                levelMax.position.x,
                player.position.x
            );


        normalizedX =
            Mathf.Clamp01(normalizedX);


        float trackWidth =
            horizontalTrack.rect.width;


        float thumbWidth =
            horizontalThumb.rect.width;


        float availableDistance =
            Mathf.Max(
                0f,
                trackWidth - thumbWidth
            );


        float newX =
            Mathf.Lerp(
                -availableDistance * 0.5f,
                availableDistance * 0.5f,
                normalizedX
            );


        Vector2 position =
            horizontalThumb.anchoredPosition;


        position.x =
            newX;


        horizontalThumb.anchoredPosition =
            position;
    }


    // ==================================================
    // VERTICAL
    // ==================================================

    private void UpdateVerticalScrollbar()
    {
        if (
            verticalTrack == null ||
            verticalThumb == null
        )
        {
            return;
        }


        float normalizedY =
            Mathf.InverseLerp(
                levelMin.position.y,
                levelMax.position.y,
                player.position.y
            );


        normalizedY =
            Mathf.Clamp01(normalizedY);


        float trackHeight =
            verticalTrack.rect.height;


        float thumbHeight =
            verticalThumb.rect.height;


        float availableDistance =
            Mathf.Max(
                0f,
                trackHeight - thumbHeight
            );


        float newY =
            Mathf.Lerp(
                -availableDistance * 0.5f,
                availableDistance * 0.5f,
                normalizedY
            );


        Vector2 position =
            verticalThumb.anchoredPosition;


        position.y =
            newY;


        verticalThumb.anchoredPosition =
            position;
    }
}