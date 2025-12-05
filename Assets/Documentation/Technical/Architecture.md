# 16BitHero - 기술 아키텍처

## 적 시스템 (Enemy System)

### 현재 구조 (Phase 1)

**상속 기반 아키텍처**:
```
CharacterBase (캐릭터 공통)
  └─ EnemyBase (적 공통)
      ├─ WobbleEnemy (방랑형)
      ├─ RangedEnemy (원거리)
      │   └─ WanderingRangedEnemy (방랑 + 원거리)
      └─ ChargerEnemy (돌격형)
```

**장점**:
- ✅ 간단하고 직관적
- ✅ 빠른 프로토타이핑
- ✅ Unity Inspector 친화적

**단점**:
- ⚠️ 조합 증가 시 클래스 폭발
- ⚠️ 코드 중복 가능성

---

### 잠재적 리팩토링: 컴포지션 패턴

**시점**: 다음 조건 중 하나 충족 시
- [ ] 이동 패턴 5개 이상
- [ ] 공격 패턴 3개 이상
- [ ] 적 타입 조합 10개 이상
- [ ] 코드 중복 50줄 이상

**목표 구조**:
```
EnemyBase
├─ IMovementBehavior (인터페이스)
│   ├─ ChaseMovement (추적)
│   ├─ WanderMovement (방랑)
│   └─ ChargeMovement (돌진)
│
└─ IAttackBehavior (인터페이스)
    ├─ MeleeAttack (근접)
    ├─ RangedAttack (원거리)
    └─ ChargeAttack (돌진 공격)
```

**예시**:
```csharp
public class Enemy : EnemyBase
{
    [SerializeField] IMovementBehavior movement;
    [SerializeField] IAttackBehavior attack;
    
    // 조합 자유: 방랑 + 원거리, 추적 + 근접, 등등
}
```

**예상 작업 시간**: 4~6시간

---

## 무기 시스템 (Weapon System)

### 현재 구조

```
WeaponBase (추상 클래스)
├─ MeleeWeapon (근접 무기)
└─ RangeWeapon (원거리 무기)
```

**특징**:
- 플레이어 전용 시스템
- 적 시스템과 분리됨 (의도적)

---

## 경고 시스템 (Warning System)

### 구현 위치
- `EnemyBase.warningIndicator` (모든 적 공유)

### 사용처
- ✅ RangedEnemy: 공격 0.5초 전
- ✅ ChargerEnemy: 돌진 준비 1초 전
- ✅ WanderingRangedEnemy: 공격 0.5초 전

### 일관성
- '!' 스프라이트로 통일
- 색상 점멸 제거 (간소화)

---

## 성능 최적화

### 오브젝트 풀링
- ✅ Bullet: `ObjectPool<Bullet>` 사용
- ❌ Enemy: 미적용 (향후 고려)

### 향후 개선
- [ ] Enemy 오브젝트 풀링
- [ ] 화면 밖 적 비활성화
- [ ] LOD (Level of Detail) 시스템

---

## 버전 히스토리

### v0.1 - Phase 1 완료 (2025-12-05)
- ✅ 기본 적 타입 4종 구현
- ✅ 경고 시스템 통일
- ✅ 상속 기반 구조 확립

### 다음 버전 계획
- Phase 2: 추가 적 타입 (분열형, 버프형, 회복형)
- Phase 3: 특수 적 (폭발형, 보스)
