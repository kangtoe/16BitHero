using UnityEngine;

public class WanderingRangedEnemy : RangedEnemy
{
    [Header("Wandering Movement")]
    [SerializeField] float targetCircleRadius = 1f;
    [SerializeField] LayerMask wallLayer;
    
    Vector2 wanderDirection;

    protected override void Start()
    {
        base.Start();
        UpdateWanderDirection();
    }

    protected override void MoveCheck(float deltaTime)
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        // 공격 범위 안에 있으면 정지하지만 플레이어를 바라봄
        if (distanceToPlayer <= rangedAttackRange)
        {
            Move(Vector2.zero);
            LookAtPlayer();
        }
        else
        {
            // 공격 범위 밖이면 방랑
            Move(wanderDirection * moveSpeed * deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 벽 충돌 시 방향 변경
        if ((wallLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            UpdateWanderDirection();
        }
    }

    void UpdateWanderDirection()
    {
        // 랜덤한 방향으로 변경
        Vector2 randomPoint = Random.insideUnitCircle.normalized * targetCircleRadius;
        wanderDirection = (randomPoint - (Vector2)transform.position).normalized;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 방랑 방향 시각화 (청록색)
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, wanderDirection * 2f);
        }

        // 방랑 범위 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.zero, targetCircleRadius);
    }
}
