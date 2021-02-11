using System.Collections;
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

    public bool zoomCameraActiveAndFarLeft = false;
    public bool zoomCameraActiveAndFarRight = false;

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
            ConstrainPlayerPosition();

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

    // Make sure the player stays on the screen.
    void ConstrainPlayerPosition()
    {
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

        if (zoomCameraActiveAndFarLeft || zoomCameraActiveAndFarRight)
        {
            // Players position in viewport (?).
            Vector3 playersPositionOnZoomScreen = GameObject.Find("Zoom Camera").GetComponent<Camera>().WorldToViewportPoint(transform.position);

            // ??
            playersPositionOnZoomScreen.y = Mathf.Clamp01(playersPositionOnZoomScreen.y);
            playersPositionOnZoomScreen.x = Mathf.Clamp01(playersPositionOnZoomScreen.x);

            // ??
            transform.position = GameObject.Find("Zoom Camera").GetComponent<Camera>().ViewportToWorldPoint(playersPositionOnZoomScreen);


            if (zoomCameraActiveAndFarLeft && playersPositionOnZoomScreen.x < 0)
            {
                transform.position = GameObject.Find("Zoom Camera").GetComponent<Camera>().ViewportToWorldPoint(playersPositionOnScreen);
            }

            if (zoomCameraActiveAndFarRight && playersPositionOnZoomScreen.x > screenWidth)
            {
                transform.position = GameObject.Find("Zoom Camera").GetComponent<Camera>().ViewportToWorldPoint(playersPositionOnScreen);
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
            addFuel = true;
        }
    }

    // Calculates both the vertical and horizontal direction and speed.
    private void CallculateDirectionAndSpeed()
    {
        // TODO display the speed slower. Maybe it helps if I change the force? The movement should work a bit differently anyways.

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
        Destroy(gameObject);
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
}
