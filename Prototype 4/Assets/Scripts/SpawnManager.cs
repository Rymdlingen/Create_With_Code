using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    private float spawnRange = 9.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Spawns an enemy.
        Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Generate a random position for an enemy.
    private Vector3 GenerateSpawnPosition()
    {
        float spawnPositionX = Random.Range(-spawnRange, spawnRange);
        float spawnPositionZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPosition = new Vector3(spawnPositionX, 0, spawnPositionZ);

        return randomPosition;
    }
}
