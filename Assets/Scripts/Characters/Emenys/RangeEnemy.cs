using UnityEngine;
using System;
using UnityEngine.Pool;

public class RangeEnemy : EnemyBase
{
    [Header(" Range Attack Elements ")]
    [SerializeField] RangeWeapon rangeWeapon;    
    bool IsCloseEnough => rangeWeapon?.Target;

    protected override void Start()
    {
        base.Start();        
    }

    protected override void Update()
    {
        if (!isActive()) return;
            
        MoveToPlayer(Time.deltaTime, IsCloseEnough ? 0 : moveSpeed);        
        if(IsCloseEnough) FlipSpriteCheck(LookDir);
    }
}
