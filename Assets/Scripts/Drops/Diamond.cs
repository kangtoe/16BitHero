using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : DropItemBase
{
    [Header("Diamond")]
    [SerializeField] int diamondValue = 1;

    protected override void CollectItem()
    {
        base.CollectItem();
        PlayerResManager.Instance.EarnDiamond(diamondValue);
    }
}
