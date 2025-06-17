using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] int currentXp = 0;
    [SerializeField] int level = 0;
    int RequiredXp => (level + 1) * 5;

    [Header(" Visuals ")]
    [SerializeField] Image xpBar;
    [SerializeField] Text levelText;

    void Start()
    {        
        UpdateUI();
    }

    public void GetXp(int amount)
    {
        currentXp += amount;

        while (currentXp >= RequiredXp)
        {
            currentXp -= RequiredXp;
            level++;            
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        xpBar.fillAmount = (float)currentXp / RequiredXp;
        levelText.text = "lv." + level;
    }

}


