# Item & Stat System - 기술 설계 문서

## 📋 문서 정보
- **작성일**: 2025-12-09
- **버전**: 1.1
- **대상**: 프로그래머
- **관련 기획 문서**:
  - [Item_System.md](../Systems/Item_System.md)
  - [Stat_System.md](../Systems/Stat_System.md)
  - [Shop_System.md](../Systems/Shop_System.md)

---

## 🎯 핵심 설계 결정 사항

### 최종 확정된 사항 (2025-12-09)
1. ✅ **무기 장착 제한**: 6개
2. ✅ **장신구 장착 제한**: 무제한 (Brotato 방식)
3. ✅ **무기도 플레이어 스탯 변경**: 긍정/부정 효과 보유
4. ✅ **중복 구매 허용**: 같은 아이템 여러 개 구매 가능
5. ✅ **Phase 1 범위**: 스탯 증가만 (특수 효과는 Phase 2)
6. ✅ **Armor 계산**: `받는 데미지 = 원본 × (1 - Armor/(Armor+100))`
7. ✅ **데이터 구조**: ScriptableObject 방식

---

## 🏗️ 시스템 아키텍처

### 전체 구조 다이어그램

```
┌─────────────────────────────────────────────────────┐
│                    Shop System                      │
│  ┌──────────────┐         ┌──────────────┐         │
│  │ ShopManager  │────────>│ ItemDatabase │         │
│  └──────────────┘         └──────────────┘         │
│         │                        │                  │
│         │ 구매                   │ 아이템 풀        │
│         ▼                        ▼                  │
└─────────────────────────────────────────────────────┘
         │
         │ 아이템 획득
         ▼
┌─────────────────────────────────────────────────────┐
│                 Inventory System                     │
│  ┌──────────────────┐    ┌──────────────────┐      │
│  │ WeaponInventory  │    │ AccessoryInventory│      │
│  │   (최대 6개)     │    │    (무제한)       │      │
│  └──────────────────┘    └──────────────────┘      │
│         │                        │                  │
│         │ 스탯 적용              │ 스탯 적용        │
│         └────────────┬───────────┘                  │
└──────────────────────┼──────────────────────────────┘
                       ▼
         ┌─────────────────────────┐
         │   PlayerStatsManager    │
         │  - 스탯 계산 및 적용    │
         │  - 아이템 효과 누적     │
         └─────────────────────────┘
                       │
                       ▼
         ┌─────────────────────────┐
         │    PlayerCharacter      │
         │  - 최종 스탯 사용       │
         └─────────────────────────┘
```

---

## 📊 데이터 구조 설계

### 클래스 다이어그램

```
ItemDataSO (abstract)
├─ itemName: string
├─ description: string
├─ icon: Sprite
├─ tier: ItemTier
├─ basePrice: int
├─ positiveEffects: PlayerStat
├─ negativeEffects: PlayerStat
└─ Category: ItemCategory (abstract)

WeaponDataSO : ItemDataSO
├─ Category = Weapon
├─ Sprite: Sprite
├─ AttackSound: AudioClip
├─ WeaponType: WeaponType
├─ WeaponCommonStat: WeaponStat
├─ WeaponUpgradeStat: WeaponStat
├─ WeaponRangeStat: WeaponRangeStat
├─ PurchasePrice: int
└─ RecyclePrice: int

AccessoryDataSO : ItemDataSO
├─ Category = Accessory
├─ hasSpecialEffect: bool
├─ specialEffectType: SpecialEffectType
└─ specialEffectValue: float

ItemDatabase : ScriptableObject
├─ allWeapons: List<WeaponDataSO>
├─ allAccessories: List<AccessoryDataSO>
├─ GetWeaponsByTier(tier): List<WeaponDataSO>
├─ GetAccessoriesByTier(tier): List<AccessoryDataSO>
└─ GetAllShopItems(): List<ItemDataSO>
```

### Enum 정의

```csharp
public enum ItemCategory { Weapon, Accessory }
public enum ItemTier { Common = 1, Uncommon = 2, Rare = 3, Legendary = 4 }
public enum SpecialEffectType { None, Thorns, Regeneration, DoubleStrike, ... }
```

---

## 🎒 인벤토리 시스템

### InventoryManager

**책임**: 무기와 장신구 인벤토리 통합 관리

**주요 메서드**:
```csharp
// 무기 관리
bool CanAddWeapon()
bool AddWeapon(WeaponDataSO weaponData)
bool RemoveWeapon(WeaponDataSO weaponData)

// 장신구 관리
void AddAccessory(AccessoryDataSO accessoryData)
int GetAccessoryCount(AccessoryDataSO accessoryData)

// 내부 메서드
void ApplyItemStats(ItemDataSO item)
void RemoveItemStats(ItemDataSO item)
```

**이벤트**:
- `OnWeaponAdded`, `OnWeaponRemoved`
- `OnAccessoryAdded`
- `OnInventoryChanged`

---

## 📊 PlayerStatsManager 확장

### 핵심 변경사항

**추가 필드**:
```csharp
private Dictionary<ItemDataSO, PlayerStat> itemStatModifiers;
```

**주요 메서드**:
```csharp
void AddItemStats(ItemDataSO item, PlayerStat stats)
void RemoveItemStats(ItemDataSO item)
void UpdateStats() // 모든 아이템 스탯 누적 계산
int GetStatValue(PlayerStatType statType)
List<ItemDataSO> GetAllAppliedItems()
```

**스탯 계산 흐름**:
```
1. 모든 아이템 스탯 누적 → totalItemStats
2. 기본 스탯 + totalItemStats → sumStats
3. sumStats × 배율 → currPlayerStat
4. currPlayerStat × 0.01 → 최종 스탯
```

---

## 🛒 ShopManager

### 책임
- 웨이브별 아이템 생성
- 확률 기반 등급 선택 (Luck 스탯 영향)
- 구매 처리
- 리롤 기능

### 주요 메서드
```csharp
void OpenShop(int wave)
ItemDataSO GenerateRandomItem(int wave, int luck)
ItemTier SelectRandomTier(int wave, int luck)
bool TryPurchaseItem(int slotIndex)
bool TryReroll()
int CalculateRerollCost() // 5 + (웨이브×2) + (리롤횟수×5)
void ToggleLock(int slotIndex)
```

### 웨이브별 등급 확률

| 웨이브 | Common | Uncommon | Rare | Legendary |
|:---:|:---:|:---:|:---:|:---:|
| 1~3 | 70% | 25% | 5% | 0% |
| 4~7 | 50% | 35% | 13% | 2% |
| 8~15 | 30% | 40% | 22% | 8% |
| 16+ | 15% | 35% | 35% | 15% |

**Luck 보정**: `상위 등급 확률 × (1 + Luck/100)`

---

## 🔄 시퀀스 다이어그램

### 아이템 구매 흐름

```
User → ShopUI: 아이템 클릭
ShopUI → ShopManager: TryPurchaseItem(slotIndex)
ShopManager → PlayerResManager: 골드 확인
ShopManager → InventoryManager: 무기 슬롯 확인 (무기인 경우)
InventoryManager → InventoryManager: AddWeapon() or AddAccessory()
InventoryManager → PlayerStatsManager: AddItemStats(item, stats)
PlayerStatsManager → PlayerStatsManager: UpdateStats()
PlayerStatsManager → PlayerCharacter: 최종 스탯 적용
PlayerStatsManager → StatUI: UI 업데이트
ShopManager → PlayerResManager: 골드 차감
ShopManager → ShopUI: 슬롯 품절 표시
```

### 무기 스탯 적용 예시

```
검 (Sword)
├─ 무기 스탯: 공격력 15, 속도 1.0s
├─ 긍정 효과: 공격속도 +5%
└─ 부정 효과: 이동속도 -5%

구매 시:
1. InventoryManager.AddWeapon(sword)
2. PlayerStatsManager.AddItemStats(sword, +5% 공격속도)
3. PlayerStatsManager.AddItemStats(sword, -5% 이동속도)
4. PlayerStatsManager.UpdateStats() 호출
5. 최종 플레이어 스탯 계산 및 적용
```

---

## ⚠️ 주의사항 및 제약조건

### 1. 무기 장착 제한
- **제약**: 최대 6개
- **처리**: `InventoryManager.CanAddWeapon()` 체크 필수
- **초과 시**: 교체 UI 표시 (Phase 2)

### 2. 중복 구매 처리
- **허용**: 같은 아이템 여러 개 구매 가능
- **구현**: `Dictionary`에서 기존 값에 누적
- **예**: 강철 갑옷 2개 = 방어력 +10

### 3. 스탯 제거
- **무기 제거 시**: `RemoveItemStats()` 호출 필수
- **주의**: Dictionary에서 완전히 제거
- **재계산**: 제거 후 `UpdateStats()` 자동 호출

### 4. 부정 효과 처리
- **저장 시**: positiveEffects, negativeEffects 별도 저장
- **적용 시**: negativeEffects에 -1 곱해서 적용
- **예**: `-5% 이동속도` → `AddItemStats(item, stat × -1)`

### 5. Phase 1 제약
- **특수 효과**: 구현하지 않음 (hasSpecialEffect = false)
- **무기 합성**: 구현하지 않음
- **무기 판매**: 구현하지 않음

---

## ✅ 구현 체크리스트

### Phase 1: 데이터 구조
- [ ] `ItemDataSO` 베이스 클래스
- [ ] `ItemCategory`, `ItemTier`, `SpecialEffectType` enum
- [ ] `WeaponDataSO` 확장 (ItemDataSO 상속)
- [ ] `AccessoryDataSO` 생성
- [ ] `ItemDatabase` ScriptableObject

### Phase 2: 매니저 시스템
- [ ] `InventoryManager` 생성
- [ ] `PlayerStatsManager` 확장 (Dictionary 추가)
- [ ] `ShopManager` 생성

### Phase 3: 샘플 데이터
- [ ] 무기 데이터 5개 (각 등급)
- [ ] 장신구 데이터 10개 (각 등급)
- [ ] ItemDatabase 에셋 생성

### Phase 4: UI 연동
- [ ] ShopSlotUI 컴포넌트
- [ ] ShopUI 매니저
- [ ] 인벤토리 UI

### Phase 5: 테스트
- [ ] 구매 흐름 테스트
- [ ] 스탯 적용 검증
- [ ] 중복 구매 테스트
- [ ] 무기 제거 테스트

---

## 📁 폴더 구조

```
Assets/
├── Scripts/
│   ├── ScriptableObjects/
│   │   ├── ItemDataSO.cs (베이스)
│   │   ├── WeaponDataSO.cs (확장)
│   │   ├── AccessoryDataSO.cs (신규)
│   │   └── ItemDatabase.cs (신규)
│   ├── Managers/
│   │   ├── InventoryManager.cs (신규)
│   │   ├── PlayerStatsManager.cs (확장)
│   │   └── ShopManager.cs (신규)
│   └── UIs/
│       ├── ShopSlotUI.cs (신규)
│       └── ShopUI.cs (신규)
├── Datas/
│   └── Items/
│       ├── ItemDatabase.asset
│       ├── Weapons/
│       │   ├── Common/
│       │   ├── Uncommon/
│       │   ├── Rare/
│       │   └── Legendary/
│       └── Accessories/
│           ├── Common/
│           ├── Uncommon/
│           ├── Rare/
│           └── Legendary/
```

---

## 🔧 구현 시 참고사항

### 1. ItemDataSO 생성 위치
- **기존**: `Assets/Scripts/ScriptableObjects/`
- **신규 파일**: 같은 폴더에 `ItemDataSO.cs`, `AccessoryDataSO.cs` 추가
- **WeaponDataSO**: 기존 파일 수정

### 2. PlayerStatsManager 수정
- **기존 코드**: `additionalPlayerStats` 필드 (미사용)
- **신규 로직**: `Dictionary<ItemDataSO, PlayerStat>` 사용
- **주의**: 기존 `UpdateStats()` 로직 보존

### 3. 네이밍 컨벤션
- **무기**: `Weapon_Sword_Common.asset`
- **장신구**: `Accessory_SteelArmor_Common.asset`
- **데이터베이스**: `ItemDatabase.asset`

### 4. 코드 예시 위치
- **전체 코드**: Git 커밋 히스토리 또는 별도 파일 참조
- **이 문서**: 구조와 핵심 메서드 시그니처만 포함

---

## 🔗 관련 문서

- [Item_System.md](../Systems/Item_System.md) - 아이템 시스템 기획
- [Stat_System.md](../Systems/Stat_System.md) - 스탯 시스템 기획
- [Shop_System.md](../Systems/Shop_System.md) - 상점 시스템 기획

---

## 📝 문서 관리

### 업데이트 이력
- 2025-12-09: v1.1 - 코드 예시 제거, 구조 및 핵심만 간결하게 정리
- 2025-12-09: v1.0 - 초안 작성

### 다음 단계
1. Phase 1 데이터 구조 구현
2. 샘플 아이템 3~5개 생성
3. 구매 흐름 테스트
