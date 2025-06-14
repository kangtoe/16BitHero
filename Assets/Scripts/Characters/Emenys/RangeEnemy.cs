using UnityEngine;
using System;
using UnityEngine.Pool;

public class RangeEnemy : EnemyBase
{
    [Header(" Range Attack Elements ")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private BulletPool bulletPool;
    public BulletPool BulletPool => bulletPool;

    [Header(" Range Attack Settings ")]
    [SerializeField] private int bulletDamage;
    [SerializeField] private float rangeAttackFrequency;
    [SerializeField] private float rangeAttackRadius;
    private float rangeAttackDelay;
    private float rangeAttackCooldown;

    float DistanceToPlayer => Vector2.Distance(transform.position, Player.transform.position);
    bool IsCloseEnough => DistanceToPlayer <= rangeAttackRadius;

    protected override void Start()
    {
        base.Start();
        rangeAttackDelay = 1f / rangeAttackFrequency;
        rangeAttackCooldown = 0f;
    }

    protected override void Update()
    {
        if (!isActive()) return;
            
        if(IsCloseEnough) TryRangeAttack();
        else MoveToPlayer(Time.deltaTime);        
    }
    
    private void TryRangeAttack()
    {
        if(rangeAttackCooldown > 0f)
        {
            rangeAttackCooldown -= Time.deltaTime;
            return;
        }

        rangeAttackCooldown = rangeAttackDelay;
        Shoot();
    }

    private void Shoot()
    {
        Vector2 direction = (Player.CenterPos - (Vector2)shootingPoint.position).normalized;

        Bullet bulletInstance = bulletPool.GetBullet();
        bulletInstance.SetBulletPool(bulletPool);
        bulletInstance.Init(bulletDamage, direction);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeAttackRadius);
    }
}
