using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables.
    public float force;
    public bool gameActive = true;
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

    // Private variables.
    private Rigidbody playerRigidbody;
    private AudioSource engineAudio;
    private Camera sceneCameraComponent;
    private Camera zoomCameraComponent;
    // For rotating.
    private float rotationInput;
    private float rotationAngle;
    private bool hasRotated = false;
    private float rotationCoolDown = 0.25f;
    private float rotationCoolDownCounter;
    private float oneRotation = 11.25f;
    // For calculating the direction.
    private float verticalDirection;
    private float horizontalDirection;

    private GameManager gameManagerScript;

    public ParticleSystem[] landerFire;
    public Sprite[] arrows;

    //Rotate the lander to upright position.
    private float rotationTimeRemaining;

    // Start is called before the first frame update
    void Start()
    {
        // Used to move the player.
        playerRigidbody = GetComponent<Rigidbody>();

        engineAudio = GetComponent<AudioSource>();

        sceneCameraComponent = GameObject.Find("Scene Camera").GetComponent<Camera>();

        zoomCameraComponent = GameObject.Find("Zoom Camera").GetComponent<Camera>();

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }


    private void FixedUpdate()
    {
        // Calculates the distance between the lander and the ground directly below.
        Altitude();

        // TODO do not work after changing to pausing the time.
        // Rotate the lander to upright position if landed at an angle
        if (rotationTimeRemaining > 0)
        {
            transform.rotation = new Quaternion(0, 0, Mathf.MoveTowardsAngle(transform.rotation.z, 0, Time.unscaledDeltaTime), transform.rotation.w);
            rotationTimeRemaining = Mathf.MoveTowards(rotationTimeRemaining, 0, Time.unscaledDeltaTime);
            if (rotationTimeRemaining <= 0)
            {
                StopPlayer();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is on screen.
        CheckPlayerPosition();

        if (gameActive)
        {
            // Move player.
            RotatePlayer();
            AcceleratePlayer();

            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3"))
            {
                engineAudio.mute = false;
            }

            if (Input.GetButtonUp("Jump") || Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2") || Input.GetButtonUp("Fire3"))
            {
                engineAudio.mute = true;
            }
        }
        else
        {
            // If the game is not active, fuel is not used.
            usingFuel = false;
        }

        // Calculate and display direction and speed of lander.
        CallculateDirectionAndSpeed();
    }

    // Rotate player around z axis.
    void RotatePlayer()
    {
        rotationInput = Input.GetAxis("Horizontal");

        // Rotation cooldown.
        if (hasRotated)
        {
            // Restarts the timer.
            rotationCoolDownCounter = rotationCoolDown;
            hasRotated = false;
        }
        else
        {
            // Counts down.
            rotationCoolDownCounter -= Time.deltaTime;
        }

        // Rotates player 11.25 degrees to the left or right based on input, max rotation is 90 degrees left or right.
        if (rotationCoolDownCounter < 0 && rotationInput < 0 && (transform.rotation.eulerAngles.z < 90 || transform.rotation.eulerAngles.z > 180))
        {
            // Rotation to the left.
            rotationAngle = -oneRotation;
            hasRotated = true;
        }
        else if (rotationCoolDownCounter < 0 && rotationInput > 0 && (transform.rotation.eulerAngles.z > 270 || transform.rotation.eulerAngles.z < 180))
        {
            // Rotation to the right.
            rotationAngle = oneRotation;
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
        if (Input.GetButton("Jump") || Input.GetButton("Fire1") || Input.GetButton("Fire2") || Input.GetButton("Fire3"))
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
        // Takes away the force.
        playerRigidbody.velocity = Vector3.zero;
        // Freezes the players position and rotation.
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Moves the player downwards and restricts the rotation when there is no fuel left.
    public void OutOfFuel()
    {
        // Game is not active.
        gameActive = false;

        // Add downward force and freeze the players rotation.
        playerRigidbody.AddForce(Vector3.down * force * Time.deltaTime);
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Looks if the players position is on the screen or not.
    // Constrains the player inside the zoom camera area if necessary and tells the game manager if the lander drifts out in space from the scene camera.
    void CheckPlayerPosition()
    {
        // Screen boundaries in pixels (+ buffert for the size of the lander).
        int screenBoundaryBuffert = 5;
        int screenHeightBoundary = sceneCameraComponent.targetTexture.height + screenBoundaryBuffert * 2;
        int screenWidthBoundary = sceneCameraComponent.targetTexture.width + screenBoundaryBuffert;

        // Players position in screen points (pixels).
        Vector3 playersPositionOnScreen = sceneCameraComponent.WorldToScreenPoint(transform.position);

        // When the scene camera is active, this checks if the lander drifts out in space.
        if (sceneCameraComponent.isActiveAndEnabled == true)
        {
            // When the lander is spawned it is to the left of the screen, moving to the right and will not trigger the out in space message. 
            if (playersPositionOnScreen.x < 0 && playerRigidbody.velocity.x > 0)
            {
                return;
            }

            // If the player gets outside the boundaries of the main camera, tell the game manager that the lander has drifted out in outer space and destroy the lander.
            if (playersPositionOnScreen.y > screenHeightBoundary || playersPositionOnScreen.y < 0 - screenBoundaryBuffert * 2 || playersPositionOnScreen.x > screenWidthBoundary || playersPositionOnScreen.x < 0 - screenBoundaryBuffert * 2)
            {
                // Only shows the message if the game is active otherwise only destroyes the lander (for when the player is out of fuel).
                if (gameActive)
                {
                    hasDrifteOutInSpace = true;
                }

                DestroyLander();
            }
        }

        // Left or right boundary for when the zoom camera is active to the far left or right of the screen.
        if (zoomCameraActiveAndFarLeft || zoomCameraActiveAndFarRight)
        {
            // Players position in viewport points, values between 0 and 1 for all positions on screen, values for positions outside the screen is higher or lower.
            Vector3 playersPositionOnZoomScreen = GameObject.Find("Zoom Camera").GetComponent<Camera>().WorldToViewportPoint(transform.position);

            // Takes the x value of the players position and puts them inside the range 0 to 1, if the player is outside the screen the value is set back to 0 or 1.
            playersPositionOnZoomScreen.x = Mathf.Clamp01(playersPositionOnZoomScreen.x);

            // Takes tha possibly new values and possibly changes the players position, to keep the player on the screen.
            // Keeps the player inside the screen. The only time the player can reach the screen edge of the zoom camera is when a platform is so far to the left or right that the camera is positioned based on the screen width instead of based on the platforms position.
            transform.position = GameObject.Find("Zoom Camera").GetComponent<Camera>().ViewportToWorldPoint(playersPositionOnZoomScreen);

            // Stops the left motion if the player hits the boundary.
            if (zoomCameraActiveAndFarLeft && playersPositionOnZoomScreen.x <= 0)
            {
                playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);
            }

            // Stops the right motion if the player hits the boundary.
            if (zoomCameraActiveAndFarRight && playersPositionOnZoomScreen.x >= screenWidthBoundary)
            {
                playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);

            }
        }
    }

    // Used to see if the player collides with the ground or a platform.
    private void OnCollisionEnter(Collision collision)
    {
        // If the player collides with the ground they crash.
        if (collision.gameObject.CompareTag("Ground") && (gameActive || gameManagerScript.fuelLeft < 1))
        {
            Crash();
        }

        if (collision.gameObject.CompareTag("Platform") && (gameActive || gameManagerScript.fuelLeft < 1))
        {
            // If the player is not paralell (on rotation to left or right is okay for a hard landing) with the platform when landing, they crash.
            // If the player is falling to fast into the platform, they crash.

            // Calculates if the player is rotated too much to land.
            bool rotatedToMuch = transform.rotation.eulerAngles.z < 360 - oneRotation * 2 + 1 && transform.rotation.eulerAngles.z > oneRotation * 2 - 1;

            // If the player is rotated too much or the speed is too high they crash.
            if (rotatedToMuch || verticalSpeed > 35 || horizontalSpeed > 31)
            {
                Crash();
            }
            else
            {
                // Successfull landing, calculates if it was a good landing from the speed (and the rotation). 
                Landed(verticalSpeed, horizontalSpeed);

                // Pauses the game.
                gameActive = false;
            }
        }
    }

    // Used for detecting if player touches an powerup.
    private void OnTriggerEnter(Collider other)
    {
        // If the game is active and the player touches a powerup the powerup is destroyed and fuel is added.
        if (other.gameObject.CompareTag("Powerup") && gameActive)
        {
            Destroy(other.gameObject);
            addFuel = true;
        }
    }

    // Calculates both the vertical and horizontal direction and speed.
    private void CallculateDirectionAndSpeed()
    {
        // Calculating the vertical direction and speed.
        verticalDirection = Mathf.RoundToInt(playerRigidbody.velocity.y * 3.6f);
        verticalSpeed = Mathf.Abs(verticalDirection);

        // Calculating the horizontal direction and speed.
        horizontalDirection = Mathf.RoundToInt(playerRigidbody.velocity.x * 3.6f);
        horizontalSpeed = Mathf.Abs(horizontalDirection);

        // Depending on the vertical direction up, down or no arrow is displayed.
        if (verticalSpeed == 0)
        {
            // No arrow.
            GameObject.Find("Canvas").transform.Find("Vertical speed").Find("Vertical Arrow").GetComponent<SpriteRenderer>().sprite = null;
        }
        else if (verticalDirection > 0)
        {
            // Up arrow.
            GameObject.Find("Canvas").transform.Find("Vertical speed").Find("Vertical Arrow").GetComponent<SpriteRenderer>().sprite = arrows[0];
        }
        else if (verticalDirection < 0)
        {
            // Down arrow.
            GameObject.Find("Canvas").transform.Find("Vertical speed").Find("Vertical Arrow").GetComponent<SpriteRenderer>().sprite = arrows[1];
        }

        // Depending on the horizontal direction right, left or no arrow is displayed.
        if (horizontalSpeed == 0)
        {
            // No arrow.
            GameObject.Find("Canvas").transform.Find("Horizontal speed").Find("Horizontal Arrow").GetComponent<SpriteRenderer>().sprite = null;
        }
        else if (horizontalDirection > 0)
        {
            // Right arrow.
            GameObject.Find("Canvas").transform.Find("Horizontal speed").Find("Horizontal Arrow").GetComponent<SpriteRenderer>().sprite = arrows[3];
        }
        else if (horizontalDirection < 0)
        {
            // Left arrow.
            GameObject.Find("Canvas").transform.Find("Horizontal speed").Find("Horizontal Arrow").GetComponent<SpriteRenderer>().sprite = arrows[2];
        }
    }

    public void Crash()
    {
        // Activate the failed landning screen.
        gameManagerScript.FailedLandingScreen(true);

        DestroyLander();
        usingFuel = false;
        gameActive = false;

        // Activate screen shake on the active camera.
        ScreenShake(1, 3);
    }

    public void Landed(float verticalSpeed, float horizontalSpeed)
    {
        // TODO change so it freezes if good landing and bounces/gets into upright position if hard landning.

        bool rotatedToMuch = transform.rotation.eulerAngles.z < 360 - oneRotation + 1 && transform.rotation.eulerAngles.z > oneRotation - 1;

        // Decide on base points for the landing dependning on the speed and angle of the landing.
        if (verticalSpeed > 15 || horizontalSpeed > 15 || rotatedToMuch)
        {
            // Hard landing.
            basePoints = 15;

            // If the lander lands at an angle, start the rotation timer by setting it to 1 second.
            if (transform.rotation.z != 0)
            {
                rotationTimeRemaining = 1;
            }

            // TODO
            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionX;
            //playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
            //playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;

            BounceLander();
            //StopPlayer();

            // Activate small screen shake on the active camera.
            ScreenShake(0.3f, 0.5f);
        }
        else
        {
            // Freeze the players position when landed.
            StopPlayer();

            // Soft landing.
            basePoints = 50;
        }

        gameActive = false;
        usingFuel = false;
    }

    // Sends an ray straight down to se how far the ground is.
    private void Altitude()
    {
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore);
    }

    // Destroys the lander.
    private void DestroyLander()
    {
        Destroy(gameObject);
    }

    // Calls the screen shake in the screen shake controller.
    private void ScreenShake(float duration, float power)
    {
        // Activate screen shake on the active camera.
        if (sceneCameraComponent.isActiveAndEnabled)
        {
            sceneCameraComponent.GetComponent<ScreenShakeController>().ShakeScreen(duration, power);
        }
        else if (zoomCameraComponent.isActiveAndEnabled)
        {
            zoomCameraComponent.GetComponent<ScreenShakeController>().ShakeScreen(duration, power);
        }
    }

    // Bounces the lander if it was a hard landing.
    private void BounceLander()
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.AddForce(Vector3.up, ForceMode.Impulse);
    }
}
