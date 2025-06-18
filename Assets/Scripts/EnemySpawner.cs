using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoSingleton<EnemySpawner>
{
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
            Vector2 spawnPosition = GetSpawnPosition();
            Instantiate(spawnIndicatorPrefab, spawnPosition, Quaternion.identity).Init(enemyPrefab);

            lastSpawnTime = Time.time;
        }    
    }

    Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.onUnitSphere;
        Vector2 offset = direction.normalized * Random.Range(spawnRadiusMinMax.x, spawnRadiusMinMax.y);
        Vector2 targetPosition = spawnCenter + offset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -18, 18);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -8, 8);

        return targetPosition;
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
