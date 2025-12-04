using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class WeaponStat
{
    [field: SerializeField] public float Range { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public float AttackDelay { get; private set; }
    [field: SerializeField] public float Knockback { get; private set; }
    [field: SerializeField] public int CriticalChance { get; private set; }
    [field: SerializeField] public float CriticalDamageMultiplier { get; private set; }
}

[System.Serializable]
public class WeaponRangeStat
{
    [field: SerializeField] public Bullet BulletPrefab { get; private set; }
    [field: SerializeField] public float BulletSpeed { get; private set; }
    [field: SerializeField] public int MagazineSize { get; private set; }
    [field: SerializeField] public float ReloadTimeMultiplier { get; private set; } // AttackDelay * ReloadTimeMultiplier = ReloadTime
}
