using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
   public string gameSceneName = "Tut";

   public void LoadGameScene()
   {
        SceneManager.LoadScene(gameSceneName);
   }
}
