using UnityEngine;
using UnityEngine.UI;

// All layout values are measured in pixels of the original Camera Frame artwork.
public class PlayerScrollbarUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Level Bounds")]
    [SerializeField] private Transform levelMin;
    [SerializeField] private Transform levelMax;

    [Header("Horizontal Scrollbar")]
    [SerializeField] private RectTransform horizontalTrack;
    [SerializeField] private RectTransform horizontalThumb;

    [Header("Vertical Scrollbar")]
    [SerializeField] private RectTransform verticalTrack;
    [SerializeField] private RectTransform verticalThumb;

    [Header("Camera Frame Layout")]
    [Tooltip("The CameraFrame Image under PresentationRoot. Auto-found if left empty.")]
    [SerializeField] private RectTransform cameraFrame;
    [SerializeField] private Vector2 frameReferenceSize = new Vector2(740f, 416f);
    [Tooltip("Left, bottom, width, height in the original frame artwork. Excludes arrows.")]
    [SerializeField] private Rect horizontalTrackPixels = new Rect(106f, 4f, 575f, 22f);
    [SerializeField] private Rect verticalTrackPixels = new Rect(711f, 56f, 22f, 280f);
    [SerializeField] private Vector2 horizontalThumbPixels = new Vector2(50f, 21f);
    [SerializeField] private Vector2 verticalThumbPixels = new Vector2(21f, 50f);

    private Image horizontalImage, verticalImage;
    private Sprite originalHorizontal, originalVertical;
    private Sprite croppedHorizontal, croppedVertical;
    private bool initialized;

    private void OnEnable()
    {
        Initialize();
        Canvas.willRenderCanvases += Refresh;
    }

    private void Initialize()
    {
        if (initialized) return;
        if (cameraFrame == null)
        {
            Transform scope = horizontalTrack != null ? horizontalTrack.parent : transform.parent;
            if (scope != null)
            {
                Transform found = scope.Find("CameraFrame");
                if (found != null) cameraFrame = found as RectTransform;
            }
            if (cameraFrame == null)
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                if (canvas != null)
                    foreach (Image image in canvas.GetComponentsInChildren<Image>(true))
                        if (image.name == "CameraFrame") { cameraFrame = image.rectTransform; break; }
            }
        }

        if (cameraFrame == null)
        {
            Debug.LogWarning("PlayerScrollbarUI: Assign the CameraFrame RectTransform to align the thumbs.", this);
            return;
        }

        if (horizontalThumb != null)
        {
            horizontalImage = horizontalThumb.GetComponent<Image>();
            if (horizontalImage != null)
            {
                originalHorizontal = horizontalImage.sprite;
                croppedHorizontal = CropKnownThumb(originalHorizontal, true);
                if (croppedHorizontal != null) horizontalImage.sprite = croppedHorizontal;
                horizontalImage.preserveAspect = true;
            }
        }
        if (verticalThumb != null)
        {
            verticalImage = verticalThumb.GetComponent<Image>();
            if (verticalImage != null)
            {
                originalVertical = verticalImage.sprite;
                croppedVertical = CropKnownThumb(originalVertical, false);
                if (croppedVertical != null) verticalImage.sprite = croppedVertical;
                verticalImage.preserveAspect = true;
            }
        }
        initialized = true;
    }

    private Sprite CropKnownThumb(Sprite source, bool horizontal)
    {
        // These two existing 50 x 50 slices contain off-center transparent padding.
        // Reuse just their visible artwork; no texture read/write or asset edit is needed.
        if (source == null || source.texture == null || source.packed ||
            source.texture.name != "slideingbar" || source.rect.width != 50f || source.rect.height != 50f)
            return null;
        Rect region = source.rect;
        region = horizontal ? new Rect(region.x, region.y + 5f, 50f, 21f) :
            new Rect(region.x + 15f, region.y, 21f, 50f);
        Sprite result = Sprite.Create(source.texture, region, new Vector2(0.5f, 0.5f),
            source.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        result.name = horizontal ? "Aligned Horizontal Thumb" : "Aligned Vertical Thumb";
        return result;
    }

    private void LateUpdate() => Refresh();

    private void Refresh()
    {
        if (cameraFrame == null || frameReferenceSize.x <= 0f || frameReferenceSize.y <= 0f) return;
        Vector2 scale = new Vector2(cameraFrame.rect.width / frameReferenceSize.x,
            cameraFrame.rect.height / frameReferenceSize.y);
        if (scale.x <= 0f || scale.y <= 0f) return;

        // The render callback repeats this after the Canvas/AspectRatioFitter lays out.
        // Reparenting is runtime only, so all four objects inherit the frame's scaling.
        LayoutTrack(horizontalTrack, horizontalTrackPixels, scale);
        LayoutTrack(verticalTrack, verticalTrackPixels, scale);
        LayoutThumb(horizontalTrack, horizontalThumb, horizontalThumbPixels, scale);
        LayoutThumb(verticalTrack, verticalThumb, verticalThumbPixels, scale);

        float x = 0.5f, y = 0.5f;
        if (player != null && levelMin != null && levelMax != null)
        {
            x = Mathf.InverseLerp(levelMin.position.x, levelMax.position.x, player.position.x);
            y = Mathf.InverseLerp(levelMin.position.y, levelMax.position.y, player.position.y);
        }
        PositionThumb(horizontalTrack, horizontalThumb, x, true);
        PositionThumb(verticalTrack, verticalThumb, y, false);
    }

    private void LayoutTrack(RectTransform track, Rect reference, Vector2 scale)
    {
        if (track == null || track == cameraFrame) return;
        if (track.parent != cameraFrame) track.SetParent(cameraFrame, false);
        track.anchorMin = track.anchorMax = Vector2.zero;
        track.pivot = Vector2.zero;
        track.localScale = Vector3.one;
        track.localRotation = Quaternion.identity;
        track.anchoredPosition = new Vector2(reference.x * scale.x, reference.y * scale.y);
        track.sizeDelta = new Vector2(reference.width * scale.x, reference.height * scale.y);
    }

    private void LayoutThumb(RectTransform track, RectTransform thumb, Vector2 reference, Vector2 scale)
    {
        if (track == null || thumb == null || thumb == track) return;
        if (thumb.parent != track) thumb.SetParent(track, false);
        thumb.anchorMin = thumb.anchorMax = new Vector2(0.5f, 0.5f);
        thumb.pivot = new Vector2(0.5f, 0.5f);
        thumb.localScale = Vector3.one;
        thumb.localRotation = Quaternion.identity;
        thumb.sizeDelta = new Vector2(Mathf.Min(reference.x * scale.x, track.rect.width),
            Mathf.Min(reference.y * scale.y, track.rect.height));
    }

    private void PositionThumb(RectTransform track, RectTransform thumb, float progress, bool horizontal)
    {
        if (track == null || thumb == null) return;
        Vector2 travel = new Vector2(Mathf.Max(0f, track.rect.width - thumb.rect.width),
            Mathf.Max(0f, track.rect.height - thumb.rect.height));
        // Always set BOTH axes: the old code retained the misaligned manual offset.
        thumb.anchoredPosition = horizontal ? new Vector2((Mathf.Clamp01(progress) - 0.5f) * travel.x, 0f) :
            new Vector2(0f, (Mathf.Clamp01(progress) - 0.5f) * travel.y);
    }

    private void OnDisable() => Canvas.willRenderCanvases -= Refresh;

    private void OnDestroy()
    {
        Canvas.willRenderCanvases -= Refresh;
        if (horizontalImage != null && horizontalImage.sprite == croppedHorizontal)
            horizontalImage.sprite = originalHorizontal;
        if (verticalImage != null && verticalImage.sprite == croppedVertical)
            verticalImage.sprite = originalVertical;
        if (croppedHorizontal != null) Destroy(croppedHorizontal);
        if (croppedVertical != null) Destroy(croppedVertical);
    }
}
