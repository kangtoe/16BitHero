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
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header(" Pooling ")]
    ObjectPool<Bullet> bulletPool;

    [Header(" Settings ")]
    [SerializeField] float bulletSpeed = 1f;

    [Header(" Actions ")]
    public static Action onBulletShot;

    [Header(" Reload ")]
    [SerializeField] float reloadTimeMultiplier = 1f;
    float currentReloadTime = 0f;

    [Header(" Magazine ")]
    [SerializeField] int magazineSize = 10;
    int currentMagazine = 0;

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

    protected override void Update()
    {
        base.Update();

        if (currentReloadTime > 0)
        {
            currentReloadTime -= Time.deltaTime;
            if (currentReloadTime <= 0)
            {
                currentReloadTime = 0;
                EndReload();
            }
        }
    }

    protected override bool TryAttack()
    {
        if (!base.TryAttack()) return false;

        if (currentReloadTime > 0) return false;

        if (currentMagazine <= 0)
        {
            StartReload();
            return false;
        }

        currentMagazine--;
        Shoot();

        return true;
    }

    void Shoot()
    {
        int damage = GetDamage(out bool isCriticalHit);

        Bullet bullet = bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.Init(
            targetMask,
            damage,
            knockback,
            (ShouldFlip ? -1 : 1) * firePoint.right,
            bulletSpeed,
            isCriticalHit
        );

        onBulletShot?.Invoke();
        StartCoroutine(RecoilIE());
    }

    void StartReload()
    {
        currentReloadTime = attackDelay * reloadTimeMultiplier;
    }

    void EndReload()
    {
        currentMagazine = magazineSize;
    }

    IEnumerator RecoilIE()
    {
        float recoilDistance = 0.1f;
        float recoilDuration = Mathf.Min(0.05f, attackDelay / 4);
        float recoilReturnDuration = Mathf.Min(0.1f, attackDelay / 2);

        Vector3 start = spriteRenderer.transform.localPosition;
        Vector3 target = start - Vector3.right * recoilDistance;

        // 후퇴
        bool recoilDone = false;
        LeanTween.moveLocal(spriteRenderer.gameObject, target, recoilDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => recoilDone = true);

        yield return new WaitUntil(() => recoilDone);

        // 복귀
        LeanTween.moveLocal(spriteRenderer.gameObject, start, recoilReturnDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() => EndAttack());
    }
    
    void EndAttack()
    {
        state = State.Idle;
    }
}
