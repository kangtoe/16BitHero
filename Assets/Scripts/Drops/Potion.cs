using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : DropItemBase
{
    [Header("Potion")]
    [SerializeField] int healAmount = 10;

    protected override void CollectItem()
    {
        base.CollectItem();
        GameManager.Instance.Player.Heal(healAmount);
    }

}
