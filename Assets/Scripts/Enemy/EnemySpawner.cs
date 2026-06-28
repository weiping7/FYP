using System.Collections;
using System.Collections.Generic;
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

    [Header("Pooling")]
    public bool useObjectPooling = true;

    public static readonly List<EnemyStats> activeEnemies = new List<EnemyStats>();

    public List<Wave> waves; // A list of all the waves in the game
    public int currentWaveCount; // The index of the current wave [list start from 0]
    bool isWaitingForNextWave;

    [Header("Spawner Attributes")]
    float spawnTimer; //Timer use to dtermine when to spawn the next enemy
    public float waveInterval;//The interval between each wave
    public int enemiesAlive;
    public int maxEnemiesAllowed; // The maximum number of enemies allowed on the map at once
    public bool maxEnemiesReached = false; // A flag indacading of the maximum number of enemies has been reached
    Transform player;

    [Header("Spawn Position")]
    public List<Transform> relativeSpawnPoints; // A list to store all the relative spawn points of enemies


    [Header("Spawn Randomness")]
    public float spawnRadius = 4f;
    public float enemySpacing = 0.5f;
    public float minDistanceFromPlayer = 5f;

    void Start()
    {
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("EnemySpawner: PlayerMovement not found.");
            enabled = false;
            return;
        }

        if (waves == null || waves.Count == 0)
        {
            Debug.LogError("EnemySpawner: No waves assigned.");
            enabled = false;
            return;
        }

        if (relativeSpawnPoints == null || relativeSpawnPoints.Count == 0)
        {
            Debug.LogError("EnemySpawner: No spawn points assigned.");
            enabled = false;
            return;
        }

        player = playerMovement.transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveCount >= waves.Count)
        {
            return;
        }

        if (waves[currentWaveCount].spawnCount >= waves[currentWaveCount].waveQuota
            && enemiesAlive <= 0
            && !isWaitingForNextWave)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaitingForNextWave = true;

        yield return new WaitForSeconds(waveInterval);

        if (currentWaveCount < waves.Count - 1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
            spawnTimer = 0f;
        }

        isWaitingForNextWave = false;
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

    Vector2 GetSpawnPosition()
    {
        Vector2 fallbackPosition = player.position;

        for (int i = 0; i < 20; i++)
        {
            Transform spawnPoint = relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)];

            Vector2 spawnCenter = spawnPoint.position;
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPosition = spawnCenter + randomOffset;

            fallbackPosition = spawnPosition;

            if (Vector2.Distance(player.position, spawnPosition) < minDistanceFromPlayer)
            {
                continue;
            }

            if (MapBoundary.Instance == null || MapBoundary.Instance.IsInside(spawnPosition, enemySpacing))
            {
                return spawnPosition;
            }
        }

        if (MapBoundary.Instance != null)
        {
            return MapBoundary.Instance.ClampPosition(fallbackPosition, enemySpacing);
        }

        return fallbackPosition;
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

                    if (enemyGroup.enemyPrefab == null)
                    {
                        Debug.LogWarning($"EnemySpawner: Enemy prefab missing in group {enemyGroup.enemyName}.");
                        continue;
                    }


                    Vector2 spawnPosition = GetSpawnPosition();

                    if (useObjectPooling && ObjectPoolManager.Instance != null)
                    {
                        ObjectPoolManager.Instance.GetObject(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
                    }

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
