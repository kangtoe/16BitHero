using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnInfo
{
    public string desc;

    [Space]
    public EnemyBase enemyBase;

    [Range(0, 100)]
    public int spawnChance = 100;
    public float spawnStartTime = 1;
    public float spawnInterval = 1; // 0 means spawn only once
    public int spawnAmount = 1;
    public float spawnAreaRadius = 2;
}

[Serializable]
public class WaveInfo
{
    public string desc;

    [Space]
    public SpawnInfo[] spawnInfos;
    public int waveDuration = 30;
}

public class EnemySpawner : MonoSingleton<EnemySpawner>
{
    [SerializeField] WaveInfo[] waveSpawns;

    [Space]
    [SerializeField] EnemyBase enemyPrefab;
    [SerializeField] SpawnIndicator spawnIndicatorPrefab;
    [SerializeField] float spawnDelay = 1f;
    float lastSpawnTime = 0;

    [Header("SpawnArea")]
    [SerializeField] Vector2 spawnRadiusMinMax = new Vector2(5, 8);
    Vector2 spawnCenter => GameManager.Instance.Player.transform.position;

    [Header("SpawnedEnemies")]
    List<EnemyBase> enemies = new List<EnemyBase>();
    //[SerializeField] int maxEnemies;

    void OnValidate()
    {
        for (int i = 0; i < waveSpawns.Length; i++)
        {
            string waveInfo = "Wave" + i;
            waveSpawns[i].desc = waveInfo;

            for (int j = 0; j < waveSpawns[i].spawnInfos.Length; j++)
            {
                var v = waveSpawns[i].spawnInfos[j];
                v.desc = waveInfo + " -> " + (v.enemyBase ? "Spawn: " + v.enemyBase.name : "! Should Register Spawn Enemy !");
            }
        }
    }

    void Start()
    {
        lastSpawnTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnCheck(Time.deltaTime);
    }

    void SpawnCheck(float deltaTime)
    {
        if (Time.time - lastSpawnTime < spawnDelay) return;
        else
        {
            Vector2 spawnPosition = GetSpawnPosition() ?? spawnCenter;
            Instantiate(spawnIndicatorPrefab, spawnPosition, Quaternion.identity).Init(enemyPrefab);

            lastSpawnTime = Time.time;
        }
    }

    Vector2? GetSpawnPosition(int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 direction = Random.onUnitSphere;
            float range = Random.Range(spawnRadiusMinMax.x, spawnRadiusMinMax.y);
            Vector2 offset = direction.normalized * range;
            Vector2 targetPosition = spawnCenter + offset;

            if (ArenaArea.Instance.CheckInWall(targetPosition))
            {
                return targetPosition;
            }
        }

        return null;
    }

    public void RegisterEnemy(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }

    private void DefeatAllEnemies()
    {
        // foreach (Enemy enemy in transform.GetComponentsInChildren<Enemy>())
        //     enemy.PassAwayAfterWave();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnCenter, spawnRadiusMinMax.y);
        Gizmos.DrawWireSphere(spawnCenter, spawnRadiusMinMax.x);
    }
}
