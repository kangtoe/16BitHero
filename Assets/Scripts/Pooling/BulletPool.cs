using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private ObjectPool<Bullet> bulletPool;

    private void Awake()
    {
        Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        void OnGetBullet(Bullet bullet)
        {
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

    private void OnDestroy()
    {
        bulletPool.Dispose();
    }
    
    public Bullet GetBullet()
    {
        return bulletPool.Get();
    }

    public void ReleaseBullet(Bullet bullet)
    {
        bulletPool.Release(bullet);
    }
} 