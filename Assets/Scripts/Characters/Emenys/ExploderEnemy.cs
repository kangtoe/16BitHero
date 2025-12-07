using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class ExploderEnemy : EnemyBase
{
    [Header("Explosion Settings")]
    [SerializeField] float triggerRange = 1.5f;     // 자폭 감지 거리
    [SerializeField] float explosionRadius = 2.5f;  // 폭발 데미지 반경    
    //[SerializeField] int explosionDamage = 10;      // 폭발 데미지    

    [Header("Effects")]
    [SerializeField] GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹 (ParticleSystem)

    [Header("Projectile Settings")]
    [SerializeField] Bullet projectilePrefab; // 투사체 프리팹
    [SerializeField] int projectileCount = 8;
    [SerializeField] float projectileSpeed = 3f;
    [SerializeField] int projectileDamage = 2;

    private bool isExploding = false;

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

        // 이동 멈춤 (MoveCheck에서 isExploding 체크하므로 자동 정지)

        if (warningIndicator != null)
            warningIndicator.SetActive(true);

        yield return new WaitForSeconds(warningDelay);

        Explode();
    }

    void Explode()
    {
        // 폭발 반경 내 플레이어 피격 판정
        // (적들에게도 데미지를 줄지 여부는 기획에 따라 다름. 여기서는 플레이어만)
        // Collider2D hit = Physics2D.OverlapCircle(transform.position, explosionRadius, 1 << Player.gameObject.layer);
        // if (hit)
        // {
        //     Player.TakeDamage(transform.position, explosionDamage);
        //     Player.Knockback((Player.transform.position - transform.position).normalized);
        // }

        // 전방향 투사체 발사
        if (projectilePrefab != null)
        {
            float angleStep = 360f / projectileCount;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector2 dir = rotation * Vector2.right;

                Bullet proj = Instantiate(projectilePrefab, transform.position, rotation);
                proj.Init(Player.gameObject.layer, projectileDamage, 0f, dir, projectileSpeed, false);
            }
        }

        // 폭발 이펙트
        if (explosionEffectPrefab)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 자폭으로 인한 사망 (보상 없음?)
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
