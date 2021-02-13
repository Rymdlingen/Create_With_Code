﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables.
    public float force;
    public bool gameActive = true;
    public GameObject mainCamera;
    public Camera mainCameraComponent;
    public float verticalSpeed;
    public float horizontalSpeed;
    public string verticalArrow;
    public string horizontalArrow;
    public bool usingFuel = false;
    public int basePoints = 0;
    public RaycastHit hit;
    public bool gameOver = false;
    public bool addFuel = false;

    public bool hasDrifteOutInSpace = false;
    public bool zoomCameraActiveAndFarLeft = false;
    public bool zoomCameraActiveAndFarRight = false;

    public int successfulLandings;
    public int crashes;

    // Private variables.
    private Rigidbody playerRigidbody;
    // For rotating.
    private float rotationInput;
    private float rotationAngle;
    private bool hasRotated = false;
    private float rotationCoolDown = 0.25f;
    private float rotationCoolDownCounter;
    // For calculating the direction.
    private float verticalDirection;
    private float horizontalDirection;

    FollowPlayer followPlayerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Used to move the player.
        playerRigidbody = GetComponent<Rigidbody>();

        mainCamera = GameObject.Find("Main Camera");

        mainCameraComponent = mainCamera.GetComponent<Camera>();

        followPlayerScript = GameObject.Find("Focal Point").GetComponent<FollowPlayer>();
    }


    private void FixedUpdate()
    {
        // Calculates the distance between the lander and the ground directly below.
        Altitude();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            // Keep player on screen.
            CheckPlayerPosition();

            // Move player.
            RotatePlayer();
            AcceleratePlayer();
        }
        else
        {

        }

        // Calculate and display direction and speed of lander.
        CallculateDirectionAndSpeed();
    }

    // Rotate player around z axis.
    void RotatePlayer()
    {
        rotationInput = Input.GetAxis("Horizontal");

        if (hasRotated)
        {
            rotationCoolDownCounter = rotationCoolDown;
            hasRotated = false;
        }
        else
        {
            rotationCoolDownCounter -= Time.deltaTime;
        }

        // Rotates player 11.25 degrees to the left or right based on input, max rotation is 90 degrees left or right.
        if (rotationCoolDownCounter < 0 && rotationInput < 0 && (transform.rotation.eulerAngles.z < 90 || transform.rotation.eulerAngles.z > 269))
        {
            // Rotation to the left.
            rotationAngle = -11.25f;
            hasRotated = true;
        }
        else if (rotationCoolDownCounter < 0 && rotationInput > 0 && (transform.rotation.eulerAngles.z > 270 || transform.rotation.eulerAngles.z < 91))
        {
            // Rotation to the right.
            rotationAngle = 11.25f;
            hasRotated = true;
        }
        else
        {
            // No rotation.
            rotationAngle = 0;
        }


        // Rotate player.
        transform.Rotate(Vector3.back * rotationAngle);
    }


    // Accelerate player in its local direction.
    void AcceleratePlayer()
    {
        // If the player hold space, add force in the (local) upward direction of the player object.
        // Use up fuel when space is pressed.
        if (Input.GetAxis("Jump") > 0)
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
    public void StopPlayer()
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void OutOfFuel()
    {
        //playerRigidbody.velocity = Vector3.down;
        gameActive = false;
        playerRigidbody.AddForce(Vector3.down * force * Time.deltaTime);
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Looks if the players position is on the screen or not.
    // Constrains the player inside the zoom camera area if necessary and tells the game manager if the lander drifts out in space from the main camera.
    void CheckPlayerPosition()
    {
        // Screen boundaries in pixels (+ buffert for the size of the lander).
        int screenBoundaryBuffert = 10;
        int screenHeightBoundary = Screen.height + screenBoundaryBuffert * 2;
        int screenWidthBoundary = Screen.width + screenBoundaryBuffert;

        // Players position in screen points (pixels).
        Vector3 playersPositionOnScreen = mainCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position);

        // When the main camera is active, this checks if the lander drifts out in space.
        if (mainCameraComponent.isActiveAndEnabled == true)
        {
            // When the lander is spawned it is to the left of the screen, moving to the right. 
            if (playersPositionOnScreen.x < 0 && playerRigidbody.velocity.x > 0)
            {
                return;
            }

            // If the player gets outside the boundaries of the main camer, tell the game manager that the lander has drifted out in outer space.
            if (playersPositionOnScreen.y > screenHeightBoundary || playersPositionOnScreen.y < 0 || playersPositionOnScreen.x > screenWidthBoundary || playersPositionOnScreen.x < 0)
            {
                DestroyLander();
                hasDrifteOutInSpace = true;
            }
        }

        // Boundries for when the zoom camera is active to the far left or right of the screen.
        if (zoomCameraActiveAndFarLeft || zoomCameraActiveAndFarRight)
        {
            // Players position in viewport points, values between 0 and 1 for all positions on screen, of screen values go higher and lower.
            Vector3 playersPositionOnZoomScreen = GameObject.Find("Zoom Camera").GetComponent<Camera>().WorldToViewportPoint(transform.position);

            // Takes the x and y values of the players position and puts them inside the range 0 to 1, if the player is outside the screen the value is set to 0 or 1.
            playersPositionOnZoomScreen.y = Mathf.Clamp01(playersPositionOnZoomScreen.y);
            playersPositionOnZoomScreen.x = Mathf.Clamp01(playersPositionOnZoomScreen.x);

            // Takes tha possibly new values and possibly changes the players position.
            // Keeps the player inside the screen. The only time the player can reach the screen edge of the zoom camera is when a platform is so far to the left or right that the camera is positioned based on the screen width instead of based on the platforms position.
            transform.position = GameObject.Find("Zoom Camera").GetComponent<Camera>().ViewportToWorldPoint(playersPositionOnZoomScreen);


            // Stops the left motion if the player hits the boundry.
            if (zoomCameraActiveAndFarLeft && playersPositionOnZoomScreen.x <= 0)
            {
                playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);
            }

            // Stops the right motion if the player hits the boundry.
            if (zoomCameraActiveAndFarRight && playersPositionOnZoomScreen.x >= screenWidthBoundary)
            {
                playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player collides with the ground they lose and get destroyed.
        if (collision.gameObject.CompareTag("Ground"))
        {
            Crash();
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            // If the player is not paralell with the platform when landing, they lose.
            // If the player is falling to fast into the platform, they lose.
            if (transform.rotation.z < -0.1 || transform.rotation.z > 0.1 || verticalSpeed > 35 || horizontalSpeed > 31)
            {
                Crash();
            }
            else
            {
                successfulLandings++;
                Landed(verticalSpeed, horizontalSpeed);

                gameActive = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player touches a powerup the powerup is destroyed.
        if (other.gameObject.CompareTag("Powerup") && gameActive)
        {
            Destroy(other.gameObject);
            addFuel = true;
        }
    }

    // Calculates both the vertical and horizontal direction and speed.
    private void CallculateDirectionAndSpeed()
    {
        // TODO display the speed slower? Maybe it helps if I change the force? The movement should work a bit differently anyways.

        // Calculating the vertical direction and speed.
        verticalDirection = Mathf.RoundToInt(playerRigidbody.velocity.y * 3.6f);
        verticalSpeed = Mathf.Abs(verticalDirection);

        // Calculating the horizontal direction and speed.
        horizontalDirection = Mathf.RoundToInt(playerRigidbody.velocity.x * 3.6f);
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

    public void Crash()
    {
        GameObject.Find("Game Manager").GetComponent<GameManager>().FailedLandingScreen(true);
        DestroyLander();
        crashes++;
        gameActive = false;
    }

    public void Landed(float verticalSpeed, float horizontalSpeed)
    {
        // Freeze the players position when landed.
        StopPlayer();

        if (verticalSpeed > 15 || horizontalSpeed > 15)
        {
            basePoints = 15;
        }
        else
        {
            basePoints = 50;
        }
    }

    private void Altitude()
    {
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore);
    }

    private void DestroyLander()
    {
        Destroy(gameObject);
    }
}
