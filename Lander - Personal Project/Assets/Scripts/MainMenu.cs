using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Activated by pressing the play button in the main menu.
    public void PlayGame()
    {
        SceneManager.LoadScene("Lander");
    }

    // Activated by pressing the quit button in the main menu.
    public void QuitGame()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
