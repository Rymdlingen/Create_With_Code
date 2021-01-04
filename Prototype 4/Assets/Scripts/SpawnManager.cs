using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public int enemyCount;
    public int waveNumber = 1;
    private float spawnRange = 9.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Counting the enemies that are still alive.
        enemyCount = FindObjectsOfType<Enemy>().Length;

        // Spawning a new wave of enemies when all are defeated.
        if (enemyCount == 0)
        {
            SpawnEnemyWave(waveNumber);
            // Also spawning ine new powerup.
            SpawnPowerup();
            // Increasing the number of enemies in the next wave.
            waveNumber++;
        }
    }

    // Spawn power up.
    void SpawnPowerup()
    {
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }

    // Spawn enemys.
    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int enemy = 0; enemy < enemiesToSpawn; enemy++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
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
