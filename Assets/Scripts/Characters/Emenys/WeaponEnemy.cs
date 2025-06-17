using UnityEngine;
using System;
using UnityEngine.Pool;

public class WeaponEnemy : EnemyBase
{
    [Header("Range Attack")]
    [SerializeField] WeaponBase weapon;    
    bool IsCloseEnough => weapon?.Target;

    protected override void Start()
    {
        base.Start();        
    }

    protected override void Update()
    {
        if (!IsActive) return;
            
        MoveToPlayer(Time.deltaTime, IsCloseEnough ? 0 : moveSpeed);        
        if(IsCloseEnough) FlipSpriteCheck(LookDir);
    }
}
