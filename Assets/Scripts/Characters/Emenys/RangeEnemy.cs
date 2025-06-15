// using UnityEngine;
// using System;
// using UnityEngine.Pool;

// public class RangeEnemy : EnemyBase
// {
//     [Header(" Range Attack Elements ")]
//     [SerializeField] private Transform shootingPoint;
//     [SerializeField] private RangeWeapon rangeWeapon;
//     public RangeWeapon RangeWeapon => rangeWeapon;

//     [Header(" Range Attack Settings ")]
//     [SerializeField] private float rangeAttackRadius;
//     private float rangeAttackCooldown;

//     float DistanceToPlayer => Vector2.Distance(transform.position, Player.transform.position);
//     bool IsCloseEnough => DistanceToPlayer <= rangeAttackRadius;

//     protected override void Start()
//     {
//         base.Start();
//         rangeAttackCooldown = 0f;
//     }

//     protected override void Update()
//     {
//         if (!isActive()) return;
            
//         if(IsCloseEnough) TryRangeAttack();
//         else MoveToPlayer(Time.deltaTime);        
//     }
    
//     private void TryRangeAttack()
//     {
//         if(rangeAttackCooldown > 0f)
//         {
//             rangeAttackCooldown -= Time.deltaTime;
//             return;
//         }

//         rangeAttackCooldown = rangeWeapon.AttackCooldown;
//         Shoot();
//     }

//     private void Shoot()
//     {
//         Vector2 direction = (Player.CenterPos - (Vector2)shootingPoint.position).normalized;
//         rangeWeapon.Shoot(direction);
//     }

//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position, rangeAttackRadius);
//     }
// }
