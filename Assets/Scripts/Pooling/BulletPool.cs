using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private EnemyBullet bulletPrefab;
    private ObjectPool<EnemyBullet> bulletPool;

    private void Awake()
    {
        bulletPool = new ObjectPool<EnemyBullet>(
            CreateBullet,
            OnGetBullet,
            OnReleaseBullet,
            OnDestroyBullet
        );
    }

    private EnemyBullet CreateBullet()
    {
        EnemyBullet bullet = Instantiate(bulletPrefab);
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    private void OnGetBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(EnemyBullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    public EnemyBullet GetBullet()
    {
        return bulletPool.Get();
    }

    public void ReleaseBullet(EnemyBullet bullet)
    {
        bulletPool.Release(bullet);
    }

    private void OnDestroy()
    {
        bulletPool.Dispose();
    }
} 