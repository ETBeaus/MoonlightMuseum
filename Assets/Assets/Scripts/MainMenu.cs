using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Starts the game by loading the Game Scene.
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("Game_V3");
    }

    /// <summary>
    /// Loads the gym scene with all the functionalities.
    /// </summary>
    public void LoadGym()
    {
        SceneManager.LoadScene("Gym");
    }

    /// <summary>
    /// Alt-F4 but make it fancy.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
