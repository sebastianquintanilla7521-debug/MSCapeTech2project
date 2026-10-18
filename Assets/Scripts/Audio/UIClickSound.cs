using UnityEngine;

// A scene-local relay: resolves the surviving manager on every interaction.
// Add Play to a Button's On Click event BEFORE its existing action.
[DisallowMultipleComponent]
public sealed class UIClickSound : MonoBehaviour
{
    public void Play()
    {
        GameAudio.Instance?.PlayUIClick();
    }
}
