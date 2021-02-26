using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RemoveTitleScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HideLoadingScreen());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator HideLoadingScreen()
    {
        // After 2 seconds delete the title screen.
        yield return new WaitForSeconds(2);
        SceneManager.UnloadSceneAsync("Title");
    }
}
