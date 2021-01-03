using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float jumpForce = 10;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is on the ground and press space, the player jumps.
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Checks if the player is on the ground or if the player collided with an obstacle.
        if (collision.gameObject.CompareTag("Ground"))
        {
            // If the player is on the ground they can jump again.
            isOnGround = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // If the player collides with an obstacle the game is over.
            Debug.Log("Game Over!");
            gameOver = true;
        }
    }
}
