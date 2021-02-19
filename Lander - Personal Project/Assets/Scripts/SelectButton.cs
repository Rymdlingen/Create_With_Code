using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelected;

    // Start is called before the first frame update
    void Start()
    {
        //eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //eventSystem.SetSelectedGameObject(firstSelected);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
