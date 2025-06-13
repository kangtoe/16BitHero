using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBase : CharacterBase
{
    #region References
    protected PlayerCharacter Player => GameManager.Instance.PlayerController;
    #endregion

    #region Spawn Settings
    [Header(" Spawn Sequence Related ")]
    [SerializeField] protected SpriteRenderer spawnIndicator;
    protected bool hasSpawned = true; // true for debug
    #endregion

    #region Attack Settings
    [Header(" Attack ")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackFrequency;
    protected float attackDelay;
    protected float attackCooldown;
    #endregion

    #region Effects
    [Header(" Effects ")]
    [SerializeField] protected ParticleSystem deathParticles;
    #endregion

    Vector2 LookDir => (Player.transform.position - transform.position).normalized;
    bool isPlayerInRange = false;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();
        health = maxHealth;    

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

        MoveToPlayer(Time.deltaTime);

        if(attackCooldown > 0f) attackCooldown -= Time.deltaTime;         
        else
        {
            if(attackCooldown <= 0f && isPlayerInRange)
            {  
                attackCooldown = attackDelay;
                Attack();
            }
        }                        
    }
    #endregion

    #region Spawn Logic
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
    #endregion

    #region Attack Logic
    protected bool isActive()
    {
        return spriteRenderer.enabled && hasSpawned;
    }

    protected virtual void Attack()
    {        
        Debug.Log("Attack");
        Player.TakeDamage(damage);
        Player.Knockback(LookDir);
    }
    #endregion

    #region Movement Logic
    protected void MoveToPlayer(float deltaTime)
    {
        if((Player.transform.position - transform.position).magnitude < 0.5f) return;

        Vector2 direction = (Player.transform.position - transform.position).normalized;        
        Move(direction * moveSpeed * deltaTime);
    }
    #endregion

    #region Death Logic
    protected override void Die()
    {
        onDeath?.Invoke(transform.position);
        if (deathParticles)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
    #endregion

    
    #region Trigger Events
    private void OnTriggerStay2D(Collider2D other)
    {
        isPlayerInRange = other.gameObject == Player.gameObject;        
    }
    #endregion
}
