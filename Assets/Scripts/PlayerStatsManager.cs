using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStatsManager : MonoSingleton<PlayerStatsManager>
{
    [SerializeField]
    PlayerCharacterDataSO playerCharacter;

    PlayerStat additionalPlayerStats = new PlayerStat(); // from items


    [SerializeField] // for debug
    static PlayerStat currPlayerStat = new PlayerStat();

    static public PlayerStat CurrPlayerStat => currPlayerStat;

    [SerializeField]
    StatUI statUI;

    // Start is called before the first frame update
    void Start()
    {
        UpdateStats();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnValidate()
    {
        //UpdateStats();
    }

    void UpdateStats()
    {
        var sumStats = playerCharacter.BaseStatAdjust.GetAddedStats(additionalPlayerStats);
        var modifyPercent = playerCharacter.StatModifier.GetAddedStats(100);
        currPlayerStat = sumStats.GetMultipliedStats(modifyPercent);
        currPlayerStat = currPlayerStat.GetMultipliedStats(0.01f);

        int maxHealth = currPlayerStat.GetStatValue(PlayerStatType.MaxHealth);
        GameManager.Instance.Player.AdjustMaxHealth(maxHealth);

        statUI.UpdateStatsUI(currPlayerStat);
    }

    void UpdateStat()
    {

    }
}
