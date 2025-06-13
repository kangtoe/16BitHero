using UnityEngine;
using System;

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

    protected virtual void Move()
    {
        rig.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
        
        if (animator != null)
            animator.SetBool("bMove", rig.velocity.magnitude > 0);

        // 이동 방향에 따라 스프라이트 뒤집기
        if (rig.velocity.x != 0)
        {
            SetDirection(Mathf.Sign(rig.velocity.x));
        }
    }

    protected void SetDirection(float direction)
    {
        transform.localScale = new Vector3(direction, 1, 1);
    }

    protected void FlipTowards(Vector2 targetPosition)
    {
        float direction = Mathf.Sign(targetPosition.x - transform.position.x);
        SetDirection(direction);
    }
} 