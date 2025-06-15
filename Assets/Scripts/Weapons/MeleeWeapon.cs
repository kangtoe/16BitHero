using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    enum State
    {
        Idle,
        OnAttack
    }

    State state;

    [Header(" Elements ")]
    [SerializeField] CapsuleCollider2D hitCollider;

    [Header(" Settings ")]
    private List<CharacterBase> damagedCharacters = new List<CharacterBase>();
    
    void Start()
    {
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:       
                target = GetClosestTarget(range);
                if(target) AutoAim();                     

                if (attackCooldown <= 0)
                {
                    attackCooldown = attackDelay;
                    StartAttack();
                }
                else attackCooldown -= Time.deltaTime;                
                break;

            case State.OnAttack:                
                break;
        }

        if(target) Debug.DrawLine(transform.position, target.transform.position, Color.red);
    }

    void StartAttack()
    {
        if(state == State.OnAttack) return;
        damagedCharacters.Clear();

        hitCollider.enabled = true;
        state = State.OnAttack;
                
        PlayAttackSound();
        StartCoroutine(ThrustIE(range));
    }

     IEnumerator ThrustIE(float thrustDistance = 1f,float thrustDuration = 0.1f, float returnDuration = 0.2f)
        {            
            Vector3 start = startLocalPosition;
            Vector3 target = startLocalPosition + Vector3.right * thrustDistance;

            // 찌르기 전진
            bool forwardDone = false;
            LeanTween.moveLocal(hitCollider.gameObject, target, thrustDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => forwardDone = true);

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
        hitCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterBase target = collision.GetComponent<CharacterBase>();
        if(!target) return;
        if (damagedCharacters.Contains(target)) return;

        int damage = GetDamage(out bool isCriticalHit);
        target.TakeDamage(damage, isCriticalHit);
        damagedCharacters.Add(target);
    }    
}
