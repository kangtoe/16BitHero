using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResManager : MonoSingleton<PlayerResManager>
{    
    [Header("Resources")]
    [SerializeField] int gold = 0;
    public int Gold => gold;

    [SerializeField] int diamond = 0;
    public int Diamond => diamond;

    [Header("UI")]
    [SerializeField] Text goldText;
    [SerializeField] Text diamondText;

    void Start()
    {
        UpdateUI();
    }

    public void EarnGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    public void EarnDiamond(int amount)
    {
        diamond += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        goldText.text = gold.ToString();
        diamondText.text = diamond.ToString();
    }
}
