using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    private Rigidbody enemyRb;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Calculates the direction that the enemy is going to move.
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;

        // Moves the enemy towards the player.
        enemyRb.AddForce(lookDirection * speed);
    }
}
