using UnityEngine;

// Derived preview/runtime track objects only; never contains hand-placed scenery.
[ExecuteAlways]
public sealed class UniversalAutoTrackLayout : MonoBehaviour
{
    [SerializeField, HideInInspector] private UniversalMovingPlatform owner;
    private float nextLayoutCheck;
    public UniversalMovingPlatform Owner => owner;
    public void SetOwner(UniversalMovingPlatform value) => owner = value;

    private void LateUpdate()
    {
        if (owner == null)
        {
            RemoveSelf();
            return;
        }
        if (Time.realtimeSinceStartup >= nextLayoutCheck)
        {
            nextLayoutCheck = Time.realtimeSinceStartup + 0.15f;
            owner.RefreshAutomaticTracks();
            if (this == null || owner == null) return;
        }
        bool visible = owner.gameObject.activeInHierarchy;
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).gameObject.activeSelf != visible)
                transform.GetChild(i).gameObject.SetActive(visible);
    }

    private void RemoveSelf()
    {
        if (Application.isPlaying) Destroy(gameObject);
        else DestroyImmediate(gameObject);
    }
}
