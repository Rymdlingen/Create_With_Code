using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody targetRb;
    private GameManager gameManager;
    private float minSpeed = 12;
    private float maxSpeed = 16;
    private float maxTorque = 10;
    private float xRange = 4;
    private float ySpawnPosition = -2;

    public int pointValue;
    public ParticleSystem explosionParticle;

    // Start is called before the first frame update
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // Random force an torque is set to the object.
        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        targetRb.AddTorque(new Vector3(RandomTorque(), RandomTorque(), RandomTorque()), ForceMode.Impulse);

        // Random x position is set to the object.
        transform.position = RandomSpawnPosition();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        // When the player press an object it disapers, starts an explosion and updates the score with that objects points.
        if (gameManager.isGameActive)
        {
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            gameManager.UpdateScore(pointValue);
        }
    }

    private void OnTriggerEnter()
    {
        // Destroy all objects that falls outside the screen.
        Destroy(gameObject);

        // When the player miss a good object and it falls of the screen, the game ends.
        if (!gameObject.CompareTag("Bad"))
        {
            gameManager.GameOver();
        }
    }

    // Randomizes force to put on an object.
    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }

    // Randomizes torque to put on an object.
    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }

    // Randomizes the objects x position.
    Vector3 RandomSpawnPosition()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPosition);
    }
}
