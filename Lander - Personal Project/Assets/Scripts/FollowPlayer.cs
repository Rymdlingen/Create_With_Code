using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Public variables.
    public GameObject player;
    public Camera mainCamera;
    public Camera zoomCamera;

    float zoomCameraMinDistanceToPlatform = 40.0f;
    float zoomCameraOffset = 55;

    Vector3 latestPlatformPosition;
    float zoomCameraMinY;
    float zoomCameraXPositionRange = 260;

    // Private variables.
    private PlayerController playerControllerScript;

    public bool zoom = false;

    public RaycastHit hit;

    // Start is called before the first frame update.
    void Start()
    {
        zoomCameraXPositionRange = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x / 3;
        Debug.Log(zoomCameraXPositionRange);
    }

    // Update is called once per frame.
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            transform.position = playerControllerScript.gameObject.transform.position;
        }

        if (!zoom)
        {
            playerControllerScript.zoomCameraActiveAndFarLeft = false;
            playerControllerScript.zoomCameraActiveAndFarRight = false;
            return;
        }

        float zoomCameraY = Mathf.Max(transform.position.y - zoomCameraOffset, zoomCameraMinY);
        float zoomCameraX;

        if (latestPlatformPosition.x < 0)
        {
            zoomCameraX = Mathf.Max(latestPlatformPosition.x, -zoomCameraXPositionRange);
            if (zoomCameraX == -zoomCameraXPositionRange)
            {
                playerControllerScript.zoomCameraActiveAndFarLeft = true;
            }
        }
        else
        {
            zoomCameraX = Mathf.Min(latestPlatformPosition.x, zoomCameraXPositionRange);
            if (zoomCameraX == zoomCameraXPositionRange)
            {
                playerControllerScript.zoomCameraActiveAndFarRight = true;
            }
        }

        zoomCamera.transform.position = new Vector3(zoomCameraX, zoomCameraY, zoomCamera.transform.position.z);

        if (transform.position.y - latestPlatformPosition.y > 130)
        {
            // Switches back to the main camera.
            EnableMainCamera();
        }
        else if (latestPlatformPosition.x - transform.position.x > 100 || latestPlatformPosition.x - transform.position.x < -100)
        {
            // Switches back to the main camera.
            EnableMainCamera();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            // Changes too the zoom camera if the player enters a platforms trigger.
            EnableZoomCamera();

            latestPlatformPosition = other.transform.position;

            zoomCameraMinY = latestPlatformPosition.y + zoomCameraMinDistanceToPlatform;
        }
    }

    public void EnableMainCamera()
    {
        mainCamera.enabled = true;
        zoomCamera.enabled = false;
        zoom = false;
    }

    private void EnableZoomCamera()
    {
        mainCamera.enabled = false;
        zoomCamera.enabled = true;
        zoom = true;
    }
}
