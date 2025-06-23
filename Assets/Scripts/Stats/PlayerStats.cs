using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum PlayerStatType
{
    Damage,
    MeleeAttack,
    RangeAttack,
    AttackSpeed,
    CriticalChance,
    CriticalMultiplier,
    MoveSpeed,
    MaxHealth,
    Range,
    HealthRecoverySpeed,
    Armor,
    Luck,
    Dodge,
    LifeSteal
}

[Serializable]
public struct StatPair
{
    public string desc;
    public PlayerStatType statType;
    public int value;
}

[System.Serializable]
public class PlayerStat
{
    public PlayerStat()
    {
        CheckStat();
    }

    PlayerStat(List<StatPair> _stats)
    {
        stats = _stats;
        CheckStat();
    }

    [SerializeField]
    private List<StatPair> stats = new List<StatPair>();
    public List<StatPair> Stats => stats;

    public PlayerStat GetAddedStats(PlayerStat other)
    {
        CheckStat();
        other.CheckStat();

        List<StatPair> _stats = new List<StatPair>();
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            // other에서 같은 statType의 값을 찾아 더함
            var otherStat = other.stats.FirstOrDefault(s => s.statType == stat.statType);
            stat.value += otherStat.value;
            _stats.Add(stat);
        }
        return new PlayerStat(_stats);
    }
    public PlayerStat GetAddedStats(int added)
    {
        CheckStat();

        List<StatPair> _stats = new List<StatPair>();
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            stat.value += added;
            _stats.Add(stat);
        }
        return new PlayerStat(_stats);
    }
    public PlayerStat GetMultipliedStats(PlayerStat other)
    {
        CheckStat();
        other.CheckStat();

        List<StatPair> _stats = new List<StatPair>();
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            // other에서 같은 statType의 값을 찾아 곱함
            var otherStat = other.stats.FirstOrDefault(s => s.statType == stat.statType);
            stat.value *= otherStat.value;
            _stats.Add(stat);
        }
        return new PlayerStat(_stats);
    }
    public PlayerStat GetMultipliedStats(float Multplied)
    {
        CheckStat();

        List<StatPair> _stats = new List<StatPair>();
        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            stat.value = Mathf.RoundToInt(stat.value * Multplied);
            _stats.Add(stat);
        }
        return new PlayerStat(_stats);
    }

    public void InitStat()
    {
        stats.Clear();
        CheckStat();
    }

    public void CheckStat()
    {
        // 중복 요소 제거: 같은 statType을 가진 중복된 StatPair 제거
        var uniqueStats = new List<StatPair>();
        var seenStatTypes = new HashSet<PlayerStatType>();
        var removedDuplicates = new List<PlayerStatType>();

        foreach (var stat in stats)
        {
            if (!seenStatTypes.Contains(stat.statType))
            {
                uniqueStats.Add(stat);
                seenStatTypes.Add(stat.statType);
            }
            else
            {
                // 중복된 요소를 발견했을 때 로깅
                removedDuplicates.Add(stat.statType);
            }
        }
        stats = uniqueStats;

        // 제거된 중복 요소들을 로깅
        if (removedDuplicates.Count > 0)
        {
            Debug.LogWarning($"[PlayerStats] 중복 요소 {removedDuplicates.Count}개가 제거되었습니다:");
            foreach (var removedStatType in removedDuplicates)
            {
                Debug.LogWarning($"[PlayerStats] 제거된 중복 요소: {removedStatType}");
            }
        }

        // 기존 stats 리스트를 유지하면서 누락된 PlayerStatType들을 추가
        var existingStatTypes = stats.Select(s => s.statType).ToHashSet();

        foreach (PlayerStatType statType in Enum.GetValues(typeof(PlayerStatType)))
        {
            // 이미 존재하는 statType은 건너뛰고, 없는 것만 추가
            if (!existingStatTypes.Contains(statType))
            {
                var stat = new StatPair();
                stat.statType = statType;
                stat.value = 0;
                stat.desc = statType.ToString() + ": " + stat.value;
                stats.Add(stat);
            }
            else
            {
                // 기존 statType의 desc를 갱신
                var existingStat = stats.FirstOrDefault(s => s.statType == statType);
                if (existingStat.statType == statType)
                {
                    var index = stats.IndexOf(existingStat);
                    var updatedStat = existingStat;
                    updatedStat.desc = statType.ToString() + ": " + updatedStat.value;
                    stats[index] = updatedStat;
                }
            }
        }
    }
}
