using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private float speed = 30;
    private PlayerController playerControllerScript;
    private float leftBoundry = -15;

    // Start is called before the first frame update
    void Start()
    {
        // Communicate with the player controller script.
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stop movement if player hits an obstacle.
        if (playerControllerScript.gameOver == false)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }

        // Destroy obstacle that is out of the screen.
        if (gameObject.CompareTag("Obstacle") && transform.position.x < leftBoundry)
        {
            Destroy(gameObject);
        }
    }
}
