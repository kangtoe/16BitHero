using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptable Objects/New Weapon Data", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public AudioClip AttackSound { get; private set; }
    [field: SerializeField] public WeaponType WeaponType { get; private set; }

    [field: HorizontalLine]
    [field: SerializeField] public int PurchasePrice { get; private set; }
    [field: SerializeField] public int RecyclePrice { get; private set; }

    [field: HorizontalLine]
    [field: SerializeField] public WeaponStat WeaponCommonStat { get; private set; }
    [field: SerializeField] public WeaponStat WeaponUpgradeStat { get; private set; }

    [field: HorizontalLine]
    [field: SerializeField] public WeaponRangeStat WeaponRangeStat { get; private set; }

    //     public Dictionary<Stat, float> BaseStats
    //     {
    //         get
    //         {
    //             return new Dictionary<Stat, float>
    //             {
    //                 {Stat.Attack,                   attack },
    //                 {Stat.AttackSpeed,              attackSpeed },
    //                 {Stat.CriticalChance,           criticalChance },
    //                 {Stat.CriticalPercent,          criticalPercent },
    //                 {Stat.Range,                    range }
    //             };
    //         }

    //         private set { }
    //     }

    //     public float GetStatValue(Stat stat)
    //     {
    //         foreach (KeyValuePair<Stat, float> kvp in BaseStats)
    //             if (kvp.Key == stat)
    //                 return kvp.Value;

    //         Debug.LogError("Stat not Found... This is not normal...");
    //         return 0;
    //     }
}
