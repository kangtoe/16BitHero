using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class ExploderEnemy : EnemyBase
{
    [Header("Explosion Settings")]
    [SerializeField] float triggerRange = 1.5f;     // 자폭 감지 거리
    [SerializeField] float explosionRadius = 2.5f;  // 폭발 데미지 반경    
    [SerializeField] int explosionDamage = 5;      // 폭발 데미지
    [SerializeField] float fuseTime = 1.0f;         // 감지 후 폭발까지 걸리는 시간

    [Header("Effects")]
    [SerializeField] GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹 (ParticleSystem)

    [Header("Projectile Settings")]
    [SerializeField] GameObject projectilePrefab; // 투사체 프리팹
    [SerializeField] int projectileCount = 8;
    [SerializeField] float projectileSpeed = 3f;
    [SerializeField] int projectileDamage = 2;

    private bool isExploding = false;
    private bool isUnstoppable = false; // CC 면역 상태

    public override void Knockback(Vector2 direction)
    {
        if (isUnstoppable) return; // 자폭 중에는 넉백 면역
        base.Knockback(direction);
    }

    protected override void MoveCheck(float deltaTime)
    {
        // 자폭 시퀀스 중이거나 비활성 상태면 이동 중지
        if (isExploding || !IsActive) return;

        base.MoveCheck(deltaTime);
    }

    protected override void AttackCheck(float deltaTime)
    {
        if (isExploding) return;

        // 플레이어가 감지 범위 내에 들어왔는지 확인
        float distanceToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        if (distanceToPlayer <= triggerRange)
        {
            StartCoroutine(ExplosionSequence());
        }
    }

    IEnumerator ExplosionSequence()
    {
        isExploding = true;
        isUnstoppable = true; // CC 면역 시작

        // 이동 멈춤 (MoveCheck에서 isExploding 체크하므로 자동 정지)

        // 오버라이드된 경고 시퀀스 실행
        yield return StartCoroutine(WarningSequence(fuseTime));

        Explode();
    }

    void Explode()
    {
        // 폭발 반경 내 플레이어 피격 판정
        Collider2D hit = Physics2D.OverlapCircle(transform.position, explosionRadius, 1 << Player.gameObject.layer);
        if (hit)
        {
            Player.TakeDamage(transform.position, explosionDamage);
            Player.Knockback((Player.transform.position - transform.position).normalized);
        }

        // 8방향 투사체 발사
        if (projectilePrefab != null)
        {
            float angleStep = 360f / projectileCount;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector2 dir = rotation * Vector2.right;

                GameObject proj = Instantiate(projectilePrefab, transform.position, rotation);
                
                // 임시로 Rigidbody2D 사용 (투사체 스크립트가 있다면 해당 Init 호출 권장)
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb) rb.velocity = dir * projectileSpeed;
                
                Destroy(proj, 5f); // 5초 후 삭제 (안전장치)
            }
        }

        // 폭발 이펙트
        if (explosionEffectPrefab)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 자폭으로 인한 사망 (보상 없음)
        ForceDie(false);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRange);

        // 폭발 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
