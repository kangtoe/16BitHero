using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnInfo
{
    public string desc;

    [Space]
    public EnemyBase enemyBase;

    [Range(0, 100)]
    public int spawnChance = 100;
    public float spawnStartTime = 1;
    public float spawnInterval = 1; // 0 means spawn only once
    public int spawnAmount = 1;
    public float spawnAreaRadius = 2;
}

[Serializable]
public class WaveInfo
{
    public string desc;

    [Space]
    public SpawnInfo[] spawnInfos;
    public int waveDuration = 30;
}

public class WaveSpawnManager : MonoSingleton<WaveSpawnManager>
{
    public Action OnWaveEnd;

    bool canSpawn;
    public bool CanSpawn => canSpawn;

    [Header("data")]
    [SerializeField] WaveInfo[] waveInfos;

    [Space]

    [SerializeField] SpawnIndicator spawnIndicatorPrefab;

    [Header("SpawnArea")]
    [SerializeField] Vector2 spawnRadiusMinMax = new Vector2(5, 8);
    Vector2 spawnCenter => GameManager.Instance.Player.transform.position;

    [Header("SpawnedEnemies")]
    List<EnemyBase> enemies = new List<EnemyBase>();
    //[SerializeField] int maxEnemies;

    [Header("UIs")]
    [SerializeField] Text waveText;
    [SerializeField] Text timerText;

    void OnValidate()
    {
        for (int i = 0; i < waveInfos.Length; i++)
        {
            string waveInfo = "Wave" + i;
            waveInfos[i].desc = waveInfo;

            for (int j = 0; j < waveInfos[i].spawnInfos.Length; j++)
            {
                var v = waveInfos[i].spawnInfos[j];
                v.desc = waveInfo + " -> " + (v.enemyBase ? "Spawn: " + v.enemyBase.name : "! Should Register Spawn Enemy !");
            }
        }
    }

    void Start()
    {
        StartWave(0);
    }

    Vector2? GetSpawnPosition(int maxAttempts = 10)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 direction = Random.onUnitSphere;
            float range = Random.Range(spawnRadiusMinMax.x, spawnRadiusMinMax.y);
            Vector2 offset = direction.normalized * range;
            Vector2 targetPosition = spawnCenter + offset;

            if (ArenaArea.Instance.CheckInWall(targetPosition))
            {
                return targetPosition;
            }
        }

        return null;
    }

    public void RegisterEnemy(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }

    private void DefeatAllEnemies()
    {
        // todo: 적 생성 시 list 등록 + 사망시 list 제외, list를 통한 모든 적 오브젝트에 접근하여 사망 처리 수행
        foreach (EnemyBase enemy in transform.GetComponentsInChildren<EnemyBase>())
            enemy.ForceDie(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnCenter, spawnRadiusMinMax.y);
        Gizmos.DrawWireSphere(spawnCenter, spawnRadiusMinMax.x);
    }

    IEnumerator SpawnIE(SpawnInfo spawnInfo)
    {
        yield return new WaitUntil(() => Time.time > spawnInfo.spawnStartTime);

        while (true)
        {
            bool spawnCheck = Random.Range(1, 101) <= spawnInfo.spawnChance;
            if (spawnCheck)
            {
                Vector2 spawnPosition = GetSpawnPosition() ?? spawnCenter;
                Instantiate(spawnIndicatorPrefab, spawnPosition, Quaternion.identity).Init(spawnInfo.enemyBase);
            }

            yield return new WaitForSeconds(spawnInfo.spawnInterval);
        }
    }

    IEnumerator WaveIE(WaveInfo waveInfo)
    {
        foreach (SpawnInfo spawnInfo in waveInfo.spawnInfos)
        {
            StartCoroutine(SpawnIE(spawnInfo));
        }

        while (true)
        {
            int timeLeft = (int)(waveInfo.waveDuration - Time.time);
            UpdateTimerUI(timeLeft);
            if (timeLeft <= 0) break;

            yield return null;
        }

        EndWave();
    }

    public void StartWave(int wave)
    {
        if (wave >= waveInfos.Length)
        {
            Debug.LogError("wave number error");
            return;
        }

        waveText.text = "Wave " + wave;

        WaveInfo waveInfo = waveInfos[wave];
        StartCoroutine(WaveIE(waveInfo));

        canSpawn = true;
    }

    void EndWave()
    {
        StopAllCoroutines();
        DefeatAllEnemies();
        //OnWaveEnd.Invoke();

        canSpawn = false;

        Debug.Log("wave end!");
    }

    void UpdateTimerUI(int leftWaveDuration)
    {
        timerText.text = string.Format("{0:D2}:{1:D2}", leftWaveDuration / 60, leftWaveDuration % 60);
    }
}
