using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common {
  public class LevelManager : MonoBehaviour {
    public static void LoadLevel(string levelName) {
      SceneManager.LoadScene(levelName);
    }

    public static void ExitGame() {
      Application.Quit();
    }
  }
}
