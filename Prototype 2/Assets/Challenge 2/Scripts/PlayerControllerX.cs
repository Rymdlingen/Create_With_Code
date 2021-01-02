using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public GameObject dogPrefab;

    private bool spawnDog = true;
    private float coolDown = 0.25f;
    private float time;

    // Update is called once per frame
    void Update()
    {
        // On spacebar press, send dog
        if (Input.GetKeyDown(KeyCode.Space) && spawnDog)
        {
            Instantiate(dogPrefab, transform.position, dogPrefab.transform.rotation);
            spawnDog = false;
            time = Time.time;
        }

        if (time + coolDown < Time.time)
        {
            spawnDog = true;
        }
    }
}
