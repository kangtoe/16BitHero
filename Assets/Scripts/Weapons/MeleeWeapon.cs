using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    [Header(" Elements ")]
    [SerializeField] CapsuleCollider2D hitCollider;

    [Header(" Settings ")]
    private List<CharacterBase> damagedCharacters = new List<CharacterBase>();

    void Start()
    {
        state = State.Idle;
    }

    protected override bool TryAttack()
    {
        if (!base.TryAttack()) return false;

        damagedCharacters.Clear();
        state = State.OnProcess;

        StartCoroutine(ThrustIE(range));
        return true;
    }

    IEnumerator ThrustIE(float thrustDistance = 1f, float thrustDuration = 0.1f, float returnDuration = 0.2f)
    {
        hitCollider.enabled = true;

        Vector3 start = hitCollider.transform.localPosition;
        Vector3 target = start + Vector3.right * thrustDistance;

        // 찌르기 전진
        bool forwardDone = false;
        LeanTween.moveLocal(hitCollider.gameObject, target, thrustDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                forwardDone = true;
                hitCollider.enabled = false;
            });

        yield return new WaitUntil(() => forwardDone);

        // 복귀
        LeanTween.moveLocal(hitCollider.gameObject, start, returnDuration)
            .setEase(LeanTweenType.easeInQuad)
            .setOnComplete(() => EndAttack());
    }

    private void EndAttack()
    {
        state = State.Idle;
        damagedCharacters.Clear();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetMask) == 0) return;
        CharacterBase target = collision.GetComponent<CharacterBase>();
        if (!target) return;
        if (damagedCharacters.Contains(target)) return;

        // 충돌 지점 계산
        Vector2 hitPoint = collision.ClosestPoint(hitCollider.bounds.center);

        int damage = GetDamage(out bool isCriticalHit);
        target.TakeDamage(hitPoint, damage, isCriticalHit);
        damagedCharacters.Add(target);

        if (knockback > 0f)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            target.Knockback(direction * knockback);
        }
    }
}
