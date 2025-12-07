using UnityEngine;

public class SupplyEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        // 보급품은 움직이지 않으므로 Rigidbody 타입을 정적으로 바꾸거나 제약 조건을 걸어둠
        // 하지만 넉백 등을 고려하면 Dynamic을 유지하되 드래그를 높이거나 Move 로직을 안 쓰는 게 나음
        rig.mass = 100f; // 잘 안 밀리게
    }

    protected override void MoveCheck(float deltaTime)
    {
        // 움직이지 않음
    }

    protected override void AttackCheck(float deltaTime)
    {
        // 공격하지 않음
    }

    protected override void OnDrawGizmosSelected()
    {
        // 공격 범위 표시 안 함
    }
}
