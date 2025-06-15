using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header(" Elements ")]
    Rigidbody2D rig;
    Collider2D projectileCollider;
    ObjectPool<Bullet> pool;

    [Header(" Settings ")]
    LayerMask targetMask;    
    int damage;
    bool isCriticalHit;
    float lifetime = 5f;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
    }

    public void SetBulletPool(ObjectPool<Bullet> pool)
    {
        this.pool = pool;
    }

    public void Init(LayerMask targetMask, int damage, Vector2 direction, float moveSpeed, bool isCriticalHit)
    {
        this.targetMask = targetMask;
        this.damage = damage;
        transform.right = direction;
        rig.velocity = direction * moveSpeed;
        this.isCriticalHit = isCriticalHit;
        gameObject.SetActive(this);

        projectileCollider.enabled = true;

        // lifetime 설정           
        Invoke(nameof(OnHit), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetMask) == 0) return;
        CharacterBase target = other.GetComponent<CharacterBase>();
        if(!target) return;                
        
        target.TakeDamage(damage, isCriticalHit);
        projectileCollider.enabled = false;

        OnHit();        
    }

    public void OnHit()
    {        
        CancelInvoke(nameof(OnHit));     
        pool.Release(this);
    }
}
