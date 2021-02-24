using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Public variables.
    public GameObject player;
    public Camera sceneCamera;
    public Camera zoomCamera;

    private float zoomCameraMinDistanceToPlatform = 40.0f;
    private float zoomCameraOffset = 55;

    private Vector3 latestPlatformPosition;
    private float zoomCameraMinY;
    public float zoomCameraXPositionRange;

    // Private variables.
    private PlayerController playerControllerScript;
    private Canvas canvas;

    public bool zoom = false;

    public RaycastHit hit;

    // Start is called before the first frame update.
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Update is called once per frame.
    void Update()
    {
        zoomCameraXPositionRange = sceneCamera.targetTexture.width;

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

        zoomCamera.transform.position = new Vector3(zoomCameraX, Mathf.Round(zoomCameraY), zoomCamera.transform.position.z);

        if (transform.position.y - latestPlatformPosition.y > 130 || latestPlatformPosition.y - transform.position.y > 30)
        {
            // Switches back to the scene camera.
            EnableSceneCamera();
        }
        else if (latestPlatformPosition.x - transform.position.x > 100 || latestPlatformPosition.x - transform.position.x < -100)
        {
            // Switches back to the scene camera.
            EnableSceneCamera();
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

    public void EnableSceneCamera()
    {
        sceneCamera.enabled = true;
        zoomCamera.enabled = false;
        // Change the canvas render camera to scene camera.
        canvas.worldCamera = sceneCamera;
        zoom = false;
    }

    private void EnableZoomCamera()
    {
        sceneCamera.enabled = false;
        zoomCamera.enabled = true;
        // Change the canvas render camera to zoom camera.
        canvas.worldCamera = zoomCamera;
        zoom = true;
    }
}
