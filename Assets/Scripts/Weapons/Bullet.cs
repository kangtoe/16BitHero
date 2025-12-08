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
    float knockback;
    bool isCriticalHit;
    float lifetime = 5f;

    [Header(" Flags ")]
    bool isReleased; // 총알이 풀에 반환되었는지 확인
    bool isHit; // 물리 시스템에서 Collider.enabled = false는 다음 프레임부터 적용되므로 충돌 처리를 위해 별도의 플래그 사용
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
    }

    public void Init(LayerMask targetMask, int damage, float knockback, Vector2 direction, float moveSpeed, bool isCriticalHit)
    {
        this.targetMask = targetMask;
        this.damage = damage;
        this.knockback = knockback;
        transform.right = direction;
        rig.velocity = direction * moveSpeed;
        this.isCriticalHit = isCriticalHit;

        this.isHit = false;
        gameObject.SetActive(true);
        projectileCollider.enabled = true;

        // lifetime 설정           
        Invoke(nameof(SelfDestroy), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHit) return;
        if (((1 << other.gameObject.layer) & targetMask) == 0) return;
        CharacterBase target = other.GetComponent<CharacterBase>();
        if(!target) return;                
        
        isHit = true;
        target.TakeDamage(transform.position, damage, isCriticalHit);
        if(knockback > 0f)
        {
            target.Knockback(transform.right * knockback);
        }
        projectileCollider.enabled = false;

        SelfDestroy();        
    }

    public void SelfDestroy()
    {        
        CancelInvoke(nameof(SelfDestroy));
        Destroy(gameObject);
    }
}
