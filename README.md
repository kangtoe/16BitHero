# 16BitHero

### TODO
- 빌드시 최적화
  - 개발용 빌드: Scripting Backend -> Mono
  - 배포용 빌드: Scripting Backend -> IL2CPP
- 기능 구현
  - 스폰 관련
    - (디버그용) 적 종류 별 스폰 버튼 자동배치 로직 작성
    - 스폰 한계를 정하고, 한계 이상 스폰 시 기존 적을 보상없이 사망하게 하는 로직 추가
    - 적 무리 스폰 로직 구현
  - 적 관련
    - 원거리 적 공격 전 '!'를 머리 위에 띄워서 플레이어에게 경고하는 로직 추가
    - 적 사망 시, 추가 적 출현 구현
    - 적 종류 다양화
      - 돌격형 적
      - 버프형 적
      - 회복형 적
      - 보급품 (브로타토 나무)
      - 보스

