using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header(" Elements ")]
    private Rigidbody2D rig;
    private Collider2D projectileCollider;
    private BulletPool pool;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;
    private int damage;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
    }

    public void SetBulletPool(BulletPool pool)
    {
        this.pool = pool;
    }

    public void Init(int damage, Vector2 direction)
    {
        this.damage = damage;
        transform.right = direction;
        rig.velocity = direction * moveSpeed;
        gameObject.SetActive(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var Player = GameManager.Instance.PlayerController;
        if(other.gameObject == Player.gameObject)
        {
            LeanTween.cancel(gameObject);

            Player.TakeDamage(damage);
            this.projectileCollider.enabled = false;

            OnHit();
        }
    }

    public void OnHit()
    {        
        pool.ReleaseBullet(this);
        gameObject.SetActive(false);
    }
}
