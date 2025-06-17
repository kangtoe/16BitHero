using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class DropItemBase : MonoBehaviour
{
    public Rigidbody2D Rigidbody => rig;
    [SerializeField] Rigidbody2D rig;

    [Header("Drop Item Settings")]        
    [SerializeField] bool isAutoCollectable = true;
    public bool IsAutoCollectable => isAutoCollectable;
    
    protected bool isCollected = false;        
    public event Action<DropItemBase> OnItemCollectedEvent;
    
    protected virtual void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

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
        
        OnItemCollectedEvent?.Invoke(this);
        
        Destroy(gameObject);
    }
}