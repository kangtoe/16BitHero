using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour//, IPlayerStatsDependency
{
    [field: SerializeField] public WeaponDataSO WeaponData { get; private set; }

    [Header("Settings")]
    [SerializeField] protected float range;
    [SerializeField] protected LayerMask targetMask;

    [Header("Attack")]
    [SerializeField] protected int damage;
    [SerializeField] protected float attackDelay;

    protected float attackCooldown;

    [Header("Critical")]
    protected int criticalChance;
    protected float criticalPercent;

    [Header("Level")]
    [SerializeField] int level = 0;
    public int Level => level;

    [Header("Audio")]
    protected AudioSource audioSource;

    protected Vector3 startLocalPosition;
    protected Quaternion startLocalRotation;
    protected CharacterBase target;

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
        }

        transform.right = Vector3.Lerp(transform.right, targetVector, deltaTime * 10);
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

        if (Random.Range(0, 101) <= criticalChance)
        {
            isCriticalHit = true;
            return Mathf.RoundToInt(damage * criticalPercent);
        }

        return damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}