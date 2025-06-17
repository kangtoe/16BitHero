using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

using Random = UnityEngine.Random;

public class DropManager : MonoSingleton<DropManager>
{
    [Header("Drop Settings")]
    [SerializeField] float dropForceMin = 2f;
    [SerializeField] float dropForceMax = 5f;

    [Header("Prefabs")]
    [SerializeField] DropItemBase coinPrefab;    
    [SerializeField] DropItemBase potionPrefab;
    [SerializeField] DropItemBase chestPrefab;
    [SerializeField] DropItemBase diamondPrefab;

    [Header("Premium Drop")]
    [SerializeField] float premiumDropChance = 0.1f;
    [SerializeField] float preminumDropMinDelay = 1f;
    float lastPremiumDropTime = 0f;

    static void InitPool<T>(T prefab, ObjectPool<T> pool) where T : MonoBehaviour
    {
        pool = new ObjectPool<T>(
            () => Instantiate(prefab).GetComponent<T>(),
            (obj) => obj.gameObject.SetActive(true),
            (obj) => obj.gameObject.SetActive(false),
            (obj) => Destroy(obj.gameObject)
        );
    }

    public void DropItem(Vector2 dropPosition, int coinAmount = 1, int chestAmount = 0, int potionAmount = 0, int diamondAmount = 0)
    {
        int totalAmount = coinAmount + chestAmount + potionAmount + diamondAmount;

        DropPrefab(coinPrefab, dropPosition, coinAmount, totalAmount);
        DropPrefab(chestPrefab, dropPosition, chestAmount, totalAmount);
        DropPrefab(potionPrefab, dropPosition, potionAmount, totalAmount);

        // Check additional diamond drop
        if (Time.time > lastPremiumDropTime + preminumDropMinDelay)
        {
            lastPremiumDropTime = Time.time;
            if (Random.Range(0f, 100f) <= premiumDropChance)
            {
                diamondAmount++;
            }
        }
        DropPrefab(diamondPrefab, dropPosition, diamondAmount, totalAmount);
    }

    void DropPrefab(DropItemBase prefab, Vector2 position, int amount, int totalAmount)
    {
        float forceScale = Mathf.Lerp(0.5f, 1f, Mathf.InverseLerp(1f, 10f, totalAmount));

        for (int i = 0; i < amount; i++)
        {
            DropItemBase item = Instantiate(prefab, position, Quaternion.identity, transform);
            
            if (totalAmount > 1)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float randomForce = Random.Range(dropForceMin, dropForceMax) * forceScale;
                item.Rigidbody.AddForce(randomDir * randomForce, ForceMode2D.Impulse);
            }
        }
    }

}
