using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePointsToPlayer : MonoBehaviour
{
    // The multiplier is different depending on the size of the platform, set in Unity.
    public int multiplier;

    private int basePoints = 50;

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
        if (collision.gameObject.CompareTag("Player"))
        {
            playerControllerScript = collision.gameObject.GetComponent<PlayerController>();
            gameManagerScript.score += Points();
        }
    }

    public int Points()
    {
        return playerControllerScript.basePoints * multiplier;
    }
}
