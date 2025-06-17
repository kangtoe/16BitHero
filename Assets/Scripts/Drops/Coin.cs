using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropItemBase
{
    [Header("Coin")]
    [SerializeField] int coinValue = 1;

    protected override void CollectItem()
    {
        Debug.Log($"Coin collected: {coinValue}");
        base.CollectItem();
        //GameManager.Instance.AddCoin(coinValue);
    }
}
