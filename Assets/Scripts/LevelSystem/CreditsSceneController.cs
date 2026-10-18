using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSceneController : MonoBehaviour
{
    [Header("Scene")]

    [SerializeField]
    private string startMenuSceneName =
        "StartMenu";


    public void ReturnToMenu()
    {
        CheckpointSystem.ClearCheckpoint();

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            startMenuSceneName
        );
    }
}