using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
    public static bool reloaded = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Wait()
    {
        reloaded = true;
        yield return new WaitForSecondsRealtime(0);

        SceneManager.LoadScene("Lander", LoadSceneMode.Additive);
    }
}
