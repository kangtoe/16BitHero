using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIndicator : MonoBehaviour
{
    EnemyBase spawnPrefab;
    float spawnDelay = 1f;

    void Start()
    {
        Invoke(nameof(TrySpawn), spawnDelay);
    }
    
    public void Init(EnemyBase _spawnPrefab, float _spawnDelay = 3f)
    {
        spawnPrefab = _spawnPrefab;
        spawnDelay = _spawnDelay;
        
    }

    void TrySpawn()
    {
        Destroy(gameObject);
        
        // should check game state
        if (spawnPrefab != null)
        {
            EnemyBase spawnObject = Instantiate(spawnPrefab, transform.position, Quaternion.identity);      
            EnemySpawner.Instance.RegisterEnemy(spawnObject);  
        }        
    }
}
