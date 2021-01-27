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
    public Camera mainCameraComponent;
    public float verticalSpeed;
    public float horizontalSpeed;
    public string verticalArrow;
    public string horizontalArrow;
    public bool usingFuel = false;


    // Private variables.
    private Rigidbody playerRigidbody;
    private float rotationInput;
    private float verticalDirection;
    private float horizontalDirection;

    FollowPlayer followPlayerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Used to move the player.
        playerRigidbody = GetComponent<Rigidbody>();


        mainCameraComponent = mainCamera.GetComponent<Camera>();

        followPlayerScript = GameObject.Find("Focal Point").GetComponent<FollowPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            // Keep player on screen.
            ConstrainPlayerPosition();

            // Move player.
            RotatePlayer();
            AcceleratePlayer();
        }
        else
        {
            // Freeze the players position when the game ends.
            StopPlayer();
        }

        // Calculate and display direction and speed of lander.
        CallculateDirectionAndSpeed();
    }

    // Rotate player around z axis.
    void RotatePlayer()
    {
        // Rotates player 11.25 degrees to the left or right based on input, max rotation is 90 degrees left or right.
        if (Input.GetKeyDown(KeyCode.LeftArrow) && (transform.rotation.eulerAngles.z < 90 || transform.rotation.eulerAngles.z > 269))
        {
            // Rotation to the left.
            rotationInput = -11.25f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && (transform.rotation.eulerAngles.z > 270 || transform.rotation.eulerAngles.z < 91))
        {
            // Rotation to the right.
            rotationInput = 11.25f;
        }
        else
        {
            // No rotation.
            rotationInput = 0;
        }

        // Rotate player.
        transform.Rotate(Vector3.back * rotationInput);
    }


    // Accelerate player in its local direction.
    void AcceleratePlayer()
    {
        // If the player hold space, add force in the (local) upward direction of the player object.
        // Use up fuel when space is pressed.
        if (Input.GetKey(KeyCode.Space))
        {
            playerRigidbody.AddForce(transform.up * force * Time.deltaTime);
            usingFuel = true;
        }
        else
        {
            // Not using fuel when space is not pressed.
            usingFuel = false;
        }
    }

    // For freezing the players position when the game is not active. (used when player successfully lands on a platform.)
    void StopPlayer()
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Make sure the player stays on the screen.
    void ConstrainPlayerPosition()
    {
        // TODO needs to work smoother. Maybe stop the forward force?

        // Screen boundaries in pixels.
        int screenHeight = Screen.height;
        int screenWidth = Screen.width;

        // Players position in viewport (?).
        Vector3 playersPositionOnScreen = mainCamera.GetComponent<Camera>().WorldToViewportPoint(transform.position);

        // ??
        playersPositionOnScreen.y = Mathf.Clamp01(playersPositionOnScreen.y);
        playersPositionOnScreen.x = Mathf.Clamp01(playersPositionOnScreen.x);

        // ??
        if (mainCameraComponent.isActiveAndEnabled == true)
        {
            transform.position = Camera.main.ViewportToWorldPoint(playersPositionOnScreen);
        }

        // Upper boundry.
        if (playersPositionOnScreen.y > screenHeight)
        {
            transform.position = Camera.main.ViewportToWorldPoint(playersPositionOnScreen);
        }

        // Lower boundry.
        if (playersPositionOnScreen.y < 0)
        {
            transform.position = Camera.main.ViewportToWorldPoint(playersPositionOnScreen);
        }

        // Right boundry.
        if (playersPositionOnScreen.x > screenWidth)
        {
            transform.position = Camera.main.ViewportToWorldPoint(playersPositionOnScreen);
        }

        // Left boundry.
        if (playersPositionOnScreen.x < 0)
        {
            transform.position = Camera.main.ViewportToWorldPoint(playersPositionOnScreen);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player collides with the ground they lose and get destroyed.
        if (collision.gameObject.CompareTag("Ground"))
        {
            Lost();
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            // If the player is not paralell with the platform when landing, they lose.
            // If the player is falling to fast into the platform, they lose.
            if (transform.rotation.z < -0.1 || transform.rotation.z > 0.1 || verticalSpeed > 25 || horizontalSpeed > 31)
            {
                Lost();
            }
            else
            {
                Landed(verticalSpeed, horizontalSpeed);
                gameActive = false;
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
            // zoom = true;
            //followPlayerScript.gameObject.transform.position = other.gameObject.transform.position + Vector3.up * 15;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player gets further away from a platform the camera zooms out (happens in FollowPlayer).
        if (other.gameObject.CompareTag("Platform"))
        {
            //zoom = false;
        }
    }

    // Calculates both the vertical and horizontal direction and speed.
    private void CallculateDirectionAndSpeed()
    {
        // TODO display the speed slower. Maybe it helps if I change the force? The movement should work a bit differently anyways.

        // Calculating the vertical direction and speed.
        verticalDirection = Mathf.RoundToInt(Vector3.Dot(playerRigidbody.velocity, transform.up) * 3.6f);
        verticalSpeed = Mathf.Abs(verticalDirection);

        // Calculating the horizontal direction and speed.
        horizontalDirection = Mathf.RoundToInt(playerRigidbody.velocity.x * transform.right.x * 3.6f);
        horizontalSpeed = Mathf.Abs(horizontalDirection);

        // Depending on the vertical direction different arrows are displayed.
        if (verticalSpeed == 0)
        {
            verticalArrow = " ";
        }
        else if (verticalDirection > 0)
        {
            verticalArrow = "↑";
        }
        else if (verticalDirection < 0)
        {
            verticalArrow = "↓";
        }

        // Depending on the horizontal direction different arrows are displayed.
        if (horizontalSpeed == 0)
        {
            horizontalArrow = " ";
        }
        else if (horizontalDirection > 0)
        {
            horizontalArrow = "→";
        }
        else if (horizontalDirection < 0)
        {
            horizontalArrow = "←";
        }
    }

    public void Lost()
    {
        Destroy(gameObject);
        gameActive = false;
    }

    public void Landed(float verticalSpeed, float horizontalSpeed)
    {
        if (verticalSpeed > 15 || horizontalSpeed > 15)
        {
            // hard landing = 15 base points.
        }
        else
        {
            // soft landing = 50 base points.
        }
    }
}
