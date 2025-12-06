using UnityEngine;

public class BufferEnemy : EnemyBase
{
    [Header("Buff Aura Settings")]
    [SerializeField] float buffRange = 4.0f; // 버프 범위
    [SerializeField] float buffSpeedIncrease = 1.3f; // 이동속도 +30%
    [SerializeField] float buffDamageIncrease = 1.5f; // 공격력 +50%
    [SerializeField] float buffRefreshRate = 0.5f; // 버프 갱신 주기
    [SerializeField] float buffDuration = 0.6f; // 버프 지속 시간 (갱신 주기보다 약간 길게)

    [Header("Movement Settings")]
    [SerializeField] float fleeDistance = 5.0f; // 도망 시작 거리
    [SerializeField] float maintainDistance = 5.0f; // 유지하려는 거리

    float buffTimer = 0f;

    protected override void Start()
    {
        base.Start();
        // 초기 이동속도 설정 (기획서: 1.2)
        moveSpeed = 1.2f;        
    }

    protected override void Update()
    {
        base.Update();
        
        // 버프 오라 업데이트
        UpdateBuffAura(Time.deltaTime);
    }

    void UpdateBuffAura(float deltaTime)
    {
        buffTimer -= deltaTime;
        if (buffTimer <= 0f)
        {
            ApplyBuffToNeighbors();
            buffTimer = buffRefreshRate;
        }
    }

    void ApplyBuffToNeighbors()
    {
        // 범위 내 적 탐색 (자신의 레이어 사용)
        int layerMask = 1 << gameObject.layer;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, buffRange, layerMask);

        foreach (var col in colliders)
        {
            // 자기 자신은 제외
            if (col.gameObject == gameObject) continue;

            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.ApplyBuff(buffSpeedIncrease, buffDamageIncrease, buffDuration);
            }
        }
    }

    protected override void MoveCheck(float deltaTime)
    {
        // 플레이어와의 거리
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        Vector2 directionToPlayer = (Player.transform.position - transform.position).normalized;

        if (distanceToPlayer < fleeDistance)
        {
            // 너무 가까우면 도망 (플레이어 반대 방향)
            Vector2 fleeDir = -directionToPlayer;
            Move(fleeDir * moveSpeed * deltaTime);
        }
        else if (distanceToPlayer > maintainDistance + 1.0f)
        {
            // 너무 멀면 접근 (플레이어 방향) - 버프 줄 적들이 플레이어 근처에 있을 확률 높음
            Move(directionToPlayer * moveSpeed * deltaTime);
        }
        else
        {
            // 적정 거리 유지 (정지)
            Move(Vector2.zero);
        }

        // 항상 플레이어 바라봄 (선택 사항)
        LookAtPlayer();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 버프 범위 시각화 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, buffRange);

        // 도망 거리 시각화 (빨간색 점선 느낌)
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, fleeDistance);
    }
}
