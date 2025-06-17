using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBase : CharacterBase
{
    protected PlayerCharacter Player => GameManager.Instance.Player;
    public bool IsActive => spriteRenderer.enabled && hasSpawned;

    [Header("Drop Settings")]
    [SerializeField] protected int coinDropAmount = 1;
    [SerializeField] protected int chestDropAmount = 0;
    [SerializeField] protected int potionDropAmount = 0;
    [SerializeField] protected int diamondDropAmount = 0;

    [Header("Spawn Sequence")]
    [SerializeField] protected SpriteRenderer spawnIndicator;
    protected bool hasSpawned = true; // true for debug

    [Header("Melee Attack")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackFrequency;
    protected float attackDelay;
    protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected Vector2 attackAreaOffset;

    protected Vector2 LookDir => (Player.transform.position - transform.position).normalized;
    bool isPlayerInAttackArea = false;

    protected override void Start()
    {
        base.Start();
        CurrHealth = maxHealth;    

        if (Player == null)
        {
            Debug.LogWarning("No player found, Auto-destroying...");
            Destroy(gameObject);
        }

        attackDelay = 1f / attackFrequency;
        attackCooldown = 0f;
        //StartSpawnSequence();
    }

    protected virtual void Update()
    {
        if (!IsActive) return;            

        MoveToPlayer(Time.deltaTime, moveSpeed);
        AttackCheck(Time.deltaTime);
    }

    protected virtual void AttackCheck(float deltaTime)
    {
        if(attackCooldown > 0f) attackCooldown -= deltaTime;         
        else
        {
            Collider2D coll = Physics2D.OverlapCircle(
                CenterPos + attackAreaOffset, 
                attackRange, 
                1 << Player.gameObject.layer);
            if(coll)
            {
                Attack();
                attackCooldown = attackDelay;
            }         
        }   
    }

    protected virtual void Attack()
    {                
        Vector2 hitPoint = characterCollider.ClosestPoint(Player.CharacterCollider.bounds.center);
        Player.TakeDamage(hitPoint, damage);
        //Player.Knockback(LookDir);
    }

    protected void MoveToPlayer(float deltaTime, float moveSpeed)
    {
        bool isCloseEnough = (Player.transform.position - transform.position).magnitude < 0.5f;
        if(isCloseEnough) moveSpeed = 0f;

        Vector2 direction = (Player.transform.position - transform.position).normalized;        
        Move(direction * moveSpeed * deltaTime);
    }

    protected override void Die()
    {
        base.Die();
        DropManager.Instance.DropItem(transform.position, coinDropAmount, chestDropAmount, potionDropAmount, diamondDropAmount);
    }

    private void StartSpawnSequence()
    {
        SetVisibility(false);

        // Scale up & down the spawn indicator
        Vector3 targetScale = spawnIndicator.transform.localScale * 1.2f;
        LeanTween.scale(spawnIndicator.gameObject, targetScale, .3f)
            .setLoopPingPong(4)
            .setOnComplete(SpawnSequenceCompleted);
    }

    private void SpawnSequenceCompleted()
    {
        SetVisibility(true);
        hasSpawned = true;
    }

    private void SetVisibility(bool visible)
    {
        spriteRenderer.enabled = visible;
        characterCollider.enabled = visible;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CenterPos + attackAreaOffset, attackRange);
    }
}
