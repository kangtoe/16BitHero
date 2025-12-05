using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SplitterEnemy : EnemyBase
{
    [Header("Split Settings")]
    [SerializeField] GameObject smallEnemyPrefab; // 작은 적 프리팹 (EnemyBase)
    [SerializeField] int minSplitCount = 2;
    [SerializeField] int maxSplitCount = 3;
    [SerializeField] float splitSpawnRadius = 0.5f; // 분열 시 생성 반경

    [SerializeField] LayerMask wallLayer; // 벽 레이어

    protected override void Die()
    {
        if (isDead) return;

        // 분열 처리
        if (smallEnemyPrefab != null)
        {
            SpawnSplitEnemies();
        }

        base.Die();
    }

    void SpawnSplitEnemies()
    {
        int count = Random.Range(minSplitCount, maxSplitCount + 1);

        for (int i = 0; i < count; i++)
        {
            // 랜덤 위치에 생성
            Vector2 offset = Random.insideUnitCircle * splitSpawnRadius;
            Vector2 spawnPos = (Vector2)transform.position + offset;

            // 벽 체크: 생성 위치에 벽이 있으면 부모 위치(안전한 곳)로 변경
            if (Physics2D.OverlapCircle(spawnPos, 0.2f, wallLayer))
            {
                spawnPos = transform.position;
            }

            Instantiate(smallEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 분열 생성 범위 시각화 (보라색)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, splitSpawnRadius);
    }
}
