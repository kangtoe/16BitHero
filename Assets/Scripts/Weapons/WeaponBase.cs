using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour//, IPlayerStatsDependency
{
    [field: SerializeField] public WeaponDataSO WeaponData { get; private set; }

    [Header("Settings")]
    [SerializeField] protected LayerMask targetMask;    
    [SerializeField] protected float range = 1f;
    public float Range => range;

    [Header("Attack")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float attackDelay = 1f;
    public float AttackDelay => attackDelay;
    protected float attackCooldown;

    [Header("Knockback")]
    [SerializeField] protected float knockback = 0f;

    [Header("Critical")]
    [SerializeField] protected int criticalChance = 0;
    [SerializeField] protected float criticalDamageMultiplier = 1.5f;

    [Header("Level")]
    [SerializeField] int level = 0;
    public int Level => level;

    [Header("Audio")]
    protected AudioSource audioSource;

    protected bool shouldFlip;
    public bool ShouldFlip => shouldFlip;

    protected enum State
    {
        Idle,
        OnProcess
    }
    protected State state;

    protected Vector3 startLocalPosition;
    protected Quaternion startLocalRotation;

    [SerializeField] protected CharacterBase owner;
    protected CharacterBase target;
    public CharacterBase Target => target;

    protected void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if(audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.clip = WeaponData?.AttackSound ?? null;
        }

        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
    }

    protected virtual void Update()
    {
        switch (state)
        {
            case State.Idle:       
                if(attackCooldown > 0) attackCooldown -= Time.deltaTime;
                target = GetClosestTarget(range);
                AutoAim(Time.deltaTime);                
                if(target && attackCooldown <= 0)
                {                                         
                    attackCooldown = attackDelay;
                    TryAttack();
                }                         
                break;

            case State.OnProcess:                
                break;
        }        
    }

    protected virtual void TryAttack()
    {
        if(state == State.OnProcess) return;
        PlayAttackSound();
    }

    protected void PlayAttackSound()
    {
        // if (!AudioManager.instance.IsSFXOn)
        //     return;

        // audioSource.pitch = Random.Range(.95f, 1.05f);
        // audioSource.Play();
    }

    protected void AutoAim(float deltaTime)
    {    
        Vector2 targetVector = Vector3.right;

        if (target)
        {
            targetVector = (target.CenterPos - (Vector2)transform.position).normalized;
            transform.right = targetVector;

            // 무기가 90도 이상 회전했을 때 스프라이트 반전 및 회전 보정
            float angle = Vector2.SignedAngle(Vector2.right, transform.right);
            shouldFlip = angle > 90 || angle < -90;

            // 회전 보정
            if (shouldFlip)
            {
                transform.rotation = Quaternion.Euler(0, 0, angle + 180);
            } 
        }
        else
        {
            shouldFlip = owner.IsFlipped;                        
            transform.right = Vector3.Lerp(transform.right, targetVector, deltaTime * 10);
        }        

        // 스프라이트 반전
        transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * (shouldFlip ? -1 : 1),
                transform.localScale.y,
                transform.localScale.z
            );

    }

    protected CharacterBase GetClosestTarget(float range)
    {
        CharacterBase closestTarget = null;

        Collider2D[] targetList = Physics2D.OverlapCircleAll(transform.position, range, targetMask);
        if (targetList.Length <= 0) return null;        
            
        float minDistance = range;
        for (int i = 0; i < targetList.Length; i++)
        {
            CharacterBase newTarget = targetList[i].GetComponent<CharacterBase>();
            if(!newTarget) continue;

            float distanceToTarget = Vector2.Distance(transform.position, newTarget.transform.position);

            if (distanceToTarget < minDistance)
            {
                closestTarget = newTarget;
                minDistance = distanceToTarget;
            }
        }

        return closestTarget;
    }

    protected int GetDamage(out bool isCriticalHit)
    {
        isCriticalHit = false;

        if (Random.Range(1, 101) <= criticalChance)
        {
            isCriticalHit = true;
            return Mathf.RoundToInt(damage * criticalDamageMultiplier);
        }

        return damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    
    // public override void UpdateStats(PlayerStatsManager playerStatsManager)
    // {
    //     ConfigureStats();

    //     damage = Mathf.RoundToInt(damage * (1 + playerStatsManager.GetStatValue(Stat.Attack) / 100));
    //     attackDelay /= 1 + (playerStatsManager.GetStatValue(Stat.AttackSpeed) / 100);

    //     criticalChance = Mathf.RoundToInt(criticalChance * (1 + playerStatsManager.GetStatValue(Stat.CriticalChance) / 100));
    //     criticalPercent += playerStatsManager.GetStatValue(Stat.CriticalPercent);

    //     range += playerStatsManager.GetStatValue(Stat.Range) / 10;
    // }
}