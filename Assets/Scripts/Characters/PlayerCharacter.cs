using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : CharacterBase
{
    [Header("Elements")]
    [SerializeField] MobileJoystick playerJoystick;

    [Header("Collect")]
    [SerializeField] float collectRadius = 1f;    
    [SerializeField] float collectPower = 10f;    

    private void FixedUpdate()
    {
        // 이동 처리
        Move(playerJoystick.GetMoveVector().normalized * moveSpeed * Time.fixedDeltaTime);

        // 아이템 끌어당기기 처리
        PullInNearbyItems(Time.fixedDeltaTime);
    }

    private void PullInNearbyItems(float deltaTime)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(CenterPos, collectRadius);
        foreach (var col in colliders)
        {
            DropItemBase item = col.GetComponent<DropItemBase>();
            if (!item) continue;
            
            Vector3 direction = (CenterPos - (Vector2)col.transform.position).normalized;
            item.transform.position += direction * collectPower * deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(CenterPos, collectRadius);
    }
}
