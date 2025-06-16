using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBase : CharacterBase
{
    protected PlayerCharacter Player => GameManager.Instance.PlayerController;

    [Header(" Spawn Sequence Related ")]
    [SerializeField] protected SpriteRenderer spawnIndicator;
    protected bool hasSpawned = true; // true for debug

    [Header(" Attack ")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackFrequency;
    protected float attackDelay;
    protected float attackCooldown;

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
        if (!isActive()) return;            

        MoveToPlayer(Time.deltaTime, moveSpeed);

        if(attackCooldown > 0f) attackCooldown -= Time.deltaTime;         
        else
        {
            if(attackCooldown <= 0f && isPlayerInAttackArea)
            {  
                attackCooldown = attackDelay;
                Attack();
            }
        }                        
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

    protected bool isActive()
    {
        return spriteRenderer.enabled && hasSpawned;
    }

    protected virtual void Attack()
    {        
        Debug.Log("Attack");
        Player.TakeDamage(damage);
        //Player.Knockback(LookDir);
    }

    protected void MoveToPlayer(float deltaTime, float moveSpeed)
    {
        bool isCloseEnough = (Player.transform.position - transform.position).magnitude < 0.5f;
        if(isCloseEnough) moveSpeed = 0f;

        Vector2 direction = (Player.transform.position - transform.position).normalized;        
        Move(direction * moveSpeed * deltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        isPlayerInAttackArea = other.gameObject == Player.gameObject;        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Player.gameObject)
        {
            isPlayerInAttackArea = false;
        }
    }
}
