using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class CharacterBase : MonoBehaviour
{
    public Vector2 CenterPos => characterCollider.bounds.center;

    [SerializeField] protected bool showDamageText = true;

    [Header("Components")]
    [SerializeField]protected Rigidbody2D rig;
    [SerializeField] protected Collider2D characterCollider;
    public Collider2D CharacterCollider => characterCollider;
    [SerializeField]protected Animator animator;
    [SerializeField]protected SpriteRenderer spriteRenderer;
    

    [Header("Health")]
    [SerializeField] protected int maxHealth = 10;
    protected int CurrHealth{
        get{
            return currHealth;
        }
        set{
            currHealth = value;
            if(healthBar) healthBar.fillAmount = (float)currHealth / maxHealth;
            if(healthText) healthText.text = $"{currHealth}/{maxHealth}";
        }
    }
    [SerializeField] protected int currHealth;
    [SerializeField] protected Image healthBar;
    [SerializeField] protected Text healthText;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 1f;
    protected Vector2 moveDirection;
    protected bool isFlipped;
    public bool IsFlipped => isFlipped;

    [Header("Knockback")]
    protected bool isKnockback = false;
    protected Coroutine knockbackCoroutine;
    protected Vector2 knockbackDirection;

    [Header("Effects")]
    [SerializeField] protected ParticleSystem deathParticles;

    [Header("Events")]
    public static Action<int, Vector2, bool> onDamageTaken;
    public static Action<Vector2> onDeath;

    protected virtual void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        CurrHealth = maxHealth;
    }

    public virtual void TakeDamage(Vector3 hitPoint, int damage, bool isCriticalHit = false)
    {
        int realDamage = Mathf.Min(damage, CurrHealth);
        CurrHealth -= realDamage;

        onDamageTaken?.Invoke(damage, transform.position, isCriticalHit);

        if(showDamageText && hitPoint != null)
        {
            Text3dMaker.Instance.MakeText(
                damage.ToString(), 
                hitPoint - Vector3.forward, // 표기 지점을 캐릭터 앞으로 조정
                isCriticalHit? Color.yellow : Color.white);
        }

        if (CurrHealth <= 0)
            Die();
    }

    public virtual void Heal(int amount)
    {
        CurrHealth += amount;
        if (CurrHealth > maxHealth)
            CurrHealth = maxHealth;
    }

    protected virtual void Die()
    {
        onDeath?.Invoke(transform.position);
        if (deathParticles)
            Instantiate(deathParticles, CenterPos, Quaternion.identity);

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
        
        rig.velocity = vel;
                
        animator?.SetBool("bMove", vel.magnitude > 0);

        FlipSpriteCheck(vel);
    }

    protected virtual void FlipSpriteCheck(Vector2 lookDir)
    {
        if (lookDir.x != 0)
        {
            isFlipped = lookDir.x < 0;
            spriteRenderer.transform.localScale = new Vector3(isFlipped ? -1 : 1, 1, 1);
        }
    }
} 