using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    [Header(" Elements ")]
    private Rigidbody2D rig;
    private Collider2D collider;
    private RangeEnemy rangeEnemy;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;
    private int damage;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        LeanTween.delayedCall(gameObject, 5, () => rangeEnemy.BulletPool.ReleaseBullet(this));
    }

    public void Configure(RangeEnemy rangeEnemy)
    {
        this.rangeEnemy = rangeEnemy;
    }

    public void Shoot(int damage, Vector2 direction)
    {
        this.damage = damage;

        transform.right = direction;
        rig.velocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // if(collider.TryGetComponent(out Player player))
        // {
        //     LeanTween.cancel(gameObject);

        //     player.TakeDamage(damage);
        //     this.collider.enabled = false;

        //     rangeEnemy.ReleaseBullet(this);
        // }
    }

    public void Reload()
    {
        rig.velocity = Vector2.zero;
        collider.enabled = true;
    }
}
