using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePointsToPlayer : MonoBehaviour
{
    // The multiplier is different depending on the size of the platform, set in Unity.
    public int multiplier;

    private GameManager gameManagerScript;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        // If a platform collides with the player new points are added to the player controller script.
        if (collision.gameObject.CompareTag("Player"))
        {
            playerControllerScript = collision.gameObject.GetComponent<PlayerController>();
            gameManagerScript.newPoints += Points();
        }
    }

    // Calculates the new points based of the base points and this platforms multiplier.
    public int Points()
    {
        return playerControllerScript.basePoints * multiplier;
    }
}
