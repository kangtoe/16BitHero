using UnityEngine;

[RequireComponent(typeof(PlayerLevel))]
public class PlayerCharacter : CharacterBase
{
    [Header("Elements")]
    [SerializeField] MobileJoystick playerJoystick;

    [Header("Collect")]
    [SerializeField] float autoCollectRadius = 1f;
    [SerializeField] float autoCollectPower = 10f;

    [Header("Level")]
    [SerializeField] PlayerLevel playerLevel;
    public PlayerLevel PlayerLevel => playerLevel;

    void Update()
    {
        Debug.Log(currHealth);
    }

    private void FixedUpdate()
    {
        // 이동 처리
        Move(playerJoystick.GetMoveVector().normalized * moveSpeed * Time.fixedDeltaTime);

        // 아이템 끌어당기기 처리
        PullInNearbyItems(Time.fixedDeltaTime);
    }

    private void PullInNearbyItems(float deltaTime)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(CenterPos, autoCollectRadius);
        foreach (var col in colliders)
        {
            DropItemBase item = col.GetComponent<DropItemBase>();
            if (!item || !item.IsAutoCollectable) continue;

            Vector3 direction = (CenterPos - (Vector2)col.transform.position).normalized;
            item.transform.position += direction * autoCollectPower * deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(CenterPos, autoCollectRadius);
    }

    public override void TakeDamage(Vector3 hitPoint, int damage, bool isCriticalHit = false)
    {
        int dodge = PlayerStatsManager.CurrPlayerStat.GetStatValue(PlayerStatType.Dodge);
        if (dodge > Random.Range(1, 101))
        {
            if (hitPoint != null)
            {
                Text3dMaker.Instance.MakeText(
                    "DODGE",
                    hitPoint - Vector3.forward, // 표기 지점을 캐릭터 앞으로 조정
                    Color.white);
            }
            return;
        }

        base.TakeDamage(hitPoint, damage, isCriticalHit);


    }


}
