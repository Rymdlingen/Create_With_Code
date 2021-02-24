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
        // The range on X where the zoom camera can be placed, based of the render size.
        zoomCameraXPositionRange = sceneCamera.targetTexture.width;

        // Makes the focal point follow the player.
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
        {
            playerControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            Vector3 playerPosition = playerControllerScript.gameObject.transform.position;
            transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z - 10);
        }

        // When the zoom camera is not used, none of the code after this statement is needed, so we continue to the next frame.
        if (!zoom)
        {
            playerControllerScript.zoomCameraActiveAndFarLeft = false;
            playerControllerScript.zoomCameraActiveAndFarRight = false;
            return;
        }

        // Zoom cameras Y position is based of the focal points position with an offset or the set minimum value, witchever is a higher value.
        float zoomCameraY = Mathf.Max(transform.position.y - zoomCameraOffset, zoomCameraMinY);

        // The zoom camera X position is in the center of the platform but never so it displayes something that is outside the scene camera.
        float zoomCameraX;

        // If the platform is to the left.
        if (latestPlatformPosition.x < 0)
        {
            // Chose whatever number is bigger, this platforms x position or the negative x range.
            zoomCameraX = Mathf.Max(latestPlatformPosition.x, -zoomCameraXPositionRange);
            // If the negative x range was bigger position the camera there and turn on the constraints in the player controller script.
            if (zoomCameraX == -zoomCameraXPositionRange)
            {
                playerControllerScript.zoomCameraActiveAndFarLeft = true;
            }
        }
        else
        {
            // Chose whatever number is smaller, this platforms x position or the x range.
            zoomCameraX = Mathf.Min(latestPlatformPosition.x, zoomCameraXPositionRange);
            // If the x range was smaller position the camera there and turn on the constraints in the player controller script.
            if (zoomCameraX == zoomCameraXPositionRange)
            {
                playerControllerScript.zoomCameraActiveAndFarRight = true;
            }
        }

        // Set the position of the zoom camera, sing only integers for a good pixel feeling.
        zoomCamera.transform.position = new Vector3(zoomCameraX, Mathf.Round(zoomCameraY), zoomCamera.transform.position.z);

        // Checks if the player exits the zoom area.
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

    // Checks if the player enters the zoom trigger.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            // Changes too the zoom camera if the player enters a platforms trigger.
            EnableZoomCamera();

            // Saves the platforms position.
            latestPlatformPosition = other.transform.position;

            // Sets the minimum Y for the zoom camera based of the platforms position.
            zoomCameraMinY = latestPlatformPosition.y + zoomCameraMinDistanceToPlatform;
        }
    }

    // Change to scene camaera.
    public void EnableSceneCamera()
    {
        sceneCamera.enabled = true;
        zoomCamera.enabled = false;
        // Change the canvas render camera to scene camera.
        canvas.worldCamera = sceneCamera;
        zoom = false;
    }

    // Change to zoom camera.
    private void EnableZoomCamera()
    {
        sceneCamera.enabled = false;
        zoomCamera.enabled = true;
        // Change the canvas render camera to zoom camera.
        canvas.worldCamera = zoomCamera;
        zoom = true;
    }
}
