﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Public variables.
    public GameObject player;
    public Camera mainCamera;
    public Camera zoomCamera;

    float followDistance = 30.0f;
    float zoomCameraOffset;

    // Private variables.
    private PlayerController playerControllerScript;

    bool zoom = false;

    // Start is called before the first frame update.
    void Start()
    {
        // Connects to the PlayerController script, used to see if the game is active and if the camera should zoom.
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame.
    void Update()
    {

        transform.position = playerControllerScript.gameObject.transform.position;

        // Changes too the zoom camera if the player enters a platforms trigger.
        if (zoom)
        {
            mainCamera.enabled = false;
            zoomCamera.enabled = true;
        }
        // Changes too the main camera if the player exits a platforms trigger.
        else
        {
            mainCamera.enabled = true;
            zoomCamera.enabled = false;

            // TODO use altitude to see when to go back to big camera.
            if (zoom)
            {
                // The zoom camera follows the player upwards for a bit after leaving the zoom area.
                zoomCamera.transform.position = new Vector3(zoomCamera.transform.position.x, transform.position.y - zoomCameraOffset, zoomCamera.transform.position.z);
            }
            else
            {
                // Switches back to the main camera.
                mainCamera.enabled = true;
                zoomCamera.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            zoom = true;
            zoomCamera.transform.position = new Vector3(other.gameObject.transform.position.x, (other.gameObject.transform.position + Vector3.up * 30f).y, -14);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player gets further away from a platform the camera zooms out.
        if (other.gameObject.CompareTag("Platform"))
        {
            zoomCameraOffset = transform.position.y - zoomCamera.transform.position.y;
            zoom = false;
        }
    }
}
