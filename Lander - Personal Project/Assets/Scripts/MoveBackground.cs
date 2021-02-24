using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    public Camera sceneCamera;
    public Camera zoomCamera;

    private GameObject spaceBackground;
    private GameObject middleGround;
    private bool changeBackgroundPosition = false;

    // Start is called before the first frame update
    void Start()
    {
        spaceBackground = transform.Find("Space").gameObject;
        middleGround = transform.Find("Middleground").gameObject;
    }

    void LateUpdate()
    {
        // Check if the scene or the zoom camera is active.
        if (sceneCamera.isActiveAndEnabled)
        {
            // Set the backgrounds potiton to the center of the scene camera.
            transform.position = new Vector3(sceneCamera.transform.position.x, sceneCamera.transform.position.y, transform.position.z);
            // Resize the space background to 3 times the normal size.
            spaceBackground.transform.localScale = new Vector3(3, 3, 3);
            // Resize the middleground to 2 times the normal size.
            middleGround.transform.localScale = new Vector3(2, 2, 2);
            // Position the middleground.
            middleGround.transform.position = new Vector3(transform.position.x, transform.position.y - 25, middleGround.transform.position.z);
            changeBackgroundPosition = true;
        }
        else if (changeBackgroundPosition)
        {
            // Set the backgrounds potiton to the center of the zoom camera.
            transform.position = new Vector3(zoomCamera.transform.position.x, zoomCamera.transform.position.y, transform.position.z);
            // Set the size of the middle and space background to the normal size.
            spaceBackground.transform.localScale = new Vector3(1, 1, 1);
            middleGround.transform.localScale = new Vector3(1, 1, 1);
            // Position the middleground.
            middleGround.transform.position = new Vector3(transform.position.x, transform.position.y + 25, middleGround.transform.position.z);
            changeBackgroundPosition = false;

        }
    }
}
