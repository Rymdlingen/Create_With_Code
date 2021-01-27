using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Public variables.
    public GameObject player;
    public Camera mainCamera;
    public Camera zoomCamera;

    float followTime = 3.0f;
    float followCounter;

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

        if (zoom)
        {
            followCounter = followTime;
        }
        else
        {
            followCounter -= Time.deltaTime;
        }

        // Changes too the zoom camera if the player enters a platforms trigger (checked in PlayerController).
        if (zoom)
        {
            mainCamera.enabled = false;
            zoomCamera.enabled = true;
        }
        // Changes too the main camera if the player exits a platforms trigger (checked in PlayerController).
        else
        {
            if (followCounter > 0)
            {
                zoomCamera.transform.position = new Vector3(zoomCamera.transform.position.x, (transform.position + Vector3.up * 20).y, zoomCamera.transform.position.z);
            }
            else
            {
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
            zoomCamera.transform.position = new Vector3(other.gameObject.transform.position.x, (other.gameObject.transform.position + Vector3.up * 20).y, -14);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player gets further away from a platform the camera zooms out.
        if (other.gameObject.CompareTag("Platform"))
        {
            zoom = false;
        }
    }
}
