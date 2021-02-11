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

    // Private variables.
    private PlayerController playerControllerScript;

    public bool zoom = false;

    public RaycastHit hit;

    // Start is called before the first frame update.
    void Start()
    {

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
            return;
        }

        float zoomCameraY = Mathf.Max(transform.position.y - zoomCameraOffset, zoomCameraMinY);
        float zoomCameraX = Mathf.Max(latestPlatformPosition.x, )

        zoomCamera.transform.position = new Vector3(latestPlatformPosition.x, zoomCameraY, zoomCamera.transform.position.z);

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
