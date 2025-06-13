using UnityEngine;
using System;
using UnityEngine.Pool;

public class RangeEnemy : EnemyBase
{
    [Header(" Range Attack Elements ")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private BulletPool bulletPool;

    [Header(" Range Attack Settings ")]
    [SerializeField] private int bulletDamage;
    [SerializeField] private float rangeAttackFrequency;
    [SerializeField] private float rangeAttackRadius;
    private float rangeAttackDelay;
    private float rangeAttackTimer;

    protected override void Start()
    {
        base.Start();
        rangeAttackDelay = 1f / rangeAttackFrequency;
        rangeAttackTimer = rangeAttackDelay;
    }

    protected override void Update()
    {
        if (!CanAttack())
            return;

        CheckRangeAttack();
    }

    private void CheckRangeAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.CenterPos);

        if (distanceToPlayer > rangeAttackRadius)
            FollowPlayer();
        else
            TryRangeAttack();
    }

    private void TryRangeAttack()
    {
        rangeAttackTimer += Time.deltaTime;

        if(rangeAttackTimer >= rangeAttackDelay)
        {
            rangeAttackTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector2 direction = (Player.CenterPos - (Vector2)shootingPoint.position).normalized;

        EnemyBullet bulletInstance = bulletPool.GetBullet();
        bulletInstance.Configure(this);
        bulletInstance.Shoot(bulletDamage, direction);
    }

    public void ReleaseBullet(EnemyBullet bullet)
    {
        bulletPool.ReleaseBullet(bullet);
    }

    private void OnDrawGizmos()
    {
        if (!gizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeAttackRadius);
    }
}
