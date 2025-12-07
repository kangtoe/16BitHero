using UnityEngine;
using System.Linq;

public class HealerEnemy : EnemyBase
{
    [Header("Heal Settings")]
    [SerializeField] int healAmount = 5;
    [SerializeField] float healRange = 3f;
    [SerializeField] float healCooldown = 2f;
    [SerializeField] LayerMask enemyLayer; // Enemy layer 설정 필요

    private float currentHealCooldown = 0f;

    protected override void AttackCheck(float deltaTime)
    {
        if (currentHealCooldown > 0f)
        {
            currentHealCooldown -= deltaTime;
            return;
        }

        // 주변의 다친 적을 탐색
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, healRange);

        // 범위 내 모든 다친 적 회복
        foreach (var target in hitColliders)
        {
            HealTarget(target.GetComponent<EnemyBase>());
        }
        currentHealCooldown = healCooldown;
    }

    void HealTarget(EnemyBase target)
    {
        target.Heal(healAmount);

        // 시각 효과 (간단하게 초록색 선이나 입자 효과 등을 추가할 수 있음)
        // 여기서는 임시로 로그와 색상 변화로 표현
        StartCoroutine(ShowHealEffect(target.transform.position));
    }

    System.Collections.IEnumerator ShowHealEffect(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float duration = 0.2f;
        float elapsed = 0f;

        // 디버그용 라인 그리기 (실제 게임에서는 LineRenderer 권장)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Debug.DrawLine(startPos, targetPos, Color.green);
            yield return null;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
