using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class RangeWeapon : WeaponBase
{
    [Header(" Elements ")]
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform firePoint;

    [Header(" Pooling ")]
    ObjectPool<Bullet> bulletPool;

    [Header(" Settings ")]
    [SerializeField] float bulletSpeed = 1f;

    [Header(" Actions ")]
    public static Action onBulletShot;

    // Start is called before the first frame update
    void Start()
    {
        Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.SetBulletPool(bulletPool);
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        void OnGetBullet(Bullet bullet)
        {
            //bullet.Reload();            
            bullet.gameObject.SetActive(true);
        }

        void OnReleaseBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        void OnDestroyBullet(Bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        bulletPool = new ObjectPool<Bullet>(
            CreateBullet,
            OnGetBullet,
            OnReleaseBullet,
            OnDestroyBullet
        );
    }

    protected override void TryAttack()
    {
        base.TryAttack();

        Shoot();
    }

    void Shoot()
    {
        int damage = GetDamage(out bool isCriticalHit);

        Bullet bullet = bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.Init(targetMask, damage, firePoint.right, bulletSpeed, isCriticalHit);        

        onBulletShot?.Invoke();        
    }

    void Reload()
    {

    }
}
