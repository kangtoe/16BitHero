using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class DropItemBase : MonoBehaviour
{
    [Header("Drop Item Settings")]        
    [SerializeField] protected float pickupRadius = 1f;
    
    protected bool isCollected = false;
    
    // Action 이벤트 - 아이템 수집 시 호출될 이벤트
    public event Action<DropItemBase> OnItemCollectedEvent;
    
    // 가상 메서드 - 상속받는 클래스에서 선택적으로 오버라이드 가능
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (!player) return;

        CollectItem();
    }
    
    protected virtual void CollectItem()
    {
        if (isCollected) return;        
        isCollected = true;
        
        // Action 이벤트 호출
        OnItemCollectedEvent?.Invoke(this);
        
        // 아이템 수집 후 오브젝트 제거
        Destroy(gameObject);
    }
}