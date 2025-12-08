using UnityEngine;

public class BufferEnemy : EnemyBase
{
    [Header("Buff Aura Settings")]
    [SerializeField] float buffRange = 4.0f; // 버프 범위
    [SerializeField] float buffSpeedIncrease = 1.3f; // 이동속도 +30%
    [SerializeField] float buffDamageIncrease = 1.5f; // 공격력 +50%
    [SerializeField] float buffRefreshRate = 3.0f; // 버프 쿨다운 (3초마다 발동)
    [SerializeField] float buffDuration = 2.0f; // 버프 지속 시간 (2초간 유지)
    [SerializeField] float warningDuration = 0.5f; // 경고 시간

    [Header("Movement Settings")]
    [SerializeField] float fleeDistance = 5.0f; // 도망 시작 거리
    [SerializeField] float maintainDistance = 5.0f; // 유지하려는 거리

    float buffTimer = 0f;
    bool isWarningShown = false;

    protected override void Start()
    {
        base.Start();
        // 초기 이동속도 설정
        moveSpeed = 1.2f;
        
        // 시작 시 약간의 딜레이를 두고 시작 (스폰되자마자 쏘지 않게)
        buffTimer = buffRefreshRate;
        
        if (warningIndicator != null)
            warningIndicator.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        
        // 버프 사이클 업데이트
        UpdateBuffCycle(Time.deltaTime);
    }

    void UpdateBuffCycle(float deltaTime)
    {
        buffTimer -= deltaTime;

        // 경고 구간: 발동 직전
        if (buffTimer <= warningDuration && buffTimer > 0f)
        {
            if (!isWarningShown)
            {
                isWarningShown = true;
                // EnemyBase의 WarningSequence 재사용 (노란색 깜빡임 + '!' 표시)
                StartCoroutine(WarningSequence(warningDuration, Color.yellow));
            }
        }
        // 발동 구간
        else if (buffTimer <= 0f)
        {
            ApplyBuffToNeighbors();
            
            // 타이머 리셋 및 경고 플래그 초기화
            buffTimer = buffRefreshRate;
            isWarningShown = false;
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
            // 너무 멀면 접근 (플레이어 방향)
            Move(directionToPlayer * moveSpeed * deltaTime);
        }
        else
        {
            // 적정 거리 유지 (정지)
            Move(Vector2.zero);
        }

        // 항상 플레이어 바라봄
        LookAtPlayer();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 버프 범위 시각화 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, buffRange);

        // 도망 거리 시각화 (빨간색)
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, fleeDistance);
    }
}
