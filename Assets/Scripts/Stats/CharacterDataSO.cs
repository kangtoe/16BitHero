using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "PlayerCharacter Data", menuName = "Scriptable Objects/New Character Data", order = 0)]
public class PlayerCharacterDataSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public int PurchasePrice { get; private set; }

    [SerializeField]
    PlayerStat stat;

    void OnValidate()
    {
        stat.CheckStat();
    }

    [Button("Init Stat")]
    private void TestFunction()
    {
        stat.InitStat();
    }
}
