using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : DropItemBase
{
    [Header("Coin")]
    [SerializeField] int coinValue = 1;

    protected override void CollectItem()
    {        
        base.CollectItem();
        PlayerResManager.Instance.EarnGold(coinValue);
        GameManager.Instance.Player.PlayerLevel.GetXp(coinValue);
    }
}
