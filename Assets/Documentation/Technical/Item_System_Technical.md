# Item System - 기술 문서

## 📋 문서 정보
- **작성일**: 2025-12-09
- **버전**: 0.1
- **대상**: 프로그래머
- **관련 기획 문서**: [Item_System.md](../Systems/Item_System.md)

---

## 🏗️ 시스템 아키텍처

### 전체 구조 다이어그램

```
ItemDataSO (추상 베이스 클래스)
├── DropItemDataSO (드롭 아이템)
├── WeaponDataSO (무기)
└── PassiveItemDataSO (패시브 아이템)

PlayerInventory (관리 클래스)
├── WeaponManager (무기 관리)
└── PassiveItemManager (패시브 아이템 관리)

ShopManager (상점 시스템)
└── ItemDatabase (아이템 데이터베이스)
```

---

## 🗂️ 데이터 구조 설계

### 1. 아이템 공통 베이스 (ItemDataSO)

```csharp
using UnityEngine;

/// <summary>
/// 모든 아이템의 베이스 ScriptableObject
/// </summary>
public abstract class ItemDataSO : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("아이템 이름 (UI 표시용)")]
    public string itemName;

    [Tooltip("아이템 설명 (UI 표시용)")]
    [TextArea(3, 5)]
    public string description;

    [Tooltip("아이템 아이콘 스프라이트")]
    public Sprite icon;

    [Header("등급 및 가격")]
    [Tooltip("아이템 등급 (Common/Uncommon/Rare/Legendary)")]
    public ItemTier tier = ItemTier.Common;

    [Tooltip("기본 가격 (골드)")]
    public int basePrice = 10;

    [Header("카테고리")]
    [Tooltip("아이템 카테고리 (자동 설정됨)")]
    public ItemCategory category;

    /// <summary>
    /// 아이템 등급에 따른 색상 반환
    /// </summary>
    public Color GetTierColor()
    {
        return tier switch
        {
            ItemTier.Common => new Color(0.7f, 0.7f, 0.7f),      // 회색
            ItemTier.Uncommon => new Color(0.2f, 0.8f, 0.2f),    // 초록
            ItemTier.Rare => new Color(0.2f, 0.4f, 1f),          // 파랑
            ItemTier.Legendary => new Color(0.8f, 0.2f, 1f),     // 보라
            _ => Color.white
        };
    }

    /// <summary>
    /// 웨이브 수에 따른 가격 조정 (선택적)
    /// </summary>
    public virtual int GetAdjustedPrice(int currentWave)
    {
        return basePrice;
    }
}
```

### 2. 드롭 아이템 데이터

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "DropItem_", menuName = "Items/Drop Item")]
public class DropItemDataSO : ItemDataSO
{
    [Header("드롭 설정")]
    [Tooltip("드롭 확률 (0~100)")]
    [Range(0f, 100f)]
    public float dropChance = 50f;

    [Tooltip("자동 수집 가능 여부")]
    public bool isAutoCollectable = true;

    [Header("효과")]
    [Tooltip("드롭 아이템 효과 타입")]
    public DropItemEffectType effectType;

    [Tooltip("효과 수치 (체력 회복량, 골드 수량 등)")]
    public int effectValue = 1;

    [Tooltip("효과 지속 시간 (버프용, 초 단위)")]
    public float effectDuration = 0f;

    [Header("프리팹")]
    [Tooltip("드롭 아이템 프리팹 (DropItemBase 상속)")]
    public DropItemBase prefab;

    private void OnValidate()
    {
        category = ItemCategory.Drop;
    }

    /// <summary>
    /// Luck 스탯을 고려한 실제 드롭 확률 계산
    /// </summary>
    public float GetAdjustedDropChance(int luckStat)
    {
        return dropChance * (1 + luckStat / 100f);
    }
}
```

### 3. 무기 아이템 데이터

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_", menuName = "Items/Weapon")]
public class WeaponDataSO : ItemDataSO
{
    [Header("무기 타입")]
    [Tooltip("근접(Melee) 또는 원거리(Range)")]
    public WeaponType weaponType;

    [Header("공격 속성")]
    [Tooltip("기본 공격력")]
    public int damage = 10;

    [Tooltip("공격 간격 (초)")]
    public float attackDelay = 1f;

    [Tooltip("공격 범위")]
    public float range = 2f;

    [Tooltip("넉백 강도")]
    public float knockback = 0f;

    [Header("크리티컬")]
    [Tooltip("크리티컬 확률 (%)")]
    [Range(0, 100)]
    public int criticalChance = 5;

    [Tooltip("크리티컬 데미지 배율")]
    public float criticalMultiplier = 1.5f;

    [Header("비주얼 & 오디오")]
    [Tooltip("공격 사운드")]
    public AudioClip attackSound;

    [Tooltip("무기 스프라이트 (선택적)")]
    public Sprite weaponSprite;

    [Header("투사체 (원거리 무기용)")]
    [Tooltip("투사체 프리팹 (원거리 무기만)")]
    public GameObject projectilePrefab;

    private void OnValidate()
    {
        category = ItemCategory.Weapon;
    }

    /// <summary>
    /// 등급에 따른 스탯 배율 적용
    /// </summary>
    public int GetScaledDamage()
    {
        float multiplier = tier switch
        {
            ItemTier.Common => 1.0f,
            ItemTier.Uncommon => 1.3f,
            ItemTier.Rare => 1.7f,
            ItemTier.Legendary => 2.5f,
            _ => 1.0f
        };
        return Mathf.RoundToInt(damage * multiplier);
    }
}
```

### 4. 패시브 아이템 데이터

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItem_", menuName = "Items/Passive Item")]
public class PassiveItemDataSO : ItemDataSO
{
    [Header("스탯 효과")]
    [Tooltip("적용할 스탯 변경 목록")]
    public List<StatModifier> statModifiers = new List<StatModifier>();

    [Header("특수 효과")]
    [Tooltip("특수 효과 보유 여부")]
    public bool hasSpecialEffect = false;

    [Tooltip("특수 효과 타입")]
    public PassiveEffectType effectType = PassiveEffectType.None;

    [Tooltip("특수 효과 수치 (효과마다 다름)")]
    public float effectValue = 0f;

    [Tooltip("특수 효과 상세 설명")]
    [TextArea(2, 4)]
    public string effectDescription;

    private void OnValidate()
    {
        category = ItemCategory.Passive;

        // 특수 효과가 None이면 hasSpecialEffect를 false로
        if (effectType == PassiveEffectType.None)
        {
            hasSpecialEffect = false;
        }
    }

    /// <summary>
    /// 플레이어 스탯에 이 아이템의 효과를 적용
    /// </summary>
    public void ApplyToPlayer(PlayerStatsManager statsManager)
    {
        foreach (var modifier in statModifiers)
        {
            statsManager.AddStatModifier(modifier);
        }
    }
}

/// <summary>
/// 스탯 변경 정보
/// </summary>
[Serializable]
public struct StatModifier
{
    [Tooltip("영향을 주는 스탯")]
    public PlayerStatType statType;

    [Tooltip("증가/감소 값")]
    public int value;

    [Tooltip("% 단위 여부 (true면 배율, false면 고정값)")]
    public bool isPercentage;

    public StatModifier(PlayerStatType type, int val, bool isPercent = false)
    {
        statType = type;
        value = val;
        isPercentage = isPercent;
    }
}
```

### 5. 열거형 정의

```csharp
/// <summary>
/// 아이템 카테고리
/// </summary>
public enum ItemCategory
{
    Drop,        // 드롭 아이템
    Weapon,      // 무기
    Passive,     // 패시브 아이템
    Consumable   // 소모품 (선택적)
}

/// <summary>
/// 아이템 등급
/// </summary>
public enum ItemTier
{
    Common = 1,      // 일반 (회색)
    Uncommon = 2,    // 고급 (초록)
    Rare = 3,        // 희귀 (파랑)
    Legendary = 4    // 전설 (보라)
}

/// <summary>
/// 드롭 아이템 효과 타입
/// </summary>
public enum DropItemEffectType
{
    Heal,           // 체력 회복
    Gold,           // 골드 획득
    Diamond,        // 다이아몬드 획득
    Experience,     // 경험치 획득
    Buff,           // 버프 효과
    Magnet,         // 자석 효과 (수집 범위 증가)
    Bomb            // 폭탄 효과 (전체 데미지)
}

/// <summary>
/// 패시브 아이템 특수 효과 타입
/// </summary>
public enum PassiveEffectType
{
    None,                 // 특수 효과 없음 (순수 스탯만)
    Thorns,              // 가시 갑옷 (데미지 반사)
    Regeneration,        // 재생 (시간당 체력 회복)
    DoubleStrike,        // 이중 타격 (확률로 2번 공격)
    ExplosiveBullets,    // 폭발 탄환 (범위 데미지)
    Berserker,           // 버서커 (체력 낮을 때 강화)
    Blink,               // 순간 이동 (회피 시)
    WeakeningAura,       // 적 약화 (주변 적 약화)
    HealingAura          // 회복 오라 (지속 회복)
}

/// <summary>
/// 무기 타입
/// </summary>
public enum WeaponType
{
    Melee,   // 근접
    Range    // 원거리
}
```

---

## 📦 인벤토리 시스템

### PlayerInventory 클래스

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 무기 및 아이템 인벤토리 관리
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("무기 인벤토리")]
    [SerializeField] List<WeaponBase> equippedWeapons = new List<WeaponBase>();
    public List<WeaponBase> EquippedWeapons => equippedWeapons;

    [SerializeField] int maxWeaponSlots = 6;
    public int MaxWeaponSlots => maxWeaponSlots;

    [Header("패시브 아이템")]
    [SerializeField] List<PassiveItemDataSO> passiveItems = new List<PassiveItemDataSO>();
    public List<PassiveItemDataSO> PassiveItems => passiveItems;

    [Header("참조")]
    [SerializeField] PlayerStatsManager statsManager;
    [SerializeField] WeaponManager weaponManager;

    // 이벤트
    public event Action<WeaponBase> OnWeaponAdded;
    public event Action<WeaponBase> OnWeaponRemoved;
    public event Action<PassiveItemDataSO> OnPassiveItemAdded;

    private void Awake()
    {
        if (!statsManager) statsManager = GetComponent<PlayerStatsManager>();
        if (!weaponManager) weaponManager = GetComponent<WeaponManager>();
    }

    #region 무기 관리

    /// <summary>
    /// 무기를 추가할 수 있는지 확인
    /// </summary>
    public bool CanAddWeapon()
    {
        return equippedWeapons.Count < maxWeaponSlots;
    }

    /// <summary>
    /// 무기 추가
    /// </summary>
    public bool AddWeapon(WeaponDataSO weaponData)
    {
        if (!CanAddWeapon())
        {
            Debug.LogWarning("[PlayerInventory] 무기 슬롯이 가득 찼습니다.");
            return false;
        }

        // WeaponManager를 통해 무기 생성 및 장착
        WeaponBase weapon = weaponManager.AddWeapon(weaponData);
        if (weapon != null)
        {
            equippedWeapons.Add(weapon);
            OnWeaponAdded?.Invoke(weapon);
            Debug.Log($"[PlayerInventory] 무기 추가: {weaponData.itemName}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 무기 제거
    /// </summary>
    public bool RemoveWeapon(WeaponBase weapon)
    {
        if (equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Remove(weapon);
            weaponManager.RemoveWeapon(weapon);
            OnWeaponRemoved?.Invoke(weapon);
            Debug.Log($"[PlayerInventory] 무기 제거: {weapon.WeaponData.itemName}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 특정 인덱스의 무기 교체
    /// </summary>
    public bool ReplaceWeapon(int index, WeaponDataSO newWeaponData)
    {
        if (index < 0 || index >= equippedWeapons.Count)
        {
            Debug.LogWarning("[PlayerInventory] 잘못된 무기 인덱스입니다.");
            return false;
        }

        WeaponBase oldWeapon = equippedWeapons[index];
        RemoveWeapon(oldWeapon);
        return AddWeapon(newWeaponData);
    }

    #endregion

    #region 패시브 아이템 관리

    /// <summary>
    /// 패시브 아이템 추가 (개수 제한 없음)
    /// </summary>
    public void AddPassiveItem(PassiveItemDataSO item)
    {
        passiveItems.Add(item);

        // 스탯 적용
        item.ApplyToPlayer(statsManager);

        // 특수 효과 적용
        if (item.hasSpecialEffect)
        {
            ApplySpecialEffect(item);
        }

        OnPassiveItemAdded?.Invoke(item);
        Debug.Log($"[PlayerInventory] 패시브 아이템 추가: {item.itemName}");
    }

    /// <summary>
    /// 특정 패시브 아이템 보유 여부 확인
    /// </summary>
    public bool HasPassiveItem(PassiveItemDataSO item)
    {
        return passiveItems.Contains(item);
    }

    /// <summary>
    /// 특정 효과를 가진 아이템 개수 확인
    /// </summary>
    public int GetPassiveItemCount(PassiveEffectType effectType)
    {
        int count = 0;
        foreach (var item in passiveItems)
        {
            if (item.effectType == effectType)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 특수 효과 적용
    /// </summary>
    private void ApplySpecialEffect(PassiveItemDataSO item)
    {
        switch (item.effectType)
        {
            case PassiveEffectType.Thorns:
                // CharacterBase에 Thorns 효과 추가
                Debug.Log($"[PlayerInventory] Thorns 효과 적용: {item.effectValue}%");
                break;

            case PassiveEffectType.Regeneration:
                // CharacterBase에 Regeneration 효과 추가
                Debug.Log($"[PlayerInventory] Regeneration 효과 적용: {item.effectValue}/s");
                break;

            // 다른 특수 효과들...
            default:
                Debug.Log($"[PlayerInventory] 특수 효과 적용: {item.effectType}");
                break;
        }
    }

    #endregion

    #region 저장/로드 (선택적)

    /// <summary>
    /// 인벤토리 데이터 저장
    /// </summary>
    public void SaveInventory()
    {
        // PlayerPrefs 또는 JSON 파일로 저장
        // TODO: 구현 필요
    }

    /// <summary>
    /// 인벤토리 데이터 로드
    /// </summary>
    public void LoadInventory()
    {
        // PlayerPrefs 또는 JSON 파일에서 로드
        // TODO: 구현 필요
    }

    #endregion
}
```

---

## 🛒 상점 시스템 연동

### ItemDatabase 클래스

```csharp
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 게임 내 모든 아이템 데이터베이스
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Header("무기")]
    public List<WeaponDataSO> weapons = new List<WeaponDataSO>();

    [Header("패시브 아이템")]
    public List<PassiveItemDataSO> passiveItems = new List<PassiveItemDataSO>();

    [Header("드롭 아이템")]
    public List<DropItemDataSO> dropItems = new List<DropItemDataSO>();

    /// <summary>
    /// 특정 등급의 무기 목록 반환
    /// </summary>
    public List<WeaponDataSO> GetWeaponsByTier(ItemTier tier)
    {
        return weapons.Where(w => w.tier == tier).ToList();
    }

    /// <summary>
    /// 특정 등급의 패시브 아이템 목록 반환
    /// </summary>
    public List<PassiveItemDataSO> GetPassiveItemsByTier(ItemTier tier)
    {
        return passiveItems.Where(p => p.tier == tier).ToList();
    }

    /// <summary>
    /// 모든 구매 가능 아이템 반환 (무기 + 패시브)
    /// </summary>
    public List<ItemDataSO> GetAllShopItems()
    {
        List<ItemDataSO> allItems = new List<ItemDataSO>();
        allItems.AddRange(weapons);
        allItems.AddRange(passiveItems);
        return allItems;
    }
}
```

### ShopManager 아이템 생성 로직

```csharp
/// <summary>
/// 웨이브 수와 Luck 스탯을 고려하여 랜덤 아이템 생성
/// </summary>
public ItemDataSO GenerateRandomItem(int currentWave, int luckStat)
{
    // 1. 웨이브에 따른 등급 확률 계산
    ItemTier tier = SelectRandomTier(currentWave, luckStat);

    // 2. 무기 vs 패시브 선택 (50:50)
    bool isWeapon = Random.value > 0.5f;

    // 3. 해당 등급의 아이템 목록 가져오기
    List<ItemDataSO> candidates = isWeapon
        ? itemDatabase.GetWeaponsByTier(tier).Cast<ItemDataSO>().ToList()
        : itemDatabase.GetPassiveItemsByTier(tier).Cast<ItemDataSO>().ToList();

    // 4. 랜덤 선택
    if (candidates.Count > 0)
    {
        return candidates[Random.Range(0, candidates.Count)];
    }

    Debug.LogWarning($"[ShopManager] {tier} 등급의 아이템이 없습니다.");
    return null;
}

/// <summary>
/// 웨이브와 Luck을 고려한 랜덤 등급 선택
/// </summary>
private ItemTier SelectRandomTier(int wave, int luck)
{
    // 웨이브별 기본 확률
    Dictionary<ItemTier, float> baseProbabilities = GetTierProbabilities(wave);

    // Luck 보정 (상위 등급 확률 증가)
    float luckMultiplier = 1f + (luck / 100f);
    baseProbabilities[ItemTier.Rare] *= luckMultiplier;
    baseProbabilities[ItemTier.Legendary] *= luckMultiplier;

    // 확률 정규화
    float total = baseProbabilities.Values.Sum();
    foreach (var key in baseProbabilities.Keys.ToList())
    {
        baseProbabilities[key] /= total;
    }

    // 가중치 랜덤 선택
    float roll = Random.value;
    float cumulative = 0f;

    foreach (var kvp in baseProbabilities)
    {
        cumulative += kvp.Value;
        if (roll <= cumulative)
            return kvp.Key;
    }

    return ItemTier.Common;
}

/// <summary>
/// 웨이브별 등급 확률표
/// </summary>
private Dictionary<ItemTier, float> GetTierProbabilities(int wave)
{
    if (wave <= 3)
    {
        return new Dictionary<ItemTier, float>
        {
            { ItemTier.Common, 0.70f },
            { ItemTier.Uncommon, 0.25f },
            { ItemTier.Rare, 0.05f },
            { ItemTier.Legendary, 0.0f }
        };
    }
    else if (wave <= 7)
    {
        return new Dictionary<ItemTier, float>
        {
            { ItemTier.Common, 0.50f },
            { ItemTier.Uncommon, 0.35f },
            { ItemTier.Rare, 0.13f },
            { ItemTier.Legendary, 0.02f }
        };
    }
    else if (wave <= 15)
    {
        return new Dictionary<ItemTier, float>
        {
            { ItemTier.Common, 0.30f },
            { ItemTier.Uncommon, 0.40f },
            { ItemTier.Rare, 0.22f },
            { ItemTier.Legendary, 0.08f }
        };
    }
    else
    {
        return new Dictionary<ItemTier, float>
        {
            { ItemTier.Common, 0.15f },
            { ItemTier.Uncommon, 0.35f },
            { ItemTier.Rare, 0.35f },
            { ItemTier.Legendary, 0.15f }
        };
    }
}
```

---

## ✅ 구현 체크리스트

### Phase 1: 데이터 구조 구현
- [ ] `ItemDataSO` 베이스 클래스 생성
- [ ] `ItemCategory`, `ItemTier` enum 정의
- [ ] `DropItemDataSO` 생성
- [ ] `WeaponDataSO` 수정 (ItemDataSO 상속)
- [ ] `PassiveItemDataSO` 생성
- [ ] `StatModifier` 구조체 정의
- [ ] `PassiveEffectType` enum 정의

### Phase 2: 인벤토리 시스템
- [ ] `PlayerInventory` 클래스 생성
- [ ] 무기 추가/제거/교체 로직
- [ ] 패시브 아이템 추가 로직
- [ ] 스탯 적용 로직 (PlayerStatsManager 연동)

### Phase 3: 아이템 데이터베이스
- [ ] `ItemDatabase` ScriptableObject 생성
- [ ] 샘플 무기 데이터 3~5개 생성
- [ ] 샘플 패시브 아이템 10개 생성
- [ ] 드롭 아이템 데이터 생성

### Phase 4: 상점 연동
- [ ] ShopManager에 ItemDatabase 연동
- [ ] 랜덤 아이템 생성 로직
- [ ] 등급별 확률 시스템
- [ ] Luck 스탯 영향 적용
- [ ] 구매 로직 (무기 슬롯 체크)

### Phase 5: 특수 효과 구현
- [ ] Thorns (가시 갑옷) 효과
- [ ] Regeneration (재생) 효과
- [ ] DoubleStrike (이중 타격) 효과
- [ ] ExplosiveBullets (폭발 탄환) 효과
- [ ] Berserker (버서커) 효과

### Phase 6: UI 연동
- [ ] 패시브 아이템 인벤토리 UI
- [ ] 아이템 툴팁 시스템
- [ ] 등급별 색상 표시

---

## 🧪 테스트 가이드

### 단위 테스트

```csharp
[Test]
public void Test_ItemTierColor()
{
    ItemDataSO item = CreateTestItem(ItemTier.Legendary);
    Color color = item.GetTierColor();
    Assert.AreEqual(new Color(0.8f, 0.2f, 1f), color);
}

[Test]
public void Test_WeaponInventory_MaxSlots()
{
    PlayerInventory inventory = CreateTestInventory();

    for (int i = 0; i < 6; i++)
    {
        Assert.IsTrue(inventory.CanAddWeapon());
        inventory.AddWeapon(CreateTestWeapon());
    }

    Assert.IsFalse(inventory.CanAddWeapon());
}

[Test]
public void Test_PassiveItem_StatApplication()
{
    PlayerStatsManager statsManager = CreateTestStatsManager();
    PassiveItemDataSO item = CreateTestPassiveItem();

    int initialDamage = statsManager.GetStatValue(PlayerStatType.Damage);
    item.ApplyToPlayer(statsManager);
    int finalDamage = statsManager.GetStatValue(PlayerStatType.Damage);

    Assert.Greater(finalDamage, initialDamage);
}
```

---

## 📝 구현 참고사항

### ScriptableObject 생성 경로
```
Assets/Datas/Items/
├── Weapons/
│   ├── Common/
│   ├── Uncommon/
│   ├── Rare/
│   └── Legendary/
├── PassiveItems/
│   ├── Common/
│   ├── Uncommon/
│   ├── Rare/
│   └── Legendary/
└── DropItems/
```

### 네이밍 컨벤션
- **무기**: `Weapon_Sword_Common`, `Weapon_Bow_Rare`
- **패시브**: `Passive_SteelArmor_Common`, `Passive_CriticalRing_Rare`
- **드롭**: `Drop_Coin`, `Drop_Potion`

---

## 🔗 관련 문서

- [Item_System.md](../Systems/Item_System.md) - 아이템 시스템 기획 문서
- [Shop_System.md](../Systems/Shop_System.md) - 상점 시스템 기획
- [Architecture.md](Architecture.md) - 전체 아키텍처 문서

---

## 📞 문서 관리

### 업데이트 이력
- 2025-12-09: 초안 작성

### 다음 작업
1. ItemDataSO 및 하위 클래스 구현
2. PlayerInventory 구현
3. ItemDatabase 생성 및 샘플 데이터 추가
