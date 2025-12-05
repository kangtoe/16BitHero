using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Attack")]
    [SerializeField] float rangedAttackRange = 5.0f;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float bulletSpeed = 3.0f;
    [SerializeField] int bulletDamage = 2;

    ObjectPool<Bullet> bulletPool;

    protected override void Start()
    {
        base.Start();

        // 총알 풀 초기화
        bulletPool = new ObjectPool<Bullet>(
            createFunc: () =>
            {
                Bullet bullet = Instantiate(bulletPrefab);
                bullet.SetBulletPool(bulletPool);
                bullet.gameObject.SetActive(false);
                return bullet;
            },
            actionOnGet: bullet => bullet.gameObject.SetActive(true),
            actionOnRelease: bullet => bullet.gameObject.SetActive(false),
            actionOnDestroy: bullet => Destroy(bullet.gameObject)
        );

        // 경고 인디케이터 초기화
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(false);
        }
    }

    protected override void AttackCheck(float deltaTime)
    {
        if (isAttacking) return; // 이미 공격 중이면 스킵

        if (attackCooldown > 0f)
        {
            attackCooldown -= deltaTime;
        }
        else
        {
            // 플레이어와의 거리 체크
            float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

            if (distanceToPlayer <= rangedAttackRange)
            {
                StartCoroutine(WarningAndShootCoroutine());
            }
        }
    }

    IEnumerator WarningAndShootCoroutine()
    {
        isAttacking = true;

        // 경고 표시
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(true);
        }

        // 경고 시간 대기
        yield return new WaitForSeconds(warningDelay);

        // 경고 숨김
        if (warningIndicator != null)
        {
            warningIndicator.SetActive(false);
        }

        // 발사
        Attack();

        // 쿨다운 설정
        attackCooldown = attackDelay;
        isAttacking = false;
    }

    protected override void Attack()
    {
        if (Player == null || bulletPool == null) return;

        // 플레이어 방향 계산
        Vector2 direction = (Player.CenterPos - (Vector2)firePoint.position).normalized;

        // 총알 생성 및 발사
        Bullet bullet = bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.Init(
            targetMask: 1 << Player.gameObject.layer,
            damage: bulletDamage,
            knockback: 0f,
            direction: direction,
            moveSpeed: bulletSpeed,
            isCriticalHit: false
        );
    }

    protected override void MoveCheck(float deltaTime)
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        // 공격 범위 안에 있으면 정지하지만 플레이어를 바라봄
        if (distanceToPlayer <= rangedAttackRange)
        {
            Move(Vector2.zero);
            LookAtPlayer(); // 플레이어를 바라봄
        }
        else
        {
            // 공격 범위 밖이면 플레이어에게 접근
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            Move(direction * moveSpeed * deltaTime);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 공격 범위 시각화 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);

        // 발사 방향 시각화
        if (firePoint != null && Application.isPlaying && Player != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 direction = (Player.CenterPos - (Vector2)firePoint.position).normalized;
            Gizmos.DrawRay(firePoint.position, direction * rangedAttackRange);
        }
    }
}
