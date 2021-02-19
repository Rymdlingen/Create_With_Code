using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    public Camera sceneCamera;
    public Camera zoomCamera;

    private GameObject spaceBckground;
    private GameObject middleGround;
    private bool changeBackgroundPosition = true;

    // Start is called before the first frame update
    void Start()
    {
        spaceBckground = transform.Find("Space").gameObject;
        middleGround = transform.Find("Middleground").gameObject;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (sceneCamera.isActiveAndEnabled)
        {
            transform.position = new Vector3(sceneCamera.transform.position.x, sceneCamera.transform.position.y, transform.position.z);
            spaceBckground.transform.localScale = new Vector3(3, 3, 3);
            middleGround.transform.localScale = new Vector3(2, 2, 2);
            middleGround.transform.position = new Vector3(transform.position.x, transform.position.y - 25, middleGround.transform.position.z);
            changeBackgroundPosition = true;
        }
        else if (changeBackgroundPosition)
        {

            transform.position = new Vector3(zoomCamera.transform.position.x, zoomCamera.transform.position.y, transform.position.z);
            spaceBckground.transform.localScale = new Vector3(1, 1, 1);
            middleGround.transform.localScale = new Vector3(1, 1, 1);
            middleGround.transform.position = new Vector3(transform.position.x, transform.position.y + 25, middleGround.transform.position.z);
            changeBackgroundPosition = false;

        }
    }
}
