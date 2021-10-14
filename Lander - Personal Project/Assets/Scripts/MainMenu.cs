using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public bool arcadeMode = false;

    // Activated by pressing the play button in the main menu.
    public void PlayGame()
    {
        SceneManager.LoadScene("Lander");
    }

    // Activated by pressing the arcade mode button in the main menu.
    public void ArcadeMode()
    {
        arcadeMode = true;
        SceneManager.LoadScene("Lander");
    }

    // Activated by pressing the quit button in the main menu.
    public void QuitGame()
    {
        Application.Quit();
    }
}
