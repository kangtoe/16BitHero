using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleStatUI : MonoBehaviour
{
    [SerializeField]
    Text statName;

    [SerializeField]
    Text statValue;

    public void InitUI(string name, string value)
    {
        statName.text = name;
        statValue.text = value;
    }
}
