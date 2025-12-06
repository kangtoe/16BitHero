using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class CharacterBase : MonoBehaviour
{
    protected enum CharacterSize
    {
        Medium = 0,
        Small,
        Large
    }
    [SerializeField] protected CharacterSize characterSize = CharacterSize.Medium;

    public Vector2 CenterPos => characterCollider.bounds.center;

    [SerializeField] protected bool showDamageText = true;

    protected bool isDead = false;

    [Header("Components")]
    [SerializeField] protected Rigidbody2D rig;
    [SerializeField] protected CapsuleCollider2D characterCollider;
    public CapsuleCollider2D CharacterCollider => characterCollider;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer characterSprite;
    //[SerializeField] protected SpriteRenderer[] outlineSprites;


    [Header("Health")]
    [SerializeField] protected int maxHealth = 10;
    protected int? CurrHealth
    {
        get
        {
            if (currHealth == null) currHealth = maxHealth;
            return currHealth;
        }
        set
        {
            currHealth = value;
            if (healthBar) healthBar.fillAmount = (float)currHealth / maxHealth;
            if (healthText) healthText.text = $"{currHealth}/{maxHealth}";
        }
    }
    [SerializeField] protected int? currHealth = null;
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
        characterCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        characterSprite = GetComponentInChildren<SpriteRenderer>();
        //outlineSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        //OutlineManager.Instance.SetOutlineMaterial(outlineSprites);
        //OutlineManager.Instance.SetOutline(outlineSprites, true); // debug code

        SetCharacterSize();
    }

    void OnValidate()
    {
        SetCharacterSize();
    }

    virtual protected void SetCharacterSize()
    {
        Vector2 uiPos = Vector2.zero;
        bool overheadUI = healthBar?.canvas?.renderMode == RenderMode.WorldSpace;
        if (overheadUI) uiPos = healthBar.canvas.GetComponent<RectTransform>().anchoredPosition;

        Vector2 offset = characterCollider.offset;
        Vector2 size = characterCollider.size;

        switch (characterSize)
        {
            case CharacterSize.Small:
                uiPos = new Vector2(0, 1f);

                offset = new Vector2(0f, 0.3f);
                size = new Vector2(0.6f, 0.7f);

                break;
            case CharacterSize.Medium:
                uiPos = new Vector2(0, 1.5f);

                offset = new Vector2(0f, 0.5f);
                size = new Vector2(0.7f, 1.0f);

                break;
            case CharacterSize.Large:
                uiPos = new Vector2(0, 2.0f);

                offset = new Vector2(0f, 0.75f);
                size = new Vector2(1.2f, 1.6f);

                break;
        }
        if (overheadUI) healthBar.canvas.GetComponent<RectTransform>().anchoredPosition = uiPos;
        characterCollider.size = size;
        characterCollider.offset = offset;
    }

    public virtual void TakeDamage(Vector3 hitPoint, int damage, bool isCriticalHit = false)
    {
        int realDamage = Mathf.Min(damage, CurrHealth.Value);
        CurrHealth -= realDamage;

        onDamageTaken?.Invoke(damage, transform.position, isCriticalHit);

        if (showDamageText && hitPoint != null)
        {
            Text3dMaker.Instance.MakeText(
                damage.ToString(),
                hitPoint - Vector3.forward, // 표기 지점을 캐릭터 앞으로 조정
                isCriticalHit ? Color.yellow : Color.white);
        }

        if (CurrHealth <= 0)
            Die();
    }

    void InitHealth(int _maxHealth)
    {
        maxHealth = _maxHealth;
        CurrHealth = maxHealth;
    }

    public void AdjustMaxHealth(int new_maxHealth)
    {
        // 초기화 되지 않은 상태에서 최초 실행 시
        if (currHealth == null)
        {
            InitHealth(new_maxHealth);
            return;
        }

        int adjust = new_maxHealth - maxHealth;
        maxHealth += adjust;

        if (maxHealth < new_maxHealth)
        {
            Heal(adjust);
        }
        else if (maxHealth > new_maxHealth)
        {
            if (CurrHealth > maxHealth) CurrHealth = maxHealth;
        }
    }

    public virtual void Heal(int amount)
    {
        CurrHealth += amount;
        if (CurrHealth > maxHealth)
            CurrHealth = maxHealth;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

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
            characterSprite.transform.localScale = new Vector3(isFlipped ? -1 : 1, 1, 1);
        }
    }
}