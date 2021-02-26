using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menuScreens;

    // Start is called before the first frame update
    void Start()
    {
        // Start the time.
        Time.timeScale = 1;
        ShowTheRightMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ShowTheRightMenu()
    {
        // Hide all menu screens except the main one.
        for (int screen = 1; screen < menuScreens.Length; screen++)
        {
            menuScreens[screen].SetActive(false);
        }
    }
}
