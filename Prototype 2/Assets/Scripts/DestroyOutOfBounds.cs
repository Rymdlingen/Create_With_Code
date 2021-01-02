using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private float topBound = 30.0f;
    private float lowerBound = -10.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Destrys an object if it is outside of the players view.
        if (transform.position.z > topBound)
        {
            // Destroys the food.
            Destroy(gameObject);
        }
        else if (transform.position.z < lowerBound)
        {
            // Outputs a Game Over to the log and destroys animals.
            Debug.Log("GameOver!");
            Destroy(gameObject);
        }
    }
}
