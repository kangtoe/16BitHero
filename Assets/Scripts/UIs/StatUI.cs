using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatUI : MonoBehaviour
{
    [SerializeField]
    SingleStatUI singleStatUIPrefab;

    [NaughtyAttributes.Button]
    void InitUI()
    {
        UpdateStatsUI(new PlayerStat());
    }

    public void UpdateStatsUI(PlayerStat playerStat)
    {
        // 자식 오브젝트 삭제
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        // 자식 오브젝트 삭제 완료 후 다음 로직 처리
        foreach (var stat in playerStat.Stats)
        {
            SingleStatUI singleStatUI = Instantiate(singleStatUIPrefab, transform, false);
            singleStatUI.InitUI(stat.statType.ToString(), stat.value.ToString());
        }
    }
}
