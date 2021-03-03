using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menuScreens;
    private GameObject currentSelectedButton;
    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        // Start the time.
        Time.timeScale = 1;
        ShowTheRightMenu();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the mouse is clicked outside of the menu, the last selected button is reselected.
        // Save the currently selected button.
        if (eventSystem.currentSelectedGameObject != null)
        {
            currentSelectedButton = eventSystem.currentSelectedGameObject;
        }

        // If no button is selected, make the last selected on selected.
        if (eventSystem.currentSelectedGameObject == null)
        {
            CatchMouseClicks(currentSelectedButton);
        }
    }

    // Set the selected game object.
    public void CatchMouseClicks(GameObject setSelection)
    {
        EventSystem.current.SetSelectedGameObject(setSelection);
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
