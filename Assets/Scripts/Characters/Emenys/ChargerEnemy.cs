using System.Collections;
using UnityEngine;

public class ChargerEnemy : EnemyBase
{
    [Header("Charge Attack")]
    [SerializeField] float chargeRange = 6.0f; // 돌진 시작 거리
    [SerializeField] float chargeSpeed = 4.0f; // 돌진 속도
    [SerializeField] int chargeDamage = 3; // 돌진 데미지
    [SerializeField] float chargeDelay = 1.0f; // 돌진 준비 시간
    [SerializeField] float chargeDuration = 1.5f; // 돌진 지속 시간
    [SerializeField] float chargeCooldown = 3.0f; // 돌진 쿨다운

    // 상태 관리
    enum ChargeState
    {
        Normal,      // 평소 추적
        Preparing,   // 돌진 준비 중
        Charging     // 돌진 중
    }

    ChargeState currentState = ChargeState.Normal;
    float chargeTimer = 0f;
    float stateTimer = 0f;
    Vector2 chargeDirection;
    float originalMoveSpeed; // 원래 이동속도 저장

    protected override void Start()
    {
        base.Start();

        // 원래 이동속도 저장
        originalMoveSpeed = moveSpeed;
        chargeTimer = chargeCooldown;

        // 경고 인디케이터 초기화
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(false);
        }
    }

    protected override void Update()
    {
        if (!IsActive) return;

        // 상태별 업데이트
        switch (currentState)
        {
            case ChargeState.Normal:
                UpdateNormalState();
                break;

            case ChargeState.Preparing:
                UpdatePreparingState();
                break;

            case ChargeState.Charging:
                UpdateChargingState();
                AttackCheck(Time.deltaTime);
                break;
        }

        // 쿨다운 감소
        if (chargeTimer > 0f)
        {
            chargeTimer -= Time.deltaTime;
        }
    }

    void UpdateNormalState()
    {
        MoveCheck(Time.deltaTime);
        AttackCheck(Time.deltaTime);

        // 돌진 가능 체크
        if (chargeTimer <= 0f)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
            if (distanceToPlayer <= chargeRange)
            {
                StartChargePreparation();
            }
        }
    }

    void UpdatePreparingState()
    {
        stateTimer -= Time.deltaTime;

        // 준비 완료
        if (stateTimer <= 0f)
        {
            StartCharge();
        }
    }

    void UpdateChargingState()
    {
        stateTimer -= Time.deltaTime;

        // 돌진 속도 계산 (마지막 0.3초 동안 감속)
        float currentSpeed = chargeSpeed;
        float decelerationTime = 0.3f;
        
        if (stateTimer < decelerationTime)
        {
            // 감속: chargeSpeed → originalMoveSpeed
            currentSpeed = Mathf.Lerp(originalMoveSpeed, chargeSpeed, stateTimer / decelerationTime);
        }

        // 돌진 이동
        Move(chargeDirection * currentSpeed * Time.deltaTime);

        // 돌진 종료
        if (stateTimer <= 0f)
        {
            EndCharge();
        }
    }

    void StartChargePreparation()
    {
        currentState = ChargeState.Preparing;
        stateTimer = chargeDelay;

        // 돌진 방향 고정 (현재 플레이어 위치)
        chargeDirection = (Player.transform.position - transform.position).normalized;

        // 플레이어 바라보기
        LookAtPlayer();

        // 공통 경고 시퀀스 실행
        StartCoroutine(WarningSequence(chargeDelay));
    }

    void StartCharge()
    {
        currentState = ChargeState.Charging;
        stateTimer = chargeDuration;
        moveSpeed = chargeSpeed;
    }

    void EndCharge()
    {
        currentState = ChargeState.Normal;
        moveSpeed = originalMoveSpeed;
        chargeTimer = chargeCooldown;
    }

    protected override void MoveCheck(float deltaTime)
    {
        // 돌진 중이면 이동 안 함
        if (currentState == ChargeState.Charging)
        {
            return;
        }

        // 준비 중이면 정지
        if (currentState == ChargeState.Preparing)
        {
            Move(Vector2.zero);
            return;
        }

        // 평소에는 기본 추적
        base.MoveCheck(deltaTime);
    }

    protected override void AttackCheck(float deltaTime)
    {
        // 돌진 중에만 충돌 공격
        if (currentState == ChargeState.Charging)
        {
            // 플레이어 충돌 체크
            Collider2D playerColl = Physics2D.OverlapCircle(
                CenterPos,
                0.5f,
                1 << Player.gameObject.layer
            );

            if (playerColl != null)
            {
                // 플레이어 공격
                Vector2 hitPoint = characterCollider.ClosestPoint(Player.CharacterCollider.bounds.center);
                Player.TakeDamage(hitPoint, chargeDamage);
                
                // 넉백
                //Player.Knockback(chargeDirection * 5f);

                // 돌진 종료
                EndCharge();
            }
        }
        else
        {
            // 평소에는 기본 근접 공격
            base.AttackCheck(deltaTime);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 돌진 범위 시각화 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeRange);

        // 돌진 방향 시각화 (준비 중일 때)
        if (Application.isPlaying && currentState == ChargeState.Preparing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, chargeDirection * chargeRange);
        }
    }
}
