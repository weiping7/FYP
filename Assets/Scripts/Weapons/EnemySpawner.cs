using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    { 
        public string waveName;
        public List<EnemyGroup> enemyGroups;    // A list of groups of enemies to spawn in this wave
        public int waveQuota; // The total numberof enemies to spawn int this wave
        public float spawnInterval; //The interval at which to spawn enemies
        public int spawnCount; //The number of enemies already spanned in this wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;  // The number of enemies to spawn in this wave
        public int spawnCount;  // The number of enemies of this type already spawned in this wave
        public GameObject enemyPrefab;
    }
    
    public List<Wave> waves; // A list of all the waves in the game
    public int currentWaveCount; // The index of the current wave [list start from 0]

    [Header("Spawner Attributes")]
    float spawnTimer; //Timer use to dtermine when to spawn the next enemy
    public float waveInterval;//The interval between each wave
    public int enemiesAlive;
    public int maxEnemiesAllowed; // The maximum number of enemies allowed on the map at once
    public bool maxEnemiesReached = false; // A flag indacading of the maximum number of enemies has been reached
    Transform player;

    [Header("Spawn Position")]
    public List<Transform> relativeSpawnPoints; // A list to store all the relative spawn points of enemies

    void Start()
    {
        //!
        player = FindAnyObjectByType<PlayerMovement>().transform;
        CalculateWaveQuota();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        //Check if it's time to spawn the next enemy
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        //Wave for 'waveInterval' seconds before starting the next wave
        yield return new WaitForSeconds(waveInterval);

        //10f there more waves to start after the current wave, move on to the next wave
        if (currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;

        }
        
        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }

    
    /// <summary>
    /// The method will stop spawning enemies if the enemies on the map is maximum
    /// The method will only spawn enemies in a particular wave until it is time for the next wave's enemies to be spawned
    /// </summary>
    void SpawnEnemies()
    {
        //Check if  the minumum number of enemies in the wave have been spawned
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            //Spawn each type of enemy until the qouta is filled
            foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                //Check if the minimum number of enemies of this type have been spawned
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;

                    }

                    // spawn the enemy at a player that close to player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        //Reset the maxEnemiesReached falg if the number of enemies alive has dropped below the maximum amount
        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    //When enemy killed
    public void OnEnemyKilled()
    {
       enemiesAlive--;
    }
}
