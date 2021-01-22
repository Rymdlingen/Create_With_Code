﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables.
    public float force;
    public bool gameActive = true;
    public bool zoom = false;
    public GameObject mainCamera;
    public float verticalSpeed;
    public float horizontalSpeed;
    public string verticalArrow;
    public string horizontalArrow;


    // Private variables.
    private Rigidbody playerRigidbody;
    private float rotationInput;
    private float verticalDirection;
    private float horizontalDirection;

    // Start is called before the first frame update
    void Start()
    {
        // Used to move the player.
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move player.
        RotatePlayer();
        AcceleratePlayer();

        if (gameActive)
        {
            // Keep player on screen.
            ConstrainPlayerPosition();
        }

        CallculateDirectionAndSpeed();
    }

    // Rotate player around z axis.
    void RotatePlayer()
    {
        // TODO need to be more precise and need to start reseting I think, check how it works in lunar lander.
        // 8/1 Added drag, seems to work!

        // Get the value of the players input on the rotation, from left and right arrows.
        rotationInput = Input.GetAxis("Horizontal");

        // Add torque.
        playerRigidbody.AddTorque(Vector3.back * rotationInput);
    }

    // Accelerate player in its local direction.
    void AcceleratePlayer()
    {
        // If the player hold space, add force in the (local) upward direction of the player object.
        if (Input.GetKey(KeyCode.Space))
        {
            playerRigidbody.AddForce(transform.up * force * Time.deltaTime);
        }
    }

    // Make sure the player stays on the screen.
    void ConstrainPlayerPosition()
    {
        // TODO needs to work smoother. Maybe stop the forward force?

        // Screen boundaries.
        int screenHeight = Screen.height;
        int screenWidth = Screen.width;

        // Players position in pixels.
        Vector3 playersPositionOnScreen = mainCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position);

        // Upper boundry.
        if (playersPositionOnScreen.y > screenHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, -2);
        }

        // Lower boundry.
        if (playersPositionOnScreen.y < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 3, -2);
        }

        // Right boundry.
        if (playersPositionOnScreen.x > screenWidth)
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, -2);
        }

        // Left boundry.
        if (playersPositionOnScreen.x < 0)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, -2);
        }

        //Debug.Log(playersPositionOnScreen);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player collides with the ground they lose and get destroyed.
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Collided with: " + collision.gameObject.tag);
            Destroy(gameObject);
            gameActive = false;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            Debug.Log("Collided with: " + collision.gameObject.tag);

            // If the player is not paralell with the platform when landing, the player lose.
            // TODO add speed to losing condition, if the player is falling to fast into the platform, they lose.
            if (transform.rotation.z < -0.1 || transform.rotation.z > 0.1)
            {
                Destroy(gameObject);
                gameActive = false;
                Debug.Log("Loser");
            }
            else
            {
                Debug.Log("WIN!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player touches a powerup the powerup is destroyed.
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            Debug.Log("Trigger: " + other.gameObject.tag);
            Destroy(other.transform);
        }

        // If the player gets close to a platform the camera zooms in (happens in FollowPlayer).
        // TODO Make it a static camera? Static on the x, follows up a bit when player goes up and out of the area.
        if (other.gameObject.CompareTag("Platform"))
        {
            zoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player gets further away from a platform the camera zooms out (happens in FollowPlayer).
        if (other.gameObject.CompareTag("Platform"))
        {
            zoom = false;
        }
    }

    // Calculates both the vertical and horizontal direction and speed.
    private void CallculateDirectionAndSpeed()
    {
        // TODO display the speed slower. Maybe it helps if I change the force? The movement should work a bit differently anyways.

        // Calculating the vertical direction and speed.
        verticalDirection = Mathf.RoundToInt(Vector3.Dot(playerRigidbody.velocity, transform.up) * 50);
        verticalSpeed = Mathf.Abs(verticalDirection);

        // Calculating the horizontal direction and speed.
        horizontalDirection = Mathf.RoundToInt(Vector3.Dot(playerRigidbody.velocity, transform.right) * 50);
        horizontalSpeed = Mathf.Abs(horizontalDirection);

        // Depending on the vertical direction different arrows are displayed.
        if (verticalSpeed == 0)
        {
            verticalArrow = " ";
        }
        else if (verticalDirection > 0)
        {
            verticalArrow = "↑";
            Debug.Log("up");
        }
        else if (verticalDirection < 0)
        {
            verticalArrow = "↓";
            Debug.Log("down");
        }

        // Depending on the horizontal direction different arrows are displayed.
        if (horizontalSpeed == 0)
        {
            horizontalArrow = " ";
        }
        else if (horizontalDirection > 0)
        {
            horizontalArrow = "→";
            Debug.Log("right");
        }
        else if (horizontalDirection < 0)
        {
            horizontalArrow = "←";
            Debug.Log("left");
        }
    }
}
