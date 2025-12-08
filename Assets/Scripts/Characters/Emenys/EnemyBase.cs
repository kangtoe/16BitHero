using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] protected Text warningIndicator; // '!' 스프라이트
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

    [Header("Buff System")]
    protected bool isBuffed = false;
    protected float buffSpeedMultiplier = 1f;
    protected float buffDamageMultiplier = 1f;
    protected float buffDurationTimer = 0f;

    protected virtual void Start()
    {
        base.Start();
        CurrHealth = maxHealth;

        if (warningIndicator != null)
            warningIndicator.gameObject.SetActive(false);

        // 아웃라인 머티리얼 설정
        if (characterSprite != null)
        {
            OutlineManager.Instance.SetOutlineMaterial(characterSprite);
        }

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

        UpdateBuffState(Time.deltaTime);

        MoveCheck(Time.deltaTime);
        AttackCheck(Time.deltaTime);
    }

    // --- Buff System Methods ---

    public void ApplyBuff(float speedMult, float damageMult, float duration = 0.5f)
    {
        isBuffed = true;
        buffSpeedMultiplier = speedMult;
        buffDamageMultiplier = damageMult;
        buffDurationTimer = duration; // 타이머 갱신 (지속시간 연장)

        // 아웃라인 켜기
        if (characterSprite != null)
        {
            OutlineManager.Instance.SetOutline(characterSprite, true);
        }
    }

    public void RemoveBuff()
    {
        isBuffed = false;
        buffSpeedMultiplier = 1f;
        buffDamageMultiplier = 1f;
        buffDurationTimer = 0f;

        // 아웃라인 끄기
        if (characterSprite != null)
        {
            OutlineManager.Instance.SetOutline(characterSprite, false);
        }
    }

    protected void UpdateBuffState(float deltaTime)
    {
        if (isBuffed)
        {
            buffDurationTimer -= deltaTime;
            if (buffDurationTimer <= 0f)
            {
                RemoveBuff();
            }
        }
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

        // 버프 적용된 데미지 계산
        int finalDamage = Mathf.RoundToInt(damage * buffDamageMultiplier);

        Player.TakeDamage(hitPoint, finalDamage);
        //Player.Knockback(LookDir);
    }

    protected virtual void MoveCheck(float deltaTime)
    {
        bool isCloseEnough = (Player.transform.position - transform.position).magnitude < 0.5f;
        if (isCloseEnough) Move(Vector2.zero);
        else
        {
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            // 버프 적용된 이동 속도 계산
            float finalSpeed = moveSpeed * buffSpeedMultiplier;

            Move(direction * finalSpeed * deltaTime);
        }
    }

    protected void LookAtPlayer()
    {
        if (Player == null) return;
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        FlipSpriteCheck(direction);
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

    protected virtual IEnumerator WarningSequence(float duration)
    {
        return WarningSequence(duration, Color.red);
    }

    protected virtual IEnumerator WarningSequence(float duration, Color blinkColor)
    {
        if (warningIndicator != null)
        {
            warningIndicator.gameObject.SetActive(true);
            warningIndicator.color = blinkColor;
        }

        float elapsed = 0f;
        Color originalColor = characterSprite.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 기본 점멸 효과 (일정 속도)
            float t = Mathf.PingPong(elapsed * 10f, 1f);
            characterSprite.color = Color.Lerp(originalColor, blinkColor, t);
            yield return null;
        }

        characterSprite.color = originalColor;

        if (warningIndicator != null)
            warningIndicator.gameObject.SetActive(false);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(CenterPos + meleeAttackAreaOffset, meleeAttackRange);
    }
}
