using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour//, IPlayerStatsDependency
{
    [Header("Elements")]
    [SerializeField] private MobileJoystick playerJoystick;
    private Rigidbody2D rig;
    private Animator animator;

    [Header(" Settings ")]
    [SerializeField] private float baseMoveSpeed;
    private float moveSpeed;

    void Start()
    {
        moveSpeed = baseMoveSpeed;
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rig.velocity = Vector2.right;
    }

    private void FixedUpdate()
    {
        rig.velocity = playerJoystick.GetMoveVector() * moveSpeed * Time.fixedDeltaTime;
        animator.SetBool("bMove", rig.velocity.magnitude > 0);
        
        // 이동 방향에 따라 스프라이트 뒤집기
        if (rig.velocity.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(rig.velocity.x), 1, 1);
        }        
    }

    // public void UpdateStats(PlayerStatsManager playerStatsManager)
    // {
    //     float moveSpeedPercent = playerStatsManager.GetStatValue(Stat.MoveSpeed) / 100;
    //     moveSpeed = baseMoveSpeed * (1 + moveSpeedPercent);
    // }
}