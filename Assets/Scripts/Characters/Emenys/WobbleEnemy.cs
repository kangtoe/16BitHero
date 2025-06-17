using UnityEngine;

public class WobbleEnemy : EnemyBase
{
    [Header("Wobble Settings")]        
    [SerializeField] float targetCircleRadius = 1f;

    [SerializeField] LayerMask wallLayer;
    
    protected override Vector2 LookDir => lookDir;
    Vector2 lookDir;

    protected override void Start()
    {
        base.Start();
        UpdateLookDir();
    }

    protected override void MoveCheck(float deltaTime)
    {
        if (!IsActive) return;

        Move(LookDir * moveSpeed * deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if((wallLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            UpdateLookDir();
        }
    }

    private void UpdateLookDir()
    {        
        Vector2 point = Random.insideUnitCircle.normalized * targetCircleRadius;
        lookDir = (point - (Vector2)transform.position).normalized;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // 움직임 방향 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, lookDir * 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.zero, targetCircleRadius);
    }
}
