using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private float powerupStrength = 15.0f;
    public float speed = 5;
    public bool hasPowerup = false;
    public GameObject powerupIndicator;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        // When the player press up or down arrow the player character moves in the direction of the camera.
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        // The power up indicator follows the player character.
        powerupIndicator.transform.position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player enters the trigger of the power up, the power up disapers and activates the indicator and the countdown.
        if (other.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    // The countdown for the power up.
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When the player collides with the enemy and have the power up, the enemy flys away.
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            Debug.Log("Collided with a " + collision.gameObject.name + "and powerup is set to " + hasPowerup);
        }
    }
}
