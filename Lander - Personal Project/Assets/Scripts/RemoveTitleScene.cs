using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RemoveTitleScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        StartCoroutine(HideLoadingScreen());
    }

    private IEnumerator HideLoadingScreen()
    {
        // After 2 seconds delete the title screen.
        yield return new WaitForSeconds(2);
        SceneManager.UnloadSceneAsync("Title");
    }
}
