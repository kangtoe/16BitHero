using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header(" Components ")]
    protected Rigidbody2D rig;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Collider2D characterCollider;

    [Header(" Health ")]
    [SerializeField] protected int maxHealth = 10;
    protected int health;

    [Header(" Movement ")]
    [SerializeField] protected float moveSpeed = 1f;
    protected Vector2 moveDirection;

    [Header(" Actions ")]
    public static Action<int, Vector2, bool> onDamageTaken;
    public static Action<Vector2> onDeath;

    #region Knockback
    protected bool isKnockback = false;
    protected Coroutine knockbackCoroutine;
    protected Vector2 knockbackDirection;
    #endregion

    protected virtual void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterCollider = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(int damage, bool isCriticalHit = false)
    {
        int realDamage = Mathf.Min(damage, health);
        health -= realDamage;

        onDamageTaken?.Invoke(damage, transform.position, isCriticalHit);

        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        onDeath?.Invoke(transform.position);
        Destroy(gameObject);
    }

    public virtual void Knockback(Vector2 direction)
    {
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);
            
        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(direction));
    }

    protected virtual IEnumerator KnockbackCoroutine(Vector2 direction, float knockbackPower = 10f, float knockbackDuration = 0.2f)
    {
        isKnockback = true;
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float power = knockbackPower * (1f - (elapsed / knockbackDuration));
            rig.velocity = direction * power;
            yield return null;
        }

        rig.velocity = Vector2.zero;
        isKnockback = false;
    }

    protected virtual void Move(Vector2 vel)
    {         
        if (isKnockback) return;
        
        rig.MovePosition(rig.position + vel);
                
        animator?.SetBool("bMove", vel.magnitude > 0);

        // 이동 방향에 따라 스프라이트 뒤집기
        if (vel.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
        }
    }
} 