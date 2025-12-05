using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyBase : CharacterBase
{
    protected PlayerCharacter Player => GameManager.Instance.Player;
    public bool IsActive => characterSprite.enabled && hasSpawned;

    [Header("Drop Settings")]
    [SerializeField] protected int coinDropAmount = 1;
    [SerializeField] protected int chestDropAmount = 0;
    [SerializeField] protected int potionDropAmount = 0;
    [SerializeField] protected int diamondDropAmount = 0;

    [Header("Spawn Sequence")]
    [SerializeField] protected SpriteRenderer spawnIndicator;
    protected bool hasSpawned = true; // true for debug

    [Header("Warning System")]
    [SerializeField] protected GameObject warningIndicator; // '!' 스프라이트
    [SerializeField] protected float warningDelay = 0.5f; // 경고를 표시할 쿨다운 임계값 (초)
    protected float warningTimer = 0f;
    protected bool isAttacking = false; // 경고 중이거나 발사 중

    [Header("Melee Attack")]
    [SerializeField] protected float meleeAttackRange = 1;
    [SerializeField] protected Vector2 meleeAttackAreaOffset;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float attackFrequency = 1f;
    protected float attackDelay;
    protected float attackCooldown;

    protected virtual Vector2 LookDir => (Player.transform.position - transform.position).normalized;

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

        MoveCheck(Time.deltaTime);
        AttackCheck(Time.deltaTime);
    }

    protected virtual void AttackCheck(float deltaTime)
    {
        if (attackCooldown > 0f) attackCooldown -= deltaTime;
        else
        {
            Collider2D coll = Physics2D.OverlapCircle(
                CenterPos + meleeAttackAreaOffset,
                meleeAttackRange,
                1 << Player.gameObject.layer);
            if (coll)
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

    protected virtual void MoveCheck(float deltaTime)
    {
        bool isCloseEnough = (Player.transform.position - transform.position).magnitude < 0.5f;
        if (isCloseEnough) Move(Vector2.zero);
        else
        {
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            Move(direction * moveSpeed * deltaTime);
        }
    }

    protected override void Die()
    {
        if (isDead) return;

        base.Die();
        DropManager.Instance.DropItem(transform.position, coinDropAmount, chestDropAmount, potionDropAmount, diamondDropAmount);
    }

    public void ForceDie(bool reward)
    {
        if (reward) Die();
        else base.Die();
    }

    override protected void SetCharacterSize()
    {
        base.SetCharacterSize();

        float _attackRange = meleeAttackRange;

        switch (characterSize)
        {
            case CharacterSize.Small:
                _attackRange = 0.3f;

                break;
            case CharacterSize.Medium:
                _attackRange = 0.45f;

                break;
            case CharacterSize.Large:
                _attackRange = 0.65f;

                break;
        }

        meleeAttackRange = _attackRange;
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
        characterSprite.enabled = visible;
        characterCollider.enabled = visible;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CenterPos + meleeAttackAreaOffset, meleeAttackRange);
    }
}
