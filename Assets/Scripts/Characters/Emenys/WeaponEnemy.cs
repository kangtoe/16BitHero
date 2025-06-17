using UnityEngine;
using System;
using UnityEngine.Pool;

public class WeaponEnemy : EnemyBase
{
    [Header("Range Attack")]
    [SerializeField] WeaponBase weapon;    
    bool IsCloseEnough => weapon?.Target;

    protected override void MoveCheck(float deltaTime)
    {
        if(IsCloseEnough)
        {
            Move(Vector2.zero);
            FlipSpriteCheck(LookDir * moveSpeed);
        }
        else
        {
            base.MoveCheck(deltaTime);
        }  
    }
}
