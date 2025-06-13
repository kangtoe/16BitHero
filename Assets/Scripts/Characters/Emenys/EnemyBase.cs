using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyBase : CharacterBase
{
    protected PlayerCharacter Player => GameManager.Instance.PlayerController;

    [Header(" Spawn Sequence Related ")]
    [SerializeField] protected SpriteRenderer spawnIndicator;
    protected bool hasSpawned;

    [Header(" Effects ")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header(" Attack ")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackFrequency;
    [SerializeField] protected Collider2D attackTrigger;
    protected float attackDelay;
    protected float attackTimer;
    protected bool isPlayerInRange;

    [Header(" DEBUG ")]
    [SerializeField] protected bool gizmos;

    // Start is called before the first frame update
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
        StartSpawnSequence();
    }

    protected virtual void Update()
    {
        if (!CanAttack())
            return;

        if(attackTimer < attackDelay) 
            attackTimer += Time.deltaTime;        

        FollowPlayer();
        FlipTowards(Player.CenterPos);
    }

    // Update is called once per frame
    protected bool CanAttack()
    {
        return spriteRenderer.enabled;
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

    protected virtual void TryAttack()
    {
        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        Player.TakeDamage(damage);
    }

    protected override void Die()
    {
        onDeath?.Invoke(transform.position);
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    protected void FollowPlayer()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        moveDirection = direction;
        Move();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other == Player.gameObject)
            TryAttack();
    }
}
