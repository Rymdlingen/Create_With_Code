using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables.
    public float speed = 10.0f;

    // Private variables.
    private Rigidbody playerRigidbody;
    private float rotationInput;

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

        // Keep player on screen.
        ConstrainPlayerPosition();
    }

    // Rotate player around z axis.
    void RotatePlayer()
    {
        // TODO need to be more precise.

        // Get the value of the players input on the rotation, from lefta and right arrows.
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
            playerRigidbody.AddForce(transform.up * speed);
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
        Vector3 playersPositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);

        // Upper boundry.
        if (playersPositionOnScreen.y > screenHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, -2);
        }

        // Lower boundy.
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

        Debug.Log(playersPositionOnScreen);
    }
}
