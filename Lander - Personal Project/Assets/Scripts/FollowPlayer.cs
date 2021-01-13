using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Public variables.
    public GameObject player;
    public Camera mainCamera;
    public Camera zoomCamera;

    // Private variables.
    private PlayerController playerControllerScript;

    // Start is called before the first frame update.
    void Start()
    {
        // Connects to the PlayerController script, used to see if the game is active and if the camera should zoom.
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame.
    void Update()
    {
        // The zoom camera follows the player if the game is active.
        if (playerControllerScript.gameActive)
        {
            transform.position = player.transform.position;
        }

        // Changes too the zoom camera if the player enters a platforms trigger (checked in PlayerController).
        if (playerControllerScript.zoom)
        {
            mainCamera.enabled = false;
            zoomCamera.enabled = true;
        }
        // Changes too the main camera if the player exits a platforms trigger (checked in PlayerController).
        else
        {
            mainCamera.enabled = true;
            zoomCamera.enabled = false;
        }
    }
}
